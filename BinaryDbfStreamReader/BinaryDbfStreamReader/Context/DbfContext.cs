using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.IO.DbfStream
{
    /// <summary>
    /// Конкретный класс Dbf для чтения
    /// </summary>
    public class DbfContextReader: IDbfContextReader
    {
        private BinaryDbfStreamReader _dbfReader;
        /// <summary>
        /// Кодировка DBF Атрибута
        /// </summary>
        public Encoding Encoding => _dbfReader.Encoding;
        /// <summary>
        /// Список колонок
        /// </summary>
        public string[] Columns => _dbfReader.Columns;
        /// <summary>
        /// Макс. кол-во записей в файле
        /// </summary>
        public int MaxRows => _dbfReader.MaxRows;
        /// <summary>
        /// Получить данные из контекста
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DbfRow> GetData()
        {
            while (!_dbfReader.Eof)
            {
                DbfRow row = _dbfReader.ReadRow();
                if (row != null)
                    yield return row;
            }
            _dbfReader.Position = 0;
        }
        /// <summary> ctor </summary>
        public DbfContextReader(string fileName, Encoding enc = default(Encoding))
        {
            _dbfReader = new BinaryDbfStreamReader(enc);
            _dbfReader.Open(fileName);
        }
        /// <summary>
        /// Освободить рессурсы
        /// </summary>
        public void Dispose()
        {
            if (_dbfReader == null) return;

            _dbfReader.Dispose();
            _dbfReader = null;
        }

        #region IDbfContext
        /// <summary>
        /// Получить Объектную модель данных из контекста
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetData<T>() where T: class, new()
        {
            return GetData().Select(ObjectParser.ToObject<T>);
        }
        /// <summary>
        /// Получить Объектную модель данных из контекста
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetData(Type objectType)
        {
            return GetData().Select(e => ObjectParser.ToObject(e, objectType));
        }
        #endregion

        private static class ObjectParser
        {
            public static T ToObject<T>(Dictionary<string, object> value) where T : new()
            {
                return (T)ToObject(value, typeof(T));
            }
            public static object ToObject(Dictionary<string, object> value, Type objType)
            {
                object instance = Activator.CreateInstance(objType);
                foreach (PropertyInfo propertyInfo in objType.GetProperties().Where(e => !e.GetCustomAttributes(true).OfType<IgnoreAttribute>().Any()))
                {

                    string columnKey = propertyInfo.Name;
                    ColumnAttribute c;
                    if ((c = propertyInfo.GetCustomAttributes(true).OfType<ColumnAttribute>().LastOrDefault()) != null)
                        columnKey = c.Name;

                    if (string.IsNullOrEmpty(value[columnKey].ToString())
                        && HasTypeDecimal(propertyInfo.PropertyType))
                        value[columnKey] = 0;

                    propertyInfo.SetValue(instance, Convert.ChangeType(value[columnKey], propertyInfo.PropertyType, CultureInfo.InvariantCulture), null);
                }
                return instance;
            }

            private static readonly Type[] InegersNumbers = {
            typeof (int),
            typeof (decimal),
            typeof (float),
            typeof (double)
        };
            private static bool HasTypeDecimal(Type type)
            {
                return InegersNumbers.Any(e => e == type);
            }
        }
    }
}
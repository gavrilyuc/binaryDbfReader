using System.Collections.Generic;
using System.Text;

namespace System.IO.DbfStream
{
    /// <summary>
    /// Предаставление Dbf Контекста
    /// </summary>
    public interface IDbfContextReader : IContextReader
    {
        /// <summary>
        /// Кодировка файла, указаная пользователем
        /// </summary>
        Encoding Encoding { get; }
        /// <summary>
        /// Список имён столбцов
        /// </summary>
        string[] Columns { get; }
        /// <summary>
        /// Максимальное кол-во записей
        /// </summary>
        int MaxRows { get; }
        /// <summary>
        /// получить данные файла
        /// </summary>
        /// <returns></returns>
        IEnumerable<DbfRow> GetData();
    }
}
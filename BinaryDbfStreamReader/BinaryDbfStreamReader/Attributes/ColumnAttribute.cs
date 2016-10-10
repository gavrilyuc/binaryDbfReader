namespace System.IO.DbfStream
{
    /// <summary>
    /// Атрибут, указывающий имя столбца
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Наименование колонки
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// данная колонка Унильный ключ
        /// </summary>
        public bool PrimaryKey { get; }
        /// <summary> ctor </summary>
        /// <param name="name"></param>
        /// <param name="hasPrimaryKey"></param>
        public ColumnAttribute(string name, bool hasPrimaryKey = false)
        {
            Name = name;
            PrimaryKey = hasPrimaryKey;
        }

        /// <summary> </summary>
        public override object TypeId => Name;
        /// <summary> </summary>
        public override bool IsDefaultAttribute() => true;
        /// <summary> </summary>
        public override bool Match(object obj)
        {
            return obj.Equals(this);
        }
        /// <summary> </summary>
        public override bool Equals(object other)
        {
            return other is ColumnAttribute && base.Equals(other) && string.Equals(Name, ((ColumnAttribute)other).Name);
        }
        /// <summary> </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Name?.GetHashCode() ?? 0);
            }
        }
    }
}
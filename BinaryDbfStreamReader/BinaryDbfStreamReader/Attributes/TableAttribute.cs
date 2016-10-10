namespace System.IO.DbfStream
{
    /// <summary>
    /// Атрибут, указывающий имя таблицы
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TableAttribute : Attribute
    {
        /// <summary> Имя таблицы </summary>
        public string Name { get; }
        /// <summary> ctor </summary>
        public TableAttribute(string name)
        {
            Name = name;
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
            return other is TableAttribute && base.Equals(other) && string.Equals(Name, ((TableAttribute)other).Name);
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
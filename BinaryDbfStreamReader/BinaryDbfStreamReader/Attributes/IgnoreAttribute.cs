namespace System.IO.DbfStream
{
    /// <summary>
    /// Атрибут указывающий что свойство должно быть проигнорировано объектом Context
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IgnoreAttribute: Attribute
    {
        /// <summary> </summary>
        public override object TypeId => "Ignore";
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
            return other is IgnoreAttribute && base.Equals(other);
        }
        /// <summary> </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() * 397;
            }
        }

    }
}
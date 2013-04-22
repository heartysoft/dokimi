namespace dokimi.core
{
    public class SpecificationCategory
    {
        public static SpecificationCategory Unspecified
        {
            get{ return new SpecificationCategory("Unspecified", "Unspecified");}
        }

        public bool IsUnspecified { get { return Unspecified == this; } }

        public string ContextName { get; private set; }
        public string CategoryName { get; private set; }

        public SpecificationCategory(string contextName, string categoryName)
        {
            ContextName = contextName;
            CategoryName = categoryName;
        }

        protected bool Equals(SpecificationCategory other)
        {
            return string.Equals(ContextName, other.ContextName) && string.Equals(CategoryName, other.CategoryName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecificationCategory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContextName != null ? ContextName.GetHashCode() : 0)*397) ^ (CategoryName != null ? CategoryName.GetHashCode() : 0);
            }
        }

        public static bool operator ==(SpecificationCategory left, SpecificationCategory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpecificationCategory left, SpecificationCategory right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ContextName, CategoryName);
        }
    }
}
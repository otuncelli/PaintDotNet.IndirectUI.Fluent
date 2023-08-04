using System.Collections.Generic;

namespace PaintDotNet.PropertySystem
{
    internal class PropertyEqualityComparer : IEqualityComparer<Property>
    {
        public bool Equals(Property x, Property y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.Name.Equals(y.Name) && x.Value.Equals(y.Value);
        }

        public int GetHashCode(Property obj)
        {
            return obj.Name.GetHashCode() ^ obj.Value.GetHashCode();
        }
    }
}

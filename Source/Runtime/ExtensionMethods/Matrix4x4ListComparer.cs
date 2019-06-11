using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal class Matrix4x4IEnumerableComparer : IEqualityComparer<IEnumerable<Matrix4x4>>
    {
        public bool Equals (IEnumerable<Matrix4x4> x, IEnumerable<Matrix4x4> y)
        {
            if (x.Count() != y.Count()) { return false; }

            for (int i = 0; i < x.Count(); i++)
            {
                if (!x.ElementAt(i).Equals(y.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(IEnumerable<Matrix4x4> matrices)
        {
            unchecked
            {
                int hash = 17;
                foreach (var matrix in matrices)
                {
                    hash = hash * 23 + (matrix.GetHashCode());
                }
 
                return hash;
            }
        }
    }
}

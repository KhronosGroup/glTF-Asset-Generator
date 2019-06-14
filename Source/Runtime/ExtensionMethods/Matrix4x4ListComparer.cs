using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal class Matrix4x4IEnumerableComparer : IEqualityComparer<IEnumerable<Matrix4x4>>
    {
        public bool Equals (IEnumerable<Matrix4x4> x, IEnumerable<Matrix4x4> y)
        {
            //Use sequence equal
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
            return matrices.Any() ? matrices.First().GetHashCode() : 0;
        }
    }
}

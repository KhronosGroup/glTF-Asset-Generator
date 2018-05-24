using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class GenericExtensionMethods
    {
        /// <summary>
        /// Function which determines if two objects equivalent
        /// </summary>
        public static bool ObjectsEqual(this object object1, object object2)
        {
            // Checks if the objects are the same instance.
            if (ReferenceEquals(object1, object2)) return true;

            // Checks both objects in regards to being null. They're equal if both null, and not if only one of them is.
            if ((object1 == null) || (object2 == null))
            {
                return CheckForNullEquivalence(object1, object2);
            }

            // Compare two object's class, return false if they are different.
            if (object1.GetType() != object2.GetType()) return false;

            var result = true;
            // Get all the properties of obj1 and compare the two objects.
            foreach (var property in object1.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0))
            {
                var valueObject1 = property.GetValue(object1);
                var valueObject2 = property.GetValue(object2);

                // Checks each property in regards to being null. They're equal if both null, and not if only one of them is.
                if ((valueObject1 == null) || (valueObject2 == null))
                {
                    return CheckForNullEquivalence(valueObject1, valueObject2);
                }
                else
                {
                    Type type = valueObject1.GetType();
                    if (type.IsValueType || type == typeof(string))
                    {
                        // Compares the value between the two objects.
                        if (!valueObject1.Equals(valueObject2)) return false;
                    }
                    else if(type.IsArray)
                    {
                        IEnumerable<object> array1 = ((IEnumerable)valueObject1).Cast<object>();
                        IEnumerable<object> array2 = ((IEnumerable)valueObject2).Cast<object>();

                        // Checks both arrays in regards to being null. They're equal if both null, and not if only one of them is.
                        if (array1 == null || array2 == null)
                        {
                            return CheckForNullEquivalence(array1, array2);
                        }
                        else
                        {
                            // Compares both arrays one value at a time.
                            int count = array1.Count();
                            for (int x = 0; x < count; x++)
                            {
                                if (!array1.ElementAt(x).Equals(array2.ElementAt(x))) return false;
                            }
                        }
                    }
                    else if (type.IsClass)
                    {
                        // The property is a class, so this function is called recursively against it.
                        if (!valueObject1.ObjectsEqual(valueObject2)) return false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks two objects, at least one of which is null. If both are null then they are equivalent. Otherwise they are not.
        /// </summary>
        private static bool CheckForNullEquivalence<T>(T object1, T object2)
        {
            if ((object1 == null) && (object2 == null))
            {
                // They are equal.
                return true; 
            }
            else
            {
                // They are not equal.
                return false; 
            }
        }
    }
}

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
        public static bool ObjectsEqual(this object obj1, object obj2)
        {
            // Checks if the objects are the same instance
            if (ReferenceEquals(obj1, obj2)) return true;

            // Checks both objects in regards to being null. They're equal if both null, and not if only one of them is.
            if ((obj1 == null) || (obj2 == null))
            {
                if ((obj1 == null) && (obj2 == null)) return true;
                else return false;
            }

            // Compare two object's class, return false if they are different
            if (obj1.GetType() != obj2.GetType()) return false;

            var result = true;
            // Get all the properties of obj1 and compare the two objects
            foreach (var property in obj1.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0))
            {
                var obj1Value = property.GetValue(obj1);
                var obj2Value = property.GetValue(obj2);

                // Checks each property in regards to being null. They're equal if both null, and not if only one of them is.
                if ((obj1Value == null) || (obj2Value == null))
                {
                    if ((obj1Value == null) && (obj2Value == null)) result = true;
                    else return false;
                }
                else
                {
                    Type type = obj1Value.GetType();
                    if (type.IsValueType || type == typeof(string))
                    {
                        // Compares the value between the two objects
                        if (!obj1Value.Equals(obj2Value)) return false;
                    }
                    else if(type.IsArray)
                    {
                        IEnumerable<object> array1 = ((IEnumerable)obj1Value).Cast<object>();
                        IEnumerable<object> array2 = ((IEnumerable)obj2Value).Cast<object>();


                        // Checks both arrays in regards to being null. They're equal if both null, and not if only one of them is.
                        if (array1 == null || array2 == null)
                        {
                            if (array1 == null && array2 == null) result = true;
                            else return false;
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
                        // The property is a class, so this function is called recursively against it
                        if (!obj1Value.ObjectsEqual(obj2Value)) return false;
                    }
                }
            }

            return result;
        }
    }
}

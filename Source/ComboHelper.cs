using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator
{
    internal static class ComboHelper
    {
        public static List<List<Property>> AttributeCombos(Test test)
        {
            List<List<Property>> finalResult;
            List<List<Property>> removeTheseCombos = new List<List<Property>>();
            List<List<Property>> keepTheseCombos = new List<List<Property>>();
            List<Property> isRequired = new List<Property>();
            List<Property> isPrerequisite = new List<Property>();
            bool hasPrerequisiteAttribute;

            //var combos = PowerSet<Attribute>(attributes);
            var combos = BasicSet<Property>(test.properties);

            // Include any special combos
            if (test.specialCombos.Any())
            {
                foreach (var x in test.specialCombos)
                {
                    var comboIndex = combos.FindIndex(e => e.Any() && e[0].name == x[0].name && e.Count() == 1);
                    combos.Insert(comboIndex + 1, x);
                }
            }

            // Remove the explicitly excluded combos
            if (test.removeCombos.Any())
            {
                foreach (var x in test.removeCombos)
                {
                    combos.RemoveAll(e => e.Count == 1 && e[0].name == x[0].name);
                }
            }

            if (test.noPrerequisite == false)
            {
                // Makes a list of names of possible prerequisites
                List<Propertyname> Prerequisites = new List<Propertyname>();
                foreach (var x in test.properties)
                {
                    if (x.prerequisite != Propertyname.Undefined)
                    {
                        if (Prerequisites.Any())
                        {
                            bool isNew = true;
                            foreach (var y in Prerequisites)
                            {
                                if (y == x.prerequisite)
                                {
                                    isNew = false;
                                    break;
                                }
                            }
                            if (isNew == true)
                            {
                                Prerequisites.Add(x.prerequisite);
                            }
                        }
                        else
                        {
                            Prerequisites.Add(x.prerequisite);
                        }
                    }
                }
                // Convert the name list into a list of the actual prerequisites
                foreach (var x in Prerequisites)
                {
                    foreach (var y in test.properties)
                    {
                        if (x == y.name)
                        {
                            isPrerequisite.Add(y);
                            break;
                        }
                    }
                }
                // Add combos where prerequisite property have all dependant property set
                foreach (var x in isPrerequisite)
                {
                    // Start a list with the prerequisite attribute 
                    var addList = new List<Property>
                    {
                        x
                    };

                    // Populate that list will all of the required property
                    foreach (var y in test.properties)
                    {
                        if (y.prerequisite == x.name)
                        {
                            addList.Add(y);
                        }
                    }
                    // Then include the combo with the rest
                    var comboIndex = combos.FindIndex(e => e.Any() && e[0].name == addList[0].name && e.Count() == 1);
                    combos.Insert(comboIndex + 1, addList);
                }
            }

            // Handle non-binary property in the first combo
            if (test.onlyBinaryProperties == false)
            {
                List<Property> keep = new List<Property>();
                foreach (var x in combos[1])
                {
                    // Keep property if it is the first found or is binary
                    if (x.propertyGroup == 0 || (x.propertyGroup > 0 && !keep.Any()))
                    {
                        keep.Add(x);
                    }
                    else if (x.propertyGroup > 0)
                    {
                        bool alreadyKept = false;
                        foreach (var y in keep)
                        {
                            // Don't keep the nonbinary property if there is already one of that set on the list
                            if (y.propertyGroup == x.propertyGroup)
                            {
                                alreadyKept = true;
                                break;
                            }
                        }
                        if (alreadyKept == false) // Keep nonbinary property 
                        {
                            keep.Add(x);
                        }
                    }
                }
                // Remove the extra nonbinary attributes
                combos[1] = keep;
            }

            // Removes sets that duplicate binary entries for a single property (e.g. alphaMode)
            // Removes sets where an attribute is missing a required property
            if (test.onlyBinaryProperties == false || test.noPrerequisite == false)
            {
                // Are there any prerequisite property? 
                hasPrerequisiteAttribute = isPrerequisite.Any();

                // Makes a list of combos to remove
                int combosCount = combos.Count();
                for (int x = 2; x < combosCount; x++) // The first two combos are already taken care of
                {
                    bool usedPrereq = false;
                    List<int> binarySets = new List<int>();
                    List<Propertyname> usedPrerequisite = new List<Propertyname>();

                    // Makes a list of each prerequisite property in the current combo
                    if (hasPrerequisiteAttribute == true)
                    {
                        foreach (var prereq in isPrerequisite)
                        {
                            foreach (var attribute in combos[x])
                            {
                                if (attribute.name == prereq.name)
                                {
                                    usedPrerequisite.Add(prereq.name);
                                }
                            }
                        }
                        usedPrereq = usedPrerequisite.Any();
                    }

                    foreach (var attribute in combos[x])
                    {
                        // Remove combos that have multiple of the same binary combo
                        if (attribute.propertyGroup > 0)
                        {
                            if (binarySets.Contains(attribute.propertyGroup))
                            {
                                removeTheseCombos.Add(combos[x]);
                                break;
                            }
                            else
                            {
                                binarySets.Add(attribute.propertyGroup);
                            }
                        }
                        // Removes combos that have a property missing a prerequisite
                        if (usedPrereq == true && attribute.prerequisite != Propertyname.Undefined)
                        {
                            bool prereqNotFound = true;
                            foreach (var prereq in usedPrerequisite)
                            {
                                if (attribute.prerequisite == prereq)
                                {
                                    prereqNotFound = false;
                                    break;
                                }
                            }
                            if (prereqNotFound != false)
                            {
                                removeTheseCombos.Add(combos[x]);
                                break;
                            }
                        }
                        else if (usedPrereq == false && attribute.prerequisite != Propertyname.Undefined)
                        {
                            removeTheseCombos.Add(combos[x]);
                            break;
                        }
                    }
                }

                // Uses the list of bad combos to trim down the original set
                int numCombos = combos.Count();
                int numRemoveTheseCombos = removeTheseCombos.Count();
                for (int x = 0; x < numCombos; x++)
                {
                    bool excludeCombo = false;
                    for (int y = 0; y < numRemoveTheseCombos; y++)
                    {
                        if (combos[x] == removeTheseCombos[y])
                        {
                            excludeCombo = true;
                            break;
                        }
                    }
                    if (excludeCombo == false)
                    {
                        keepTheseCombos.Add(combos[x]);
                    }
                }
                finalResult = keepTheseCombos;
            }
            else
            {
                // If there are only binary attributes, we don't need to check for duplicates
                finalResult = combos;
            }

            // Properties in this list need to be handeled within the specific test group.
            if (test.specialProperties.Any())
            {
                finalResult = test.ApplySpecialProperties(test, finalResult);
            }

            return finalResult;
        }

        public static List<Property> CustomComboCreation(Property propertyA, Property propertyB = null, Property propertyC = null, Property propertyD = null, Property propertyE = null, Property propertyF = null)
        {
            List<Property> newCombo = new List<Property>();

            newCombo.Add(propertyA);

            if (propertyB != null)
            {
                newCombo.Add(propertyB);
            }

            if (propertyC != null)
            {
                newCombo.Add(propertyC);
            }

            if (propertyD != null)
            {
                newCombo.Add(propertyD);
            }

            if (propertyE != null)
            {
                newCombo.Add(propertyE);
            }

            if (propertyF != null)
            {
                newCombo.Add(propertyF);
            }

            return newCombo;
        }

        /// <summary>
        /// Given a,b,c this returns [a,b,c], [a], [b], [c]
        /// </summary>
        /// <param name="seq"></param>
        /// <returns>List of lists containing a basic set of combinations.</returns>
        public static List<List<T>> BasicSet<T>(List<T> seq)
        {
            var basicSet = new List<List<T>>();
            basicSet.Add(new List<T>()); // Will contain the empty set
            basicSet.Add(new List<T>()); // Will contain the full set
            foreach (var x in seq)
            {
                basicSet[1].Add(x);
                var addList = new List<T>
                {
                    x
                };
                basicSet.Add(addList);
            }
            return basicSet;
        }

        /// <summary>
        /// Given a,b,c this returns all possible combinations including a full and empty set.
        /// </summary>
        /// <param name="seq"></param>
        /// <returns>List of lists containing a powerset.</returns>
        //https://stackoverflow.com/questions/19890781/creating-a-power-set-of-a-sequence
        public static List<List<T>> PowerSet<T>(List<T> seq)
        {
            var powerSet = new List<List<T>>();
            powerSet.Add(new List<T>()); // starting only with empty set
            for (int i = 0; i < seq.Count; i++)
            {
                var cur = seq[i];
                int count = 1 << i; // doubling list each time
                for (int j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    powerSet.Add(new List<T>());
                    powerSet[count + j] = new List<T>();
                    var destination = powerSet[count + j];
                    for (int q = 0; q < source.Count; q++)
                        destination.Add(source[q]);
                    destination.Add(cur);
                }
            }
            return powerSet;
        }

        /// <summary>
        /// Compares two sets of model properties and returns whether they are equal or not, regardless of list order
        /// </summary>
        /// <param name="comboToCheck"></param>
        /// <param name="comboToFind"></param>
        /// <returns>Returns a bool, true if they contain the exact same attributes in any order</returns>
        public static bool FindCombo(Property[] comboToCheck, Property[] comboToFind)
        {
            if (comboToCheck.Count() == comboToFind.Count())
            {
                bool isEqual = true;
                foreach (var x in comboToCheck)
                {
                    bool containsElement = false;
                    foreach (var y in comboToFind)
                    {
                        if (x.name == y.name)
                        {
                            containsElement = true;
                            break;
                        }
                    }
                    if (containsElement == false)
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

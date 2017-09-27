using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public List<Attribute> attributes;
        public List<Attribute> requiredAttributes = null;
        public ImageAttribute[] imageAttributes;
        private List<List<Attribute>> specialCombos = new List<List<Attribute>>();
        private List<List<Attribute>> removeCombos = new List<List<Attribute>>();
        bool onlyBinaryAttributes = true;
        bool noPrerequisite = true;
        string texture = "UVmap2017.png";

        public TestValues(Tests testType)
        {
            testArea = testType;

            switch (testArea)
            {
                case Tests.Material:
                    {
                        onlyBinaryAttributes = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.MetallicFactor, 0.0f),
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                            new Attribute(AttributeName.EmissiveTexture, image),
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.Scale, 2.0f, AttributeName.NormalTexture),
                            new Attribute(AttributeName.OcclusionTexture, image),
                            new Attribute(AttributeName.Strength, 0.5f, AttributeName.OcclusionTexture)
                        };
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.EmissiveFactor),
                            attributes.Find(e => e.name == AttributeName.EmissiveTexture)));
                        break;
                    }
                case Tests.Material_Alpha:
                    {
                        onlyBinaryAttributes = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                            new Attribute(AttributeName.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                            new Attribute(AttributeName.AlphaCutoff, 0.2f),
                            new Attribute(AttributeName.DoubleSided, true),
                        };
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                            attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                            attributes.Find(e => e.name == AttributeName.DoubleSided)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Blend),
                            attributes.Find(e => e.name == AttributeName.DoubleSided)));
                        removeCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
                        break;
                    }
                case Tests.Material_MetallicRoughness:
                    {
                        onlyBinaryAttributes = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                            new Attribute(AttributeName.BaseColorTexture, image),
                            new Attribute(AttributeName.MetallicFactor, 0.5f),
                            new Attribute(AttributeName.RoughnessFactor, 0.5f),
                            new Attribute(AttributeName.MetallicRoughnessTexture, image)
                        };
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.BaseColorTexture),
                            attributes.Find(e => e.name == AttributeName.BaseColorFactor)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.RoughnessFactor),
                            attributes.Find(e => e.name == AttributeName.MetallicFactor)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.MetallicFactor)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.RoughnessFactor)));
                        break;
                    }
                case Tests.Texture_Sampler:
                    {
                        // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
                        // attributes will have no visible affect unless mipmapping is implemented by the client
                        onlyBinaryAttributes = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorTexture, image)
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.MagFilter_Nearest, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, group:1),
                            new Attribute(AttributeName.MagFilter_Linear, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, group:1),
                            new Attribute(AttributeName.MinFilter_Nearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_Linear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, group:2),
                            new Attribute(AttributeName.MinFilter_NearestMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_LinearMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_NearestMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, group:2),
                            new Attribute(AttributeName.MinFilter_LinearMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, group:2),
                            new Attribute(AttributeName.WrapS_ClampToEdge, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, group:3),
                            new Attribute(AttributeName.WrapS_MirroredRepeat, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, group:3),
                            new Attribute(AttributeName.WrapT_ClampToEdge, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, group:4),
                            new Attribute(AttributeName.WrapT_MirroredRepeat, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, group:4)
                        };
                        break;
                    }
                case Tests.Primitive_Attribute:
                    {
                        onlyBinaryAttributes = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorTexture, image),
                            new Attribute(AttributeName.NormalTexture, image)
                        };
                        List<Vector3> planeNormals = new List<Vector3>()
                        {
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f)
                        };
                        List<Vector2> uvCoord1 = new List<Vector2>()
                        {
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 1.0f, 1.0f),
                            new Vector2( 1.0f, 0.0f),
                            new Vector2( 0.5f, 0.0f)
                        };
                        List<Vector2> uvCoord2 = new List<Vector2>()
                        {
                            new Vector2( 0.0f, 1.0f),
                            new Vector2( 0.5f, 1.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f)
                        };
                        List<Vector4> colorCoord = new List<Vector4>()
                        {
                            new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
                        };
                        List<Vector4> tanCoord = new List<Vector4>()
                        {
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f)
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.Normal, planeNormals),
                            new Attribute(AttributeName.Tangent, tanCoord),
                            new Attribute(AttributeName.TexCoord0_Float, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord0_Byte, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord0_Short, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord1_Float, uvCoord2, AttributeName.TexCoord0_Float, 2),
                            new Attribute(AttributeName.TexCoord1_Byte, uvCoord2, AttributeName.TexCoord0_Byte, 2),
                            new Attribute(AttributeName.TexCoord1_Short, uvCoord2, AttributeName.TexCoord0_Short, 2),
                            new Attribute(AttributeName.Color_Vector3_Float, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector3_Byte, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector3_Short, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Float, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Byte, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Short, colorCoord, group:3),
                        };
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.Normal),
                            attributes.Find(e => e.name == AttributeName.Tangent)));
                        removeCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.Tangent)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.TexCoord0_Byte),
                            attributes.Find(e => e.name == AttributeName.TexCoord1_Byte),
                            attributes.Find(e => e.name == AttributeName.Color_Vector4_Byte)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.TexCoord0_Byte),
                            attributes.Find(e => e.name == AttributeName.TexCoord1_Byte),
                            attributes.Find(e => e.name == AttributeName.Color_Vector3_Byte)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.TexCoord0_Short),
                            attributes.Find(e => e.name == AttributeName.TexCoord1_Short),
                            attributes.Find(e => e.name == AttributeName.Color_Vector4_Short)));
                        specialCombos.Add(ComboCreation(
                            attributes.Find(e => e.name == AttributeName.TexCoord0_Short),
                            attributes.Find(e => e.name == AttributeName.TexCoord1_Short),
                            attributes.Find(e => e.name == AttributeName.Color_Vector3_Short)));
                        
                        break;
                    }
            }
        }

        public List<List<Attribute>> AttributeCombos()
        {
            List<List<Attribute>> finalResult;
            List<List<Attribute>> removeTheseCombos = new List<List<Attribute>>();
            List<List<Attribute>> keepTheseCombos = new List<List<Attribute>>();
            List<Attribute> isRequired = new List<Attribute>();
            List<Attribute> isPrerequisite = new List<Attribute>();
            bool hasPrerequisiteAttribute;

            //var combos = PowerSet<Attribute>(attributes);
            var combos = BasicSet<Attribute>(attributes);

            // Include any special combos
            if (specialCombos.Any())
            {
                foreach (var x in specialCombos)
                {
                    var comboIndex = combos.FindIndex(e => e.Any() && e[0].name == x[0].name && e.Count() == 1);
                    combos.Insert(comboIndex + 1, x);
                }
            }

            // Remove the explicitly excluded combos
            if (removeCombos.Any())
            {
                foreach (var x in removeCombos)
                {
                    combos.RemoveAll(e => e.Count == 1 && e[0].name == x[0].name);
                }
            }

            if (noPrerequisite == false)
            {
                // Makes a list of names of possible prerequisites
                List<AttributeName> Prerequisites = new List<AttributeName>();
                foreach (var x in attributes)
                {
                    if (x.prerequisite != AttributeName.Undefined)
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
                    foreach (var y in attributes)
                    {
                        if (x == y.name)
                        {
                            isPrerequisite.Add(y);
                            break;
                        }
                    }
                }
                // Add combos where prerequisite attributes have all dependant attributes set
                foreach (var x in isPrerequisite)
                {
                    // Start a list with the prerequisite attribute 
                    var addList = new List<Attribute>
                    {
                        x
                    };

                    // Populate that list will all of the required attributes
                    foreach (var y in attributes)
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

            // Handle non-binary attributes in the first combo
            if (onlyBinaryAttributes == false)
            {
                List<Attribute> keep = new List<Attribute>();
                foreach (var x in combos[1])
                {
                    // Keep attribute if it is the first found or is binary
                    if (x.attributeGroup == 0 || (x.attributeGroup > 0 && !keep.Any()))
                    {
                        keep.Add(x);
                    }
                    else if (x.attributeGroup > 0)
                    {
                        bool alreadyKept = false;
                        foreach (var y in keep)
                        {
                            // Don't keep the nonbinary attribute if there is already one of that set on the list
                            if (y.attributeGroup == x.attributeGroup)
                            {
                                alreadyKept = true;
                                break;
                            }
                        }
                        if (alreadyKept == false) // Keep nonbinary attribute 
                        {
                            keep.Add(x);
                        }
                    }
                }
                // Remove the extra nonbinary attributes
                combos[1] = keep;
            }

            // Removes sets that duplicate binary entries for a single attribute (e.g. alphaMode)
            // Removes sets where an attribute is missing a required attribute
            if (onlyBinaryAttributes == false || noPrerequisite == false)
            {
                // Are there any prerequisite attributes? 
                hasPrerequisiteAttribute = isPrerequisite.Any();

                // Makes a list of combos to remove
                int combosCount = combos.Count();
                for (int x = 2; x < combosCount; x++) // The first two combos are already taken care of
                {
                    bool usedPrereq = false;
                    List<int> binarySets = new List<int>();
                    List<AttributeName> usedPrerequisite = new List<AttributeName>();

                    // Makes a list of each prerequisite attribute in the current combo
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
                        if (attribute.attributeGroup > 0)
                        {
                            if (binarySets.Contains(attribute.attributeGroup))
                            {
                                removeTheseCombos.Add(combos[x]);
                                break;
                            }
                            else
                            {
                                binarySets.Add(attribute.attributeGroup);
                            }
                        }
                        // Removes combos that have a attribute missing a prerequisite
                        if (usedPrereq == true && attribute.prerequisite != AttributeName.Undefined)
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
                        else if (usedPrereq == false && attribute.prerequisite != AttributeName.Undefined)
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

            return finalResult;
        }

        private List<Attribute> ComboCreation(Attribute attributeA)
        {
            List<Attribute> newCombo = new List<Attribute>();

            newCombo.Add(attributeA);

            return newCombo;
        }

        private List<Attribute> ComboCreation(Attribute paramA, Attribute paramB)
        {
            List<Attribute> newCombo = new List<Attribute>();

            newCombo.Add(paramA);
            newCombo.Add(paramB);

            return newCombo;
        }

        private List<Attribute> ComboCreation(Attribute paramA, Attribute paramB, Attribute paramC)
        {
            List<Attribute> newCombo = new List<Attribute>();

            newCombo.Add(paramA);
            newCombo.Add(paramB);
            newCombo.Add(paramC);

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
        /// Compares two sets of model attributes and returns whether they are equal or not, regardless of list order
        /// </summary>
        /// <param name="comboToCheck"></param>
        /// <param name="comboToFind"></param>
        /// <returns>Returns a bool, true if they contain the exact same attributes in any order</returns>
        bool FindCombo(Attribute[] comboToCheck, Attribute[] comboToFind)
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

    public class Attribute
    {
        public AttributeName name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public AttributeName prerequisite = AttributeName.Undefined;
        public int attributeGroup;

        public Attribute(AttributeName attributeName, dynamic attributeValue, AttributeName ParentAttribute = AttributeName.Undefined, int group = 0)
        {
            name = attributeName;
            value = attributeValue;
            prerequisite = ParentAttribute;
            attributeGroup = group;
        }
    }

    public enum Tests
    {
        Undefined,
        Material,
        Material_Alpha,
        Material_MetallicRoughness,
        Texture_Sampler,
        Primitive_Attribute,
    }

    public enum AttributeName
    {
        Undefined,
        Name,
        BaseColorFactor,
        BaseColorTexture,
        MetallicFactor,
        RoughnessFactor,
        MetallicRoughnessTexture,
        PbrTextures,
        EmissiveFactor,
        AlphaMode_Mask,
        AlphaMode_Blend,
        AlphaMode_Opaque,
        AlphaCutoff,
        Color_Vector3_Float,
        Color_Vector4_Float,
        Color_Vector3_Byte,
        Color_Vector4_Byte,
        Color_Vector3_Short,
        Color_Vector4_Short,
        DoubleSided,
        Sampler,
        MagFilter_Nearest,
        MagFilter_Linear,
        MinFilter_Nearest,
        MinFilter_Linear,
        MinFilter_NearestMipmapNearest,
        MinFilter_LinearMipmapNearest,
        MinFilter_NearestMipmapLinear,
        MinFilter_LinearMipmapLinear,
        Normal,
        Position,
        Tangent,
        TexCoord0_Float,
        TexCoord0_Byte,
        TexCoord0_Short,
        TexCoord1_Float,
        TexCoord1_Byte,
        TexCoord1_Short,
        WrapS_ClampToEdge,
        WrapS_MirroredRepeat,
        WrapS_Repeat,
        WrapT_ClampToEdge,
        WrapT_MirroredRepeat,
        WrapT_Repeat,
        Source,
        TexCoord,
        NormalTexture,
        OcclusionTexture,
        EmissiveTexture,
        Scale,
        Strength
    }
}

using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_Children : ModelGroup
    {
        public Node_Children()
        {
            modelGroupName = ModelGroupName.Node_Children;
            onlyBinaryProperties = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.ChildNode0, null),
                new Property(Propertyname.ChildNode1, null),
                new Property(Propertyname.ChildNode2, null),
            };
            var chld0 = properties.Find(e => e.name == Propertyname.ChildNode0);
            var chld1 = properties.Find(e => e.name == Propertyname.ChildNode1);
            var chld2 = properties.Find(e => e.name == Propertyname.ChildNode2);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                chld0,
                chld1,
            });
            removeCombos.Add(new List<Property>()
            {
                chld1
            });
            removeCombos.Add(new List<Property>()
            {
                chld2
            });
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

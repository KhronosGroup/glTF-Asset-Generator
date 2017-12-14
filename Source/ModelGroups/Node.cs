using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node : ModelGroup
    {
        public Node()
        {
            modelGroupName = ModelGroupName.Node;
            onlyBinaryProperties = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image iconNodes = new Runtime.Image
            {
                Uri = icon_Nodes
            };
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            usedImages.Add(iconNodes);
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.ChildNodes, iconNodes),
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, null),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, null),
                new Property(Propertyname.Translation, null),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale, null),
            };
            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var translation = properties.Find(e => e.name == Propertyname.Translation);
            var rotation = properties.Find(e => e.name == Propertyname.Rotation);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            removeCombos.Add(new List<Property>()
            {
                matrix,
                translation,
                rotation,
                scale,
            });
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

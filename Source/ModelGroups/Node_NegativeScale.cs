using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_NegativeScale : ModelGroup
    {
        public Node_NegativeScale()
        {
            modelGroupName = ModelGroupName.Node_NegativeScale;
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
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, null),
                new Property(Propertyname.Translation, null),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale, null),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, null),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);

        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

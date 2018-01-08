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
            usedFigures.Add(normalTexture);
            usedFigures.Add(baseColorTexture);
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, null),
                new Property(Propertyname.Scale, null),
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, null),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, null),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                matrix,
                normal,
                tangent,
                normTex,
                colorTex
            });
            specialCombos.Add(new List<Property>()
            {
                scale,
                normal,
                tangent,
                normTex,
                colorTex
            });
            removeCombos.Add(new List<Property>()
            {
                normal
            });
            removeCombos.Add(new List<Property>()
            {
                tangent
            });
            removeCombos.Add(new List<Property>()
            {
                normTex
            });
            removeCombos.Add(new List<Property>()
            {
                colorTex
            });
            removeCombos.Add(new List<Property>()
            {
                matrix,
                scale,
                normal,
                tangent,
                normTex,
                colorTex
            });
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

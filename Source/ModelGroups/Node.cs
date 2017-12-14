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
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix_Positive, "Positive Scale", group: 1),
                new Property(Propertyname.Matrix_Negative, "Negative Scale", group: 1),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale_Positive, "Positive Scale", group: 2),
                new Property(Propertyname.Scale_Negative, "Negative Scale", group: 2),
                new Property(Propertyname.Translation, null),
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
            var matrixP = properties.Find(e => e.name == Propertyname.Matrix_Positive);
            var matrixN = properties.Find(e => e.name == Propertyname.Matrix_Negative);
            var rotation = properties.Find(e => e.name == Propertyname.Rotation);
            var scaleP = properties.Find(e => e.name == Propertyname.Scale_Positive);
            var scaleN= properties.Find(e => e.name == Propertyname.Scale_Negative);
            var translation = properties.Find(e => e.name == Propertyname.Translation);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                translation,
                matrixP
            });
            specialCombos.Add(new List<Property>()
            {
                scaleP,
                matrixP
            });
            specialCombos.Add(new List<Property>()
            {
                rotation,
                matrixP
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
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Creates another set of models, that all have normals, tangent, and normal texture
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            int count = combos.Count - 1;
            for (int x = 1; x <= count; x++) // Ignores the empty and full sets
            {
                var newCombo = DeepCopy.CloneObject(combos[x]);
                newCombo.Add(normal);
                newCombo.Add(tangent);
                newCombo.Add(normTex);
                newCombo.Add(colorTex);
                combos.Add(newCombo);
            }

            // Change the normal full set to not have normal attributes
            combos[1].RemoveRange(4, 4);

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

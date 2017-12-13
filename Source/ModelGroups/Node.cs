using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node : ModelGroup
    {
        public Node()
        {
            modelGroupName = ModelGroupName.Node;
            onlyBinaryProperties = false;
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix_Positive, "Positive", group: 1),
                new Property(Propertyname.Matrix_Negative, "Negative", group: 1),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale_Positive, "Positive", group: 2),
                new Property(Propertyname.Scale_Negative, "Negative", group: 2),
                new Property(Propertyname.Translation, null),
            };
            var matrixP = properties.Find(e => e.name == Propertyname.Matrix_Positive);
            var matrixN = properties.Find(e => e.name == Propertyname.Matrix_Negative);
            var rotation = properties.Find(e => e.name == Propertyname.Rotation);
            var scaleP = properties.Find(e => e.name == Propertyname.Scale_Positive);
            var scaleN= properties.Find(e => e.name == Propertyname.Scale_Negative);
            var translation = properties.Find(e => e.name == Propertyname.Translation);
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
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

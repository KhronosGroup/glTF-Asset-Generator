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
                new Property(Propertyname.Camera_0, "Camera 0", group: 1),
                new Property(Propertyname.Camera_1, "Camera 1", group: 1),
                new Property(Propertyname.Matrix, null),
                new Property(Propertyname.Mesh0, null),
                new Property(Propertyname.Mesh1, null),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale, null),
                new Property(Propertyname.Translation, null),
            };
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            return wrapper;
        }
    }
}

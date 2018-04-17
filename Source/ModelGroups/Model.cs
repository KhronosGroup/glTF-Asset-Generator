using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Model
    {
        public ModelGroupName modelGroupName;
        public List<Property> usedProperties = new List<Property>();

        public virtual Runtime.GLTF SetModelAttributes()
        {
            var gltf = new Runtime.GLTF();

            return gltf;
        }

        public virtual glTFLoader.Schema.Gltf PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            return gltf;
        }
    }
}

using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class GLTF
    {
        public IEnumerable<Scene> Scenes { get; set; }
        public Scene Scene { get; set; }
        public IEnumerable<Animation> Animations { get; set; }
        public IEnumerable<string> ExtensionsUsed { get; set; }
        public IEnumerable<string> ExtensionsRequired { get; set; }
        public Asset Asset { get; set; }
    }
}

using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class Scene
    {
        public string Name { get; set; }
        public IEnumerable<Node> Nodes { get; set; }
    }
}

using System.Collections.Generic;

namespace AssetGenerator
{
    public class Manifest
    {
        public string folder;
        public List<string> files = new List<string>();

        public Manifest(ModelGroupName name)
        {
            folder = name.ToString();
        }
    }
}

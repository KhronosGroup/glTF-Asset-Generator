using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    public class Extras: glTFLoader.Schema.Extras
    {
        public string Attributes { get; set; }
    }
}

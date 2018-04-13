using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class initializeModelGroup
    {
        public string modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty;
        public List<Property> specialProperties;
        public List<List<Property>> combos = new List<List<Property>>();
        public List<Runtime.Image> usedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> usedFigures = new List<Runtime.Image>();
        public int id = -1;
        public bool noSampleImages = false;
    }
}

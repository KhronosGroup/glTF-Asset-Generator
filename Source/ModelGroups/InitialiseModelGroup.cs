using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.ModelGroups
{
    class InitialiseModelGroup
    {
        public ModelGroupName modelGroupName;
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

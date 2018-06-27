using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal class Skin
    {
        /// <summary>
        /// Name of the skeleton
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of nodes used as joints to animate the skin
        /// </summary>
        public IList<VertexJoint> VertexJoints { get; set; }

        public Skin(IList<VertexJoint> vertexJoints)
        {
            VertexJoints = vertexJoints;
        }
    }
}

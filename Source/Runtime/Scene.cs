﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Scene
    /// </summary>
    internal class Scene
    {
        /// <summary>
        /// List of nodes in the scene
        /// </summary>
        public List<Runtime.Node> Nodes { get; set; }
        /// <summary>
        /// List of meshes in the scene
        /// </summary>
        //public List<Runtime.Mesh> Meshes { get; set; }

        /// <summary>
        /// The user-defined name of the scene
        /// </summary>
        public string Name { get; set; }
        public Scene()
        {
            Nodes = new List<Runtime.Node>();
           // Meshes = new List<Runtime.Mesh>();
        }
        /// <summary>
        /// Adds a GLTFMesh to the scene
        /// </summary>
        /// <param name="mesh"></param>
    //    public void AddMesh(Runtime.Mesh mesh) { Meshes.Add(mesh); }


    }
}

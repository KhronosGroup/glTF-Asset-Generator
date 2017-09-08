using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// GLTFWrapper class for abstracting the glTF Loader API
    /// </summary>
    public class GLTF
    {
        /// <summary>
        /// List of scenes in the gltf wrapper
        /// </summary>
        public List<Runtime.Scene> Scenes { get; set; }
        /// <summary>
        /// index of the main scene
        /// </summary>
        public int? MainScene { get; set; }
        /// <summary>
        /// Initializes the gltf wrapper
        /// </summary>
        public GLTF()
        {
            Scenes = new List<Runtime.Scene>();
        }

        /// <summary>
        /// converts the wrapper data into a gltf loader object. 
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        /// <returns>Returns a gltf object</returns>
        public void BuildGLTF(ref glTFLoader.Schema.Gltf gltf, Data geometryData)
        {
            // local variables for generating gltf indices
            List<glTFLoader.Schema.Buffer> buffers = new List<glTFLoader.Schema.Buffer>();
            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Node> nodes = new List<glTFLoader.Schema.Node>();
            List<glTFLoader.Schema.Scene> scenes = new List<glTFLoader.Schema.Scene>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Mesh> meshes = new List<glTFLoader.Schema.Mesh>();

            glTFLoader.Schema.Buffer gBuffer = new glTFLoader.Schema.Buffer
            {
                Uri = geometryData.Name,
            };
            int buffer_index = 0;
            int buffer_offset = 0;


            // for each scene, create a node for each mesh and compute the indices for the scene object
            foreach (Runtime.Scene gscene in Scenes)
            {
                List<int> scene_indices_set = new List<int>();
                // loops through each mesh and converts it into a Node, with optional transformation info if available
                for (int mesh_index = 0; mesh_index < gscene.Meshes.Count(); ++mesh_index)
                {
                    Runtime.Mesh gMesh = gscene.Meshes[mesh_index];

                    glTFLoader.Schema.Mesh m = gMesh.ConvertToMesh(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref gBuffer, buffer_index, buffer_offset);
                    meshes.Add(m);

                    glTFLoader.Schema.Node node = new glTFLoader.Schema.Node
                    {
                        Mesh = meshes.Count() - 1
                    };
                    // handle node level mesh transformations
                    if (gMesh.TransformationMatrix != null)
                    {
                        node.Matrix = gMesh.TransformationMatrix.ToArray();
                    }
                    if (gMesh.Translation.HasValue)
                    {
                        node.Translation = gMesh.Translation.Value.ToArray();
                    }
                    if (gMesh.Rotation != null)
                    {
                        node.Rotation = gMesh.Rotation.ToArray();
                    }
                    if (gMesh.Scale.HasValue)
                    {
                        node.Scale = gMesh.Scale.Value.ToArray();
                    }
                    nodes.Add(node);
                    // stores index into the scene indices
                    scene_indices_set.Add(nodes.Count() - 1);
                }

                scenes.Add(new glTFLoader.Schema.Scene
                {
                    Nodes = scene_indices_set.ToArray()
                });
            }
            if (scenes != null && scenes.Count > 0)
            {
                gltf.Scenes = scenes.ToArray();
                gltf.Scene = 0;
            }
            
            if (meshes != null && meshes.Count > 0)
            {
                gltf.Meshes = meshes.ToArray();
            }
            if (materials != null && materials.Count > 0)
            {
                gltf.Materials = materials.ToArray();
            }
            if (accessors != null && accessors.Count > 0)
            {
                gltf.Accessors = accessors.ToArray();
            }
            if (bufferViews != null && bufferViews.Count > 0)
            {
                gltf.BufferViews = bufferViews.ToArray();
            }

            gltf.Buffers = new[] { gBuffer };
            if (nodes != null && nodes.Count > 0)
            {
                gltf.Nodes = nodes.ToArray();
            }

            if (images.Count > 0)
            {
                gltf.Images = images.ToArray();

            }
            if (textures.Count > 0)
            {
                gltf.Textures = textures.ToArray();
            }
            if (samplers.Count > 0)
            {
                gltf.Samplers = samplers.ToArray();
            }
            if (MainScene.HasValue)
            {
                gltf.Scene = MainScene.Value;
            }
        }
    }
}

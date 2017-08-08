using glTFLoader.Schema;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Common
    {
        public static void SingleTriangle(Gltf gltf, Data geometryData)
        {
            var positions = new[]
            {
                new Vector3( 0.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
            };

            geometryData.Writer.Write(positions);

            gltf.Buffers = new[]
            {
                new Buffer
                {
                    Uri = geometryData.Name,
                    ByteLength = sizeof(float) * 3 * positions.Length,
                }
            };

            gltf.BufferViews = new[]
            {
                new BufferView
                {
                    Buffer = 0,
                    ByteLength = sizeof(float) * 3 * positions.Length,
                }
            };

            gltf.Accessors = new[]
            {
                new Accessor
                {
                    BufferView = 0,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = positions.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 1.0f, 1.0f, 0.0f },
                    Min = new[] { 0.0f, 0.0f, 0.0f },
                }
            };

            gltf.Meshes = new[]
            {
                new Mesh
                {
                    Primitives = new[]
                    {
                        new MeshPrimitive
                        {
                            Attributes = new Dictionary<string, int>
                            {
                                { "POSITION", 0 },
                            }
                        }
                    },
                }
            };

            gltf.Nodes = new[]
            {
                new Node
                {
                    Mesh = 0
                }
            };

            gltf.Scenes = new[]
            {
                new Scene
                {
                    Nodes = new[] { 0 }
                }
            };

            gltf.Scene = 0;
        }
    }
}

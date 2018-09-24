using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreateTriangle()
            {
                return new List<Runtime.Node>
                {
                    new Runtime.Node
                    {
                        Name = "triangle",
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new[]
                            {
                                new Runtime.MeshPrimitive
                                {
                                    Positions = new List<Vector3>()
                                    {
                                        new Vector3(-0.2f, -0.1f, 0.6f),
                                        new Vector3( 0.2f, -0.1f, 0.6f),
                                        new Vector3( 0.0f, -0.1f, 0.8f),
                                    },
                                    Indices = new List<int>
                                    {
                                        0, 1, 2,
                                    },
                                    Colors = new List<Vector4>()
                                    {
                                        new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
                                        new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
                                        new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
                                    },
                                    Material = new Runtime.Material
                                    {
                                        DoubleSided = true
                                    }
                                }
                            }
                        },
                    }
                };
            }
        }
    }
}

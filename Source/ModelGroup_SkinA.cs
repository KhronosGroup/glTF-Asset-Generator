using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinA()
            {
                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin(),
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                Positions = new List<Vector3>()
                                {
                                    new Vector3(-0.25f, -0.5f,-0.5f),
                                    new Vector3( 0.25f, -0.5f,-0.5f),
                                    new Vector3(-0.25f, -0.5f, 0.0f),
                                    new Vector3( 0.25f, -0.5f, 0.0f),
                                    new Vector3(-0.25f, -0.5f, 0.5f),
                                    new Vector3( 0.25f, -0.5f, 0.5f),
                                },
                                Indices = new List<int>
                                {
                                    0, 1, 2,
                                    2, 1, 3,
                                    2, 3, 4,
                                    4, 3, 5
                                },
                                Colors = new List<Vector4>()
                                {
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                },
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true
                                }
                            }
                        }
                    },
                };

                Matrix4x4 rotation = Matrix4x4.CreateRotationX(-FloatMath.Pi / 2);
                var joint0Matrix = Matrix4x4.Multiply(rotation, Matrix4x4.CreateTranslation(new Vector3(0, -0.5f, -0.5f)));
                var joint1Translation = new Vector3(0.0f, 0.0f, 0.5f);
                var joint1Matrix = Matrix4x4.Multiply(joint0Matrix, Matrix4x4.CreateTranslation(joint1Translation));
                Matrix4x4 joint0Invert;
                Matrix4x4 joint1Invert;
                Matrix4x4.Invert(joint0Matrix, out joint0Invert);
                Matrix4x4.Invert(joint1Matrix, out joint1Invert);

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = joint1Translation,
                };

                var nodeJoint0 = new Runtime.Node
                {
                    Name = "Joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(rotation),
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint0 = new Runtime.SkinJoint
                (
                    //inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.5f, 0.0f)),//joint0Invert,
                    inverseBindMatrix: joint0Invert,
                    node: nodeJoint0
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: joint1Invert,
                    node: nodeJoint1
                );
                nodePlane.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1
                };

                var weightsList = new List<List<Runtime.JointWeight>>();
                for (int vertexIndex = 0; vertexIndex < 2; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = joint0,
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint1,
                            Weight = 0,
                        },
                    });
                }
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = joint0,
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint1,
                            Weight = 1,
                        },
                    });
                }
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = weightsList;

                return new List<Runtime.Node>
                {
                    nodePlane,
                    nodeJoint0
                };
            }
        }
    }
}

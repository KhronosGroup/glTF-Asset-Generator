using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinB()
            {
                var nodeInnerPrism = new Runtime.Node
                {
                    Name = "innerPrism",
                    Skin = new Runtime.Skin(),
                    Mesh = Mesh.CreatePrism(new Vector4(0.8f, 0.8f, 0.8f, 0.8f)),
                };

                var nodeOuterPrism = new Runtime.Node
                {
                    Name = "outerPrism",
                    Skin = new Runtime.Skin(),
                    Mesh = Mesh.CreatePrism(new Vector4(0.3f, 0.3f, 0.3f, 0.3f), new Vector3(1.6f, 1.6f, 0.6f)),
                };

                Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(15 * FloatMath.Pi / 180, 120 * FloatMath.Pi / 180, 0.0f);
                var translationVectorJoint1 = new Vector3(0.0f, 0.0f, -0.6f);
                var matrixJoint1 = Matrix4x4.CreateTranslation(translationVectorJoint1);

                Matrix4x4 invertedJoint1;
                Matrix4x4.Invert(matrixJoint1, out invertedJoint1);

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = translationVectorJoint1,
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

                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint1,
                    node: nodeJoint1
                );
                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.Identity,
                    node: nodeJoint0
                );

                nodeInnerPrism.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                };
                nodeOuterPrism.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                };

                var weightsListInnerPrism = new List<List<Runtime.JointWeight>>();
                var weightsListOuterPrism = new List<List<Runtime.JointWeight>>();
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
                    var weight = new List<Runtime.JointWeight>()
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
                    };
                    weightsListInnerPrism.Add(weight);
                    weightsListOuterPrism.Add(weight);
                }
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
                    var weight = new List<Runtime.JointWeight>()
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
                    };
                    weightsListInnerPrism.Add(weight);
                    weightsListOuterPrism.Add(weight);
                }
                nodeInnerPrism.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListInnerPrism;
                nodeOuterPrism.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListOuterPrism;

                return new List<Runtime.Node>
                {
                    nodeInnerPrism,
                    nodeJoint0,
                    nodeOuterPrism
                };
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinB()
            {
                var colorInner = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                var colorOuter = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);

                Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f);
                var translationVectorJoint1 = new Vector3(0.0f, 0.0f, -0.6f);
                var translationVectorJoint0 = new Vector3(0.0f, 0.0f, 0.3f);
                Matrix4x4 matrixJoint1 = Matrix4x4.CreateTranslation(translationVectorJoint1);
                Matrix4x4 matrixJoint0 = Matrix4x4.CreateTranslation(translationVectorJoint0);

                matrixJoint1 = Matrix4x4.Multiply(matrixJoint0, matrixJoint1);
                Matrix4x4.Invert(matrixJoint1, out Matrix4x4 invertedJoint1);
                Matrix4x4.Invert(matrixJoint0, out Matrix4x4 invertedJoint0);

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "joint1",
                    Translation = translationVectorJoint1,
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(rotation),
                    Translation = new Vector3(0.0f, -0.3f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var jointsList = new List<Runtime.Node>
                {
                    nodeJoint0,
                    nodeJoint1
                };
                var inverseBindMatricesList = new List<Matrix4x4>
                {
                    invertedJoint0,
                    invertedJoint1
                };
                var skin = new Runtime.Skin
                {
                    Joints = jointsList,
                    InverseBindMatrices = inverseBindMatricesList
                };

                var nodeInnerPrism = new Runtime.Node
                {
                    Name = "innerPrism",
                    Skin = skin,
                    Mesh = Mesh.CreatePrism(colorInner),
                };

                var nodeOuterPrism = new Runtime.Node
                {
                    Name = "outerPrism",
                    Skin = skin,
                    Mesh = Mesh.CreatePrism(colorOuter, scale: new Vector3(1.6f, 1.6f, 0.3f)),
                };

                var weightsListInnerPrism = new List<List<Runtime.JointWeight>>();
                var weightsListOuterPrism = new List<List<Runtime.JointWeight>>();
                for (var i = 0; i < 3; i++)
                {
                    var weight = new List<Runtime.JointWeight>
                    {
                        new Runtime.JointWeight
                        {
                            JointIndex = 0,
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            JointIndex = 1,
                            Weight = 0,
                        },
                    };
                    weightsListInnerPrism.Add(weight);
                    weightsListOuterPrism.Add(weight);
                }
                for (var i = 0; i < 3; i++)
                {
                    var weight = new List<Runtime.JointWeight>
                    {
                        new Runtime.JointWeight
                        {
                            JointIndex = 0,
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            JointIndex = 1,
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

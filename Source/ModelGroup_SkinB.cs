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
                var nodeOuterPrism = new Runtime.Node
                {
                    Name = "innerPrism",
                    Skin = new Runtime.Skin(),
                    Mesh = Mesh.CreatePrism(),
                };

                var nodeInnerPrism = new Runtime.Node
                {
                    Name = "outerPrism",
                    Skin = new Runtime.Skin(),
                    Mesh = Mesh.CreatePrism(new Vector3(1.3f, 1.3f, 0.8f)),
                };

                Matrix4x4 rotation = Matrix4x4.CreateRotationX(FloatMath.Pi / 2);
                var translationVectorJoint1 = new Vector3(0.0f, 0.0f, -0.2f);
                var translationVectorJoint0 = new Vector3(0.0f, -0.1f, 0.0f);
                var inverseBindJoint1 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.2f));
                var inverseBindJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -0.2f));

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = translationVectorJoint1,
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "Joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(rotation),
                    Translation = translationVectorJoint0,
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: inverseBindJoint1,
                    node: nodeJoint1
                );
                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: inverseBindJoint0,
                    node: nodeJoint0
                );

                nodeOuterPrism.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                };
                nodeInnerPrism.Skin.SkinJoints = new[]
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
                nodeOuterPrism.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListInnerPrism;
                nodeInnerPrism.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListOuterPrism;

                return new List<Runtime.Node>
                {
                    nodeOuterPrism,
                    nodeJoint0,
                    nodeInnerPrism
                };
            }
        }
    }
}

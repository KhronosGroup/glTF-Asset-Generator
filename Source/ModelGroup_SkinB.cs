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

                var skinJoints = new[]
                {
                    nodeJoint0,
                    nodeJoint1
                };
                var inverseBindMatrices = Runtime.Data.Create(new List<Matrix4x4>
                {
                    invertedJoint0,
                    invertedJoint1
                });
                var innerSkin = new Runtime.Skin
                {
                    Joints = skinJoints,
                    InverseBindMatrices = inverseBindMatrices
                };
                var outerSkin = new Runtime.Skin
                {
                    Joints = skinJoints,
                    InverseBindMatrices = inverseBindMatrices
                };

                var nodeInnerPrism = new Runtime.Node
                {
                    Name = "innerPrism",
                    Skin = innerSkin,
                    Mesh = Mesh.CreatePrism(colorInner),
                };

                var nodeOuterPrism = new Runtime.Node
                {
                    Name = "outerPrism",
                    Skin = outerSkin,
                    Mesh = Mesh.CreatePrism(colorOuter, scale: new Vector3(1.6f, 1.6f, 0.3f)),
                };

                var joints = new List<Runtime.JointVector>();
                var weights = new List<Runtime.WeightVector>();
                for (var i = 0; i < 3; i++)
                {
                    joints.Add(new Runtime.JointVector(0, 0));
                    weights.Add(new Runtime.WeightVector(1.0f, 0.0f));
                }
                for (var i = 0; i < 3; i++)
                {
                    joints.Add(new Runtime.JointVector(0, 1));
                    weights.Add(new Runtime.WeightVector(0.0f, 1.0f));
                }
                nodeInnerPrism.Mesh.MeshPrimitives.First().Joints = Runtime.Data.Create(joints, Runtime.DataType.UnsignedShort);
                nodeInnerPrism.Mesh.MeshPrimitives.First().Weights = Runtime.Data.Create(weights);
                nodeOuterPrism.Mesh.MeshPrimitives.First().Joints = Runtime.Data.Create(joints, Runtime.DataType.UnsignedShort);
                nodeOuterPrism.Mesh.MeshPrimitives.First().Weights = Runtime.Data.Create(weights);

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

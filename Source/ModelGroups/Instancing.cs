using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Instancing : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Instancing;

        public Instancing(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            var normalImage = UseTexture(imageList, "Normal_Plane");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.GLTF, Runtime.Node, Runtime.Node, Runtime.MeshPrimitive, Runtime.MeshPrimitive, Runtime.Material, Runtime.Material, Runtime.PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                var gltf = CreateGLTF(() => new Runtime.Scene());
                var nodeZero = new Runtime.Node()
                {
                    Mesh = new Runtime.Mesh()
                };
                var nodeOne = new Runtime.Node()
                {
                    Mesh = new Runtime.Mesh()
                };
                var meshPrimitiveZero = MeshPrimitive.CreateLeftPrimitiveTriangle(includeTextureCoords: false);
                var meshPrimitiveOne = MeshPrimitive.CreateRightPrimitiveTriangle(includeTextureCoords: false);
                var materialZero = new Runtime.Material();
                var materialOne = new Runtime.Material();
                var metallicRoughness = new Runtime.PbrMetallicRoughness()
                {
                    BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage }
                };

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf, nodeZero, nodeOne, meshPrimitiveZero, meshPrimitiveOne, materialZero, materialOne, metallicRoughness);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                };
            }

            void SetInstancedMesh(List<Property> properties, Runtime.GLTF gltf, Runtime.Node nodeZero, Runtime.Node nodeOne, Runtime.MeshPrimitive meshPrimitiveZero)
            {
                nodeZero.Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                nodeOne.Translation = new Vector3(0.5f, 0.0f, 0.0f);

                nodeZero.Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    meshPrimitiveZero
                };
                nodeOne.Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    meshPrimitiveZero
                };

                gltf.Scenes[0].Nodes = new List<Runtime.Node>
                {
                    nodeZero,
                    nodeOne,
                };

                properties.Add(new Property(PropertyName.InstancedMesh, ":white_check_mark:"));
            }

            void SetInstancedMaterial(List<Property> properties, Runtime.GLTF gltf, Runtime.Node nodeZero, Runtime.MeshPrimitive meshPrimitiveZero, Runtime.MeshPrimitive meshPrimitiveOne, Runtime.Material materialZero)
            {
                // Normals are set so that the material isn't empty.
                materialZero.NormalTexture = new Runtime.Texture { Source = normalImage };
                meshPrimitiveZero.TextureCoordSets = new List<List<Vector2>> { MeshPrimitive.GetLeftPrimitiveTriangleTextureCoordSets() };
                meshPrimitiveOne.TextureCoordSets = new List<List<Vector2>> { MeshPrimitive.GetRightPrimitiveTriangleTextureCoordSets() };

                meshPrimitiveZero.Material = materialZero;
                meshPrimitiveOne.Material = materialZero;

                nodeZero.Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    meshPrimitiveZero,
                    meshPrimitiveOne,
                };

                gltf.Scenes[0].Nodes = new List<Runtime.Node>
                {
                    nodeZero,
                };

                properties.Add(new Property(PropertyName.InstancedMaterial, ":white_check_mark:"));
            }

            void SetInstancedTexture(List<Property> properties, Runtime.GLTF gltf, Runtime.Node nodeZero, Runtime.MeshPrimitive meshPrimitiveZero, Runtime.MeshPrimitive meshPrimitiveOne, Runtime.Material materialZero, Runtime.Material materialOne, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                // Normal texture is set on one material to make it unique
                materialZero.NormalTexture = new Runtime.Texture { Source = normalImage };
                meshPrimitiveZero.TextureCoordSets = new List<List<Vector2>> { MeshPrimitive.GetLeftPrimitiveTriangleTextureCoordSets() };

                materialZero.MetallicRoughnessMaterial = metallicRoughness;
                materialOne.MetallicRoughnessMaterial = metallicRoughness;

                meshPrimitiveZero.Material = materialZero;
                meshPrimitiveOne.Material = materialOne;
                meshPrimitiveZero.TextureCoordSets = new List<List<Vector2>> { MeshPrimitive.GetLeftPrimitiveTriangleTextureCoordSets() };
                meshPrimitiveOne.TextureCoordSets = new List<List<Vector2>> { MeshPrimitive.GetRightPrimitiveTriangleTextureCoordSets() };

                nodeZero.Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    meshPrimitiveZero,
                    meshPrimitiveOne,
                };

                gltf.Scenes[0].Nodes = new List<Runtime.Node>
                {
                    nodeZero,
                };

                properties.Add(new Property(PropertyName.InstancedTexture, ":white_check_mark:"));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf, nodeZero, nodeOne, meshPrimitiveZero, meshPrimitiveOne, materialZero, materialOne, metallicRoughness) => {
                    SetInstancedMesh(properties, gltf, nodeZero, nodeOne, meshPrimitiveZero);
                }),
                CreateModel((properties, gltf, nodeZero, nodeOne, meshPrimitiveZero, meshPrimitiveOne, materialZero, materialOne, metallicRoughness) => {
                    SetInstancedMaterial(properties, gltf, nodeZero, meshPrimitiveZero, meshPrimitiveOne, materialZero);
                }),
                CreateModel((properties, gltf, nodeZero, nodeOne, meshPrimitiveZero, meshPrimitiveOne, materialZero, materialOne, metallicRoughness) => {
                    SetInstancedTexture(properties, gltf, nodeZero, meshPrimitiveZero, meshPrimitiveOne, materialZero, materialOne, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}

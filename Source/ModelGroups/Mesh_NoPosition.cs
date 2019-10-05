using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator.ModelGroups
{
    internal class Mesh_NoPosition : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_NoPosition;

        public Mesh_NoPosition(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            NoSampleImages = true;

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                Runtime.Mesh mesh = Mesh.CreateTriangle(includeMaterial: false, includeIndices: false, includePositions: false, includeNormals: true);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, mesh.MeshPrimitives.ElementAt(0));

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
                    {
                        Nodes = new List<Runtime.Node>
                        {
                            new Runtime.Node
                            {
                                Mesh = mesh
                            },
                        },
                    }),
                    Loadable = null
                };
            }

            Models = new List<Model>
            {

                CreateModel((properties, meshPrimitive) =>
                {
                    properties.Add(new Property(PropertyName.VertexPosition, ":x:"));
                    properties.Add(new Property(PropertyName.VertexNormal, ":white_check_mark:"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Indices = Runtime.Data.Create(Mesh.GetTriangleIndices());

                    properties.Add(new Property(PropertyName.VertexPosition, ":x:"));
                    properties.Add(new Property(PropertyName.IndicesValues, "[0, 1, 2]"));
                    properties.Add(new Property(PropertyName.VertexNormal, ":white_check_mark:"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}

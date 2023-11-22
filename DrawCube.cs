using Rubik_s_Cube.CubeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik_s_Cube
{
    static class DrawCube
    {
        public static ModelVisual3D CreateCube(Cube cube)
        {
            Model3DGroup modelGroup = new();

            // Create 3D visual for the cube
            ModelVisual3D modelVisual = new();

            for (int x = 0; x < cube.Size; x++)
            {
                for (int y = 0; y < cube.Size; y++)
                {
                    for (int z = 0; z < cube.Size; z++)
                    {
                        CreateParticle(new(x, y, z));
                    }
                }

            }

            // Create a particle with each face having a different material
            void CreateParticle(Vector3D locationInCube)
            {
                Side[] directions = (Side[])Enum.GetValues(typeof(Side));
                foreach (Side direction in directions)
                {
                    MeshGeometry3D particleMesh = CreateParticleFace(locationInCube, direction, cube.Size);
                    Material material = new DiffuseMaterial(new SolidColorBrush(DetermineParticleFaceColor(cube.Particles[(int)locationInCube.X, (int)locationInCube.Y, (int)locationInCube.Z], direction, cube.Size)));
                    GeometryModel3D geometryModel3D = new()
                    {
                        Geometry = particleMesh,
                        Material = material,
                        BackMaterial = material
                    };
                    GeometryModel3D cubeModel = geometryModel3D;
                    modelGroup.Children.Add(cubeModel);
                }
            }

            // Set the cube models in the visual
            modelVisual.Content = modelGroup;

            return modelVisual;
        }

        private static MeshGeometry3D CreateParticleFace(Vector3D location, Side face, int cubeSize)
        {
            MeshGeometry3D mesh = new();

            Vector2D[] meshPoints = { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };

            float sizeOfParticle = 0.96f;
            float marginOfParticle = (1 - sizeOfParticle) / 2;

            CreatePoints(face);


            void CreatePoints(Side side)
            {
                Axis ignoredAxis = Utils3D.GetAxis(face);
                float constant = -1;
                switch (side)
                {
                    case Side.Top:
                        {
                            constant = location.Y + marginOfParticle + sizeOfParticle; break;
                        }
                    case Side.Bottom:
                        {
                            constant = location.Y + marginOfParticle; break;
                        }
                    case Side.Left:
                        {
                            constant = location.X + marginOfParticle; break;
                        }
                    case Side.Right:
                        {
                            constant = location.X + marginOfParticle + sizeOfParticle; break;
                        }
                    case Side.Front:
                        {
                            constant = location.Z + marginOfParticle + sizeOfParticle; break;
                        }
                    case Side.Back:
                        {
                            constant = location.Z + marginOfParticle; break;
                        }

                }

                Vector2D location2D = location.To2D(ignoredAxis);

                for (int i = 0; i < meshPoints.Length; i++)
                {
                    Vector3D location3D = new Vector2D(location2D.X + marginOfParticle + meshPoints[i].X * sizeOfParticle, location2D.Y + marginOfParticle + meshPoints[i].Y * sizeOfParticle).To3D(ignoredAxis, constant);

                    //offset used to centralize the cube around the zero point of all axis, rather than Starting at the zero point:
                    float offset = cubeSize / 2f;

                    mesh.Positions.Add(new Point3D(location3D.X - offset, location3D.Y - offset, location3D.Z - offset));
                }
            }

            // Define the triangles for the face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            return mesh;
        }

        private static Color DetermineParticleFaceColor(CubeParticle particle, Side face, int cubeSize)//determine color Todo!!!!!!!!!!!!!!!!!
        {
            if(face == particle.Orientation.Y)
            {
                if(particle.Origin.Y == cubeSize - 1)
                {
                    return CubeDrawingInfo.Top;
                }
                else
                {
                    return Colors.Black;
                }
            }
            if (face == Utils3D.GetOposition(particle.Orientation.Y))
            {
                if (particle.Origin.Y == 0)
                {
                    return CubeDrawingInfo.Bottom;
                }
                else
                {
                    return Colors.Black;
                }
            }
            if (face == particle.Orientation.X)
            {
                if (particle.Origin.X == cubeSize - 1)
                {
                    return CubeDrawingInfo.Right;
                }
                else
                {
                    return Colors.Black;
                }
            }
            if (face == Utils3D.GetOposition(particle.Orientation.X))
            {
                if (particle.Origin.X == 0)
                {
                    return CubeDrawingInfo.Left;
                }
                else
                {
                    return Colors.Black;
                }
            }
            if (face == particle.Orientation.Z)
            {
                if (particle.Origin.Z == cubeSize - 1)
                {
                    return CubeDrawingInfo.Front;
                }
                else
                {
                    return Colors.Black;
                }
            }
            if (face == Utils3D.GetOposition(particle.Orientation.Z))
            {
                if (particle.Origin.Z == 0)
                {
                    return CubeDrawingInfo.Back;
                }
                else
                {
                    return Colors.Black;
                }
            }

            throw new("Not existing side was specified.");
        }
    }
}

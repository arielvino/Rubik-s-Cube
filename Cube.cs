using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rubik_s_Cube.CubeModel

{
    class Cube
    {
        public int Size { get; } = 3;
        public CubeParticle[,,] Particles { get; private set; }

        public Cube(int size)
        {
            this.Size = size;
            Particles = new CubeParticle[size, size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        //if the particle is on the surface of the cube:
                        if (true)//x == 0 || x == size - 1 || y == 0 || y == size - 1 || z == 0 || z == size - 1)
                        {
                            Particles[x, y, z] = new CubeParticle(new Vector3D(x, y, z));
                        }
                    }
                }
            }
        }

        public void RotateFace(Axis axis, int layer, bool toHigherAxis)
        {
            //verify that the layer exist:
            if (layer < 0 || layer >= Size)
            {
                throw new ArgumentOutOfRangeException("Illegal layer was specified.");
            }

            //Create a copy of the new layer:
            CubeParticle[,] face = new CubeParticle[Size, Size];

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Vector3D location = new Vector2D(x, y).To3D(axis, layer);

                    //update the orientation of the particle:
                    Particles[(int)location.X, (int)location.Y, (int)location.Z].Orientation = Rotated(Particles[(int)location.X, (int)location.Y, (int)location.Z].Orientation, axis, toHigherAxis);

                    //save the new location of the particle to a temporary array:
                    Vector2D newLocation = RotatedRelativeLocation(location, axis, toHigherAxis);
                    face[(int)newLocation.X, (int)newLocation.Y] = Particles[(int)location.X, (int)location.Y, (int)location.Z];
                }
            }

            //update the location of the particles on the cube:
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Vector3D newLocation = new Vector2D(x, y).To3D(axis, layer);
                    Particles[(int)newLocation.X, (int)newLocation.Y, (int)newLocation.Z] = face[x, y];
                }
            }
        }

        public void Shuffle()
        {
            Shuffle((int)Math.Pow(Size, 3));
        }
        public void Shuffle(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                RotateRandomally();
            }
        }

        public void RotateRandomally()
        {
            bool toHigherAxis = new Random().Next(2) % 2 == 0;
            int layer = new Random().Next(Size);
            Axis axis = (Axis)Enum.GetValues(typeof(Axis)).GetValue(new Random().Next(3));

            RotateFace(axis, layer, toHigherAxis);
        }

        private Vector2D RotatedRelativeLocation(Vector3D current, Axis rotatedAroundAxis, bool toHigherAxis)
        {
            Vector2D shortCurrent = current.To2D(rotatedAroundAxis);
            if (toHigherAxis)
            {
                return new Vector2D(Size - 1 - shortCurrent.Y, shortCurrent.X);
            }
            else
            {
                return new Vector2D(shortCurrent.Y, Size - 1 - shortCurrent.X);
            }
        }

        private static Orientation Rotated(Orientation origin, Axis axis, bool toHigherAxis)
        {
            Axis internalRotatedAxis = origin.GetInternalAxisByGlobalAxis(axis);

            return internalRotatedAxis switch
            {
                Axis.X => new Orientation(origin.X, NewDirection(origin.Y, axis, toHigherAxis), NewDirection(origin.Z, axis, toHigherAxis)),
                Axis.Y => new Orientation(NewDirection(origin.X, axis, toHigherAxis), origin.Y, NewDirection(origin.Z, axis, toHigherAxis)),
                Axis.Z => new Orientation(NewDirection(origin.X, axis, toHigherAxis), NewDirection(origin.Y, axis, toHigherAxis), origin.Z),
                _ => throw new("No such axis."),
            };
        }

        /// <summary>
        /// This method tell you which direction will you face after rotation, depending on the current direction you are facing, the axis being rotated and the rotation's direction.
        /// </summary>
        /// <param name="current">The direction that currently faced.</param>
        /// <param name="rotatedAxis">The axis that you plan to rotate.</param>
        /// <param name="toHigher">The direction of the rotattion. X toward Y toward Z. </param>
        /// <returns></returns>
        private static Side NewDirection(Side current, Axis rotatedAxis, bool toHigher)
        {
            const int facesAroundAxis = 4;


            //define the direction that you face when you rotate around an axis, ordered by to-higher-axis rotate direction:
            Side[] xDir = { Side.Top, Side.Front, Side.Bottom, Side.Back};
            Side[] yDir = { Side.Right, Side.Front, Side.Left, Side.Back};
            Side[] zDir = { Side.Right, Side.Top, Side.Left, Side.Bottom };


            //choose which array to use, depending on the axis:
            Side[] directions = null;
            switch (rotatedAxis)
            {
                case Axis.X:
                    directions = xDir;
                    break;
                case Axis.Y:
                    directions = yDir;
                    break;
                case Axis.Z:
                    directions = zDir;
                    break;
            }

            //find index of current face:
            int index = -1;
            for (int i = 0; i < facesAroundAxis; i++)
            {
                if (current == directions[i])
                {
                    index = i;
                }
            }

            //update it to be the next index in the array, according to the otation dirextion:
            if (toHigher)
            {
                index++;
            }
            else
            {
                index += facesAroundAxis - 1;
            }
            index %= facesAroundAxis;

            return directions[index];
        }
    }
}
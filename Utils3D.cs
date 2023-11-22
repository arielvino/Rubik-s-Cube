using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik_s_Cube
{
    static class Utils3D
    {
        public static Vector3D Up { get; } = new(0, 1, 0);
        public static Vector3D Down { get; } = new(0, -1, 0);
        public static Vector3D Left { get; } = new(-1, 0, 0);
        public static Vector3D Right { get; } = new(-1, 0, 0);
        public static Vector3D Back { get; } = new(0, 0, 1);
        public static Vector3D Front { get; } = new(0, 0, -1);

        public static Vector3D ToVector3D(Side side)
        {
            return side switch
            {
                Side.Top => Up,
                Side.Bottom => Down,
                Side.Left => Left,
                Side.Right => Right,
                Side.Front => Front,
                Side.Back => Back,
                _ => throw new("There is a problem with the specified side."),
            };
        }

        public static Side ToSide(Vector3D vector)
        {
            if (vector == Up)
            {
                return Side.Top;
            }
            if (vector == Down)
            {
                return Side.Bottom;
            }
            if (vector == Left)
            {
                return Side.Left;
            }
            if(vector == Right)
            {
                return Side.Right;
            }
            if(vector == Front)
            {
                return Side.Front;
            }
            if(vector == Back)
            {
                return Side.Back;
            }
            throw new("There is a problem with the specified vector.");
        }

        public static Axis GetAxis(Side side)
        {
            if(side == Side.Left || side == Side.Right)
            {
                return Axis.X;
            }
            if(side == Side.Top || side == Side.Bottom)
            {
                return Axis.Y;
            }
            if (side == Side.Front || side == Side.Back)
            {
                return Axis.Z;
            }
            throw new("No such side.");
        }

        public static Side GetOposition(Side side)
        {
            return side switch
            {
                Side.Top => Side.Bottom,
                Side.Bottom => Side.Top,
                Side.Left => Side.Right,
                Side.Right => Side.Left,
                Side.Front => Side.Back,
                Side.Back => Side.Front,
                _ => throw new("No such Side."),
            };
        }
        
        public static bool IsHigherIndeed(Axis higher, Axis lower)
        {
            if(higher == lower)
            {
                throw new("The axes are identical.");
            }

            //find their indexes in the array, and compare them:
            Axis[] axes = { Axis.X, Axis.Y, Axis.Z };
            
            int h = -1, l = -1;

            for (int i = 0; i < axes.Length; i++)
            {
                if (axes[i] == higher)
                {
                    h = i;
                }
                if (axes[i] == lower)
                {
                    l = i;
                }
            }

            return h > l;
        }
    }
}

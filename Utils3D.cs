using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public static Vector3D Right { get; } = new(1, 0, 0);
        public static Vector3D Back { get; } = new(0, 0, -1);
        public static Vector3D Front { get; } = new(0, 0, 1);

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

    }
}

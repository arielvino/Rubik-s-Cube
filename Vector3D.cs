using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik_s_Cube
{
    readonly struct Vector3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public readonly Vector2D To2D(Axis ignoredAxis)
        {
            if(ignoredAxis == Axis.X)
            {
                return new(Y, Z);
            }
            if (ignoredAxis == Axis.Y)
            {
                return new(X, Z);
            }
            if (ignoredAxis == Axis.Z)
            {
                return new(X, Y);
            }
            throw new("The axis is not initialized.");
        }

        public static bool operator ==(Vector3D current, Vector3D other)
        {
            return current.X == other.X && current.Y == other.Y && current.Z == other.Z;
        }
        public static bool operator !=(Vector3D current, Vector3D other)
        {
            return !(current.X == other.X && current.Y == other.Y && current.Z == other.Z);
        }
    }
}
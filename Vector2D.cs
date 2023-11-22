using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik_s_Cube
{
    readonly struct Vector2D
    {
        public float X { get; }
        public float Y { get; }

        public Vector2D(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly Vector3D To3D(Axis ignoreAxis, float constantValueForIgnoredAxis)
        {
            if(ignoreAxis == Axis.X)
            {
                return new(constantValueForIgnoredAxis, X, Y);
            }
            if(ignoreAxis == Axis.Y)
            {
                return new(X, constantValueForIgnoredAxis, Y);
            }
            if(ignoreAxis == Axis.Z)
            {
                return new(X, Y, constantValueForIgnoredAxis);
            }
            throw new("Axis is not initialized.");
        }
    }
}

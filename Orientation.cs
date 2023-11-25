using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik_s_Cube
{
    readonly struct Orientation
    {
        public Side X
        {
            get;
        }
        public Side Y
        {
            get;
        }
        public Side Z
        {
            get;
        }

        public Orientation(Side x, Side y, Side z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Side GetValueOfInternalAxis(Axis internalAxis)
        {
            return internalAxis switch
            {
                Axis.X => X,
                Axis.Y => Y,
                Axis.Z => Z,
                _ => throw new("No such axis."),
            };
        }

        public Axis GetInternalAxisByGlobalAxis(Axis globalAxis)
        {
            if(globalAxis == Utils3D.GetAxis(X))
            {
                return Axis.X;
            }
            if (globalAxis == Utils3D.GetAxis(Y))
            {
                return Axis.Y;
            }
            if (globalAxis == Utils3D.GetAxis(Z))
            {
                return Axis.Z;
            }
            throw new("An error occured.");
        }

        public Side GetInternalSide(Side externalSide)
        {
            if(externalSide == X)
            {
                return Side.Right;
            }
            if(externalSide == Utils3D.GetOposition(X))
            {
                return Side.Left;
            }
            if(externalSide == Y)
            {
                return Side.Top;
            }
            if(externalSide == Utils3D.GetOposition(Y))
            {
                return Side.Bottom;
            }
            if(externalSide == Z)
            {
                return Side.Front;
            }
            if (externalSide == Utils3D.GetOposition(Z))
            {
                return Side.Back;
            }
            throw new("No such side.");
        }
    }
}
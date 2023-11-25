using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik_s_Cube
{
    class CubeParticle
    {
        public Vector3D Origin
        {
            get;
        }

        public Vector3D Location
        {
            get;
            set;
        }

        public Orientation Orientation
        {
            get;
            set;
        }

        public CubeParticle(Vector3D origin)
        {
            this.Origin = origin;
            this.Location = origin;
            this.Orientation = new(Side.Right, Side.Top, Side.Front);
        }

    }
}

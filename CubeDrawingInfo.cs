using Rubik_s_Cube.CubeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rubik_s_Cube
{
    static class CubeDrawingInfo
    {
        public static Color Top { get; } = Colors.Red;
        public static Color Bottom { get; } = Colors.Orange;
        public static Color Left{ get; } = Colors.White;
        public static Color Right { get; } = Colors.Yellow;
        public static Color Front { get; } = Colors.Blue;
        public static Color Back { get; } = Colors.Green;
    }
}

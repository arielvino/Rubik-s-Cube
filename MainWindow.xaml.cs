using Rubik_s_Cube.CubeModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik_s_Cube
{
    public partial class MainWindow : Window
    {
        Cube cube;
        int cubeSize = 3;
        Orientation currentOrientation = new(Side.Right, Side.Top, Side.Front);

        private void RotateView(Side direction)
        {
            if(direction == Side.Right)
            {
                currentOrientation = Rotated(currentOrientation, Axis.Y, true);
            }
            if(direction == Side.Left)
            {
                currentOrientation = Rotated(currentOrientation, Axis.Y, false);
            }
            if(direction == Side.Top)
            {
                currentOrientation = Rotated(currentOrientation, Axis.X, true);
            }
            if(direction == Side.Bottom)
            {
                currentOrientation = Rotated(currentOrientation, Axis.X, false);
            }

            UpdateCameraState();
            terminal.Focus();
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
            Side[] xDir = { Side.Top, Side.Front, Side.Bottom, Side.Back };
            Side[] yDir = { Side.Right, Side.Front, Side.Left, Side.Back };
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

        private void UpdateCameraState()
        {
            float distance = 15, sideDistance = 7.5f;

            Side x = currentOrientation.GetInternalSide(Side.Left);
            Side y = currentOrientation.GetInternalSide(Side.Top);
            Side z = currentOrientation.GetInternalSide(Side.Front);

            Vector3D position = (Utils3D.ToVector3D(x) * sideDistance) + (Utils3D.ToVector3D(y) * (distance)) + (Utils3D.ToVector3D(z) * distance);
            Vector3D upDirection = Utils3D.ToVector3D(y);

            camera.Position = new(position.X, position.Y, position.Z);//one should be 7.5 instead of 15
            camera.UpDirection = new(upDirection.X, upDirection.Y, upDirection.Z);
            camera.LookDirection = new(-position.X / distance, -position.Y / distance, -position.Z/distance);//one shoul be half
        }

        private void RoteateCorners()
        {
            cube.RotateFace(Axis.Y, cubeSize - 1, true);
            cube.RotateFace(Axis.X, cubeSize - 1, false);
            cube.RotateFace(Axis.Y, cubeSize - 1, false);
            cube.RotateFace(Axis.X, 0, false);
            cube.RotateFace(Axis.Y, cubeSize - 1, true);
            cube.RotateFace(Axis.X, cubeSize - 1, true);
            cube.RotateFace(Axis.Y, cubeSize - 1, false);
            cube.RotateFace(Axis.X, 0, true);
        }
        private void ChessCube()
        {
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                for (int i = 0; i < cube.Size; i += 2)
                {
                    for (int t = 0; t < 2; t++)
                    {
                        cube.RotateFace(axis, i, true);
                        RefreshCube();
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            cube = new(cubeSize);

            RefreshCube();
        }

        private void RefreshCube()
        {
            mainViewport.Children.Clear();
            mainViewport.Children.Add(DrawCube.CreateCube(cube));

            //light:
            Model3DGroup lightGroup = new();
            lightGroup.Children.Add(new AmbientLight(Colors.White));
            ModelVisual3D lightVisual = new()
            {
                Content = lightGroup
            };


            mainViewport.Children.Add(lightVisual);
            terminal.Focus();
        }

        private void CreateNewCube_Button(object sender, RoutedEventArgs e)
        {
            cube = new Cube(cubeSize);
            RefreshCube();
        }

        private void ShuffleCube_Button(object sender, RoutedEventArgs e)
        {
            cube.Shuffle();
            RefreshCube();
        }

        private void terminal_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //evaluate command:
            if(e.Key == Key.Enter)
            {
                string[] commands = terminal.Text.Split(" ");
                foreach (var command in commands)
                {
                    if(command != "")
                    {
                        ParseCommand(command);
                    }
                }
                terminal.Text = "";
            }

            //rotate cube:
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (e.Key == Key.W)
                {
                    Rotate(Side.Top);
                }
                if(e.Key == Key.S)
                {
                    Rotate(Side.Bottom);
                }
                if(e.Key == Key.A)
                {
                    Rotate(Side.Left);
                }
                if(e.Key == Key.D)
                {
                    Rotate(Side.Right);
                }

                void Rotate(Side side)
                {
                    RotateView(side);
                    e.Handled = true;
                }
            }
        }

        private void ParseCommand(string command)
        {
            int charIndex = 0;


            //parse side:
            string side = command.Substring(charIndex, 1);
            Side face;
            switch (side.ToLower())
            {
                case "u": face = Side.Top;break;
                case "d": face = Side.Bottom;break;
                case "l": face = Side.Left;break;
                case "r": face = Side.Right;break;
                case "f": face = Side.Front;break;
                case "b": face = Side.Back;break;
                default:
                    return;
            }
            Side actualSide = currentOrientation.GetInternalSide(face);//that should match the cube orientation even if the cube is rotated!!!!!!!!!!!!!!!!!!!
            /**
             * 
             */
            charIndex++;

            //parse layer:
            int numOfDigits = 0;//Warning: work only for one-digit size!!!!!
            for (int i = charIndex; i<command.Length; i++)
            {
                if (Char.IsDigit(command[i]))
                {
                    numOfDigits++;
                }
                else
                {
                    break;
                }
            }
            int layer = 0;
            if (numOfDigits > 0)
            {
                layer = Convert.ToInt16(command.Substring(charIndex, numOfDigits)) - 1;
            }
            if (layer < 0 || layer >= cubeSize)
            {
                return;
            }
            charIndex += numOfDigits;

            //parse direction - optional:
            bool clockwise = true;
            if (command.Length > charIndex)
            {
                if (command.Substring(charIndex, 1) == "'")
                {
                    clockwise = false;
                    charIndex++;
                }
            }

            //parse repeat - optional:
            int repeat = 1;
            if (command.Length > charIndex)
            {
                if (command.Substring(charIndex, 1) == "*")
                {
                    repeat = 2;
                    charIndex++;
                }
            }


            //perform:
            for (int i = 0; i < repeat; i++)
            {
                cube.RotateFace(actualSide, clockwise, layer);
            }
            RefreshCube();
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender == upButton)
            {
                RotateView(Side.Top);
            }
            if (sender == downButton)
            {
                RotateView(Side.Bottom);
            }
            if(sender == leftButton)
            {
                RotateView(Side.Left);
            }
            if(sender == rightButton)
            {
                RotateView(Side.Right);
            }
        }

        private void Guide_Click(object sender, RoutedEventArgs e)
        {
            String message = "Here is how it's work:" +
                "\r\n" +
                "\r\n" +
                "In order to rotate a face of the cube, you have to type commands in the terminal-line at the button of the window." +
                "\r\r" +
                "You can write a sequence of commands, separated by one or more empty spaces. The command(s) will be evaluated when you hit Enter." +
                "\r\n" +
                "\r\n" +
                "The format of the commands is as follow:" +
                "\r\n" +
                "1) The first letter is representing the face that you want to rotate:" +
                "\r\n" +
                "\"u\" for the upper face, \"d\" for the downside, \"l\" for left, \"r\" for right, \"f\" for the front, and \"b\" for the back. You can use either uppercase or lowercase." +
                "\r\n" +
                "2) Determine which layer to rotate. If you want to rotate the most right layer of the cube, you can just type \"r\". But if you want to rotate the second-right layer you have to write \"r2\". Similarly, you can write \"u3\" in order to rotate the third-from-the-top layer of the cube." +
                "\r\n" +
                "It works the same for all sides and layers." +
                "\r\n" +
                "3) The default direction of the rotatetion is clockwise. If you want to rotate counterclockwise you need to add an ' after you have specified the layer." +
                "\r\n" +
                "The direction is determined by the face you declare. For example, if your cube is the size of 3, the command r3 and the command l1' are doing the exact same action." +
                "\r\n" +
                "4) If you want to rotate a face twice, you can add \"*\" at the end of the command. For example: \"f2*\" is the same as \"f2 f2\"." +
                "\r\n" +
                "\r\n" +
                "In order to move the whole cube around and being able to see it from different angles you can use the ugly and absolutly temporary buttons at the outline, or you can use the keyboard shortkey Shift + w/a/s/d for the directions: up, left, down, right - in that order." +
                "\r\n" +
                "Notice: when you press at the button or shortkey \"up\" you are the one who move up, not the cube, meaning you can now look at the previously upper-side of the cube instead of the front side. The same is true for the other directions.";
            MessageBox.Show(message);
        }
    }

}
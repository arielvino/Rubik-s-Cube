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
    }

}
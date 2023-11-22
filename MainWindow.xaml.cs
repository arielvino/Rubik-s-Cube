using Rubik_s_Cube.CubeModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik_s_Cube
{
    public partial class MainWindow : Window
    {
        Cube cube;
        int cubeSize = 5;

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
                        RefreshCube(cube);
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            cube = new(cubeSize);

            ChessCube();

            RefreshCube(cube);
        }

        private void RefreshCube(Cube cube)
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
        }


        private void RotateView_Button(object sender, RoutedEventArgs e)
        {
            var look = camera.LookDirection;
            var position = camera.Position;
            var up = camera.UpDirection;

            camera.LookDirection = new(-look.X, -look.Y, -look.Z);
            camera.Position = new(-position.X, -position.Y, -position.Z);
        }

        private void CreateNewCube_Button(object sender, RoutedEventArgs e)
        {
            cube = new Cube(cubeSize);
            RefreshCube(cube);
        }

        private void ShuffleCube_Button(object sender, RoutedEventArgs e)
        {
            cube.Shuffle();
            RefreshCube(cube);
        }

        private void RotateRandom_Button(object sender, RoutedEventArgs e)
        {
            cube.RotateRandomally();
            RefreshCube(cube);
        }
    }

}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DroneMesh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declare scene objects.
        private readonly Viewport3D _viewport3D = new Viewport3D();
        private readonly ModelVisual3D _modelVisual3D = new ModelVisual3D(); // defines light cast in scene
        private readonly Model3DGroup _model3DGroup = new Model3DGroup(); // holds multiple geometry objects
        private readonly GeometryModel3D _geometryModel = new GeometryModel3D(); // vertices and triangles as mesh geometry 3d

        // Defines the camera used to view the 3D object. In order to view the 3D object,
        // the camera must be positioned and pointed such that the object is within view 
        // of the camera.
        private readonly PerspectiveCamera _perspectiveCamera = new PerspectiveCamera();

        public MainWindow()
        {
            InitializeComponent();

            // Build the geometry model
            _geometryModel.Geometry = BuildSquareGeometry();
            _geometryModel.Material = BuildMaterial();

            // Add scene lighting to the group of models
            _model3DGroup.Children.Add(BuildSceneLighting());

            // Add the geometry model to the model group
            _model3DGroup.Children.Add(_geometryModel);
            _model3DGroup.Children.Add(BuildLineGeometry());

            // Add the group of models to the ModelVisual3d
            _modelVisual3D.Content = _model3DGroup;

            // Add the model to the viewport
            _viewport3D.Children.Add(_modelVisual3D);
            _viewport3D.Camera = BuildCamera();

            // Assign the viewport to the page for rendering
            Content = _viewport3D;
        }

        public Camera BuildCamera()
        {
            // Configure the camera
            _perspectiveCamera.Position = new Point3D(0, 0, 2); // Specify where in the 3D scene the camera is.
            _perspectiveCamera.LookDirection = new Vector3D(0, 0, -1); // Specify the direction that the camera is pointing.
            _perspectiveCamera.FieldOfView = 120; // Define camera's horizontal field of view in degrees.

            return _perspectiveCamera;
        }

        public Geometry3D BuildSquareGeometry()
        {
            // The geometry specifies the shape of the 3D plane. In this sample, a flat sheet 
            // is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            // Create a collection of normal vectors for the MeshGeometry3D.
            var myNormalCollection = new Vector3DCollection();
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myMeshGeometry3D.Normals = myNormalCollection;

            // Create a collection of vertex positions for the MeshGeometry3D. 
            var myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(-0.5, -0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, -0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(-0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(-0.5, -0.5, 0.5));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of texture coordinates for the MeshGeometry3D.
            var myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(0, 1));
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            var myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(5);
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            return myMeshGeometry3D;
        }

        public Material BuildMaterial()
        {
            // The material specifies the material applied to the 3D object. In this sample a  
            // linear gradient covers the surface of the 3D object.

            // Create a horizontal linear gradient with four stops.   
            //var myHorizontalGradient = new LinearGradientBrush();
            //myHorizontalGradient.StartPoint = new Point(0, 0.5);
            //myHorizontalGradient.EndPoint = new Point(1, 0.5);
            //myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0));
            //myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));
            var myHorizontalGradient = new SolidColorBrush(Colors.DarkSlateGray);

            // Define material and apply to the mesh geometries.
            var myMaterial = new DiffuseMaterial(myHorizontalGradient);

            return myMaterial;
        }

        public Model3D BuildSceneLighting()
        {
            // Define the lights cast in the scene. Without light, the 3D object cannot 
            // be seen. Note: to illuminate an object from additional directions, create 
            // additional lights.
            //var directionalLight = new DirectionalLight
            //{
            //    Color = Colors.White, 
            //    Direction = new Vector3D(-0.61, -0.5, -0.61)
            //};
            var directionalLight = new AmbientLight(Colors.White);

            return directionalLight;
        }

        public Transform3D TransformGeometry()
        {
            Random random = new Random();

            int x = random.Next(0, 360);
            int y = random.Next(0, 360);
            int z = random.Next(0, 360);

            // Apply a transform to the object. In this sample, a rotation transform is applied,  
            // rendering the 3D object rotated.
            var myRotateTransform3D = new RotateTransform3D();
            var myAxisAngleRotation3d = new AxisAngleRotation3D
            {
                //Axis = new Vector3D(0, 3, 0),
                Axis = new Vector3D(x, y, z),
                Angle = 40
            };

            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            return myRotateTransform3D;
        }

        public MeshGeometry3D ScaleGeometry()
        {
            Random random = new Random();

            var x = random.NextDouble(); // Next(0, 3);
            var y = random.NextDouble();
            var z = random.NextDouble();

            // The geometry specifies the shape of the 3D plane. In this sample, a flat sheet is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            // Create a collection of normal vectors for the MeshGeometry3D.
            //myMeshGeometry3D.Normals = new Vector3DCollection
            //{
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(1, 0, 1),
            //    new Vector3D(1, 0, 1),
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(0, 0, 1)
            //};

            // Create a collection of vertex positions for the MeshGeometry3D. 
            myMeshGeometry3D.Positions = new Point3DCollection
            {
                new Point3D(-0.5, -0.5, 0.5),   // -0.5, -0.5, 0.5      bottom left
                new Point3D(0.5, -0.5, 0.5),    //  0.5, -0.5, 0.5      bottom right
                new Point3D(0.5, 0.5, 0.5),     //  0.5,  0.5, 0.5      top right
                //new Point3D(0.5, 0.5, 0.5),     //  0.5,  0.5, 0.5      top right (dup)
                new Point3D(-0.5, 0.5, 0.5),    // -0.5,  0.5, 0.5      top left
                //new Point3D(-0.5, -0.5, 0.5)    // -0.5, -0.5, 0.5      bottom right (dup)
            };

            // Create a collection of texture coordinates for the MeshGeometry3D.
            //myMeshGeometry3D.TextureCoordinates = new PointCollection
            //{
            //    new Point(0, 0),
            //    new Point(1, 0),
            //    new Point(1, 1),
            //    new Point(1, 1),
            //    new Point(0, 1),
            //    new Point(0, 0)
            //};

            // Create a collection of triangle indices for the MeshGeometry3D.
            //myMeshGeometry3D.TriangleIndices = new Int32Collection
            //{
            //    0,
            //    1,
            //    2,
            //    3,
            //    4,
            //    5
            //};

            return myMeshGeometry3D;
        }

        public Model3D BuildLineGeometry()
        {
            return DrawLine(new Point3D(0, 0, 100), new Point3D(0, 100, 100), "line 1)");
        }

        private Model3D DrawLine(Point3D startPoint, Point3D EndPoint, string name)
        {
            var brush = new SolidColorBrush(Colors.Black);
            var material = new DiffuseMaterial(brush);
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(startPoint);
            mesh.Positions.Add(EndPoint);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);

            return new GeometryModel3D(mesh, material);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _geometryModel.Geometry = ScaleGeometry();
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _geometryModel.Transform = TransformGeometry();
        }
    }
}

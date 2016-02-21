using System.Collections;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using JelloSimulation.Models;
using HelixToolkit.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace JelloSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double animationSpeed;
        public double AnimationSpeed
        {
            get { return animationSpeed; }
            set
            {
                if (value != animationSpeed)
                {
                    animationSpeed = value;
                    OnPropertyChanged("AnimationSpeed");
                }
            }
        }

        private double mass;
        public double Mass
        {
            get { return mass; }
            set
            {
                if (value != mass)
                {
                    mass = value;
                    OnPropertyChanged("Mass");
                }
            }
        }

        private double elastictyC1;
        public double ElasticityC1
        {
            get { return elastictyC1; }
            set
            {
                if (value != elastictyC1)
                {
                    elastictyC1 = value;
                    OnPropertyChanged("Springer");
                }
            }
        }
        private double elasticityC2;
        public double ElasticityC2
        {
            get { return elasticityC2; }
            set
            {
                if (value != elasticityC2)
                {
                    elasticityC2 = value;
                    OnPropertyChanged("ElasticityC2");
                }
            }
        }

        private double viscosity;
        public double Viscosity
        {
            get { return viscosity; }
            set
            {
                if (value != viscosity)
                {
                    viscosity = value;
                    OnPropertyChanged("Viscosity");
                }
            }
        }

        private double delta;
        public double Delta
        {
            get { return delta; }
            set
            {
                if (value != delta)
                {
                    delta = value;
                    OnPropertyChanged("Delta");
                }
            }
        }

        private double x0Max;
        public double X0Max
        {
            get { return x0Max; }
            set
            {
                if (value != x0Max)
                {
                    x0Max = value;
                    OnPropertyChanged("X0Max");
                }
            }
        }

        private double v0Max;
        public double V0Max
        {
            get { return v0Max; }
            set
            {
                if (value != v0Max)
                {
                    v0Max = value;
                    OnPropertyChanged("V0Max");
                }
            }
        }

        private bool controlPointsEnabled;
        public bool ControlPointsEnabled
        {
            get { return controlPointsEnabled; }
            set
            {
                if (value != controlPointsEnabled)
                {
                    controlPointsEnabled = value;
                    OnPropertyChanged("ControlPointsEnabled");
                }
            }
        }

        private bool steeringFrameEnabled;
        public bool SteeringFrameEnabled
        {
            get { return steeringFrameEnabled; }
            set
            {
                if (value != steeringFrameEnabled)
                {
                    steeringFrameEnabled = value;
                    OnPropertyChanged("SteeringFrameEnabled");
                }
            }
        }

        private bool limitationsCuboidEnabled;
        public bool LimitationsCuboidEnabled
        {
            get { return limitationsCuboidEnabled; }
            set
            {
                if (value != limitationsCuboidEnabled)
                {
                    limitationsCuboidEnabled = value;
                    OnPropertyChanged("LimitationsCuboidEnabled");
                }
            }
        }

        private bool bezierCubeEnabled;
        public bool BezierCubeEnabled
        {
            get { return bezierCubeEnabled; }
            set
            {
                if (value != bezierCubeEnabled)
                {
                    bezierCubeEnabled = value;
                    OnPropertyChanged("BezierCubeEnabled");
                }
            }
        }

        private bool deformedSolidEnabled;
        public bool DeformedSolidEnabled
        {
            get { return deformedSolidEnabled; }
            set
            {
                if (value != deformedSolidEnabled)
                {
                    deformedSolidEnabled = value;
                    OnPropertyChanged("DeformedSolidEnabled");
                }
            }
        }

        private bool damping;
        public bool Damping
        {
            get { return damping; }
            set
            {
                if (value != damping)
                {
                    damping = value;
                    OnPropertyChanged("Damping");
                }
            }
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        private const double Epsilon = 0.0001;
        private IBezierCubeVisual3D bezierCube;
        private IFrameVisual3D steeringFrame;
        private CombinedManipulator manipulator;
        private int cubeSize = 6;
        private IRoomVisual3D limitationsCuboid;
        private BezierSurface[] surfaces;
        private PointsVisual3D spherePoints;
        private MeshGeometryVisual3D geometry;

        #endregion

        #region Public Methods
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
            InitializeScene();
            UpdateSpringData();
        }

        #endregion

        #region Private Methods
        private void InitializeScene()
        {
            limitationsCuboid = new RoomVisual3D();
            limitationsCuboid.Initialize(20.0);
            limitationsCuboid.IsDampingActive = Damping;

            bezierCube = new BezierCubeVisual3D();
            bezierCube.Initialize(cubeSize);
            bezierCube.CollisionChecker = limitationsCuboid;
            bezierCube.IsDampingActive = Damping;

            spherePoints = new PointsVisual3D();
            spherePoints.Points = bezierCube.GetSpherePoints();
            spherePoints.Size = 3;

            if (DeformedSolidEnabled)
                HelixViewport.Children.Add(spherePoints);

            if (ControlPointsEnabled)
            {
                HelixViewport.Children.Add(bezierCube.points);
                HelixViewport.Children.Add(bezierCube.lines);
            }

            steeringFrame = new FrameVisual3D();
            steeringFrame.Initialize(cubeSize);
            if (SteeringFrameEnabled)
            {
                HelixViewport.Children.Add(steeringFrame.points);
                HelixViewport.Children.Add(steeringFrame.lines);
                HelixViewport.Children.Add(steeringFrame.GetJointsPoints(bezierCube.GetCornerPoints()));
            }
            manipulator = new CombinedManipulator();
            manipulator.Diameter = 3;
            manipulator.Offset = new Vector3D(0, 0, 5);
            HelixViewport.Children.Add(manipulator);

            //geometry = new MeshGeometryVisual3D();
            //var builder = new MeshBuilder(false, false);
            //builder.AddTriangles(bezierCube.GetSpherePoints());
            //geometry.MeshGeometry = builder.ToMesh(true);
            //HelixViewport.Children.Add(geometry);

            surfaces = new BezierSurface[6];
            for (int i = 0; i < 6; i++)
            {
                surfaces[i] = new BezierSurface()
                {
                    Fill = Brushes.Red,
                    MeshSizeU = 20,
                    MeshSizeV = 20
                };
                surfaces[i].UpdateSurface(bezierCube.GetFaceControlPoints(i));
                surfaces[i].UpdateModel();
                if (BezierCubeEnabled)
                    HelixViewport.Children.Add(surfaces[i]);
            }
            //limitationsCuboid.UpdateViewport(HelixViewport);
        }

        private void Initialize()
        {
            Mass = 1;
            ElasticityC1 = 0.2;
            ElasticityC2 = 0.3;
            Viscosity = 0.05;
            Delta = 0.2;
            X0Max = 1;
            V0Max = 1;
            ControlPointsEnabled = false;
            SteeringFrameEnabled = true;
            LimitationsCuboidEnabled = true;
            BezierCubeEnabled = true;
            DeformedSolidEnabled = false;
            Damping = true;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            HelixViewport.Children.Remove(bezierCube.points);
            HelixViewport.Children.Remove(bezierCube.lines);
            HelixViewport.Children.Remove(steeringFrame.points);
            HelixViewport.Children.Remove(steeringFrame.lines);
            HelixViewport.Children.Remove(limitationsCuboid.points);
            HelixViewport.Children.Remove(limitationsCuboid.lines);
            HelixViewport.Children.Remove(spherePoints);
            for (int i = 0; i < cubeSize; i++)
            {
                HelixViewport.Children.Remove(surfaces[i]);
            }

            bezierCube.CalculateJointForces(steeringFrame.GetFramePoints());
            bezierCube.Update();
            LinesVisual3D frameLines = steeringFrame.GetJointsPoints(bezierCube.GetCornerPoints());
            HelixViewport.Children.Remove(frameLines);

            bezierCube.points.Points = bezierCube.GetControlPoints();
            bezierCube.lines.Points = bezierCube.GetControlLines();

            steeringFrame.SetTransform(manipulator.TargetTransform);
            steeringFrame.points.Points = steeringFrame.GetControlPoints();
            steeringFrame.lines.Points = steeringFrame.GetControlLines();

            for (int i = 0; i < 6; i++)
            {
                surfaces[i].UpdateSurface(bezierCube.GetFaceControlPoints(i));
                surfaces[i].UpdateModel();
            }

            spherePoints.Points = bezierCube.GetSpherePoints();

            if (controlPointsEnabled)
            {
                HelixViewport.Children.Add(bezierCube.lines);
                HelixViewport.Children.Add(bezierCube.points);
            }

            if (SteeringFrameEnabled)
            {
                HelixViewport.Children.Add(frameLines);
                HelixViewport.Children.Add(steeringFrame.lines);
                HelixViewport.Children.Add(steeringFrame.points);
            }

            if (BezierCubeEnabled)
                for (int i = 0; i < cubeSize; i++)
                {
                    HelixViewport.Children.Add(surfaces[i]);
                }
            if (LimitationsCuboidEnabled)
            {
                HelixViewport.Children.Add(limitationsCuboid.points);
                HelixViewport.Children.Add(limitationsCuboid.lines);
            }
            if (DeformedSolidEnabled)
            {
                HelixViewport.Children.Add(spherePoints);
                HelixViewport.Children.Remove(frameLines);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)animationSpeed);
            HelixViewport.Children.Remove(manipulator);
            HelixViewport.Children.Remove(bezierCube.points);
            HelixViewport.Children.Remove(bezierCube.lines);
            HelixViewport.Children.Remove(steeringFrame.points);
            HelixViewport.Children.Remove(steeringFrame.lines);
            HelixViewport.Children.Remove(limitationsCuboid.points);
            HelixViewport.Children.Remove(limitationsCuboid.lines);
            for (int i = 0; i < cubeSize; i++)
            {
                HelixViewport.Children.Remove(surfaces[i]);
            }
            HelixViewport.Children.Remove(spherePoints);
            LinesVisual3D frameLines = steeringFrame.GetJointsPoints(bezierCube.GetCornerPoints());
            HelixViewport.Children.Remove(frameLines);
            InitializeScene();
            UpdateSpringData();
            dispatcherTimer.Start();
        }

        private void UpdateSpringData()
        {
            bezierCube.spring.Mass = Mass;
            bezierCube.spring.Springer = ElasticityC1;
            bezierCube.spring.Viscosity = Viscosity;
            bezierCube.spring.Delta = Delta;
            bezierCube.X0Max = X0Max;
            bezierCube.V0Max = V0Max;
            bezierCube.IsDampingActive = Damping;

            bezierCube.springFrame.Mass = Mass;
            bezierCube.springFrame.Springer = ElasticityC2;
            bezierCube.springFrame.Viscosity = Viscosity;
            bezierCube.springFrame.Delta = Delta;
            limitationsCuboid.IsDampingActive = Damping;
        }

        #endregion
    }
}

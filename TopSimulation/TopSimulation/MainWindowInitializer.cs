using HelixToolkit.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TopSimulation
{
    /// <summary>
    /// Initialization for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Properties

        ObservableCollection<Point3D> trajectoryPoints;
        public ObservableCollection<Point3D> TrajectoryPoints
        {
            get { return trajectoryPoints; }
        }

        private double timeDelta;
        public double TimeDelta
        {
            get { return timeDelta; }
            set
            {
                if (value != timeDelta)
                {
                    timeDelta = value;
                    OnPropertyChanged("TimeDelta");
                }
            }
        }

        private double cubeSize;
        public double CubeSize
        {
            get { return cubeSize; }
            set
            {
                if (value != cubeSize)
                {
                    cubeSize = value;
                    OnPropertyChanged("CubeSize");
                }
            }
        }

        private double cubeDensity;
        public double CubeDensity
        {
            get { return cubeDensity; }
            set
            {
                if (value != cubeDensity)
                {
                    cubeDensity = value;
                    OnPropertyChanged("CubeDensity");
                }
            }
        }

        private double angle;
        public double Angle
        {
            get { return angle; }
            set
            {
                if (value != angle)
                {
                    angle = value;
                    OnPropertyChanged("Angle");
                }
            }
        }

        private double angleVelocity;
        public double AngleVelocity
        {
            get { return angleVelocity; }
            set
            {
                if (value != angleVelocity)
                {
                    angleVelocity = value;
                    OnPropertyChanged("AngleVelocity");
                }
            }
        }

        private double trajectoryLength;
        public double TrajectoryLength
        {
            get { return trajectoryLength; }
            set
            {
                if (value != trajectoryLength)
                {
                    trajectoryLength = value;
                    OnPropertyChanged("TrajectoryLength");
                }
            }
        }

        private bool gravityEnabled;
        public bool GravityEnabled
        {
            get { return gravityEnabled; }
            set
            {
                if (value != gravityEnabled)
                {
                    gravityEnabled = value;
                    OnPropertyChanged("GravityEnabled");
                }
            }
        }

        private bool cubeEnabled;
        public bool CubeEnabled
        {
            get { return cubeEnabled; }
            set
            {
                if (value != cubeEnabled)
                {
                    if (!value)
                        HelixViewport.Children.Remove(FrameStartCube);
                    else if (HelixViewport.Children.IndexOf(FrameStartCube) < 0)
                        HelixViewport.Children.Add(FrameStartCube);
                    cubeEnabled = value;
                    OnPropertyChanged("CubeEnabled");
                }
            }
        }

        private bool diagonalEnabled;
        public bool DiagonalEnabled
        {
            get { return diagonalEnabled; }
            set
            {
                if (value != diagonalEnabled)
                {
                    if (!value)
                        HelixViewport.Children.Remove(diagonalArrow);
                    else if (diagonalArrow != null && HelixViewport.Children.IndexOf(diagonalArrow) < 0)
                        HelixViewport.Children.Add(diagonalArrow);
                    diagonalEnabled = value;
                    OnPropertyChanged("DiagonalEnabled");
                }
            }
        }

        private bool trajectoryEnabled;
        public bool TrajectoryEnabled
        {
            get { return trajectoryEnabled; }
            set
            {
                if (value != trajectoryEnabled)
                {
                    if (!value)
                        HelixViewport.Children.Remove(TrajectoryChain);
                    else if (HelixViewport.Children.IndexOf(TrajectoryChain) < 0)
                        HelixViewport.Children.Add(TrajectoryChain);
                    trajectoryEnabled = value;
                    OnPropertyChanged("TrajectoryEnabled");
                }
            }
        }

        private bool gravityDirectionEnabled;
        public bool GravityDirectionEnabled
        {
            get { return gravityDirectionEnabled; }
            set
            {
                if (value != gravityDirectionEnabled)
                {
                    if (!value)
                    {
                        HelixViewport.Children.Remove(gravityPlane);
                        HelixViewport.Children.Remove(gravityArrow);
                    }
                    else
                    {
                        if (gravityPlane != null && HelixViewport.Children.IndexOf(gravityPlane) < 0)
                            HelixViewport.Children.Add(gravityPlane);
                        if (gravityArrow != null && HelixViewport.Children.IndexOf(gravityArrow) < 0)
                            HelixViewport.Children.Add(gravityArrow);
                    }
                    gravityDirectionEnabled = value;
                    OnPropertyChanged("GravityDirectionEnabled");
                }
            }
        }

        private double currentQuaternionX;
        public double CurrentQuaternionX
        {
            get { return currentQuaternionX; }
            set
            {
                if (value != currentQuaternionX)
                {
                    currentQuaternionX = value;
                    OnPropertyChanged("CurrentQuaternionX");
                }
            }
        }

        private double currentQuaternionY;
        public double CurrentQuaternionY
        {
            get { return currentQuaternionY; }
            set
            {
                if (value != currentQuaternionY)
                {
                    currentQuaternionY = value;
                    OnPropertyChanged("CurrentQuaternionY");
                }
            }
        }

        private double currentQuaternionZ;
        public double CurrentQuaternionZ
        {
            get { return currentQuaternionZ; }
            set
            {
                if (value != currentQuaternionZ)
                {
                    currentQuaternionZ = value;
                    OnPropertyChanged("CurrentQuaternionZ");
                }
            }
        }

        private double currentQuaternionW;
        public double CurrentQuaternionW
        {
            get { return currentQuaternionW; }
            set
            {
                if (value != currentQuaternionW)
                {
                    currentQuaternionW = value;
                    OnPropertyChanged("CurrentQuaternionW");
                }
            }
        }

        private double currentAngleVelocityX;
        public double CurrentAngleVelocityX
        {
            get { return currentAngleVelocityX; }
            set
            {
                if (value != currentAngleVelocityX)
                {
                    currentAngleVelocityX = value;
                    OnPropertyChanged("CurrentAngleVelocityX");
                }
            }
        }

        private double currentAngleVelocityY;
        public double CurrentAngleVelocityY
        {
            get { return currentAngleVelocityY; }
            set
            {
                if (value != currentAngleVelocityY)
                {
                    currentAngleVelocityY = value;
                    OnPropertyChanged("CurrentAngleVelocityY");
                }
            }
        }

        private double currentAngleVelocityZ;
        public double CurrentAngleVelocityZ
        {
            get { return currentAngleVelocityZ; }
            set
            {
                if (value != currentAngleVelocityZ)
                {
                    currentAngleVelocityZ = value;
                    OnPropertyChanged("CurrentAngleVelocityZ");
                }
            }
        }
        #endregion

        #region Fields

        Random rnd = new Random(DateTime.Now.Millisecond);
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        ArrowVisual3D gravityArrow;
        ArrowVisual3D diagonalArrow;
        ArrowVisual3D arrowBinormal;
        ArrowVisual3D arrowLastBinormal;
        private const double Epsilon = 0.001;
        private RectangleVisual3D gravityPlane;
        private bool animationPaused;
        private Matrix3D cubeTensor;
        private double time;
        private double[] yODEVector;
        private const double G = 9.81;
        private double cubeMass;
        private Matrix3D invertCubeTensor;
        private double cubeDiagonalLength;
        private Vector3D gravityVector;
        private Quaternion baseQuaternion;
        private Quaternion trajectoryQuaternion;
        private Vector3D rotationVector;
        private Quaternion startQuaternion;

        #endregion

        #region Methods

        /// <summary>
        /// Window initialization
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
            InitializeScene();
        }

        /// <summary>
        /// Initialization of variables and sets
        /// </summary>
        private void Initialize()
        {
            TimeDelta = 0.01;
            CubeSize = 3;
            CubeDensity = 1;
            TrajectoryLength = 500;
            cubeDiagonalLength = CubeSize * Math.Sqrt(3);
            GravityEnabled = true;
            CubeEnabled = true;
            DiagonalEnabled = true;
            GravityDirectionEnabled = true;
            TrajectoryEnabled = true;
            currentQuaternionX = 0.3647;
            currentQuaternionY = -0.2798;
            currentQuaternionZ = 0.1159;
            currentQuaternionW = 0.8805;
            baseQuaternion = new Quaternion(0.3647, -0.2798, 0.1159, 0.8805);
            startQuaternion = new Quaternion(0, 0, 0, 1);
            //currentQuaternionX = 0;
            //currentQuaternionY = 0;
            //currentQuaternionZ = 0;
            //currentQuaternionW = 1;
            currentAngleVelocityX = 0;
            currentAngleVelocityY = 0;
            currentAngleVelocityZ = 0;

            trajectoryPoints = new ObservableCollection<Point3D>();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            CalculateMass();
            CalculateGravityForceVector();
            CalculateCubeTensor();
            SetupBaseODEVector();
        }

        /// <summary>
        /// 3D scene initialization
        /// </summary>
        private void InitializeScene()
        {
            const double maxVal = 8;

            var arrowX = new ArrowVisual3D();
            arrowX.Direction = new Vector3D(1, 0, 0);
            arrowX.Point1 = new Point3D(0, 0, 0);
            arrowX.Point2 = new Point3D(maxVal, 0, 0);
            arrowX.Diameter = 0.1;
            arrowX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowX);

            var arrowMX = new ArrowVisual3D();
            arrowMX.Direction = new Vector3D(-1, 0, 0);
            arrowMX.Point1 = new Point3D(0, 0, 0);
            arrowMX.Point2 = new Point3D(-maxVal, 0, 0);
            arrowMX.Diameter = 0.1;
            arrowMX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowMX);

            var arrowY = new ArrowVisual3D();
            arrowY.Direction = new Vector3D(0, 1, 0);
            arrowY.Point1 = new Point3D(0, 0, 0);
            arrowY.Point2 = new Point3D(0, maxVal, 0);
            arrowY.Diameter = 0.1;
            arrowY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowY);

            var arrowMY = new ArrowVisual3D();
            arrowMY.Direction = new Vector3D(0, -1, 0);
            arrowMY.Point1 = new Point3D(0, 0, 0);
            arrowMY.Point2 = new Point3D(0, -maxVal, 0);
            arrowMY.Diameter = 0.1;
            arrowMY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowMY);

            var arrowZ = new ArrowVisual3D();
            arrowZ.Direction = new Vector3D(0, 0, 1);
            arrowZ.Point1 = new Point3D(0, 0, 0);
            arrowZ.Point2 = new Point3D(0, 0, maxVal);
            arrowZ.Diameter = 0.1;
            arrowZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowZ);

            var arrowMZ = new ArrowVisual3D();
            arrowMZ.Direction = new Vector3D(0, 0, -1);
            arrowMZ.Point1 = new Point3D(0, 0, 0);
            arrowMZ.Point2 = new Point3D(0, 0, -maxVal);
            arrowMZ.Diameter = 0.1;
            arrowMZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewport.Children.Add(arrowMZ);

            var xArrowText = new TextVisual3D();
            xArrowText.Text = "X";
            xArrowText.Position = new Point3D(maxVal - 0.5, 0, 0.5);
            xArrowText.Height = 0.5;
            xArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewport.Children.Add(xArrowText);

            var yArrowText = new TextVisual3D();
            yArrowText.Text = "Y";
            yArrowText.Position = new Point3D(0, maxVal - 0.5, 0.5);
            yArrowText.Height = 0.5;
            yArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewport.Children.Add(yArrowText);

            var zArrowText = new TextVisual3D();
            zArrowText.Text = "Z";
            zArrowText.Position = new Point3D(0.5, 0, maxVal - 0.5);
            zArrowText.Height = 0.5;
            zArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewport.Children.Add(zArrowText);

            gravityPlane = new RectangleVisual3D();
            gravityPlane.Width = 10;
            gravityPlane.Length = 10;
            var brush = new SolidColorBrush(Colors.Green);
            brush.Opacity = 0.5;
            gravityPlane.Fill = brush;
            HelixViewport.Children.Add(gravityPlane);

            gravityArrow = new ArrowVisual3D();
            gravityArrow.Direction = new Vector3D(0, 0, -1);
            gravityArrow.Point1 = new Point3D(0, 0, 0);
            gravityArrow.Point2 = new Point3D(0, 0, -3);
            gravityArrow.Diameter = 0.2;
            gravityArrow.Fill = Brushes.Green;
            HelixViewport.Children.Add(gravityArrow);

            diagonalArrow = new ArrowVisual3D();
            diagonalArrow.Direction = new Vector3D(1, 1, 1);
            diagonalArrow.Point1 = new Point3D(0, 0, 0);
            diagonalArrow.Point2 = new Point3D(cubeSize, cubeSize, cubeSize);
            diagonalArrow.Diameter = 0.1;
            diagonalArrow.Fill = Brushes.Blue;
            HelixViewport.Children.Add(diagonalArrow);

            SetupCube();
        }

        private void SetupCube()
        {
            FrameStartCube.SideLength = CubeSize;
            FrameStartCube.Material = new EmissiveMaterial(Brushes.Red);
            FrameStartCube.Center = new Point3D(cubeSize / 2, cubeSize / 2, cubeSize / 2);
            var quat = new Quaternion(CurrentQuaternionX, CurrentQuaternionY, CurrentQuaternionZ, CurrentQuaternionW);

            FrameStartCube.Transform = new RotateTransform3D(new QuaternionRotation3D(quat));
            FrameStartManipulator.CanTranslateX = false;
            FrameStartManipulator.CanTranslateY = false;
            FrameStartManipulator.CanTranslateZ = false;

            diagonalArrow.Point2 = new Point3D(cubeSize, cubeSize, cubeSize);
            diagonalArrow.Transform = FrameStartCube.Transform;
            rotationVector = MatrixVectorMultiply(diagonalArrow.Transform.Value, diagonalArrow.Direction);
            if(Math.Abs(rotationVector.X - rotationVector.Y) < 0.01)
            //if(rotationVector.X != rotationVector.Y)
            {
                rotationVector.X = rotationVector.Y = Math.Min(rotationVector.X, rotationVector.Y);
            }
            rotationVector.Normalize();
            rotationVector *= cubeDiagonalLength;
        }
        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace TopSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Frame Calculation

        private void CalculateCubeTensor()
        {
            var factor = cubeMass * CubeSize * cubeSize;
            var d = 2.0 / 3.0 * factor;
            var nd = -1.0 / 4.0 * factor;

            cubeTensor = new Matrix3D(d, nd, nd, 0, nd, d, nd, 0, nd, nd, d, 0, 0, 0, 0, 1);
            invertCubeTensor = new Matrix3D(d, nd, nd, 0, nd, d, nd, 0, nd, nd, d, 0, 0, 0, 0, 1);
            invertCubeTensor.Invert();
        }

        private void CalculateMass()
        {
            cubeMass = CubeDensity * CubeSize * CubeSize * CubeSize;
        }

        private void CalculateDiagonalLength()
        {
            cubeDiagonalLength = CubeSize * Math.Sqrt(3);
        }

        private void CalculateGravityForceVector()
        {
            gravityVector = new Vector3D(0, 0, -1) * cubeMass * G;
        }

        private Vector3D CalculateCubeTorque(Quaternion cubeQuaternion)
        {
            if (!GravityEnabled)
                return new Vector3D(0, 0, 0);
            var G = QuaternionVectorMultiply(cubeQuaternion, gravityVector);
            var r = rotationVector;
            return Vector3D.CrossProduct(r, G);
        }

        private void SetupBaseODEVector()
        {
            yODEVector = new double[7];
            yODEVector[0] = CurrentAngleVelocityX;
            yODEVector[1] = CurrentAngleVelocityY;
            yODEVector[2] = CurrentAngleVelocityZ;
            yODEVector[3] = startQuaternion.X; 
            yODEVector[4] = startQuaternion.Y;
            yODEVector[5] = startQuaternion.Z;
            yODEVector[6] = startQuaternion.W;
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            time += TimeDelta;
            var odeSolverData = new ODESolverData(cubeTensor, invertCubeTensor);
            var ODEVector = new double[] { time - TimeDelta, time };
            var yNew = SolveODESystem(yODEVector, ODEVector, odeSolverData);
            for (int i = 0; i < 3; i++)
                yODEVector[i] = yNew[1, i];

            var quat = new Quaternion(yNew[1, 3], yNew[1, 4], yNew[1, 5], yNew[1, 6]);
            quat.Normalize();
            //FrameStartCube.Transform = new RotateTransform3D(new QuaternionRotation3D(quat));
            var tg = new Transform3DGroup();
            tg.Children.Add(new RotateTransform3D(new QuaternionRotation3D(quat)));
            tg.Children.Add(new RotateTransform3D(new QuaternionRotation3D(
                new Quaternion(CurrentQuaternionX, CurrentQuaternionY, CurrentQuaternionZ, CurrentQuaternionW))));
            FrameStartCube.Transform = tg;
            diagonalArrow.Transform = FrameStartCube.Transform;

            var diagonalRotation = MatrixVectorMultiply(diagonalArrow.Transform.Value, diagonalArrow.Direction);
            diagonalRotation.Normalize();
            var endPoint = new Point3D(diagonalRotation.X * cubeDiagonalLength,
                diagonalRotation.Y * cubeDiagonalLength, diagonalRotation.Z * cubeDiagonalLength);
            trajectoryPoints.Add(endPoint);
            if(time - TimeDelta > 0)
                trajectoryPoints.Add(endPoint);
            if (trajectoryPoints.Count > TrajectoryLength * 2 - 1)
            {
                trajectoryPoints.RemoveAt(0);
                trajectoryPoints.RemoveAt(0);
            }

            yODEVector[3] = quat.X;
            yODEVector[4] = quat.Y;
            yODEVector[5] = quat.Z;
            yODEVector[6] = quat.W;
        }

        private Point3D MatrixPointMultiply(Matrix3D matrix, Point3D point)
        {
            return new Point3D(
                matrix.M11 * point.X + matrix.M21 * point.Y + matrix.M31 * point.Z,
                matrix.M12 * point.X + matrix.M22 * point.Y + matrix.M32 * point.Z,
                matrix.M13 * point.X + matrix.M23 * point.Y + matrix.M33 * point.Z);
        }

        private Vector3D MatrixVectorMultiply(Matrix3D matrix, Vector3D vector)
        {
            return new Vector3D(
                matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z,
                matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z,
                matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z);
        }

        private Vector3D QuaternionVectorMultiply(Quaternion q, Vector3D v)
        {
            var xx = q.X * q.X;
            var xy = q.X * q.Y;
            var xz = q.X * q.Z;
            var xw = q.X * q.W;

            var yy = q.Y * q.Y;
            var yz = q.Y * q.Z;
            var yw = q.Y * q.W;

            var zz = q.Z * q.Z;
            var zw = q.Z * q.W;

            var m00 = 1 - 2 * (yy + zz);
            var m01 = 2 * (xy - zw);
            var m02 = 2 * (xz + yw);

            var m10 = 2 * (xy + zw);
            var m11 = 1 - 2 * (xx + zz);
            var m12 = 2 * (yz - xw);

            var m20 = 2 * (xz - yw);
            var m21 = 2 * (yz + xw);
            var m22 = 1 - 2 * (xx + yy);

            var matrix = new Matrix3D(m00, m10, m20, 0, m01, m11, m21, 0, m02, m12, m22, 0, 0, 0, 0, 1);
            matrix.Invert();
            return MatrixVectorMultiply(matrix, v);
        }

        public void ODESolverFunction(double[] y, double x, double[] dy, object obj)
        {
            var odeSolverData = obj as ODESolverData;
            var N = CalculateCubeTorque(new Quaternion(y[3], y[4], y[5], y[6]));
            var I = odeSolverData.tensor;
            var IInv = odeSolverData.invertTensor;
            var W = new Vector3D(y[0], y[1], y[2]);
            var IW = MatrixVectorMultiply(I, W);
            var IWW = Vector3D.CrossProduct(IW, W);
            var NIWW = N + IWW;
            var WT = MatrixVectorMultiply(IInv, NIWW);

            var Q = new Quaternion(y[3], y[4], y[5], y[6]);
            var WQ = new Quaternion(y[0], y[1], y[2], 0);
            var QT = Quaternion.Multiply(Q, WQ);
            QT = new Quaternion(QT.X / 2, QT.Y / 2, QT.Z / 2, QT.W / 2);

            dy[0] = WT.X;
            dy[1] = WT.Y;
            dy[2] = WT.Z;
            dy[3] = QT.X;
            dy[4] = QT.Y;
            dy[5] = QT.Z;
            dy[6] = QT.W;
        }

        public double[,] SolveODESystem(double[] y, double[] x, ODESolverData data)
        {
            double eps = 0.00001;
            double h = 0;
            alglib.odesolverstate s;
            int m;
            double[] xtbl;
            double[,] ytbl;
            alglib.odesolverreport rep;
            alglib.odesolverrkck(y, x, eps, h, out s);
            alglib.odesolversolve(s, ODESolverFunction, data);
            alglib.odesolverresults(s, out m, out xtbl, out ytbl, out rep);
            return ytbl;
        }

        #endregion

        #region Event Handlers

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            HelixViewport.Children.Remove(FrameStartManipulator);
            dispatcherTimer.Start();
            ApplyChanges.IsEnabled = false;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            animationPaused = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            ApplyChanges.IsEnabled = true;
            var quat = new Quaternion(CurrentQuaternionX, CurrentQuaternionY, CurrentQuaternionZ, CurrentQuaternionW);
            quat.Normalize();
            CurrentQuaternionX = quat.X;
            CurrentQuaternionY = quat.Y;
            CurrentQuaternionZ = quat.Z;
            CurrentQuaternionW = quat.W;
            CalculateMass();
            CalculateDiagonalLength();
            CalculateGravityForceVector();
            CalculateCubeTensor();
            time = 0;
            SetupBaseODEVector();
            SetupCube();
            TrajectoryPoints.Clear();
            if (HelixViewport.Children.IndexOf(FrameStartManipulator) < 0)
                HelixViewport.Children.Add(FrameStartManipulator);
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            var matrix = FrameStartCube.Transform.Value;
            var quaternion = ExtractQuaternion(matrix);
            CurrentQuaternionX = quaternion.X;
            CurrentQuaternionY = quaternion.Y;
            CurrentQuaternionZ = quaternion.Z;
            CurrentQuaternionW = quaternion.W;
            ResetButton_Click(null, null);
        }

        private Quaternion ExtractQuaternion(Matrix3D mat)
        {
            var tr = mat.M11 + mat.M22 + mat.M33;
            double qx, qy, qz, qw;

            if (tr > 0)
            {
                var S = Math.Sqrt(tr + 1.0) * 2;
                qw = 0.25 * S;
                qx = (mat.M23 - mat.M32) / S;
                qy = (mat.M31 - mat.M13) / S;
                qz = (mat.M12 - mat.M21) / S;
            }
            else if ((mat.M11 > mat.M22) & (mat.M11 > mat.M33))
            {
                var S = Math.Sqrt(1.0 + mat.M11 - mat.M22 - mat.M33) * 2;
                qw = (mat.M23 - mat.M32) / S;
                qx = 0.25 * S;
                qy = (mat.M21 + mat.M12) / S;
                qz = (mat.M31 + mat.M13) / S;
            }
            else if (mat.M22 > mat.M33)
            {
                var S = Math.Sqrt(1.0 + mat.M22 - mat.M11 - mat.M33) * 2;
                qw = (mat.M31 - mat.M13) / S;
                qx = (mat.M21 + mat.M12) / S;
                qy = 0.25 * S;
                qz = (mat.M32 + mat.M23) / S;
            }
            else
            {
                var S = Math.Sqrt(1.0 + mat.M33 - mat.M11 - mat.M22) * 2;
                qw = (mat.M12 - mat.M21) / S;
                qx = (mat.M31 + mat.M13) / S;
                qy = (mat.M32 + mat.M23) / S;
                qz = 0.25 * S;
            }

            return new Quaternion(qx, qy, qz, qw);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ODESolverData
    {
        public Matrix3D tensor;
        public Matrix3D invertTensor;

        public ODESolverData(Matrix3D tensor, Matrix3D invertTensor)
        {
            this.tensor = tensor;
            this.invertTensor = invertTensor;
        }
    }
}

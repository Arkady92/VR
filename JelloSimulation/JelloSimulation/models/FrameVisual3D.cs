using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using JelloSimulation.Physics;
using HelixToolkit.Wpf;

namespace JelloSimulation.Models
{
    public class FrameVisual3D : BaseVisual3D, IFrameVisual3D
    {
        private double cubeLength;
        public LinesVisual3D joints { get; set; }
        public Vector3D[,,] jointForces;
        private Point3D[, ,] controlPointsOrigin;


        public override void Initialize(double cubeSize)
        {
            base.N = 2;
            base.Initialize();
            cubeLength = cubeSize;
            joints = new LinesVisual3D();
            jointForces = new Vector3D[N,N,N];
            controlPointsOrigin = new Point3D[N, N, N];
            double x, y, z;
            double halfCubeLength = cubeLength/2;
            for (int i = 0; i < N; i++)
            {
                z = i * cubeLength - halfCubeLength;
                for (int j = 0; j < N; j++)
                {
                    y = j * cubeLength - halfCubeLength;
                    for (int k = 0; k < N; k++)
                    {
                        x = k * cubeLength - halfCubeLength;
                        controlPoints[i,j,k] = new Point3D(x, y, z);
                        controlPointsOrigin[i, j, k] = new Point3D(x, y, z);
                    }
                }
            }
            points.Points = GetControlPoints();
            lines.Points = GetControlLines();
        }

        public void SetFrameCenter(Point3D center)
        {
            double x, y, z;
            double halfCubeLength = cubeLength/2;
            for (int i = 0; i < N; i++)
            {
                z = i*cubeLength - halfCubeLength + center.Z;
                for (int j = 0; j < N; j++)
                {
                    y = j*cubeLength - halfCubeLength + center.Y;
                    for (int k = 0; k < N; k++)
                    {
                        x = k*cubeLength - halfCubeLength + center.X;
                        controlPoints[i, j, k] = new Point3D(x, y, z);
                    }
                }
            }
        }

        public void SetTransform(Transform3D transform)
        {
            double x, y, z;
            double halfCubeLength = cubeLength / 2;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < N; k++)
                    {
                        controlPoints[i, j, k] = transform.Transform(controlPointsOrigin[i, j, k]);
                    }
                }
            }
        }

        public LinesVisual3D GetJointsPoints(Vector3D[, ,] cornerPoints)
        {
            IList<Point3D> lines = new List<Point3D>();
            Vector3D p;

            lines.Add(controlPoints[0,0,0]);
            p = cornerPoints[0, 0, 0];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[0, 0, 1]);
            p = cornerPoints[0, 0, 1];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[0, 1, 0]);
            p = cornerPoints[0, 1, 0];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[0, 1, 1]);
            p = cornerPoints[0, 1, 1];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[1, 0, 0]);
            p = cornerPoints[1, 0, 0];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[1, 0, 1]);
            p = cornerPoints[1, 0, 1];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[1, 1, 0]);
            p = cornerPoints[1, 1, 0];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            lines.Add(controlPoints[1, 1, 1]);
            p = cornerPoints[1, 1, 1];
            lines.Add(new Point3D(p.X, p.Y, p.Z));

            joints.Points = lines;
            return joints;
        }

        public Point3D[,,] GetFramePoints()
        {
            return controlPoints;
        }
    }
}

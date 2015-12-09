using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace JelloSimulation.Models
{
    public class RoomVisual3D : BaseVisual3D, IRoomVisual3D
    {
        private double cubeLength;
        public double roomSize { get; set; }
        public bool IsDampingActive { get; set; }

        public override void Initialize(double cubeSize)
        {
            roomSize = cubeSize;
            IsDampingActive = true;
            double halfRoomSize = roomSize/2;
            double halfRoomBiggerSize = (roomSize * 1.5) / 2;

            base.N = 2;
            base.Initialize();
            cubeLength = cubeSize;
            double cubeBiggerLength = cubeSize * 1.5;
            double x, y, z;
            double halfCubeLength = cubeLength / 2;
            double halfCubeBiggerLength = cubeBiggerLength / 2;
            for (int i = 0; i < N; i++)
            {
                z = i * cubeLength - halfCubeLength;
                for (int j = 0; j < N; j++)
                {
                    y = j * cubeBiggerLength - halfCubeBiggerLength;
                    for (int k = 0; k < N; k++)
                    {
                        x = k * cubeBiggerLength - halfCubeBiggerLength;
                        controlPoints[i, j, k] = new Point3D(x, y, z);
                    }
                }
            }
            points.Points = GetControlPoints();
            lines.Points = GetControlLines();
        }

        public bool CheckCollision(Vector3D P, ref Vector3D v)
        {
            double b = roomSize / 2;
            double a = roomSize * 1.5 / 2;
            double eps = 0;
            double d;

            if (IsDampingActive)
                d = 0.1;
            else
            {
                d = 1;
            }
            if (P.X <= -a + eps)// || P.X >= a - eps)
            {
                v.X = d * Math.Abs(v.X);
                return true;
            }
            if (P.X >= a - eps)
            {
                v.X = -d * Math.Abs(v.X);
                return true;
            }
            if (P.Y <= -a + eps) //|| P.Y >= a - eps)
            {
                v.Y = Math.Abs(v.Y);
                v = v * d;
                return true;
            }
            if (P.Y >= a - eps)
            {
                v.Y = -Math.Abs(v.Y);
                v = v * d;
                return true;
            }
            if (P.Z <= -b + eps )//|| P.Z >= a - eps)
            {
                v.Z = Math.Abs(v.Z);
                v = v * d;
                return true;
            }
            if (P.Z >= b - eps)
            {
                v.Z = -Math.Abs(v.Z);
                v = v * d;
                return true;
            }
            return false;
        }

        public void TrimPoint(ref Vector3D P)
        {
            double b = roomSize / 2;
            double a = roomSize * 1.5 / 2;
            if (P.X < -a)
            {
                P.X = -a;
            }
            if (P.X > a)
            {
                P.X = a;
            }
            if (P.Y < -a)
            {
                P.Y = -a;
            }
            if (P.Y > a)
            {
                P.Y = a;
            }
            if (P.Z < -b)
            {
                P.Z = -b;
            }
            if (P.Z > b)
            {
                P.Z = b;
            }
        }
    }
}

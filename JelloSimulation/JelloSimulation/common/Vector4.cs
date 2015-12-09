using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloSimulation.Common
{
    public class Vector4
    {
        public double X
        {
            get { return PointsArray[0]; }
            set { PointsArray[0] = value; }
        }

        public double Y
        {
            get { return PointsArray[1]; }
            set { PointsArray[1] = value; }
        }

        public double Z
        {
            get { return PointsArray[2]; }
            set { PointsArray[2] = value; }
        }

        public double W
        {
            get { return PointsArray[3]; }
            set { PointsArray[3] = value; }
        }

        public double[] PointsArray;

        public void NormalizeSecond()
        {
            var norm = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (Math.Abs(norm) < Double.Epsilon) return;
            X /= norm;
            Z /= norm;
            Y /= norm;
        }

        public void NormalizeW()
        {
            for (int i = 0; i < 4; i++)
            {
                PointsArray[i] /= W;
            }
        }

        public Vector4()
        {
            PointsArray = new double[4];
        }

        public Vector4(double x, double y, double z, double w = 1)
        {
            PointsArray = new double[4];
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector4 vector)
        {
            PointsArray = new double[4];
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public static Vector4 Zero()
        {
            return new Vector4(0, 0, 0);
        }

        public static Vector4 Center(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector2.X / 2 + vector1.X / 2, vector2.Y / 2 + vector1.Y / 2, vector2.Z / 2 + vector1.Z / 2);
        }

        public static Vector4 OneThird(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector2.X / 3 + vector1.X * 2 / 3, vector2.Y / 3 + vector1.Y * 2 / 3, vector2.Z / 3 + vector1.Z * 2 / 3);
        }

        public static double Distance3(Vector4 vector1, Vector4 vector2)
        {
            return Math.Sqrt((vector1.X - vector2.X) * (vector1.X - vector2.X) +
                (vector1.Y - vector2.Y) * (vector1.Y - vector2.Y) +
                (vector1.Z - vector2.Z) * (vector1.Z - vector2.Z));
        }

        public static double Distance2(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static Vector4 operator +(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z, vector1.W + vector2.W);
        }

        public static Vector4 operator -(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z, vector1.W - vector2.W);
        }

        public static Vector4 operator *(Matrix4 matrix, Vector4 vector)
        {
            var result = new Vector4();
            for (int i = 0; i < 4; i++)
            {
                result.X += matrix[0, i] * vector.PointsArray[i];
                result.Y += matrix[1, i] * vector.PointsArray[i];
                result.Z += matrix[2, i] * vector.PointsArray[i];
                result.W += matrix[3, i] * vector.PointsArray[i];
            }
            return result;
        }

        public static Vector4 operator *(Vector4 vector, Matrix4 matrix)
        {
            var result = new Vector4();
            for (int i = 0; i < 4; i++)
            {
                result.X += matrix[i, 0] * vector.PointsArray[i];
                result.Y += matrix[i, 1] * vector.PointsArray[i];
                result.Z += matrix[i, 2] * vector.PointsArray[i];
                result.W += matrix[i, 3] * vector.PointsArray[i];
            }
            return result;
        }

        public static double operator *(Vector4 vector, Vector4 vector2)
        {
            double result = 0;
            for (int i = 0; i < 4; i++)
                result += vector.PointsArray[i] * vector2.PointsArray[i];
            return result;
        }

        public static Vector4 LinearMultiply(Vector4 vector, Vector4 vector2)
        {
            Vector4 result = Vector4.Zero();
            for (int i = 0; i < 4; i++)
                result.PointsArray[i] += vector.PointsArray[i] * vector2.PointsArray[i];
            return result;
        }

        public static Vector4 operator *(Vector4 vector, double value)
        {
            return new Vector4(vector.X * value, vector.Y * value, vector.Z * value, vector.W * value);
        }

        public void NormalizeFirst()
        {
            var sum = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            if (Math.Abs(sum) < Double.Epsilon) return;
            X /= sum;
            Z /= sum;
            Y /= sum;
        }

        public Vector4 Clone()
        {
            return new Vector4(X, Y, Z, W);
        }
    }
}

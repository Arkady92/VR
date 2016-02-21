using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using JelloSimulation.Physics;
using JelloSimulation.Common;
using JelloSimulation.Models;

namespace JelloSimulation.Models
{
    public interface IBezierCubeVisual3D : IVisual3dProvidable
    {
        ISpring spring { get; set; }

        ISpring springFrame { get; set; }

        double X0Max { get; set; }

        double V0Max { get; set; }

        ICollisionChecker CollisionChecker { get; set; }

        void Update();

        Vector3D[,,] GetCornerPoints();

        void CalculateJointForces(Point3D[,,] framePoints);

        IList<Point3D> GetFaceControlPoints(int faceNumber);
        IList<Point3D> GetSpherePoints();

        bool IsDampingActive { get; set; }
    }
}

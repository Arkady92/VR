using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using JelloSimulation.Common;

namespace JelloSimulation.Models
{
    public interface IFrameVisual3D : IVisual3dProvidable
    {
        LinesVisual3D GetJointsPoints(Vector3D[, ,] cornerPoints);

        Point3D[,,] GetFramePoints();

        void SetFrameCenter(Point3D center);

        void SetTransform(Transform3D transform);
    }
}

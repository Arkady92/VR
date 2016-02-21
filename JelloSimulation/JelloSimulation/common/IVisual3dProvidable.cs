using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace JelloSimulation.Common
{
    public interface IVisual3dProvidable
    {
        PointsVisual3D points { get; set; }
        LinesVisual3D lines{ get; set; }

        void Initialize(double cubeSize);

        IList<Point3D> GetControlPoints();
        IList<Point3D> GetControlLines();
    }
}

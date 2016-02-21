using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using HelixToolkit.Wpf;
using JelloSimulation.Common;

namespace JelloSimulation.Models
{
    public interface IRoomVisual3D : ICollisionChecker, IVisual3dProvidable
    {
        double roomSize { get; set; }

        bool IsDampingActive { get; set; }

        void Initialize(double size);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PrototypeLayoutTesting
{
    internal class GradientBrushData
    {
        public string Name { get; set; }

        public GradientType Type { get; set; }

        public GradientStopCollection Stops { get; set; }

        public enum GradientType
        {
            Linear,
            Radial
        }
    }

    internal class LinearGradientBrushData : GradientBrushData
    {
        public Point StartLocation { get; set; }

        public Point EndLocation { get; set; }

        public LinearGradientBrushData()
        {
            Stops = new GradientStopCollection();
        }
    }

    internal class RadialGradientBrushData : GradientBrushData
    {
        public Point CenterLocation { get; set; }

        public double Radius { get; set; }

        public MatrixTransform MatrixTransform { get; set; }

        public RadialGradientBrushData()
        {
            Stops = new GradientStopCollection();
        }
    }
}

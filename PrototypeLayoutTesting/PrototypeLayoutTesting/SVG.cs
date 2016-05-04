using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PrototypeLayoutTesting
{
    internal class Svg
    {
        public string Name { get; set; }

        public List<Layer> Layers { get; set; }
        public List<LinearGradientBrushData> LinearGradients { get; set; }
        public List<RadialGradientBrushData> RadialGradients { get; set; }

        public Metadata Metadata { get; set; }

        public Svg()
        {
            Layers = new List<Layer>();
            LinearGradients = new List<LinearGradientBrushData>();
            RadialGradients = new List<RadialGradientBrushData>();
        }
    }

    internal class Metadata
    {
        public DateTime CreationDate { get; set; }

        public DateTime DateLastEdited { get; set; }

        public string FilePath { get; set; }
    }

    internal class Layer
    {
        public string Name { get; set; }

        public List<XElement> Paths { get; set; }
    }
}

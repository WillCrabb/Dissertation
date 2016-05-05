using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace PrototypeLayoutTesting
{
    class Model
    {
        public Svg Svg { get; set; }

        /// <summary>
        /// Extracts the XML from the SVG of the filepath passed in
        /// </summary>
        /// <param name="filePath">Location of the file to be extracted</param>
        public void ExtractSvgData(string filePath)
        {
            XDocument svgToExtract = XDocument.Load(filePath);

            Svg = new Svg
            {
                Name = filePath,
                Metadata = new Metadata
                {
                    FilePath = filePath
                }
            };

            List<XElement> layers =
                svgToExtract.Descendants()
                    .Where(a => a.Name.LocalName == "g")
                    .Where(e => e.Attribute("id") != null)
                    .ToList();

            foreach (var layer in layers)
            {
                foreach (var linearGradient in layer.Elements().Where(e => e.Name.LocalName == "linearGradient"))
                {
                    LinearGradientBrushData linearGradientBrushData = new LinearGradientBrushData();

                    linearGradientBrushData.Type = LinearGradientBrushData.GradientType.Linear;

                    linearGradientBrushData.Name = linearGradient.Attribute("id").Value.ToString();
                    linearGradientBrushData.StartLocation = new Point(double.Parse(linearGradient.Attribute("x1").Value),
                                                                double.Parse(linearGradient.Attribute("y1").Value));

                    linearGradientBrushData.EndLocation = new Point(double.Parse(linearGradient.Attribute("x2").Value),
                                                              double.Parse(linearGradient.Attribute("y2").Value));
                    foreach (XElement gradientStop in linearGradient.Descendants())
                    {
                        var value = gradientStop.Attribute("style").Value;
                        var index = value.IndexOf('#');
                        var theColor = value.Substring(index);

                        var color = (Color)ColorConverter.ConvertFromString(theColor);
                        var offset = double.Parse(gradientStop.Attribute("offset").Value);

                        linearGradientBrushData.Stops.Add(new GradientStop(color, offset));
                    }

                    Svg.LinearGradients.Add(linearGradientBrushData);
                }
                foreach (var radialGradient in layer.Elements().Where(e => e.Name.LocalName == "radialGradient"))
                {
                    RadialGradientBrushData radialGradientBrushData = new RadialGradientBrushData();

                    radialGradientBrushData.Type = RadialGradientBrushData.GradientType.Radial;

                    radialGradientBrushData.Name = radialGradient.Attribute("id").Value.ToString();
                    radialGradientBrushData.CenterLocation = new Point(double.Parse(radialGradient.Attribute("cx").Value),
                                                                double.Parse(radialGradient.Attribute("cy").Value));

                    radialGradientBrushData.Radius = double.Parse(radialGradient.Attribute("r").Value);
                    foreach (XElement gradientStop in radialGradient.Descendants())
                    {
                       var color = new Color();

                       if (gradientStop.Attribute("style").Value.Contains(";"))
                       {
                          var attirbutes = gradientStop.Attribute("style").Value.Split(';');

                          var value = attirbutes[0];
                          var index = value.IndexOf('#');
                          var theColor = value.Substring(index);
                          color = (Color)ColorConverter.ConvertFromString(theColor);

                          value = attirbutes[1];
                          index = value.IndexOf(':');
                          var theAlpha = double.Parse(value.Substring(index + 1));

                          color.A = theAlpha == 0 ? (byte) 0 : (byte) (255*theAlpha);
                       }
                       else
                       {
                          var value = gradientStop.Attribute("style").Value;
                          var index = value.IndexOf('#');
                          var theColor = value.Substring(index);

                          color = (Color)ColorConverter.ConvertFromString(theColor); 
                       }

                      
                        var offset = double.Parse(gradientStop.Attribute("offset").Value);
                        radialGradientBrushData.Stops.Add(new GradientStop(color, offset));

          
                    }

                    if (radialGradient.Attribute("gradientTransform") != null)
                    {
                        var matrixTransformationValues = Helper.GetMatrixValuesFromXml(radialGradient, Helper.MatrixType.GradientMatrix);

                        MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                            double.Parse(matrixTransformationValues[1]),
                            double.Parse(matrixTransformationValues[2]),
                            double.Parse(matrixTransformationValues[3]),
                            double.Parse(matrixTransformationValues[4]),
                            double.Parse(matrixTransformationValues[5]));

                        radialGradientBrushData.MatrixTransform = matrixTransform;
                    }

                    Svg.RadialGradients.Add(radialGradientBrushData);
                }
               
                Svg.Layers.Add(new Layer{Name = layer.Attribute("id").Value.ToString(), Paths = layer.Elements().Where(e => e.Name.LocalName != "linearGradient").ToList()});
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;


namespace PrototypeLayoutTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel m_viewModel;

        public MainWindow()
        {
            InitializeComponent();

            m_viewModel = new ViewModel();
            this.DataContext = m_viewModel;

        }

        private List<XElement> m_dataToUse;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        //    // Look for layers with names that start with screen.
        //    // When we find one, use the background rectangle to create a window of the same size containing a canvas
        //    // Look within the layer for a set of names with valid prefixes e.g. ButtonNext
        //    // Hook up events based on event postfixes e.g. ButtonNext-Click-Open-Screen2

        //    OpenFileDialog dialog = new OpenFileDialog();
        //    dialog.Filter = "SVG files (*.svg*)|*.svg*";

        //    if (dialog.ShowDialog().Value)
        //    {
        //        XDocument svg = XDocument.Load(dialog.FileName);
        //        m_dataToUse = svg.Descendants().Where(a => a.Name.LocalName == "g").Descendants().Distinct().ToList();

        //        // Look for the rect called Bounds - Everything drawn will be relative to this.
        //        XElement boundsElement = m_dataToUse.First(b => b.Attribute("id").Value == "Bounds");

        //        LayoutRoot.Width = double.Parse(boundsElement.Attribute("width").Value);
        //        LayoutRoot.Height = double.Parse(boundsElement.Attribute("height").Value);
        //        LayoutRoot.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(boundsElement.Attribute("fill").Value));

        //        Canvas canvas = new Canvas();

        //        // We no longer have use for the bounds
        //        m_dataToUse.Remove(boundsElement);

        //        foreach (XElement element in m_dataToUse)
        //        {
        //            if (element.Name.LocalName == "rect")
        //            {
        //                Rectangle rectangleToAdd = new Rectangle
        //                {
        //                    Width = double.Parse(element.Attribute("width").Value),
        //                    Height = double.Parse(element.Attribute("height").Value),
        //                    Margin = new Thickness(double.Parse(element.Attribute("x").Value), double.Parse(element.Attribute("y").Value), 0, 0),
        //                    Fill = GetBrushFromXElement(element),
        //                    Opacity = GetOpacityFromXElement(element)
        //                };

        //                string tooltipString = "Fill = " + element.Attribute("fill").Value;
        //                rectangleToAdd.ToolTip = tooltipString;

        //                canvas.Children.Add(rectangleToAdd);
        //            }
        //            else if (element.Name.LocalName == "circle")
        //            {
        //                Ellipse ellipseToAdd = new Ellipse
        //                {
        //                    Width = double.Parse(element.Attribute("r").Value) * 2,
        //                    Height = double.Parse(element.Attribute("r").Value) * 2,
        //                    Margin = new Thickness(double.Parse(element.Attribute("cx").Value) - double.Parse(element.Attribute("r").Value), double.Parse(element.Attribute("cy").Value) - double.Parse(element.Attribute("r").Value), 0, 0),
        //                    Fill = GetBrushFromXElement(element),
        //                    Opacity = GetOpacityFromXElement(element)
        //                };

        //                string tooltipString = "Fill = " + element.Attribute("fill").Value;
        //                ellipseToAdd.ToolTip = tooltipString;

        //                canvas.Children.Add(ellipseToAdd);
        //            }
        //            else if (element.Name.LocalName == "path")
        //            {
        //                Path pathToAdd = new Path
        //                {
        //                    Data = Geometry.Parse(element.Attribute("d").Value),
        //                    Fill = GetBrushFromXElement(element),
        //                    Opacity = GetOpacityFromXElement(element)
        //                };

        //                string tooltipString = "Fill = " + element.Attribute("fill").Value;
        //                pathToAdd.ToolTip = tooltipString;

        //                canvas.Children.Add(pathToAdd);
        //            }
        //            else if (element.Name.LocalName == "text")
        //            {
        //                string matrixString = element.Attribute("transform").Value;
        //                string[] allData = matrixString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        //                double xLoc = double.Parse(allData.GetValue(allData.Count() - 2).ToString());
        //                double yLoc = double.Parse(allData.GetValue(allData.Count() - 1).ToString().Replace(")", ""));

        //                TextBlock textToAdd = new TextBlock
        //                {
        //                    Foreground = GetBrushFromXElement(element),
        //                    FontFamily = new FontFamily("Segoe UI"),
        //                    Text = element.Value,
        //                    FontSize = GetFontSizeFromXElement(element),
                           
        //                    Opacity = GetOpacityFromXElement(element)
        //                };


        //                var formattedText = new FormattedText(textToAdd.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
        //new Typeface(textToAdd.FontFamily, textToAdd.FontStyle, textToAdd.FontWeight, textToAdd.FontStretch),
        //textToAdd.FontSize,
        //Brushes.Black);

        //                yLoc = yLoc - (formattedText.Height * 0.8);
        //                textToAdd.Margin = new Thickness(xLoc, yLoc, 0, 0);


        //                string tooltipString = "Fill = " + element.Attribute("fill").Value;
        //                textToAdd.ToolTip = tooltipString;

        //                canvas.Children.Add(textToAdd);
        //            }
        //        }

        //        LayoutRoot.Children.Add(canvas);
            //}
        }

        #region Methods

        private Brush GetBrushFromXElement(XElement element)
        {
            if (element.Attribute("fill").Value.Contains("url"))
            {
                return GetLinearGradientBrushFromXElement(element);
            }
            else
            {
                return GetSolidBrushFromXElement(element);
            }
        }

        private SolidColorBrush GetSolidBrushFromXElement(XElement element)
        {
            SolidColorBrush returnBrush = new SolidColorBrush();
            returnBrush.Color = (Color)ColorConverter.ConvertFromString(element.Attribute("fill").Value);

            return returnBrush;
        }

        private LinearGradientBrush GetLinearGradientBrushFromXElement(XElement element)
        {
            LinearGradientBrush returnBrush = new LinearGradientBrush();
            returnBrush.MappingMode = BrushMappingMode.Absolute;

            string fillID = element.Attribute("fill").Value;
            fillID = GetSubstringByString("#", ")", fillID);

            var linearGradient = m_dataToUse.Where(e => e.Name.LocalName == "linearGradient")
                                            .First(e => e.Attribute("id").Value == fillID);

            // pass in the rectangle the fill is being applied to
            returnBrush.StartPoint = new Point(0, 0);

            double startX = double.Parse(linearGradient.Attribute("x1").Value);
            double startY = double.Parse(linearGradient.Attribute("y1").Value);
            double endX = double.Parse(linearGradient.Attribute("x2").Value);
            double endY = double.Parse(linearGradient.Attribute("y2").Value);

            if (endX > startX)
            {
                endX = endX - startX;
            }
            else 
            {
                endX = startX - endX;
            }

            if (endY > startY)
            {
                endY = endY - startY;
            }
            else
            {
                endY = startY - endY;
            }

            returnBrush.EndPoint = new Point(endX, endY);

            if (double.Parse(linearGradient.Attribute("y2").Value) < double.Parse(linearGradient.Attribute("y1").Value) ||
                double.Parse(linearGradient.Attribute("x2").Value) < double.Parse(linearGradient.Attribute("x1").Value))
            {
                foreach (XElement gradientStop in linearGradient.Descendants())
                {
                    var value = gradientStop.Attribute("style").Value;
                    var index = value.IndexOf('#');
                    var theColor = value.Substring(index);

                    var color = (Color)ColorConverter.ConvertFromString(theColor);
                    var offset = 1 - double.Parse(gradientStop.Attribute("offset").Value);

                    returnBrush.GradientStops.Add(new GradientStop(color, offset));
                }
            }

            else
            {
                foreach (XElement gradientStop in linearGradient.Descendants())
                {
                    var value = gradientStop.Attribute("style").Value;
                    var index = value.IndexOf('#');
                    var theColor = value.Substring(index);

                    var color = (Color)ColorConverter.ConvertFromString(theColor);
                    var offset = double.Parse(gradientStop.Attribute("offset").Value);

                    returnBrush.GradientStops.Add(new GradientStop(color, offset));
                }
            }
            return returnBrush;
        }

        private double GetFontSizeFromXElement(XElement element)
        {
            return double.Parse(element.Attribute("font-size").Value.Replace("px", ""));
        }

        private double GetOpacityFromXElement(XElement element)
        {
            if (element.Attribute("opacity") != null)
            {
                return double.Parse(element.Attribute("opacity").Value);
            }

            return 1.0;
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        #endregion Methods


        private void btnFileDialog_Click(object sender, RoutedEventArgs e)
        {
            m_viewModel.OpenFileDialog();
        }
    }
}

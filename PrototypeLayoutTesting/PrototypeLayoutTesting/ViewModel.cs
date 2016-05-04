using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace PrototypeLayoutTesting
{
   class ViewModel : DependencyObject, IDisposable
   {
      Model m_model;


      public static readonly DependencyProperty LayersProperty = DependencyProperty.Register(
          "Layers", typeof(ObservableCollection<UIElement>), typeof(ViewModel), new PropertyMetadata(new ObservableCollection<UIElement>()));

      public ObservableCollection<UIElement> Layers
      {
         get { return (ObservableCollection<UIElement>)GetValue(LayersProperty); }
         set { SetValue(LayersProperty, value); }
      }

      public static readonly DependencyProperty CurrentSelectedShapeProperty = DependencyProperty.Register(
         "CurrentSelectedShape", typeof(string), typeof(ViewModel), new PropertyMetadata(default(string)));

      public string CurrentSelectedShape
      {
         get { return (string)GetValue(CurrentSelectedShapeProperty); }
         set { SetValue(CurrentSelectedShapeProperty, value); }
      }

      public ViewModel()
      {
         m_model = new Model();
      }



      public void OpenFileDialog()
      {
         OpenFileDialog dialog = new OpenFileDialog();

         if (dialog.ShowDialog().Value)
         {
            if (Path.GetExtension(dialog.FileName) == ".svg")
            {
               m_model.GetSvg(dialog.FileName);
            }

            DisplayPathsOnForm();
         }
      }
    
      private void DisplayPathsOnForm()
      {
         foreach (var layer in m_model.Svg.Layers)
         {
            foreach (var path in layer.Paths)
            {
               switch (path.Name.LocalName)
               {
                  case "ellipse":
                     {
                        Layers.Add(ConvertPathToEllipse(path));
                        break;
                     }
                  case "circle":
                     {
                        Layers.Add(ConvertPathToCircle(path));
                        break;
                     }
                  case "path":
                     {
                        Layers.Add(ConvertCustomPathToPath(path));
                        break;
                     }
                  case "polygon":
                     {
                        Layers.Add(ConvertPathToPolygon(path));
                        break;
                     }
                  case "rect":
                     {
                        Layers.Add(ConvertPathToRectangle(path));
                        break;
                     }
                  case "text":
                     {
                        Layers.Add(ConvertTextToLabel(path));
                        break;
                     }

               }
            }
         }
      }

      private UIElement ConvertTextToLabel(XElement text)
      {
         OutlinedText outlinedText = new OutlinedText();



         if (text.Attribute("fill") != null)
         {
            outlinedText.Fill = GetBrushFromXElement(text, "fill");
         }
         else
         {
            outlinedText.Fill = Brushes.Black;
         }




         //                double xLoc = double.Parse(allData.GetValue(allData.Count() - 2).ToString());
         //                double yLoc = double.Parse(allData.GetValue(allData.Count() - 1).ToString().Replace(")", ""));


         if (text.Attribute("stroke") != null)
         {
            outlinedText.Stroke = GetBrushFromXElement(text, "stroke");
            outlinedText.StrokeThickness = 1;
         }

         if (text.Attribute("stroke-width") != null)
         {
            outlinedText.StrokeThickness = ushort.Parse(text.Attribute("stroke-width").Value);
         }

         if (text.Attribute("font-size") != null)
         {
            outlinedText.FontSize = double.Parse(text.Attribute("font-size").Value);
         }

         if (text.Attribute("font-family") != null)
         {
            var font = text.Attribute("font-family").Value.Split(new Char[] { '/' },
                StringSplitOptions.RemoveEmptyEntries);

            outlinedText.Font = new FontFamily(font[0]);
         }
         else
         {
            outlinedText.Font = new FontFamily("Segoe UI");

         }

         outlinedText.Text = text.Value;

         if (text.Attribute("transform") != null)
         {
            var matrixTransformationValues = Helper.GetMatrixValuesFromXml(text, Helper.MatrixType.PathMatrix);

            MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                double.Parse(matrixTransformationValues[1]),
                double.Parse(matrixTransformationValues[2]),
                double.Parse(matrixTransformationValues[3]),
                double.Parse(matrixTransformationValues[4]),
                double.Parse(matrixTransformationValues[5]));




            double xLoc = double.Parse(matrixTransformationValues[4]);
            double yLoc = double.Parse(matrixTransformationValues[5]) * 0.8;
       

            outlinedText.RenderTransform = matrixTransform;
         }

         outlinedText.Opacity = GetOpacityFromXElement(text);


         return outlinedText;


         //                var formattedText = new FormattedText(textToAdd.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
         //new Typeface(textToAdd.FontFamily, textToAdd.FontStyle, textToAdd.FontWeight, textToAdd.FontStretch),
         //textToAdd.FontSize,
         //Brushes.Black);

         //                yLoc = yLoc - (formattedText.Height * 0.8);
         //                textToAdd.Margin = new Thickness(xLoc, yLoc, 0, 0);


         //                string tooltipString = "Fill = " + element.Attribute("fill").Value;
         //                textToAdd.ToolTip = tooltipString;

         //                canvas.Children.Add(textToAdd);
      }

      private UIElement ConvertCustomPathToPath(XElement path)
      {
         System.Windows.Shapes.Path customPath = new System.Windows.Shapes.Path();

         var geometryData = new PathGeometry();

         if (path.Attribute("transform") != null)
         {
            var matrixTransformationValues = Helper.GetMatrixValuesFromXml(path, Helper.MatrixType.PathMatrix);

            MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                double.Parse(matrixTransformationValues[1]),
                double.Parse(matrixTransformationValues[2]),
                double.Parse(matrixTransformationValues[3]),
                double.Parse(matrixTransformationValues[4]),
                double.Parse(matrixTransformationValues[5]));

            geometryData.Transform = matrixTransform;
         }

         if (path.Attribute("d") != null)
         {
            geometryData.AddGeometry(Geometry.Parse(path.Attribute("d").Value));
         }
         if (path.Attribute("fill") != null)
         {
            customPath.Fill = GetBrushFromXElement(path, "fill");
         }
         else
         {
            customPath.Fill = Brushes.Black;
         }
         if (path.Attribute("stroke") != null)
         {
            customPath.Stroke = GetBrushFromXElement(path, "stroke");
         }
         if (path.Attribute("stroke-width") != null)
         {
            customPath.StrokeThickness = double.Parse(path.Attribute("stroke-width").Value);
         }


         customPath.Opacity = GetOpacityFromXElement(path);

         customPath.Data = geometryData;

         string fillData = string.Empty;

         Brush brush = customPath.Fill;

         if (brush is LinearGradientBrush || brush is RadialGradientBrush)
         {
             GradientBrush linearBrush = brush as GradientBrush;

             foreach (var gradientStop in linearBrush.GradientStops)
             {
                 fillData += "Stop: " + gradientStop.Offset + " Colour: " + gradientStop.Color + Environment.NewLine;
             }
         }
         else
         {
             fillData = brush.ToString();
         }

         customPath.Tag = "Data: " + customPath.Data + Environment.NewLine +
         "Stroke: " + customPath.Stroke + Environment.NewLine +
                           "Fill: " + fillData + Environment.NewLine;

         customPath.PreviewMouseUp += customPath_MouseUp;
        

         return customPath;
      }

      void customPath_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         var control = sender as FrameworkElement;
         CurrentSelectedShape = control.Tag.ToString();
      }

      private UIElement ConvertPathToRectangle(XElement path)
      {
         var rectanglePath = new System.Windows.Shapes.Path();

         var geometryData = new RectangleGeometry();


         if ((path.Attribute("x") != null) && (path.Attribute("y") != null) &&
             (path.Attribute("width") != null) && (path.Attribute("height") != null))
         {
            geometryData.Rect = new Rect
            {
               Location = new Point(double.Parse(path.Attribute("x").Value),
                                    double.Parse(path.Attribute("y").Value)),
               Width = double.Parse(path.Attribute("width").Value),
               Height = double.Parse(path.Attribute("height").Value)
            };
         }

         if (path.Attribute("transform") != null)
         {
            var matrixTransformationValues = Helper.GetMatrixValuesFromXml(path, Helper.MatrixType.PathMatrix);

            MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                double.Parse(matrixTransformationValues[1]),
                double.Parse(matrixTransformationValues[2]),
                double.Parse(matrixTransformationValues[3]),
                double.Parse(matrixTransformationValues[4]),
                double.Parse(matrixTransformationValues[5]));

            geometryData.Transform = matrixTransform;
         }

         rectanglePath.Data = geometryData;

         if (path.Attribute("fill") != null)
         {

            rectanglePath.Fill = GetBrushFromXElement(path, "fill");
         }
         else
         {
            rectanglePath.Fill = Brushes.Black;
         }
         if (path.Attribute("stroke") != null)
         {
            rectanglePath.Stroke = GetBrushFromXElement(path, "stroke");
         }
         if (path.Attribute("stroke-width") != null)
         {
            rectanglePath.StrokeThickness = double.Parse(path.Attribute("stroke-width").Value);
         }

         rectanglePath.Opacity = GetOpacityFromXElement(path);

         return rectanglePath;
      }

      private UIElement ConvertPathToCircle(XElement path)
      {
         System.Windows.Shapes.Path circlePath = new System.Windows.Shapes.Path();

         var geometryData = new EllipseGeometry();

         if ((path.Attribute("cx") != null) && (path.Attribute("cy") != null))
         {
            geometryData.Center = new Point(double.Parse(path.Attribute("cx").Value),
                double.Parse(path.Attribute("cy").Value));
         }

         if (path.Attribute("r") != null)
         {
            geometryData.RadiusX = double.Parse(path.Attribute("r").Value);
            geometryData.RadiusY = double.Parse(path.Attribute("r").Value);
         }

         if (path.Attribute("transform") != null)
         {
            var matrixTransformationValues = Helper.GetMatrixValuesFromXml(path, Helper.MatrixType.PathMatrix);

            MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                double.Parse(matrixTransformationValues[1]),
                double.Parse(matrixTransformationValues[2]),
                double.Parse(matrixTransformationValues[3]),
                double.Parse(matrixTransformationValues[4]),
                double.Parse(matrixTransformationValues[5]));

            geometryData.Transform = matrixTransform;
         }

         circlePath.Data = geometryData;

         if (path.Attribute("fill") != null)
         {
            circlePath.Fill = GetBrushFromXElement(path, "fill");
         }
         else
         {
            circlePath.Fill = Brushes.Black;
         }

         if (path.Attribute("stroke") != null)
         {
            circlePath.Stroke = GetBrushFromXElement(path, "stroke");
         }
         if (path.Attribute("stroke-width") != null)
         {
            circlePath.StrokeThickness = double.Parse(path.Attribute("stroke-width").Value);
         }

         circlePath.Opacity = GetOpacityFromXElement(path);

         return circlePath;
      }

      private UIElement ConvertPathToEllipse(XElement path)
      {
         System.Windows.Shapes.Path ellipsePath = new System.Windows.Shapes.Path();

         var geometryData = new EllipseGeometry();

         if ((path.Attribute("cx") != null) && (path.Attribute("cy") != null))
         {
            geometryData.Center = new Point(double.Parse(path.Attribute("cx").Value),
                double.Parse(path.Attribute("cy").Value));
         }

         if (path.Attribute("rx") != null)
         {
            geometryData.RadiusX = double.Parse(path.Attribute("rx").Value);
         }

         if (path.Attribute("ry") != null)
         {
            geometryData.RadiusY = double.Parse(path.Attribute("ry").Value);
         }

         if (path.Attribute("transform") != null)
         {
            var matrixTransformationValues = Helper.GetMatrixValuesFromXml(path, Helper.MatrixType.PathMatrix);

            MatrixTransform matrixTransform = new MatrixTransform(double.Parse(matrixTransformationValues[0]),
                double.Parse(matrixTransformationValues[1]),
                double.Parse(matrixTransformationValues[2]),
                double.Parse(matrixTransformationValues[3]),
                double.Parse(matrixTransformationValues[4]),
                double.Parse(matrixTransformationValues[5]));

            geometryData.Transform = matrixTransform;
         }

         ellipsePath.Data = geometryData;

         if (path.Attribute("fill") != null)
         {
            ellipsePath.Fill = GetBrushFromXElement(path, "fill");
         }
         else
         {
            ellipsePath.Fill = Brushes.Black;
         }

         if (path.Attribute("stroke") != null)
         {
            ellipsePath.Stroke = GetBrushFromXElement(path, "stroke");
         }
         if (path.Attribute("stroke-width") != null)
         {
            ellipsePath.StrokeThickness = double.Parse(path.Attribute("stroke-width").Value);
         }

         ellipsePath.Opacity = GetOpacityFromXElement(path);

         return ellipsePath;
      }

      private UIElement ConvertPathToPolygon(XElement path)
      {
         Polygon polygonPath = new Polygon();

         if (path.Attribute("points") != null)
         {
            polygonPath.Points = GetPointValuesFromXml(path);
         }

         if (path.Attribute("fill") != null)
         {
            polygonPath.Fill = GetBrushFromXElement(path, "fill");
         }
         else
         {
            polygonPath.Fill = Brushes.Black;
         }

         if (path.Attribute("stroke") != null)
         {
            polygonPath.Stroke = GetBrushFromXElement(path, "stroke");
         }
         if (path.Attribute("stroke-width") != null)
         {
            polygonPath.StrokeThickness = double.Parse(path.Attribute("stroke-width").Value);
         }

         polygonPath.Opacity = GetOpacityFromXElement(path);

         return polygonPath;
      }

      private PointCollection GetPointValuesFromXml(XElement path)
      {
         var points = path.Attribute("points").Value.Split(new Char[] { ',', ' ' },
                              StringSplitOptions.RemoveEmptyEntries);


         PointCollection pointCollection = new PointCollection();

         for (int index = 0; index < points.Length; index += 2)
         {

            pointCollection.Add(new Point(double.Parse(points[index]),
                                          double.Parse(points[index + 1])));
         }
         return pointCollection;
      }



      private Brush GetBrushFromXElement(XElement element, string attribute)
      {
         if (element.Attribute(attribute).Value.Contains("url"))
         {
            return GetLinearGradientBrushFromXElement(element, attribute);
         }
         else
         {
            return GetSolidBrushFromXElement(element, attribute);
         }
      }

      private SolidColorBrush GetSolidBrushFromXElement(XElement element, string attribute)
      {
         if (element.Attribute(attribute).Value == "none")
         {
            return new SolidColorBrush();
         }

         SolidColorBrush returnBrush = new SolidColorBrush();
         returnBrush.Color = (Color)ColorConverter.ConvertFromString(element.Attribute(attribute).Value);

         return returnBrush;
      }

      private double GetOpacityFromXElement(XElement element)
      {
         if (element.Attribute("opacity") != null)
         {
            return double.Parse(element.Attribute("opacity").Value);
         }

         return 1.0;
      }

      private GradientBrush GetLinearGradientBrushFromXElement(XElement element, string attribute)
      {

         if (m_model.Svg.LinearGradients.Any(e => element.Attribute(attribute).Value.Contains(e.Name)))
         {
            LinearGradientBrushData linearGradientBrushData =
                m_model.Svg.LinearGradients.First(e => element.Attribute(attribute).Value.Contains(e.Name));

            if (linearGradientBrushData.Type == LinearGradientBrushData.GradientType.Linear)
            {
               LinearGradientBrush linearGradient = new LinearGradientBrush(
               linearGradientBrushData.Stops, linearGradientBrushData.StartLocation, linearGradientBrushData.EndLocation)
               {
                  MappingMode = BrushMappingMode.Absolute
               };

               return linearGradient;
            }
         }
         else
         {
            RadialGradientBrushData radialGradientBrushData =
                m_model.Svg.RadialGradients.First(e => element.Attribute(attribute).Value.Contains(e.Name));

            if (radialGradientBrushData.Type == RadialGradientBrushData.GradientType.Radial)
            {
               RadialGradientBrush radialGradient = new RadialGradientBrush(radialGradientBrushData.Stops);
               radialGradient.Center = radialGradientBrushData.CenterLocation;
               radialGradient.GradientOrigin = radialGradientBrushData.CenterLocation;
               radialGradient.RadiusX = radialGradientBrushData.Radius;
               radialGradient.RadiusY = radialGradientBrushData.Radius;
               radialGradient.MappingMode = BrushMappingMode.Absolute;



               radialGradient.Transform = radialGradientBrushData.MatrixTransform;
               return radialGradient;
            }
         }

         //LinearGradientBrush returnBrush = new LinearGradientBrush();
         ////returnBrush.MappingMode = BrushMappingMode.Absolute;

         ////string fillID = element.Attribute("fill").Value;


         ////var linearGradient = m_dataToUse.Where(e => e.Name.LocalName == "linearGradient")
         ////                                .First(e => e.Attribute("id").Value == fillID);

         //// pass in the rectangle the fill is being applied to

         //double startX = double.Parse(element.Attribute("x1").Value);
         //double startY = double.Parse(element.Attribute("y1").Value);
         //double endX = double.Parse(element.Attribute("x2").Value);
         //double endY = double.Parse(element.Attribute("y2").Value);

         //returnBrush.StartPoint = new Point(startX, startY);
         //returnBrush.EndPoint = new Point(endX, endY);

         //foreach (XElement gradientStop in element.Descendants())
         //{
         //    var value = gradientStop.Attribute("style").Value;
         //    var index = value.IndexOf('#');
         //    var theColor = value.Substring(index);

         //    var color = (Color)ColorConverter.ConvertFromString(theColor);
         //    var offset = 1 - double.Parse(gradientStop.Attribute("offset").Value);

         //    returnBrush.GradientStops.Add(new GradientStop(color, offset));
         //}

         //if (endX > startX)
         //{
         //    endX = endX - startX;
         //}
         //else
         //{
         //    endX = startX - endX;
         //}

         //if (endY > startY)
         //{
         //    endY = endY - startY;
         //}
         //else
         //{
         //    endY = startY - endY;
         //}

         //returnBrush.EndPoint = new Point(endX, endY);

         //if (double.Parse(linearGradient.Attribute("y2").Value) < double.Parse(linearGradient.Attribute("y1").Value) ||
         //    double.Parse(linearGradient.Attribute("x2").Value) < double.Parse(linearGradient.Attribute("x1").Value))
         //{

         //}

         //else
         //{
         //    foreach (XElement gradientStop in linearGradient.Descendants())
         //    {
         //        var value = gradientStop.Attribute("style").Value;
         //        var index = value.IndexOf('#');
         //        var theColor = value.Substring(index);

         //        var color = (Color)ColorConverter.ConvertFromString(theColor);
         //        var offset = double.Parse(gradientStop.Attribute("offset").Value);

         //        returnBrush.GradientStops.Add(new GradientStop(color, offset));
         //    }
         //}
         return null;
      }

      public void Dispose()
      {
         throw new NotImplementedException();
      }
   }


}

using RG_PZ1.Commands;
using RG_PZ1.Models;
using RG_PZ1.Models.Enums;
using RG_PZ1.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Point = RG_PZ1.Models.Point;

namespace RG_PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Entities
        private List<CanvasPowerEntity> _powerEntities = new List<CanvasPowerEntity>();
        private List<CanvasLineEntity> _lineEntities = new List<CanvasLineEntity>();

        private readonly double _canvasWidth = 600;
        private readonly double _canvasHeight = 600;

        private readonly int _fieldWidth = 3;
        private readonly int _fieldHeight = 3;

        private bool[,] _usedFieldsMatrix = new bool[200, 200];
        private List<CanvasField> _usedFields = new List<CanvasField>();

        // Drawing
        private bool _isDrawingStarted = false;
        private DrawingType _drawingType = DrawingType.None;
        private Polygon _polygon = null;

        // History
        private List<IDrawCommand> _done = new List<IDrawCommand>();
        private List<IDrawCommand> _undone = new List<IDrawCommand>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadEntities()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("Geographic.xml");

            double newX, newY;

            // Substations
            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            List<CanvasPowerEntity> substationEntities = new List<CanvasPowerEntity>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                SubstationEntity substationEntity = new SubstationEntity();
                substationEntity.Id = long.Parse(xmlNode.SelectSingleNode("Id").InnerText);
                substationEntity.Name = xmlNode.SelectSingleNode("Name").InnerText;
                substationEntity.X = double.Parse(xmlNode.SelectSingleNode("X").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                substationEntity.Y = double.Parse(xmlNode.SelectSingleNode("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture);

                ToLatLon(substationEntity.X, substationEntity.Y, 34, out newX, out newY);
                substationEntity.X = newX;
                substationEntity.Y = newY;

                CanvasPowerEntity canvasSubstationEntity = new CanvasPowerEntity()
                {
                    PowerEntity = substationEntity,
                    CanvasChildIndex = -1
                };
                
                substationEntities.Add(canvasSubstationEntity);
                _powerEntities.Add(canvasSubstationEntity);
            }

            DrawPowerEntities(substationEntities);

            // Nodes
            xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            List<CanvasPowerEntity> nodeEntities = new List<CanvasPowerEntity>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                NodeEntity nodeEntity = new NodeEntity();
                nodeEntity.Id = long.Parse(xmlNode.SelectSingleNode("Id").InnerText);
                nodeEntity.Name = xmlNode.SelectSingleNode("Name").InnerText;
                nodeEntity.X = double.Parse(xmlNode.SelectSingleNode("X").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                nodeEntity.Y = double.Parse(xmlNode.SelectSingleNode("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture);

                ToLatLon(nodeEntity.X, nodeEntity.Y, 34, out newX, out newY);
                nodeEntity.X = newX;
                nodeEntity.Y = newY;

                CanvasPowerEntity canvasNodeEntity = new CanvasPowerEntity()
                {
                    PowerEntity = nodeEntity,
                    CanvasChildIndex = -1
                };

                nodeEntities.Add(canvasNodeEntity);
                _powerEntities.Add(canvasNodeEntity);
            }

            DrawPowerEntities(nodeEntities);

            // Switches
            xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            List<CanvasPowerEntity> switchEntities = new List<CanvasPowerEntity>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                SwitchEntity switchEntity = new SwitchEntity();
                switchEntity.Id = long.Parse(xmlNode.SelectSingleNode("Id").InnerText);
                switchEntity.Name = xmlNode.SelectSingleNode("Name").InnerText;
                switchEntity.X = double.Parse(xmlNode.SelectSingleNode("X").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                switchEntity.Y = double.Parse(xmlNode.SelectSingleNode("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                switchEntity.Status = xmlNode.SelectSingleNode("Status").InnerText;

                ToLatLon(switchEntity.X, switchEntity.Y, 34, out newX, out newY);
                switchEntity.X = newX;
                switchEntity.Y = newY;

                CanvasPowerEntity canvasSwitchEntity = new CanvasPowerEntity()
                {
                    PowerEntity = switchEntity,
                    CanvasChildIndex = -1
                };

                switchEntities.Add(canvasSwitchEntity);
                _powerEntities.Add(canvasSwitchEntity);
            }

            DrawPowerEntities(switchEntities);

            // Lines
            xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                LineEntity lineEntity = new LineEntity();
                lineEntity.Id = long.Parse(xmlNode.SelectSingleNode("Id").InnerText);
                lineEntity.Name = xmlNode.SelectSingleNode("Name").InnerText;
                lineEntity.IsUnderground = bool.Parse(xmlNode.SelectSingleNode("IsUnderground").InnerText);
                lineEntity.R = float.Parse(xmlNode.SelectSingleNode("R").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                lineEntity.ConductorMaterial = xmlNode.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = xmlNode.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(xmlNode.SelectSingleNode("ThermalConstantHeat").InnerText);
                lineEntity.FirstEnd = long.Parse(xmlNode.SelectSingleNode("FirstEnd").InnerText);
                lineEntity.SecondEnd = long.Parse(xmlNode.SelectSingleNode("SecondEnd").InnerText);

                List<Point> points = new List<Point>();
                foreach (XmlNode pointNode in xmlNode.ChildNodes[9].ChildNodes)
                {
                    Point point = new Point();
                    point.X = double.Parse(pointNode.SelectSingleNode("X").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                    point.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture);

                    ToLatLon(point.X, point.Y, 34, out newX, out newY);
                    point.X = newX;
                    point.Y = newY;

                    points.Add(point);
                }

                lineEntity.Vertices = points;

                CanvasLineEntity canvasLineEntity = new CanvasLineEntity()
                {
                    LineEntity = lineEntity,
                    CanvasChildIndex = -1
                };
                
                _lineEntities.Add(canvasLineEntity);
            }

            DrawLineEntities();
        }

        private void DrawPowerEntities(List<CanvasPowerEntity> entities)
        {
            entities.ForEach(cpe =>
            {
                double posX, posY;
                ToCanvasPosition(_canvasWidth, _canvasHeight, cpe.PowerEntity.X, cpe.PowerEntity.Y, out posX, out posY);

                int indexX = (int)posX;
                int indexY = (int)(_canvasHeight / _fieldHeight) - 1 - (int)posY;

                if (_usedFieldsMatrix[indexY, indexX])
                {
                    _usedFields.Find(f => f.IndexX == indexX && f.IndexY == indexY).Entities.Add(cpe);
                }
                else
                {
                    _usedFieldsMatrix[indexY, indexX] = true;

                    _usedFields.Add(new CanvasField()
                    {
                        IndexX = indexX,
                        IndexY = indexY,
                        Entities = new List<CanvasPowerEntity>() { cpe }
                    });
                }

                Ellipse e = new Ellipse();
                e.Width = _fieldWidth;
                e.Height = _fieldHeight;
                e.Fill = cpe.PowerEntity.GetType() == typeof(SubstationEntity) ? Brushes.Red : cpe.PowerEntity.GetType() == typeof(NodeEntity) ? Brushes.Green : Brushes.Blue;
                e.ToolTip = $"TYPE:{cpe.PowerEntity.GetType().Name}\nID:{cpe.PowerEntity.Id}\nNAME:{cpe.PowerEntity.Name}";

                Canvas.SetLeft(e, indexX * _fieldWidth);
                Canvas.SetTop(e, indexY * _fieldHeight);

                cpe.CanvasChildIndex = CanvasMap.Children.Add(e);
            });

            _usedFields.ForEach(f =>
            {
                if (f.Entities.Count > 1)
                {
                    Rectangle r = new Rectangle();
                    r.Width = _fieldWidth;
                    r.Height = _fieldHeight;
                    r.Fill = Brushes.Black;
                    string tooltip = $"TYPE:EntityGroup";
                    f.Entities.ForEach(e =>
                    {
                        tooltip += $"\n-------------------------------------------";
                        tooltip += $"\nTYPE:{e.PowerEntity.GetType().Name}\nID:{e.PowerEntity.Id}\nNAME:{e.PowerEntity.Name}";
                    });
                    r.ToolTip = tooltip;

                    Canvas.SetLeft(r, f.IndexX * _fieldWidth);
                    Canvas.SetTop(r, f.IndexY * _fieldHeight);

                    CanvasMap.Children.Add(r);
                }
            });
        }

        private void DrawLineEntities()
        {
            _lineEntities.ForEach(le =>
            {
                CanvasPowerEntity firstEnd = _powerEntities.Find(pe => pe.PowerEntity.Id == le.LineEntity.FirstEnd);
                CanvasPowerEntity secondEnd = _powerEntities.Find(pe => pe.PowerEntity.Id == le.LineEntity.SecondEnd);

                if (firstEnd != null && secondEnd != null)
                {
                    // Ovim se izbegava crtanje linija izmedju vec povezanih entiteta
                    CanvasLineEntity lineExists1 = _lineEntities.Find(l => l.LineEntity.FirstEnd == le.LineEntity.FirstEnd && l.LineEntity.SecondEnd == le.LineEntity.SecondEnd && l.CanvasChildIndex != -1);
                    CanvasLineEntity lineExists2 = _lineEntities.Find(l => l.LineEntity.FirstEnd == le.LineEntity.SecondEnd && l.LineEntity.SecondEnd == le.LineEntity.FirstEnd && l.CanvasChildIndex != -1);

                    if (lineExists1 == null && lineExists2 == null)
                    {
                        Polyline pl = new Polyline();
                        pl.Stroke = Brushes.Orange;
                        pl.StrokeThickness = 1;
                        pl.ToolTip = $"TYPE:{le.LineEntity.GetType().Name}\nID:{le.LineEntity.Id}\nNAME:{le.LineEntity.Name}\nFIRST_END:{le.LineEntity.FirstEnd}\nSECOND_END:{le.LineEntity.SecondEnd}";

                        le.LineEntity.Vertices.ForEach(v =>
                        {
                            double posX, posY;
                            ToCanvasPosition(_canvasWidth, _canvasHeight, v.X, v.Y, out posX, out posY);

                            double pointX = ((int)posX * _fieldWidth) + (_fieldWidth / 2);
                            double pointY = _canvasHeight - (((int)posY * _fieldHeight) + (_fieldHeight / 2));

                            pl.Points.Add(new System.Windows.Point(pointX, pointY));
                        });

                        le.CanvasChildIndex = CanvasMap.Children.Add(pl);
                    }
                }
            });
        }

        private void ToCanvasPosition(double canvasWidth, double canvasHeight, double entityX, double entityY, out double canvasX, out double canvasY)
        {
            double minX = _powerEntities.Min(pe => pe.PowerEntity.X);
            double maxX = _powerEntities.Max(pe => pe.PowerEntity.X);

            double minY = _powerEntities.Min(pe => pe.PowerEntity.Y);
            double maxY = _powerEntities.Max(pe => pe.PowerEntity.Y);

            canvasX = ((canvasHeight / _fieldHeight) - 1) * (entityY - minY) / (maxY - minY);
            canvasY = ((canvasWidth / _fieldWidth) - 1) * (entityX - minX) / (maxX - minX);
        }

        public void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        private void CanvasMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawingStarted && _drawingType == DrawingType.Polygon)
            {
                DrawPolygonWindow wDrawPolygon = new DrawPolygonWindow();
                wDrawPolygon.ShowDialog();

                if (wDrawPolygon.CanDraw)
                {
                    BrushConverter brushConverter = new BrushConverter();

                    _polygon.Fill = brushConverter.ConvertFromString(wDrawPolygon.PolygonFillColor) as SolidColorBrush;
                    _polygon.Stroke = brushConverter.ConvertFromString(wDrawPolygon.PolygonBorderColor) as SolidColorBrush;
                    _polygon.StrokeThickness = wDrawPolygon.PolygonBorderThickness;

                    TextBlock text = new TextBlock()
                    {
                        Text = wDrawPolygon.PolygonTextContent,
                        Foreground = brushConverter.ConvertFromString(wDrawPolygon.PolygonTextColor) as SolidColorBrush,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    IDrawCommand drawCommand = new DrawShapeCommand(CanvasMap, _polygon, text);
                    drawCommand.Execute();
                    _done.Add(drawCommand);

                    _isDrawingStarted = false;
                    _drawingType = DrawingType.None;
                    _polygon = null;
                }
            }
            else if (e.OriginalSource is Shape)
            {
                Shape shape = e.OriginalSource as Shape;

                if (shape != null && shape.ToolTip == null)
                {
                    BrushConverter brushConverter = new BrushConverter();

                    ModifyShapeWindow wModifyShape = new ModifyShapeWindow();
                    wModifyShape.tbBorderThickness.Text = shape.StrokeThickness.ToString();

                    wModifyShape.ShowDialog();

                    if (wModifyShape.CanModify)
                    {
                        shape.Fill = brushConverter.ConvertFromString(wModifyShape.ShapeFillColor) as SolidColorBrush;
                        shape.Stroke = brushConverter.ConvertFromString(wModifyShape.ShapeBorderColor) as SolidColorBrush;
                        shape.StrokeThickness = wModifyShape.ShapeBorderThickness;
                    }
                }
            }
            else if (e.OriginalSource is TextBlock)
            {
                TextBlock textBlock = e.OriginalSource as TextBlock;

                if (textBlock != null)
                {
                    BrushConverter brushConverter = new BrushConverter();

                    ModifyTextWindow wModifyText = new ModifyTextWindow();
                    wModifyText.tbTextSize.Text = textBlock.FontSize.ToString();

                    wModifyText.ShowDialog();

                    if (wModifyText.CanModify)
                    {
                        textBlock.FontSize = wModifyText.TextSize;
                        textBlock.Foreground = brushConverter.ConvertFromString(wModifyText.TextColor) as SolidColorBrush;
                    }
                }
            }
        }

        private void CanvasMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawingStarted)
            {
                System.Windows.Point point;

                switch (_drawingType)
                {
                    case DrawingType.Ellipse:
                        point = e.GetPosition(CanvasMap);

                        DrawEllipseWindow wDrawEllipse = new DrawEllipseWindow();
                        wDrawEllipse.ShowDialog();

                        if (wDrawEllipse.CanDraw)
                        {
                            BrushConverter brushConverter = new BrushConverter();

                            Ellipse ellipse = new Ellipse()
                            {
                                Width = wDrawEllipse.EllipseRadiusX * 2,
                                Height = wDrawEllipse.EllipseRadiusY * 2,
                                Fill = brushConverter.ConvertFromString(wDrawEllipse.EllipseFillColor) as SolidColorBrush,
                                Stroke = brushConverter.ConvertFromString(wDrawEllipse.EllipseBorderColor) as SolidColorBrush,
                                StrokeThickness = wDrawEllipse.EllipseBorderThickness
                            };

                            TextBlock text = new TextBlock()
                            {
                                Text = wDrawEllipse.EllipseTextContent,
                                Foreground = brushConverter.ConvertFromString(wDrawEllipse.EllipseTextColor) as SolidColorBrush,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };

                            IDrawCommand drawCommand = new DrawShapeCommand(CanvasMap, ellipse, text, point.X, point.Y);
                            drawCommand.Execute();
                            _done.Add(drawCommand);
                        }
                        _isDrawingStarted = false;
                        _drawingType = DrawingType.None;
                        break;
                    case DrawingType.Polygon:
                        if (_polygon == null)
                        {
                            _polygon = new Polygon();
                        }
                        _polygon.Points.Add(e.GetPosition(CanvasMap));
                        break;
                    case DrawingType.Text:
                        point = e.GetPosition(CanvasMap);

                        AddTextWindow wAddText = new AddTextWindow();
                        wAddText.ShowDialog();

                        if (wAddText.CanDraw)
                        {
                            BrushConverter brushConverter = new BrushConverter();

                            TextBlock textBlock = new TextBlock()
                            {
                                Text = wAddText.TextContent,
                                FontSize = wAddText.TextSize,
                                Foreground = brushConverter.ConvertFromString(wAddText.TextColor) as SolidColorBrush
                            };

                            IDrawCommand drawCommand = new AddTextCommand(CanvasMap, textBlock, point.X, point.Y);
                            drawCommand.Execute();
                            _done.Add(drawCommand);
                        }
                        _isDrawingStarted = false;
                        _drawingType = DrawingType.None;
                        break;
                    case DrawingType.None:
                        break;
                }
            }
            else if (e.OriginalSource is Shape)
            {
                Shape shape = e.OriginalSource as Shape;

                if (shape == null)
                {
                    return;
                }

                if (shape.ToolTip != null)
                {
                    string[] props = shape.ToolTip.ToString().Split('\n');
                    if (props.Length == 0)
                    {
                        return;
                    }

                    string type = props[0].Split(':')[1];
                    if (type == "LineEntity")
                    {
                        long id = long.Parse(props[1].Split(':')[1]);
                        string name = props[2].Split(':')[1];
                        long firstEndId = long.Parse(props[3].Split(':')[1]);
                        long secondEndId = long.Parse(props[4].Split(':')[1]);

                        CanvasPowerEntity firstEnd = _powerEntities.Find(cpe => cpe.PowerEntity.Id == firstEndId);
                        CanvasPowerEntity secondEnd = _powerEntities.Find(cpe => cpe.PowerEntity.Id == secondEndId);

                        UIElement firstEndChild = CanvasMap.Children[firstEnd.CanvasChildIndex];
                        UIElement secondEndChild = CanvasMap.Children[secondEnd.CanvasChildIndex];

                        ScaleTransform scaleTransform = new ScaleTransform();
                        firstEndChild.RenderTransform = scaleTransform;
                        secondEndChild.RenderTransform = scaleTransform;

                        DoubleAnimation scaleAnimation = new DoubleAnimation()
                        {
                            From = 1,
                            To = 2,
                            Duration = new Duration(TimeSpan.FromSeconds(1))
                        };

                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                    }
                }
            }
        }

        private void LoadEntitiesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEntities();
        }

        private void DrawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            _isDrawingStarted = true;
            _drawingType = DrawingType.Ellipse;
        }

        private void DrawPolygonButton_Click(object sender, RoutedEventArgs e)
        {
            _isDrawingStarted = true;
            _drawingType = DrawingType.Polygon;
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            _isDrawingStarted = true;
            _drawingType = DrawingType.Text;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_done.Count > 0)
            {
                IDrawCommand lastDone = _done.Last();
                lastDone.Undo();
                _done.RemoveAt(_done.Count - 1);
                _undone.Add(lastDone);
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_undone.Count > 0)
            {
                IDrawCommand lastUndone = _undone.Last();
                lastUndone.Redo();
                _undone.RemoveAt(_undone.Count - 1);
                _done.Add(lastUndone);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _done.ForEach(c => c.Undo());
        }
    }
}

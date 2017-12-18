using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using GraphDB.Contract.Core;
using GraphDB.Core;
using GraphDB.Tool.Layout;

namespace GraphDB.Tool.Drawing
{
    public class GraphRenderer
    {
        private Brush myDrawingBrush;
        private Pen myDrawingPen;
        private Brush myTextBrush;
        private readonly Pen myTextPen;
        private readonly Dictionary<string, Visual> myVisuals;
        private int myRadius;

        private DrawingCanvas myDrawingCanvas;
        private Graph mySubGraph;
        private CircleLayout myCirco;

        public Dictionary<string, Visual> Visuals => myVisuals;

        public GraphRenderer()
        {
            myDrawingBrush = Brushes.AliceBlue;
            myDrawingPen = new Pen(Brushes.SteelBlue, 3);
            myTextBrush = Brushes.Black;
            myTextPen = new Pen(Brushes.White, 1);
            myVisuals = new Dictionary<string, Visual>();
            myRadius = 30;
        }

        public void DrawNewGraph(DrawingCanvas dc, int neibour, Graph newSubGraph )
        {
            myDrawingCanvas = dc;
            mySubGraph = newSubGraph;
            myCirco = new CircleLayout(neibour, myRadius);
            //初始化所有布局点
            myCirco.LayoutInit(mySubGraph);
            //进入退火循环
            myCirco.Float(mySubGraph);
            //绘制网络图
            DrawGraph();
        }

        //绘制节点图
        public void DrawGraph()
        {
            var nodeDrawings = GetLayoutNodePoints();
            var edgeDrawings = GetLayoutEdgePoints();
            myDrawingCanvas.ClearVisuals();
            myVisuals.Clear();
            myDrawingCanvas.Width = myCirco.Width;
            myDrawingCanvas.Height = myCirco.Height;
            foreach (EdgeDrawing edge in edgeDrawings)
            {
                DrawingVisual visual = new DrawingVisual();
                Point start = new Point(edge.StartPosition.X, edge.StartPosition.Y);
                Point end = new Point(edge.EndPosition.X, edge.EndPosition.Y);
                DrawText(visual, edge.Attribute, start, end);
                end = ModifyPositiion(start, end, myRadius);
                ((Canvas)myDrawingCanvas.Parent).Children.Add(DrawLeaderLineArrow(start, end)); 
                myDrawingCanvas.AddVisual(visual);
            }
            BringToFront(myDrawingCanvas);
            foreach (NodeDrawing node in nodeDrawings)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawEllipse(visual, node.Name, new Point(node.Position.X, node.Position.Y));
                myVisuals.Add(node.Guid, visual);
                myDrawingCanvas.AddVisual(visual);
            }
        }

        //切换并读取样式
        public void ChangeStyle(Style curStyle)
        {
            Brush bBack = Brushes.AliceBlue, bStroke = Brushes.SteelBlue, bFore = Brushes.Black;
            double dubHeight = 60.0, dubStrokeThickness = 5.0;

            if (curStyle == null)
            {
                return;
            }
            foreach (var setterBase in curStyle.Setters)
            {
                var st = (Setter) setterBase;
                switch (st.Property.ToString())
                {
                    case "Fill":
                        bBack = (Brush)st.Value;
                        break;
                    case "Stroke":
                        bStroke = (Brush)st.Value;
                        break;
                    case "StrokeThickness":
                        dubStrokeThickness = (double)st.Value;
                        break;
                    case "Height":
                        dubHeight = (double)st.Value;
                        break;
                    case "Width":
                        break;
                    case "Foreground":
                        bFore = (Brush)st.Value;
                        break;
                    default:
                        break;
                }
            }
            myDrawingBrush = bBack;
            myDrawingPen = new Pen(bStroke, dubStrokeThickness);
            myTextBrush = bFore;
            myRadius = Convert.ToInt32(dubHeight / 2);
        }

        private List<NodeDrawing> GetLayoutNodePoints()
        {
            if (myCirco == null)
            {
                return null;
            }
            var nodeDrawing = new List<NodeDrawing>();
            foreach (var curItem in mySubGraph.Nodes)
            {
                nodeDrawing.Add(new NodeDrawing(curItem.Key, curItem.Value.Name, myCirco.NodePoints[curItem.Value.Guid]));
            }

            return nodeDrawing;
        }

        private List<EdgeDrawing> GetLayoutEdgePoints()
        {
            if (myCirco == null)
            {
                return null;
            }
            var edgeDrawing = new List<EdgeDrawing>();
            foreach (IEdge edge in mySubGraph.Edges)
            {
                var startGuid = edge.FromGuid;
                var endGuid = edge.ToGuid;
                edgeDrawing.Add(new EdgeDrawing(myCirco.NodePoints[startGuid], myCirco.NodePoints[endGuid], edge.Attribute));
            }
            return edgeDrawing;
        }

        // 渲染原型.
        private void DrawEllipse(DrawingVisual visual, string sName, Point center)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Brush brush = myDrawingBrush;

                dc.DrawEllipse(brush, myDrawingPen, center, myRadius, myRadius);

                if (Encoding.Default.GetBytes(sName).Length > 8)
                {
                    sName = sName.Insert((int)Math.Ceiling(sName.Length * 1.0 / 2), "\n");
                }
                FormattedText text = new FormattedText(sName,
                                                        CultureInfo.CurrentCulture,
                                                        FlowDirection.LeftToRight,
                                                        new Typeface("Times New Roman"),
                                                        12,
                                                        myTextBrush);
                int height = Convert.ToInt32(text.Height);
                int width = Convert.ToInt32(text.Width);
                dc.DrawText(text, new Point(center.X - width * 1.0 / 2, center.Y - height * 1.0 / 2));
            }
        }
        
        // 绘制连边文本
        private void DrawText(DrawingVisual visual, string attribute, Point start, Point end)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                FormattedText text = new FormattedText(attribute,
                                                        CultureInfo.CurrentCulture,
                                                        FlowDirection.LeftToRight,
                                                        new Typeface("Times New Roman"),
                                                        12,
                                                        Brushes.Black);
                int height = Convert.ToInt32(text.Height);
                int width = Convert.ToInt32(text.Width);
                Point center = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                dc.DrawRectangle(Brushes.White, myTextPen, new Rect(new Point(center.X - width * 1.0 / 2, center.Y - height * 1.0 / 2), new Point(center.X + width * 1.0 / 2, center.Y + height * 1.0 / 2)));
                dc.DrawText(text, new Point(center.X - width * 1.0 / 2, center.Y - height * 1.0 / 2));
            }
        }
        
        //绘制连边形状
        private Arrow DrawLeaderLineArrow(Point startPt, Point endPt)
        {
            Arrow arrow = new Arrow();
            arrow.X1 = startPt.X;
            arrow.Y1 = startPt.Y;
            arrow.X2 = endPt.X;
            arrow.Y2 = endPt.Y;
            arrow.HeadWidth = 7;
            arrow.HeadHeight = 3;
            arrow.Stroke = Brushes.Gray;
            arrow.StrokeThickness = 2;
            return arrow;
        }

        //位置修正
        private Point ModifyPositiion(Point startPt, Point endPt, int iRadius)
        {
            var deltaX = startPt.X - endPt.X;
            var deltaY = startPt.Y - endPt.Y;
            var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (Math.Abs(distance) < 1e-6)
            {
                return endPt;
            }
            var x = Convert.ToInt32(deltaX * iRadius / distance);
            var y = Convert.ToInt32(deltaY * iRadius / distance);
            return new Point(endPt.X + x, endPt.Y + y);
        }

        //将节点图置于顶层
        private void BringToFront(DrawingCanvas element)//图片置于最顶层显示
        {
            Canvas parent = element?.Parent as Canvas;
            if (parent == null)
            {
                return;
            }
            if (mySubGraph.Nodes.Count == 1)
            {
                return;
            }
            var maxZ = parent.Children.OfType<UIElement>()//linq语句，取Zindex的最大值
              .Where(x => !Equals(x, element))
              .Select(x => Panel.GetZIndex(x))
              .Max();
            Panel.SetZIndex(element, maxZ + 1);
        }

    }
}
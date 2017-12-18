using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphDB.Tool.Drawing
{
    public class DrawingCanvas : Canvas
    {
        private readonly List<Visual> myVisuals = new List<Visual>();

        protected override Visual GetVisualChild(int index)
        {
            return myVisuals[index];
        }
        protected override int VisualChildrenCount => myVisuals.Count;

        public void AddVisual(Visual visual)
        {
            myVisuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void DeleteVisual(Visual visual)
        {
            myVisuals.Remove(visual);

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);            
        }

        public void ClearVisuals()
        {
            foreach(Visual visual in myVisuals)
            {
                RemoveVisualChild(visual);
                RemoveLogicalChild(visual);   
            }
            myVisuals.Clear();
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult?.VisualHit as DrawingVisual;            
        }

        private readonly List<DrawingVisual> myHits = new List<DrawingVisual>();
        public List<DrawingVisual> GetVisuals(Geometry region)
        {
            myHits.Clear();
            GeometryHitTestParameters parameters = new GeometryHitTestParameters(region);
            HitTestResultCallback callback = HitTestCallback;
            VisualTreeHelper.HitTest(this, null, callback, parameters);
            return myHits;
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result;
            DrawingVisual visual = result.VisualHit as DrawingVisual;
            if (visual != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                myHits.Add(visual);
            }
            return HitTestResultBehavior.Continue;
        }

    }
}

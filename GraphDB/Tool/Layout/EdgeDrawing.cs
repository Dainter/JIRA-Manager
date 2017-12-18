using System.Drawing;

namespace GraphDB.Tool.Layout
{
    public class EdgeDrawing
    {
        //绘图元素
        readonly Point myStartPos;
        readonly Point myEndPos;
        readonly string myAttribute;
        
        public Point StartPosition => myStartPos;

        public Point EndPosition => myEndPos;

        public string Attribute => myAttribute;

        public EdgeDrawing(Point startPos, Point endPos, string attribute)
        {
            myStartPos = startPos;
            myEndPos = endPos;
            myAttribute = attribute;
        }
    }
}

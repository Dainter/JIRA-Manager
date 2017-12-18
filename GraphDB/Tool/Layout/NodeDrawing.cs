using System.Drawing;

namespace GraphDB.Tool.Layout
{
    public class NodeDrawing
    {
        //绘图元素
        private readonly string myGuid;
        readonly Point myPoint;
        readonly string myName;

        public Point Position => myPoint;

        public string Name => myName;

        public string Guid => myGuid;

        public NodeDrawing(string guid, string sName, Point pPos)
        {
            myGuid = guid;
            myPoint = pPos;
            myName = sName;
        }
    }
}

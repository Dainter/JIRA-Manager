using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace GraphDB.Tool.Layout
{
    class CircleLayout
    {
        //常量
        const double GoldenRatio = 0.618;
        const int CanvasRatio = 8;
        const double SmallRatio = 2.2;
        const double MaskRatio = 2.5;
        const double OuterMaskRatio = 3.0;
        //变量
        readonly int myNum1StLevel;//第一层节点个数
        readonly int myRadius; //节点半径
        readonly int mySmallDistance;
        readonly int myLineLength;//连边最小距离
        readonly int myHeight;//画布高度和宽度
        readonly int myWidth;//画布高度和宽度
        readonly int myMaskRadius;//内环半径
        readonly int myOuterMaskRadius;//外环半径
        readonly Dictionary<string, Point> myPoints;//节点位置列表
        //属性
        public int Height => myHeight;

        public int Width => myWidth;

        public Dictionary<string, Point> NodePoints => myPoints;

        public int Numberof1StLevel => myNum1StLevel;
        //构造函数
        public CircleLayout(int iNum, int iRadius)
        {
            myNum1StLevel = iNum;
            myRadius = iRadius;
            var nodeDistance = Convert.ToInt32(4 * myRadius / GoldenRatio);
            mySmallDistance = Convert.ToInt32(myRadius * SmallRatio);
            if (myNum1StLevel <= 4)//连边长度下限
            {
                myLineLength = Convert.ToInt32(nodeDistance / (2 * Math.Sin(Math.PI / 4)));
            }
            else if (myNum1StLevel <= 8)//连边长度上限
            {
                myLineLength = Convert.ToInt32(nodeDistance / (2 * Math.Sin(Math.PI / myNum1StLevel)));
            }
            else
            {
                myLineLength = Convert.ToInt32(nodeDistance / (2 * Math.Sin(Math.PI / 8)));
            }
            myHeight = myWidth = myLineLength * CanvasRatio;
            myMaskRadius = Convert.ToInt32(myLineLength * MaskRatio);
            myOuterMaskRadius = Convert.ToInt32(myLineLength * OuterMaskRatio);
            myPoints = new Dictionary<string, Point>();
        }
        //布局初始化
        public void LayoutInit(Graph subgraph)
        {
            int x, y;
            //核心节点
            myPoints.Add(subgraph.Nodes.ElementAt(0).Key, new Point(myWidth/2, myHeight/2));
            //第一级邻居节点，等距均匀分布
            for (int index = 1; index < myNum1StLevel+1; index++)
            {
                var angle = index * 2 * Math.PI / myNum1StLevel;
                x = Convert.ToInt32(Math.Cos(angle) * myLineLength + myWidth * 1.0 / 2);
                y = Convert.ToInt32(Math.Sin(angle) * myLineLength + myHeight * 1.0 / 2);
                myPoints.Add(subgraph.Nodes.ElementAt(index).Key, new Point(x, y));
            }
            //其他节点，先布置在关联点的中心，随后进行位置修正
            for (int index = myNum1StLevel + 1; index < subgraph.Nodes.Count; index++)
            {
                Point newPos;
                bool cover;
                do
                {
                    cover = false;
                    var center = GetCenterPoint(index, subgraph);
                    x = GenerateRandomNumber(-myRadius, myRadius, index);
                    y = GenerateRandomNumber(-myRadius, myRadius, index * 10);
                    newPos = new Point(center.X + x, center.Y + y);
                    PositionCorrected(ref newPos);
                    //查找是否有重合节点，发现则重新生成随机数
                    foreach (var curItem in myPoints)
                    {
                        if (curItem.Value.X == newPos.X && curItem.Value.Y == newPos.Y)
                        {
                            cover = true;
                        }
                    }
                } while (cover == true);
                myPoints.Add(subgraph.Nodes.ElementAt(index).Key, newPos);
            }
        }

        //获取索引节点所有关联节点的中心坐标
        private Point GetCenterPoint(int index, Graph subgraph)
        {
            int intSumX =0, intSumY = 0;
            INode curNode = subgraph.Nodes.ElementAt(index).Value;

            foreach (IEdge edge in curNode.OutBound)
            {
                INode tarNode = edge.To;
                if( !subgraph.Nodes.ContainsKey( tarNode.Guid ) )
                {
                    continue;
                }
                intSumX += myPoints[tarNode.Guid].X;
                intSumY += myPoints[tarNode.Guid].Y;
                if (curNode.OutDegree == 1)
                {
                    intSumX += myPoints.ElementAt(0).Value.X;
                    intSumY += myPoints.ElementAt(0).Value.Y;
                    return new Point(intSumX / 2, intSumY / 2);
                }
            }
            return new Point(intSumX/curNode.OutDegree, intSumY/curNode.OutDegree);
        }
        
        //随机数生成器
        private int GenerateRandomNumber(int downLimit, int upLimit, int seed)
        {
            var magic1 = new Random(DateTime.Now.Millisecond * DateTime.Now.Second * seed);
            var magic2 = new Random(magic1.Next(downLimit, 1000 * upLimit * seed) * DateTime.Now.Millisecond);

            return magic2.Next(downLimit, upLimit);
        }
        //位置修正
        private void PositionCorrected(ref Point curPos)
        {
            int deltaX, deltaY;

            var distance = Math.Pow(curPos.X - myWidth / 2, 2)  + Math.Pow(curPos.Y - myHeight / 2, 2);
            if (distance < Math.Pow(myMaskRadius, 2))
            {//距离中心坐标小于intMaskRadius范围的节点向外推
                deltaX = Convert.ToInt32(Math.Ceiling((myMaskRadius / Math.Sqrt(distance) - 1) * (curPos.X - myWidth / 2 +0.0000001)));
                deltaY = Convert.ToInt32(Math.Ceiling((myMaskRadius / Math.Sqrt(distance) - 1) * (curPos.Y - myHeight / 2 + 0.0000001)));
                curPos.X += deltaX;
                curPos.Y += deltaY;
            }
            else if (distance > Math.Pow(myOuterMaskRadius, 2))
            {//距离中心坐标大于intOuterMaskRadius范围的节点向里拉
                deltaX = Convert.ToInt32(Math.Ceiling((myOuterMaskRadius / Math.Sqrt(distance) - 1) * (curPos.X - myWidth / 2 + 0.0000001)));
                deltaY = Convert.ToInt32(Math.Ceiling((myOuterMaskRadius / Math.Sqrt(distance) - 1) * (curPos.Y - myHeight / 2 + 0.0000001)));
                curPos.X += deltaX;
                curPos.Y += deltaY;
            }
        }
        //浮动算法函数
        public void Float(Graph subGraph)
        {
            int intCloseCount, intRound = 0;
            List<PointF> forces = new List<PointF>();
            
            do{
                intCloseCount = 0;
                //只循环随机分布的几个节点
                for (int index = myNum1StLevel + 1; index < subGraph.Nodes.Count; index++)
                {
                    var force = new PointF(0, 0);
                    //循环所有同一层节点，计算斥力
                    for (int intNei = myNum1StLevel + 1; intNei < subGraph.Nodes.Count; intNei++)
                    {
                        if (intNei == index)
                        {
                            continue;
                        }
                        if (IsClose(myPoints.ElementAt(index).Value, myPoints.ElementAt(intNei).Value, mySmallDistance) == false)
                        {//如果不靠近则不管
                            continue;
                        }
                        intCloseCount++;
                        CalRejectForce(myPoints.ElementAt(index).Value, myPoints.ElementAt(intNei).Value, ref force);
                    }
                    //加入合力列表
                    forces.Add(force);
                }
                for (int index = myNum1StLevel + 1; index < subGraph.Nodes.Count; index++)
                {
                    var curPosition = new Point(myPoints.ElementAt(index).Value.X, myPoints.ElementAt(index).Value.Y);
                    //得出合力，退火处理输出移动分量，进行移动
                    MoveNode(forces[index - (myNum1StLevel + 1)], ref curPosition);
                    //进行mask修正
                    PositionCorrected(ref curPosition);
                    string curGuid = myPoints.ElementAt( index ).Key;
                    myPoints[curGuid] = curPosition;
                }
                intRound++;
                if (intRound > 20)
                {//如果超过回合数则退出，防止死循环
                    break;
                }
            }while (intCloseCount > 0);
        }

        //检查两点间距离是否小于设定的距离
        private bool IsClose(Point pot1, Point pot2, double dSmallDistance)
        {
            var deltaX = pot2.X - pot1.X;
            var deltaY = pot2.Y - pot1.Y;
            var distance = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);
            if (distance < dSmallDistance * dSmallDistance)
            {
                return true;
            }
            return false;
        }

        //计算节点受到的排斥力
        private void CalRejectForce(Point curPos, Point tarPos, ref PointF force)
        {
            var deltaX = tarPos.X - curPos.X;
            var deltaY = tarPos.Y - curPos.Y;
            if (deltaX * deltaX + deltaY * deltaY > mySmallDistance * mySmallDistance)
            {
                return;
            }
            var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            var frx = -((mySmallDistance - distance) * deltaX) / (2 * (distance+0.1));
            var fry = -((mySmallDistance - distance) * deltaY) / (2 * (distance+0.1));
            force.X += (float)frx;
            force.Y += (float)fry;
        }

        //移动当前节点，但不能超过距离限制
        private void MoveNode(PointF force, ref Point curPos)
        {
            
            if (force.X * force.X + force.Y * force.Y > mySmallDistance * mySmallDistance)
            {
                curPos.X += (int)(force.X * mySmallDistance/Math.Sqrt(force.X * force.X + force.Y * force.Y));
                curPos.Y += (int)(force.Y * mySmallDistance / Math.Sqrt(force.X * force.X + force.Y * force.Y));
                return;
            }
            curPos.X+= (int)force.X;
            curPos.Y += (int)force.Y;
        }
    }
}

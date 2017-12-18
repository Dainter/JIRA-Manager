using System.Collections.Generic;
using System.Linq;
using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Contract.Serial;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Node:INode//图数据库节点类：负责存储单一网络节点的信息，并向上层类提供功能接口函数
    {
        //成员变量
        private readonly string myGuid;                           //节点编号
        private readonly string myName;
        private readonly List<IEdge> myOutLink;       //连边 使用字典结构存放（目标节点号，连边对象）
        private readonly List<IEdge> myInLink;
        //属性///////////////////////////////
        [Serializable]
        public string Guid => myGuid;

        [Serializable]
        public string Name => myName;

        public int InDegree => myInLink.Count;

        public int OutDegree => myOutLink.Count;

        public List<IEdge> OutBound => myOutLink;

        public List<IEdge> InBound => myInLink;

        //方法///////////////////////////////
        //节点类Node构造函数
        public Node( string name )    
        {
            myGuid = System.Guid.NewGuid().ToString();
            myName = name ?? "";
            myOutLink = new List<IEdge>();
            myInLink = new List<IEdge>();
        }

        public Node(INode oriNode )
        {
            myGuid = oriNode.Guid;
            myName = oriNode.Name;
            myOutLink = new List<IEdge>();
            myInLink = new List<IEdge>();
        }
        //xml构造函数
        public Node(XmlElement xNode)
        {
            //取出制定标签的Inner Text
            myGuid = xNode.GetText("Guid");
            myName = xNode.GetText("Name");
            myOutLink = new List<IEdge>();
            myInLink = new List<IEdge>();
        }

        //增加连边
        public bool AddEdge(IEdge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点是本节点，且终止节点不是本节点
            if (newEdge.From.Guid != Guid || newEdge.To.Guid == Guid)
            {
                return false;
            }
            //如果OutbOund已经包含该边
            if (OutBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //向Links中加入新项目  
            myOutLink.Add(newEdge);   
            return true;
        }

        //Inbound边注册
        public bool RegisterInbound(IEdge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点不是本节点，且终止节点是本节点
            if (newEdge.To.Guid != Guid || newEdge.From.Guid == Guid)
            {
                return false;
            }
            //如果Inbound包含该边则不注册
            if (InBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //加入新边
            myInLink.Add(newEdge);
            return true;
        }

        //去除连边
        public bool RemoveEdge(IEdge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点是本节点，且终止节点不是本节点
            if (curEdge.From.Guid != Guid || curEdge.To.Guid == Guid)
            {
                return false;
            }
            //如果OutbOund不包含该边则退出
            if (OutBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            myOutLink.Remove(curEdge);
            return true;
        }

        //清除所有连边,返回被清除的边列表
        public List<IEdge> ClearEdge()
        {
            List<IEdge> edgeList = new List<IEdge>();
            //首先将OutBound中所有连边的终止节点中注销该边
            foreach (IEdge edge in OutBound)
            {
                edge.To.UnRegisterInbound(edge);
                edge.From = null;
                edge.To = null;
                //当前边加入返回结果列表
                edgeList.Add(edge);
            }
            //从OutBound中清除所有边
            OutBound.Clear();
            //首先将InBound中所有连边的起始节点中去除该边
            foreach (IEdge edge in InBound)
            {
                edge.From.RemoveEdge(edge);
                edge.From = null;
                edge.To = null;
                //当前边加入返回结果列表
                edgeList.Add(edge);
            }
            //从InBound中清除所有边
            InBound.Clear();
            //返回本节点涉及的连边列表
            return edgeList;
        }

        //Inbound注销
        public bool UnRegisterInbound(IEdge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //如果Inbound不包含当前边则不注销
            if (InBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            myInLink.Remove(curEdge);
            return true;
        }

        //返回OutBound是否包含和目标节点间的连边
        private bool OutBoundContainsEdge(IEdge newEdge)
        {
            if (myOutLink.Contains(newEdge))
            {
                return true;
            }
            return myOutLink.Any( x => (x.To.Guid == newEdge.To.Guid) && (x.Attribute == newEdge.Attribute) );
        }

        //返回InBound是否包含和目标节点间的连边
        private bool InBoundContainsEdge(IEdge newEdge)
        {
            if (myInLink.Contains(newEdge))
            {
                return true;
            }
            return myInLink.Any( x => (x.From.Guid == newEdge.From.Guid) && (x.Attribute == newEdge.Attribute) );
        }

        public override string ToString()
        {
            return Guid;
        }

        public string DataOutput()
        {
            string strResult = "";

            strResult +="Name: " + Name;
            strResult += "\nType: " + GetType().Name;

            return strResult;
        }

        //查找目标为指定GUID的连边
        public IEnumerable<IEdge> GetEdgesByGuid(string nodeGuid, EdgeDirection direction)
        {
            if( nodeGuid == null )
            {
                return new List<IEdge>();
            }
            if (direction == EdgeDirection.In)
            {
                return InBound.Where(x => x.From.Guid == nodeGuid);
            }
            if (direction == EdgeDirection.Out)
            {
                return OutBound.Where(x => x.To.Guid == nodeGuid);
            }
            return new List<IEdge>();
        }

        //查找目标为指定Name的连边
        public IEnumerable<IEdge> GetEdgesByName(string nodeName, EdgeDirection direction)
        {
            if (nodeName == null)
            {
                return new List<IEdge>();
            }
            if (direction == EdgeDirection.In)
            {
                return InBound.Where(x => x.From.Name == nodeName);
            }
            if (direction == EdgeDirection.Out)
            {
                return OutBound.Where(x => x.To.Name == nodeName);
            }
            return new List<IEdge>();
        }

        //查找类型为指定Type的连边
        public IEnumerable<IEdge> GetEdgesByType(string attribute, EdgeDirection direction)
        {
            if (attribute == null)
            {
                return new List<IEdge>();
            }
            if (direction == EdgeDirection.In)
            {
                return InBound.Where(x => x.Attribute == attribute);
            }
            if (direction == EdgeDirection.Out)
            {
                return OutBound.Where(x => x.Attribute == attribute);
            }
            return new List<IEdge>();
        }
    }
}

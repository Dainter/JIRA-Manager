using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Contract.Serial;
using GraphDB.IO;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Graph
    {
        private readonly Dictionary<string, INode> myNodeList;
        private readonly List<IEdge> myEdgeList;
        private readonly IIoStrategy myIohandler;

        public Dictionary<string, INode> Nodes => myNodeList;

        public List<IEdge> Edges => myEdgeList;

        public Graph()
        {
            myNodeList = new Dictionary<string, INode>();
            myEdgeList = new List<IEdge>();
        }

        public Graph( string path )
        {
            myNodeList = new Dictionary<string, INode>();
            myEdgeList = new List<IEdge>();
            myIohandler = new XMLStrategy(path);

            if( !File.Exists( path ) )
            {
                ErrorCode err;
                SaveDataBase(out err);
                return;
            }
            GraphInit();
        }

        private void GraphInit()
        {
            ErrorCode err;
            XmlElement graph = myIohandler.ReadFile(out err);
            if (err != ErrorCode.NoError)
            {
                throw new Exception($"Error found during read DB file. Error Code:{err}");
            }
            var nodes = graph.GetNode(XmlNames.Nodes);
            var edges = graph.GetNode(XmlNames.Edges);

            //Nodes
            foreach (XmlElement curItem in nodes)
            {
                INode newNode = (INode)SerializableHelper.Deserialize(curItem);
                if (newNode == null)
                {
                    throw new Exception($"Error found during Deserialize. XML:{curItem}");
                }
                myNodeList.Add(newNode.Guid, newNode);
            }
            //Edges
            foreach (XmlElement curItem in edges)
            {
                IEdge newEdge = (IEdge)SerializableHelper.Deserialize(curItem);
                if (newEdge == null)
                {
                    throw new Exception($"Error found during Deserialize. XML:{curItem}");
                }
                //Add Link
                AddEdge(newEdge.FromGuid, newEdge.ToGuid, newEdge);
            }
        }

        public void SaveDataBase(out ErrorCode err)
        {
            XmlDocument doc = ToXML();
            myIohandler.SaveFile(doc, out err);
        }

        public void SaveAsDataBase(string newPath, out ErrorCode err )
        {
            IIoStrategy newIohandler = new XMLStrategy(newPath);
            XmlDocument doc = ToXML();
            newIohandler.SaveFile(doc, out err);
        }

        //将数据保存为XML文件（接口）
        public XmlDocument ToXML()
        {
            //所有网络数据都保存为xml格式
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement(XmlNames.Graph);

            //Nodes
            var nodes = doc.CreateElement(XmlNames.Nodes);
            nodes.SetAttribute(XmlNames.NodeNumber, myNodeList.Count.ToString());
            foreach (KeyValuePair<string, INode> curItem in myNodeList)
            {
                XmlElement newNode = SerializableHelper.Serialize(doc, curItem.Value);
                nodes.AppendChild(newNode);     
            }
            root.AppendChild(nodes);
            //Edges
            var edges = doc.CreateElement(XmlNames.Edges);
            edges.SetAttribute(XmlNames.EdgeNumber, myEdgeList.Count.ToString());
            foreach (IEdge curItem in myEdgeList)
            {
                edges.AppendChild(SerializableHelper.Serialize(doc, curItem)); 
            }
            root.AppendChild(edges);

            doc.AppendChild(root);
            return doc;
        }

        //加入节点
        private void AddNode(INode newNode)
        {
            if( newNode == null )
            {
                return;
            }
            //节点加入节点列表
            myNodeList.Add(newNode.Guid, newNode);
        }

        //删除节点 by Guid
        private void RemoveNode(string guid)
        {
            if (guid == null)
            {
                return;
            }
            INode curNode = myNodeList[ guid ];
            if( curNode == null )
            {
                return;
            }
            RemoveNode( curNode );
        }

        //删除节点 by Node
        private void RemoveNode(INode curNode)
        {
            //清除节点所有连边
            ClearUnusedEdge(curNode.ClearEdge());
            //从节点列表中移除节点
            myNodeList.Remove(curNode.Guid);
        }

        //加入连边 by Guid
        private void AddEdge( string curNodeGuid, string tarNodeGuid, IEdge newEdge )
        {
            if (curNodeGuid == null || tarNodeGuid == null || newEdge == null)
            {
                return;
            }
            INode curNode = myNodeList[curNodeGuid];
            if (curNode == null)
            {
                return;
            }
            INode tarNode = myNodeList[tarNodeGuid];
            if (tarNode == null)
            {
                return;
            }
            AddEdge( curNode, tarNode, newEdge);
        }

        //加入连边 by Node
        private void AddEdge( INode curNode, INode tarNode, IEdge newEdge )
        {
            if (curNode == null || tarNode == null || newEdge == null)
            {
                return;
            }
            //连边的头指针指向起节点
            newEdge.From = curNode;
            //连边的尾指针指向目标节点
            newEdge.To = tarNode;
            //将新连边加入起始节点的outbound
            if (curNode.AddEdge(newEdge) == false)
            {
                return;
            }
            //将新连边加入目标节点的Inbound
            if (tarNode.RegisterInbound(newEdge) == false)
            {
                return;
            }
            //全部完成后将连边加入网络连边列表
            myEdgeList.Add(newEdge);
        }

        //移除连边 by Guid
        private void RemoveEdge(string curNodeGuid, string tarNodeGuid, string attribute)
        {
            if (curNodeGuid == null || tarNodeGuid == null || attribute == null)
            {
                return;
            }
            INode curNode = myNodeList[curNodeGuid];
            if (curNode == null)
            {
                return;
            }
            INode tarNode = myNodeList[tarNodeGuid];
            if (tarNode == null)
            {
                return;
            }
            RemoveEdge(curNode, tarNode, attribute);
        }

        //移除连边 by Node
        private void RemoveEdge(INode curNode, INode tarNode, string attribute)
        {
            if (curNode == null || tarNode == null || attribute == null)
            {
                return;
            }
            //从起始节点的出边中遍历,查找终止节点编号和目标节点编号和类型一致的连边
            IEdge curEdge = curNode.OutBound.First(x => x.To.Guid == tarNode.Guid && x.Attribute == attribute);
            if (curEdge == null)
            {//没找到直接返回
                return;
            }
            //起始节点Outbound中移除连边
            curNode.RemoveEdge(curEdge);
            //从终止节点InBound中注销连边
            tarNode.UnRegisterInbound(curEdge);
            //全部完成后，从总连边列表中移除该边
            myEdgeList.Remove(curEdge);
        }

        //删除所有节点和连边
        public void ClearAll()
        {
            myEdgeList.Clear();
            myNodeList.Clear();
        }

        //删除所有被解除绑定的连边
        private void ClearUnusedEdge(List<IEdge> unusedList)
        {
            if (unusedList == null)
            {
                return;
            }
            //将入参列表中所有连边从总连边列表中删除
            foreach (IEdge edge in unusedList)
            {
                myEdgeList.Remove(edge);
            }
            //清空入参列表本身内容
            unusedList.Clear();
        }

        //删除所有连边
        public void ClearAllEdge()
        {
            myEdgeList.Clear();
        }

        //加入节点（接口）
        public void AddNode(INode oriNode, out ErrorCode err)
        {
            if (oriNode == null )
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //检查节点是否已经存在“名称一致”
            if (ContainsNode(oriNode))
            {
                err = ErrorCode.NodeExists;
                return;
            }
            AddNode(oriNode);
            err = ErrorCode.NoError;
        }

        //加入连边（接口）
        public void AddEdge(string startName, string endName, IEdge newEdge, out ErrorCode err)
        {
            if (startName == null || endName == null || newEdge == null)
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //获取起始节点，不存在报错
            var startNode = GetNodeByName(startName);
            if (startNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //获取终止节点，不存在报错
            var endNode = GetNodeByName(endName);
            if (endNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //查找两点间是否存在相同类型关系，存在报错
            if (ContainsEdge(startNode, endNode, newEdge))
            {
                err = ErrorCode.EdgeExists;
                return;
            }
            //在两点间加入新边
            AddEdge(startNode, endNode, newEdge);
            err = ErrorCode.NoError;
        }

        //加入连边（接口）
        public void AddEdge(INode startNode, INode endNode, IEdge newEdge, out ErrorCode err)
        {
            if (startNode == null || endNode == null || newEdge == null)
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            if( !ContainsNode( startNode ) || !ContainsNode(endNode))
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            AddEdge(startNode, endNode, newEdge);
            err = ErrorCode.NoError;
        }

        //加入连边（接口）
        public void AddEdgeByGuid(string startGuid, string endGuid, IEdge newEdge, out ErrorCode err)
        {
            AddEdge( startGuid, endGuid, newEdge );
            err = ErrorCode.NoError;
        }

        //检查节点是否已存在
        public bool ContainsNode(string nodeName)
        {
            if (nodeName == null)
            {
                return false;
            }
            return Nodes.Any(x => x.Value.Name == nodeName);
        }

        //检查节点是否已存在
        private bool ContainsNode(INode curNode)
        {
            if (curNode == null )
            {
                return false;
            }
            return Nodes.Any( x => x.Value.Name == curNode.Name );
        }

        //检查连边是否已存在
        private bool ContainsEdge(INode startNode, INode endNode, IEdge curEdge)
        {
            if (startNode == null || endNode == null || curEdge == null)
            {
                return false;
            }
            var query = startNode.GetEdgesByGuid( endNode.Guid, EdgeDirection.Out );
            return query.Any(x => x.Attribute == curEdge.Attribute);
        }

        //删除节点（接口）
        public void RemoveNode(string nodeName, out ErrorCode err)
        {
            if (nodeName == null )
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            var tarNode = GetNodeByName(nodeName);
            //检查节点是否已经存在“名称一致”
            if (tarNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            RemoveNode(tarNode);
            err = ErrorCode.NoError;
        }

        //删除连边（接口）
        public void RemoveEdge(string startName, string endName, string attribute, out ErrorCode err)
        {
            if (startName == null || endName == null || attribute == null)
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //获取起始节点，不存在报错
            var startNode = GetNodeByName(startName);
            if (startNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //获取终止节点，不存在报错
            INode endNode = GetNodeByName(endName);
            if (endNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            RemoveEdge( startNode, endNode, attribute);
            err = ErrorCode.NoError;
        }

        //查询函数，返回节点列表中指定名称的节点
        public INode GetNodeByGuid(string nodeGuid)
        {
            if (nodeGuid == null)
            {
                return null;
            }
            //遍历节点列表
            var query = Nodes.Where( x => x.Value.Guid == nodeGuid ).Select( x => x.Value );
            if (!query.Any())
            {
                return null;
            }
            return query.First();
        }

        //查询函数，返回节点列表中指定名称的节点
        public INode GetNodeByName(string nodeName)
        {
            if (nodeName == null)
            {
                return null;
            }
            //遍历节点列表
            var query = Nodes.Where( x => x.Value.Name == nodeName ).Select( x => x.Value );
            if (!query.Any())
            {
                return null;
            }
            return query.First();
        }

        //查询函数，返回节点列表中指定类型的节点
        public IEnumerable<INode> GetNodesByType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            //遍历节点列表
            return Nodes.Where(x => x.Value.GetType() == type).Select(x => x.Value);
        }

        //查询函数，返回节点列表中指定名称和类型的节点
        public IEnumerable<INode> GetNodesByNameAndType(string nodeName, Type type)
        {
            if (nodeName == null || type == null)
            {
                return null;
            }
            //遍历节点列表
            return Nodes.Where( x => x.Value.Name == nodeName && x.Value.GetType() == type ).Select( x => x.Value );
        }

        //查询函数，返回节点中某个GUID的索引位置
        public int IndexOf(string guid)
        {
            int index = 0;
            foreach( var curItem in Nodes )
            {
                if( curItem.Key == guid )
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        //查询函数，返回节点中某个Node的索引位置
        public int IndexOf(INode node)
        {
            int index = 0;
            foreach (var curItem in Nodes)
            {
                if (curItem.Key == node.Guid)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        //查询函数，返回指定GUID的节点间的连边
        public IEnumerable<IEdge> GetEdgesByGuid(string startGuid, string endGuid )
        {
            if (startGuid == null || endGuid == null)
            {
                return null;
            }
            return Edges.Where( x => x.FromGuid == startGuid && x.ToGuid == endGuid );
        }

        //查询函数，返回指定名称的节点间的连边
        public IEnumerable<IEdge> GetEdgesByName(string startName, string endName)
        {
            if (startName == null || endName == null)
            {
                return null;
            }
            return Edges.Where(x => x.From.Name == startName && x.To.Name == endName);
        }

        //查询函数，返回Start开始的类型为Type的连边
        public IEnumerable<IEdge> GetEdgesByType(string startName, string attribute)
        {
            if (startName == null || attribute == null)
            {
                return null;
            }
            return Edges.Where(x => x.From.Name == startName && x.Attribute == attribute);
        }

        //查询函数，返回两点之间指定Type的连边
        public IEdge GetEdgeByType(string startName, string endName, string attribute)
        {
            var query = GetEdgesByName( startName, endName );
            if( !query.Any() )
            {
                return null;
            }
            return query.First(x => x.Attribute == attribute);
        }
    }
}
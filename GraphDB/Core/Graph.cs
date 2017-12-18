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

        //�����ݱ���ΪXML�ļ����ӿڣ�
        public XmlDocument ToXML()
        {
            //�����������ݶ�����Ϊxml��ʽ
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

        //����ڵ�
        private void AddNode(INode newNode)
        {
            if( newNode == null )
            {
                return;
            }
            //�ڵ����ڵ��б�
            myNodeList.Add(newNode.Guid, newNode);
        }

        //ɾ���ڵ� by Guid
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

        //ɾ���ڵ� by Node
        private void RemoveNode(INode curNode)
        {
            //����ڵ���������
            ClearUnusedEdge(curNode.ClearEdge());
            //�ӽڵ��б����Ƴ��ڵ�
            myNodeList.Remove(curNode.Guid);
        }

        //�������� by Guid
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

        //�������� by Node
        private void AddEdge( INode curNode, INode tarNode, IEdge newEdge )
        {
            if (curNode == null || tarNode == null || newEdge == null)
            {
                return;
            }
            //���ߵ�ͷָ��ָ����ڵ�
            newEdge.From = curNode;
            //���ߵ�βָ��ָ��Ŀ��ڵ�
            newEdge.To = tarNode;
            //�������߼�����ʼ�ڵ��outbound
            if (curNode.AddEdge(newEdge) == false)
            {
                return;
            }
            //�������߼���Ŀ��ڵ��Inbound
            if (tarNode.RegisterInbound(newEdge) == false)
            {
                return;
            }
            //ȫ����ɺ����߼������������б�
            myEdgeList.Add(newEdge);
        }

        //�Ƴ����� by Guid
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

        //�Ƴ����� by Node
        private void RemoveEdge(INode curNode, INode tarNode, string attribute)
        {
            if (curNode == null || tarNode == null || attribute == null)
            {
                return;
            }
            //����ʼ�ڵ�ĳ����б���,������ֹ�ڵ��ź�Ŀ��ڵ��ź�����һ�µ�����
            IEdge curEdge = curNode.OutBound.First(x => x.To.Guid == tarNode.Guid && x.Attribute == attribute);
            if (curEdge == null)
            {//û�ҵ�ֱ�ӷ���
                return;
            }
            //��ʼ�ڵ�Outbound���Ƴ�����
            curNode.RemoveEdge(curEdge);
            //����ֹ�ڵ�InBound��ע������
            tarNode.UnRegisterInbound(curEdge);
            //ȫ����ɺ󣬴��������б����Ƴ��ñ�
            myEdgeList.Remove(curEdge);
        }

        //ɾ�����нڵ������
        public void ClearAll()
        {
            myEdgeList.Clear();
            myNodeList.Clear();
        }

        //ɾ�����б�����󶨵�����
        private void ClearUnusedEdge(List<IEdge> unusedList)
        {
            if (unusedList == null)
            {
                return;
            }
            //������б����������ߴ��������б���ɾ��
            foreach (IEdge edge in unusedList)
            {
                myEdgeList.Remove(edge);
            }
            //�������б�������
            unusedList.Clear();
        }

        //ɾ����������
        public void ClearAllEdge()
        {
            myEdgeList.Clear();
        }

        //����ڵ㣨�ӿڣ�
        public void AddNode(INode oriNode, out ErrorCode err)
        {
            if (oriNode == null )
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //���ڵ��Ƿ��Ѿ����ڡ�����һ�¡�
            if (ContainsNode(oriNode))
            {
                err = ErrorCode.NodeExists;
                return;
            }
            AddNode(oriNode);
            err = ErrorCode.NoError;
        }

        //�������ߣ��ӿڣ�
        public void AddEdge(string startName, string endName, IEdge newEdge, out ErrorCode err)
        {
            if (startName == null || endName == null || newEdge == null)
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //��ȡ��ʼ�ڵ㣬�����ڱ���
            var startNode = GetNodeByName(startName);
            if (startNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //��ȡ��ֹ�ڵ㣬�����ڱ���
            var endNode = GetNodeByName(endName);
            if (endNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //����������Ƿ������ͬ���͹�ϵ�����ڱ���
            if (ContainsEdge(startNode, endNode, newEdge))
            {
                err = ErrorCode.EdgeExists;
                return;
            }
            //�����������±�
            AddEdge(startNode, endNode, newEdge);
            err = ErrorCode.NoError;
        }

        //�������ߣ��ӿڣ�
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

        //�������ߣ��ӿڣ�
        public void AddEdgeByGuid(string startGuid, string endGuid, IEdge newEdge, out ErrorCode err)
        {
            AddEdge( startGuid, endGuid, newEdge );
            err = ErrorCode.NoError;
        }

        //���ڵ��Ƿ��Ѵ���
        public bool ContainsNode(string nodeName)
        {
            if (nodeName == null)
            {
                return false;
            }
            return Nodes.Any(x => x.Value.Name == nodeName);
        }

        //���ڵ��Ƿ��Ѵ���
        private bool ContainsNode(INode curNode)
        {
            if (curNode == null )
            {
                return false;
            }
            return Nodes.Any( x => x.Value.Name == curNode.Name );
        }

        //��������Ƿ��Ѵ���
        private bool ContainsEdge(INode startNode, INode endNode, IEdge curEdge)
        {
            if (startNode == null || endNode == null || curEdge == null)
            {
                return false;
            }
            var query = startNode.GetEdgesByGuid( endNode.Guid, EdgeDirection.Out );
            return query.Any(x => x.Attribute == curEdge.Attribute);
        }

        //ɾ���ڵ㣨�ӿڣ�
        public void RemoveNode(string nodeName, out ErrorCode err)
        {
            if (nodeName == null )
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            var tarNode = GetNodeByName(nodeName);
            //���ڵ��Ƿ��Ѿ����ڡ�����һ�¡�
            if (tarNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            RemoveNode(tarNode);
            err = ErrorCode.NoError;
        }

        //ɾ�����ߣ��ӿڣ�
        public void RemoveEdge(string startName, string endName, string attribute, out ErrorCode err)
        {
            if (startName == null || endName == null || attribute == null)
            {
                err = ErrorCode.InvalidParameter;
                return;
            }
            //��ȡ��ʼ�ڵ㣬�����ڱ���
            var startNode = GetNodeByName(startName);
            if (startNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            //��ȡ��ֹ�ڵ㣬�����ڱ���
            INode endNode = GetNodeByName(endName);
            if (endNode == null)
            {
                err = ErrorCode.NodeNotExists;
                return;
            }
            RemoveEdge( startNode, endNode, attribute);
            err = ErrorCode.NoError;
        }

        //��ѯ���������ؽڵ��б���ָ�����ƵĽڵ�
        public INode GetNodeByGuid(string nodeGuid)
        {
            if (nodeGuid == null)
            {
                return null;
            }
            //�����ڵ��б�
            var query = Nodes.Where( x => x.Value.Guid == nodeGuid ).Select( x => x.Value );
            if (!query.Any())
            {
                return null;
            }
            return query.First();
        }

        //��ѯ���������ؽڵ��б���ָ�����ƵĽڵ�
        public INode GetNodeByName(string nodeName)
        {
            if (nodeName == null)
            {
                return null;
            }
            //�����ڵ��б�
            var query = Nodes.Where( x => x.Value.Name == nodeName ).Select( x => x.Value );
            if (!query.Any())
            {
                return null;
            }
            return query.First();
        }

        //��ѯ���������ؽڵ��б���ָ�����͵Ľڵ�
        public IEnumerable<INode> GetNodesByType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            //�����ڵ��б�
            return Nodes.Where(x => x.Value.GetType() == type).Select(x => x.Value);
        }

        //��ѯ���������ؽڵ��б���ָ�����ƺ����͵Ľڵ�
        public IEnumerable<INode> GetNodesByNameAndType(string nodeName, Type type)
        {
            if (nodeName == null || type == null)
            {
                return null;
            }
            //�����ڵ��б�
            return Nodes.Where( x => x.Value.Name == nodeName && x.Value.GetType() == type ).Select( x => x.Value );
        }

        //��ѯ���������ؽڵ���ĳ��GUID������λ��
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

        //��ѯ���������ؽڵ���ĳ��Node������λ��
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

        //��ѯ����������ָ��GUID�Ľڵ�������
        public IEnumerable<IEdge> GetEdgesByGuid(string startGuid, string endGuid )
        {
            if (startGuid == null || endGuid == null)
            {
                return null;
            }
            return Edges.Where( x => x.FromGuid == startGuid && x.ToGuid == endGuid );
        }

        //��ѯ����������ָ�����ƵĽڵ�������
        public IEnumerable<IEdge> GetEdgesByName(string startName, string endName)
        {
            if (startName == null || endName == null)
            {
                return null;
            }
            return Edges.Where(x => x.From.Name == startName && x.To.Name == endName);
        }

        //��ѯ����������Start��ʼ������ΪType������
        public IEnumerable<IEdge> GetEdgesByType(string startName, string attribute)
        {
            if (startName == null || attribute == null)
            {
                return null;
            }
            return Edges.Where(x => x.From.Name == startName && x.Attribute == attribute);
        }

        //��ѯ��������������֮��ָ��Type������
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
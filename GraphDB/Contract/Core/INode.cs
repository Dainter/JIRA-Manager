using System.Collections.Generic;

using GraphDB.Contract.Enum;

namespace GraphDB.Contract.Core
{
    public interface INode
    {
        string Guid { get; }
        string Name { get; }
        int InDegree { get; }
        int OutDegree { get; }
        List<IEdge> OutBound { get; }
        List<IEdge> InBound { get; }
        //增加连边
        bool AddEdge( IEdge newEdge );
        //Inbound边注册
        bool RegisterInbound( IEdge newEdge );
        //去除连边
        bool RemoveEdge( IEdge curEdge );
        //清除所有连边,返回被清除的边列表
        List<IEdge> ClearEdge();
        //Inbound注销
        bool UnRegisterInbound( IEdge curEdge );
        //查找目标为指定GUID的连边
        IEnumerable<IEdge> GetEdgesByGuid( string nodeGuid, EdgeDirection direction );
        //查找目标为指定Name的连边
        IEnumerable<IEdge> GetEdgesByName( string nodeName, EdgeDirection direction );
        //查找类型为指定Type的连边
        IEnumerable<IEdge> GetEdgesByType( string edgeType, EdgeDirection direction );
        //节点摘要信息输出
        string DataOutput();
    }
}

namespace GraphDB.Contract.Core
{
    public interface IEdge
    {
        INode From { get; set; }
        string FromGuid { get; }
        INode To { get;  set;  }
        string ToGuid { get; }
        string Attribute { get; }
        string Value { get; set; }
    }
}
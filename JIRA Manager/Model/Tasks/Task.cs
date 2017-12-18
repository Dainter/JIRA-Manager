using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace JIRA.Manager.Model.Tasks
{
    public abstract class Task : Node
    {
        protected Task( string name ) : base( name ) {}
        protected Task( INode oriNode ) : base( oriNode ) {}
        protected Task( XmlElement xNode ) : base( xNode ) {}
    }
}

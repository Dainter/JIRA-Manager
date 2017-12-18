using System.Xml;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class SubTask : Task
    {
        public SubTask( string name ) : base( name ) {}
        public SubTask( INode oriNode ) : base( oriNode ) {}
        public SubTask( XmlElement xNode ) : base( xNode ) {}
    }
}
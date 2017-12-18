using System.Xml;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    class Defect : Task
    {
        public Defect( string name ) : base( name ) {}
        public Defect( INode oriNode ) : base( oriNode ) {}
        public Defect( XmlElement xNode ) : base( xNode ) {}
    }
}

using System.Xml;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class Wish : Task
    {
        public Wish( string name ) : base( name ) {}
        public Wish( INode oriNode ) : base( oriNode ) {}
        public Wish( XmlElement xNode ) : base( xNode ) {}
    }
}
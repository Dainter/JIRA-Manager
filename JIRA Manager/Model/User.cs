using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace JIRA.Manager.Model
{
    public class User : Node
    {
        public User( string name ) : base( name ) {}
        public User( INode oriNode ) : base( oriNode ) {}
        public User( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace JIRA.Manager.Model
{
    public class UserGroup : Node
    {
        public UserGroup( string name ) : base( name ) {}
        public UserGroup( INode oriNode ) : base( oriNode ) {}
        public UserGroup( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class AssignedTo : Edge
    {
        public AssignedTo( string newValue = "1" ) : base( CommonStrings.AssignedTo, newValue ) {}
        public AssignedTo( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class Assigned : Edge
    {
        public Assigned( string newValue = "1" ) : base( CommonStrings.Assigned, newValue ) {}
        public Assigned( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class Submit : Edge
    {
        public Submit( string newValue = "1" ) : base( CommonStrings.Submit, newValue ) {}
        public Submit( XmlElement xNode ) : base( xNode ) {}
    }
}
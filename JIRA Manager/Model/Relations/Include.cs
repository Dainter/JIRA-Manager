using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class Include : Edge
    {
        public Include( string newValue = "1" ) : base( CommonStrings.Include, newValue ) {}
        public Include( XmlElement xNode ) : base( xNode ) {}
    }
}
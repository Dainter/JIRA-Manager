using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class SubmitBy : Edge
    {
        public SubmitBy( string newValue = "1" ) : base( CommonStrings.SubmitBy, newValue ) {}
        public SubmitBy( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class Report : Edge
    {
        public Report( string newValue = "1" ) : base( CommonStrings.Report, newValue ) {}
        public Report( XmlElement xNode ) : base( xNode ) {}
    }
}
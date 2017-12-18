using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class ReportBy : Edge
    {
        public ReportBy( string newValue = "1" ) : base( CommonStrings.ReportBy, newValue ) {}
        public ReportBy( XmlElement xNode ) : base( xNode ) {}
    }
}
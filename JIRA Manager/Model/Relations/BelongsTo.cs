using System.Xml;

using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model.Relations
{
    public class BelongsTo : Edge
    {
        public BelongsTo( string newValue = "1" ) : base( CommonStrings.BelongsTo, newValue ) {}
        public BelongsTo( XmlElement xNode ) : base( xNode ) {}
    }
}
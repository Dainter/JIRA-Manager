using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    class JiraBug : JiraIssue
    {
        public JiraBug(Issue issue) : base(issue) {}
        public JiraBug( INode oriNode ) : base( oriNode ) {}
        public JiraBug( XmlElement xNode ) : base( xNode ) {}
    }
}

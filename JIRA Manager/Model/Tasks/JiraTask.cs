using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraTask : JiraIssue
    {
        public JiraTask( Issue issue) : base(issue) {}
        public JiraTask( INode oriNode ) : base( oriNode ) {}
        public JiraTask( XmlElement xNode ) : base( xNode ) {}
    }
}
using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraSubTask : JiraIssue
    {
        public JiraSubTask( Issue issue) : base(issue) {}
        public JiraSubTask( INode oriNode ) : base( oriNode ) {}
        public JiraSubTask( XmlElement xNode ) : base( xNode ) {}
    }
}
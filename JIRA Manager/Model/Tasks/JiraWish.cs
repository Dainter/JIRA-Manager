using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraWish : JiraIssue
    {
        public JiraWish( Issue issue) : base(issue) {}
        public JiraWish( INode oriNode ) : base( oriNode ) {}
        public JiraWish( XmlElement xNode ) : base( xNode ) {}
    }
}
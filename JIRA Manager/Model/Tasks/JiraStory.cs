using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraStory : JiraIssue
    {
        public JiraStory( Issue issue) : base(issue) {}
        public JiraStory( INode oriNode ) : base( oriNode ) {}
        public JiraStory( XmlElement xNode ) : base( xNode ) {}
    }
}
using System;
using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Core;

using JIRA.Manager.Utility;

using static System.String;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraIssue : Node
    {
        private readonly string mySummary;
        private readonly string myType;
        private readonly string myProject;
        private readonly string myPriority;
        private readonly JiraStatus.EnumStatus myStatus;

        public string Key => Name;
        [XmlSerializable]
        public string Summary => mySummary;
        [XmlSerializable]
        public string Type => myType;
        [XmlSerializable]
        public string Project => myProject;
        [XmlSerializable]
        public string Priority => myPriority;
        [XmlSerializable]
        public string Status => JiraStatus.ToString( myStatus );

        public JiraIssue( Issue issue ) : base( issue.Key.ToString() )
        {
            mySummary = issue.Summary;
            myType = issue.Type.Name;
            myProject = issue.Project;
            myPriority = issue.Priority.Name;
            myStatus = JiraStatus.ToEnum( issue.Status.Name );
        }

        public JiraIssue( INode oriNode ) : base( oriNode )
        {
        }

        public JiraIssue( XmlElement xNode) : base( xNode )
        {
            mySummary = xNode.GetText("Summary");
            myType = xNode.GetText("Type");
            myProject = xNode.GetText("Project");
            myPriority = xNode.GetText("Priority");
            myStatus = JiraStatus.ToEnum(xNode.GetText("Status"));
        }

        public override string ToString()
        {
            string result = Type + " " + Name;

            result += ": " + Summary;
            return result;
        }

        public int CompareTo(object obj)
        {
            var issue = obj as JiraIssue;
            if (issue != null)
            {
                return Compare(Name, issue.Name, StringComparison.Ordinal);
            }
            return 1;
        }
    }
}

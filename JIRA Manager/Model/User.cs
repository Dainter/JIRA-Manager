using System;
using System.Xml;

using Atlassian.Jira;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Core;

using JIRA.Manager.Utility;

namespace JIRA.Manager.Model
{
    public class User : Node
    {
        private readonly string myName;
        private readonly string myDepartment;
        private readonly string myEmail;

        [XmlSerializable]
        public string DisplayName => ToString();

        public string Department => myDepartment;

        [XmlSerializable]
        public string Email => myEmail;

        public User( JiraUser user ) : base(user.Username)
        {
            ExtractUserInfo(user.DisplayName, out myName, out myDepartment);
            myEmail = user.Email;
        }

        public User( INode oriNode ) : base( oriNode ) {}

        public User( XmlElement xNode ) : base( xNode )
        {
            string displayName = xNode.GetText( "DisplayName" );
            ExtractUserInfo( displayName, out myName, out myDepartment );
            myEmail = xNode.GetText("Email");
        }

        private void ExtractUserInfo(string displayName, out string name, out string department)
        {
            if (displayName.Contains("(") == false)
            {
                name = displayName;
                department = "";
                return;
            }
            name = displayName.Substring(0, displayName.IndexOf('(')).Trim();
            int iStart = displayName.LastIndexOf("(", StringComparison.Ordinal);
            int iEnd = displayName.LastIndexOf(")", StringComparison.Ordinal);
            if (iEnd - iStart - 1 < 0)
            {
                department = "";
                return;
            }
            department = displayName.Substring(iStart + 1, iEnd - iStart - 1).Trim();
        }

        public override string ToString()
        {
            string result = myName;

            result += "\n(" + Department + ")";
            return result;
        }

        public int CompareTo(object obj)
        {
            var user = obj as User;
            if( user != null )
            {
                return String.Compare(myName, user.Name, StringComparison.Ordinal);
            }
            return 1;
        }
    }
}
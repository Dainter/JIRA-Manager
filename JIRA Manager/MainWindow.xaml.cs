
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Atlassian.Jira;
using Atlassian.Jira.Remote;

using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Core;
using GraphDB.Tool;

using JIRA.Manager.Model;
using JIRA.Manager.Model.Relations;
using JIRA.Manager.Model.Tasks;

namespace JIRA.Manager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Jira myJira;
        private List<JiraIssue> myIssues;
        private List<User> myUsers;
        private readonly Graph myProjectGraph;

        public MainWindow()
        {
            InitializeComponent();
            myProjectGraph = new Graph("JiraManager.xml");
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //DataInit();
            ConfigWindow winConfig = new ConfigWindow("JiraManager.xml");
            winConfig.ShowDialog();
        }

        async void DataInit()
        {
            var settings = new JiraRestClientSettings()
            {
                EnableRequestTrace = true
            };
            ErrorCode err;
            // create a connection to JIRA using the Rest client
            myJira = Jira.CreateRestClient("http://intranet.ctdc.siemens.com.cn:8995", "z003hkns", "!QAZ5tgb", settings);
            GetIssues();
            //GetUsers();
            myUsers = new List<User>();
            foreach (Issue curItem in IssueListView.Items)
            {
                if (myUsers.Any(x => x.Name == curItem.Assignee))
                {
                    continue;
                }
                if (curItem.Assignee == null)
                {
                    continue;
                }
                var newUser = await GetUsers(curItem.Assignee);
                myUsers.Add(newUser);
                myProjectGraph.AddNode(newUser, out err);
            }

            myIssues = new List<JiraIssue>();
            foreach (Issue curItem in IssueListView.Items)
            {
                var newIssue = new JiraIssue(curItem);
                myIssues.Add(newIssue);
                myProjectGraph.AddNode(newIssue, out err);
                if( curItem.Reporter != null )
                {
                    INode reporter = myUsers.First( x => x.Name == curItem.Reporter );
                    myProjectGraph.AddEdge( newIssue, reporter, new ReportBy(), out err );
                    myProjectGraph.AddEdge(reporter, newIssue, new Report(), out err);
                }
                if (curItem.Assignee != null)
                {
                    INode assignee = myUsers.First(x => x.Name == curItem.Assignee);
                    myProjectGraph.AddEdge(newIssue, assignee, new AssignedTo(), out err);
                    myProjectGraph.AddEdge(assignee, newIssue, new Assigned(), out err);
                }
            }
            
            myProjectGraph.SaveDataBase(out err);
            return;
        }

        void GetIssues()
        {
            int requestCount = 200;
            IOrderedQueryable<Issue> issues;
            do
            {
                requestCount *= 2;
                myJira.Issues.MaxIssuesPerRequest = requestCount;
                // use LINQ syntax to retrieve issues
                issues = from i in myJira.Issues.Queryable
                         where i.Project == "SSME_VC50"
                         orderby i.Summary
                         select i;
            } while (issues.Count() >= requestCount);

            IssueListView.ItemsSource = issues;
        }

        async Task<User> GetUsers(string gid)
        {
            var users = await myJira.Users.SearchUsersAsync(gid);
            if( !users.Any() )
            {
                return null;
            }
            User newUser = new User( users.First() );
            return newUser;
        }

        async void GetWorklogs(Atlassian.Jira.Issue issue )
        {
            //var issue = await myJira.Issues.GetIssueAsync("VC50-316");
            //// add a worklog
            //await issue.AddWorklogAsync("1h");

            //// add worklog with new remaining estimate
            //await issue.AddWorklogAsync("1m", WorklogStrategy.NewRemainingEstimate, "4h");

            // retrieve worklogs
            var worklogs = await issue.GetWorklogsAsync();
            WorkLogListView.ItemsSource = worklogs;
        }
    }
}

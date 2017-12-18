
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Atlassian.Jira;
using Atlassian.Jira.Remote;

namespace JIRA.Manager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Jira myJira;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataInit();
        }

        async void DataInit()
        {
            var settings = new JiraRestClientSettings()
            {
                EnableRequestTrace = true
            };
            // create a connection to JIRA using the Rest client
            myJira = Jira.CreateRestClient("http://intranet.ctdc.siemens.com.cn:8995", "z003hkns", "!QAZ5tgb", settings);
            myJira.Issues.MaxIssuesPerRequest = 200;
            //// use LINQ syntax to retrieve issues
            var issues = from i in myJira.Issues.Queryable
                         //where i.Assignee == "Z00315RR"
                         orderby i.Summary
                         select i;
            IssueListView.ItemsSource = issues;
            var issue = await myJira.Issues.GetIssueAsync("VC50-316");
            //Issue I; I.Reporter
            return;
        }

        async void GetWorklogs( Issue issue )
        {
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

using System.Collections.Generic;

namespace JIRA.Manager.Model.Tasks
{
    public class JiraStatus
    {
        public static List<string> StatusList = 
            new List<string> { "To Do", "Done", "In Observe", "In Progress", "Defer", " Block", "SSME Colleague", "Syngo", "Wait CodeReview", "Retest", "Terminated" };

        public enum EnumStatus
        {
            ToDo = 0,
            Done = 1,
            InObserve,
            InProgress,
            Defer,
            Block,
            SsmeColleague,
            Syngo,
            Retest,
            WaitCodeReview,
            Terminated,
        }

        public static string ToString(EnumStatus eState)
        {
            switch(eState)
            {
                case EnumStatus.ToDo:
                    return "To Do";
                case EnumStatus.Done:
                    return "Done";
                case EnumStatus.InObserve:
                    return "In Observe";
                case EnumStatus.InProgress:
                    return "In Progress";
                case EnumStatus.Defer:
                    return "Defer";
                case EnumStatus.Block:
                    return "Block";
                case EnumStatus.SsmeColleague:
                    return "SSME Colleague";
                case EnumStatus.Syngo:
                    return "Syngo";
                case EnumStatus.WaitCodeReview:
                    return "Wait CodeReview";
                case EnumStatus.Retest:
                    return "Retest";
                case EnumStatus.Terminated:
                    return "Terminated";
                default:
                    return "To Do";
            }
        }

        public static EnumStatus ToEnum(string strStatus)
        {
            switch (strStatus.ToLower())
            {
                case "to do":
                    return EnumStatus.ToDo;
                case "done":
                    return EnumStatus.Done;
                case "in observe":
                    return EnumStatus.InObserve;
                case "in progress":
                    return EnumStatus.InProgress;
                case "defer":
                    return EnumStatus.Defer;
                case "block":
                    return EnumStatus.Block;
                case "ssme colleague":
                    return EnumStatus.SsmeColleague;
                case "syngo":
                    return EnumStatus.Syngo;
                case "wait codereview":
                    return EnumStatus.WaitCodeReview;
                case "retest":
                    return EnumStatus.Retest;
                case "terminated":
                    return EnumStatus.Terminated;
                default:
                    return EnumStatus.ToDo;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    /// <summary>
    /// visitor action status
    /// </summary>
    public enum ActionStatus
    {
        EndRequest = 1,
        ErrorRequest = 2
    }

    /// <summary>
    /// http user action
    /// </summary>
    public class VisitorAction
    {
        private string actionId;
        private DateTime? beginTime;
        private DateTime? endTime;
        private string url;
        private ActionStatus status;
        private string content;

        public string ActionId { get => actionId;  }
        public DateTime? BeginTime { get => beginTime;  }
        public DateTime? EndTime { get => endTime; set => endTime = value; }
        public string Content { get => content; set => content = value; }
        public string Url { get => url;  }
        public ActionStatus Status { get => status; set => status = value; }

        public VisitorAction(string requestId, string requestUrl)
        {
            actionId = requestId;
            url = requestUrl;
            beginTime = DateTime.Now;
        }

        public VisitorAction(string requestId, string requestUrl,DateTime requestBegin,DateTime requestEnd,ActionStatus requestStatus)
        {
            actionId = requestId;
            url = requestUrl;
            beginTime = requestBegin;
            endTime = requestEnd;
            status = requestStatus;
        }
    }
}

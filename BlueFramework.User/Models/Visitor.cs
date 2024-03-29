﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    /// <summary>
    /// http visitor 
    /// cache user and user's action
    /// </summary>
    public class Visitor
    {
        private string visitorId = string.Empty;
        private bool isAuthenticated;
        private DateTime lastTime;
        private DateTime loginTime;
        private Queue<VisitorAction> actions = null;
        private IUser currentUser;

        public string VisitorId { get => visitorId;  }
        public bool IsAuthenticated { get => isAuthenticated; set => isAuthenticated = value; }
        public Queue<VisitorAction> Actions { get => actions;  }
        public DateTime LastTime { get => lastTime;  }
        public DateTime LoginTime { get => loginTime; }

        public Visitor()
        {
            visitorId = Guid.NewGuid().ToString();
            isAuthenticated = false;
            currentUser = new Guest();
            currentUser.UserName = visitorId;
            loginTime = DateTime.Now;
            actions = new Queue<VisitorAction>();
        }

        public Visitor(IUser user)
        {
            visitorId = user.UserName;
            lastTime = DateTime.Now;
            loginTime = DateTime.Now;
            currentUser = user;
            actions = new Queue<VisitorAction>();
        }

        /// <summary>
        /// get current user
        /// </summary>
        /// <returns></returns>
        public IUser GetUser()
        {
            return currentUser;
        }

        /// <summary>
        /// add action
        /// </summary>
        /// <param name="userAction"></param>
        public void AddAction(VisitorAction userAction)
        {
            actions.Enqueue(userAction);
            if (actions.Count > 100)
            {
                actions.Dequeue();
            }
        }
    }

    public class VisitVO
    {
        public string name { get; set; }
        public string caption { get; set; }
        public DateTime loginTime { get; set; }
        public DateTime lastTime { get; set; }
    }
}

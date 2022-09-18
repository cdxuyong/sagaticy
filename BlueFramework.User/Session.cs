using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlueFramework.User.Models;

namespace BlueFramework.User
{
    /// <summary>
    /// all users session
    /// get current user context from session
    /// </summary>
    public class Session
    {
        private static Session currentSession;
        private Timer freshTimer = null;
        private AutoResetEvent freshTimerSingle = null;
        private Dictionary<string, Visitor> visitors;
        private Dictionary<string, VisitorAction> lastActions;

        public static Session Current
        {
            get
            {
                return currentSession;
            }
        }

        public static void Initialize(int timerSpan)
        {
            currentSession = new Session(timerSpan);
        }

        private Session(int timerSpan)
        {
            lastActions = new Dictionary<string, VisitorAction>();
            visitors = new Dictionary<string, Visitor>();
            freshTimerSingle = new AutoResetEvent(true);
            freshTimer = new Timer(FreshVisitors, null, 1000, timerSpan);
        }

        private void FreshVisitors(object state)
        {
            if (!freshTimerSingle.WaitOne(1))
            {
                return;
            }

            try
            {
                
            }
            catch (Exception ex)
            {
                //
            }
            finally
            {
                freshTimerSingle.Set();
            }
        }

        /// <summary>
        /// get visitor from session ,if not find return null
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public Visitor GetVisitor(string visitorId)
        {
            if (visitors.ContainsKey(visitorId))
            {
                return visitors[visitorId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// add visitor to session
        /// </summary>
        /// <param name="visitor"></param>
        public void AddVisitor(Visitor visitor)
        {
            if (!visitors.ContainsKey(visitor.VisitorId))
            {
                visitors.Add(visitor.VisitorId, visitor);
            }
        }

        /// <summary>
        /// remove visitor
        /// </summary>
        /// <param name="visitorId"></param>
        public void RemoveVisitor(string visitorId)
        {
            if (visitors.ContainsKey(visitorId))
            {
                visitors.Remove(visitorId);
            }
        }

        public void PushAction(VisitorAction action)
        {
            lastActions.Add(action.ActionId, action);
        }

        public VisitorAction PopAction(string actionId)
        {
            if (string.IsNullOrEmpty(actionId))
                return null;
            VisitorAction action;
            if (lastActions.TryGetValue(actionId, out action))
            {
                lastActions.Remove(actionId);
                return action;
            }
            else
                return null;
            
        }
    }
}

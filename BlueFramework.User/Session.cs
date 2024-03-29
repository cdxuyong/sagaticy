﻿using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, Visitor> visitors;
        private ConcurrentDictionary<string, VisitorAction> lastActions;

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

        public int CountOnlines()
        {
            return visitors.Count;
        }

        private Session(int timerSpan)
        {
            lastActions = new ConcurrentDictionary<string, VisitorAction>();
            visitors = new ConcurrentDictionary<string, Visitor>();
            freshTimerSingle = new AutoResetEvent(true);
            freshTimer = new Timer(FreshVisitors, null, 1000, timerSpan);
        }

        private void FreshVisitors(object state)
        {
            if (!freshTimerSingle.WaitOne(1))
            {
                return;
            }
            // 清理登录超时的用户
            try
            {
                var removes = new List<string>();
                foreach(var v in visitors.Values)
                {
                    var time = v.Actions.Where(x=>x.EndTime!=null).Max(x => x.EndTime);
                    // 超时判断
                    if (time == null || DateTime.Now.Subtract(time.Value).TotalMinutes > 60)
                    {
                        removes.Add(v.VisitorId);
                    }
                }
                if(removes.Count > 0)
                {
                    foreach(var id in removes)
                    {
                        RemoveVisitor(id);
                    }
                }
            }
            catch (Exception ex)
            {
                BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(ex.Message);
                BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(ex.StackTrace);
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
                var succ = visitors.TryAdd(visitor.VisitorId, visitor);
                int i = 0;
                while (!succ && i < 5)
                {
                    i++;
                    succ = visitors.TryAdd(visitor.VisitorId, visitor);
                }
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
                Visitor v = null;
                visitors.TryRemove(visitorId,out v);
            }
        }

        public void PushAction(VisitorAction action)
        {
            try
            {
                if (!visitors.ContainsKey(action.ActionId))
                {
                    int i = 0;
                    var succ = lastActions.TryAdd(action.ActionId, action);
                    while (!succ && i<5)
                    {
                        i++;
                        succ = lastActions.TryAdd(action.ActionId, action);
                    }
                }
            }
            catch(Exception ex)
            {
                BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(ex.Message);
            }

        }

        public VisitorAction PopAction(string actionId)
        {
            if (string.IsNullOrEmpty(actionId))
                return null;
            VisitorAction action;
            try
            {
                if (lastActions.TryGetValue(actionId, out action))
                {
                    VisitorAction a = null;
                    lastActions.TryRemove(actionId,out a);
                    return action;
                }
            }
            catch(Exception ex)
            {
                BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(ex.Message);
            }
            return null;
            
        }

        public List<VisitVO> GetOnlines()
        {
            List<VisitVO> list = new List<VisitVO>();
            foreach(var v in visitors.Values)
            {
                var u = v.GetUser();
                var o = new VisitVO()
                {
                    name = u.UserName,
                    caption = u.TrueName,
                    loginTime = v.LoginTime,
                    lastTime = v.Actions.Max(x=>x.BeginTime).Value
                };
                list.Add(o);
            }
            return list;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BlueFramework.User;
using BlueFramework.User.Models;

namespace HrServiceCenterWeb.Lib
{
    /// <summary>
    /// http request runtime
    /// </summary>
    public class RequestModule : IHttpModule, IRequiresSessionState
    {
        /// <summary>
        /// 实现接口的Init方法
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            //context.RequestCompleted
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
            context.Error += new EventHandler(context_Error);
        }

        // 请求开始无法获取用户信息
        void context_BeginRequest(object sender, EventArgs e)
        {
            System.Web.HttpApplication app = (System.Web.HttpApplication)sender;
            // url rewrite
            HttpContext context = app.Context;
            string actionId = System.Guid.NewGuid().ToString();
            context.Response.AddHeader("ACTION-ID", actionId);
            VisitorAction action = new VisitorAction(actionId, context.Request.Url.AbsoluteUri);
            BlueFramework.User.Session.Current.PushAction(action);
        }

        // 请求结束里面获取用户信息
        void context_EndRequest(object sender, EventArgs e)
        {
            System.Web.HttpApplication app = (System.Web.HttpApplication)sender;
            // url rewrite
            HttpContext context = app.Context;
            string url = context.Request.Url.AbsolutePath;
            string actionId = context.Response.Headers["ACTION-ID"];
            VisitorAction action = BlueFramework.User.Session.Current.PopAction(actionId);

            if (UserContext.CurrentVisitor != null)
            {
                if (action != null)
                {
                    action.EndTime = DateTime.Now;
                    action.Status = ActionStatus.EndRequest;
                    UserContext.CurrentVisitor.AddAction(action);
                }
            }
        }

        // 请求错误日志
        void context_Error(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            Exception ex = context.Server.GetLastError();
            string error = string.Format("错误信息：{0}. 堆栈信息：{1}.", ex.Message, ex.StackTrace);
            if (UserContext.CurrentVisitor != null)
            {
                string actionId = context.Request.Headers["ACTION-ID"];
                VisitorAction action = BlueFramework.User.Session.Current.PopAction(actionId);
                if (action != null)
                {
                    action.EndTime = DateTime.Now;
                    action.Status = ActionStatus.ErrorRequest;
                    action.Content = error;
                    UserContext.CurrentVisitor.AddAction(action);
                }
            }

        }


        public void Dispose()
        {
        }
    }
}
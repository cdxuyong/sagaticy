using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using Newtonsoft.Json;
namespace HrServiceCenterWeb.Controllers
{
    public class BaseController: Controller
    {
        public class MessageInfo
        {
            public string Id { get; set; }
            public string Message { get; set; }
            public DateTime AccessTime { get; set; }
            public string Tag { get; set; }
            public string Url { get; set; }
        }

        protected static Dictionary<string, MessageInfo> _messages = new Dictionary<string, MessageInfo>();

        protected override void OnException(ExceptionContext filterContext)
        {
            BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(
                string.Format("{0},{1} throw exception : {2}", 
                    filterContext.HttpContext.Request.Url,
                    filterContext.Exception.Message,
                    filterContext.Exception.Source
                )
            );
            base.OnException(filterContext);
        }

        /// <summary>
        /// 跳转至错误页面
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult Error(string message)
        {
            var info = new MessageInfo()
            {
                Id = Guid.NewGuid().ToString("N"),
                AccessTime = DateTime.Now,
                Tag = "error",
                Url = HttpContext.Request.Url.AbsolutePath,
                Message = message
            };
            _messages.Add(info.Id, info);
            var url = $"../Home/Message?id="+info.Id;
            return this.Redirect(url);
        }
    }
}
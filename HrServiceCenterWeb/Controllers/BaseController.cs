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
    }
}
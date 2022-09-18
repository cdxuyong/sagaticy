using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BlueFramework.User;
using BlueFramework.Common.Logger;

namespace HrServiceCenterWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoggerFactory.CreateDefault().Init();
            LoggerFactory.CreateDefault().Info("IIS start");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // refresh online users on 30s
            BlueFramework.User.Session.Initialize(1000 * 30);
            // init orm
            BlueFramework.Blood.Session.Init();
            // init hr cache
            HrServiceCenterWeb.Manager.BaseCodeProvider.Init();
        }
    }
}

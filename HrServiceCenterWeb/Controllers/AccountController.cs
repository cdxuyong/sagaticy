using System;
using System.Collections.Generic;
using System.Linq;
using HrServiceCenterWeb.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using BlueFramework.User;
using BlueFramework.User.Models;

namespace HrServiceCenterWeb.Controllers
{
    public class AccountController : BaseController
    {
        readonly string cookie_name = "UP_TESTANYSIS_NAME";
        readonly string cookie_password = "UP_TESTANYSIS_PASSWORD";
        readonly string cookie_remember = "UP_TESTANYSIS_REMEMBER";

        LoginModel lgmodel = new LoginModel();

        // GET: /Account/
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            bool remember = (BlueFramework.Common.Http.Cookie.GetCookie(cookie_remember) == "1") ? true : false;
            string userName = string.Empty;
            string password = string.Empty;
            if (remember)
            {
                userName = BlueFramework.Common.Http.Cookie.GetCookie(cookie_name);
                password = BlueFramework.Common.Http.Cookie.GetCookie(cookie_password);
                lgmodel.UserName = userName;
                lgmodel.Password = password;
                lgmodel.RememberMe = true;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(lgmodel);
        }

        //
        // POST: /Account/Create
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool pass = UserContext.Login(model.UserName, model.Password);
                    if (pass)
                    {
                        if (model.RememberMe)
                        {
                            BlueFramework.Common.Http.Cookie.WriteCookie(cookie_remember, "1");
                            BlueFramework.Common.Http.Cookie.WriteCookie(cookie_name, model.UserName);
                            BlueFramework.Common.Http.Cookie.WriteCookie(cookie_password, model.Password);
                        }
                        else
                        {
                            BlueFramework.Common.Http.Cookie.ClearCookie(cookie_remember);
                            BlueFramework.Common.Http.Cookie.ClearCookie(cookie_name);
                            BlueFramework.Common.Http.Cookie.ClearCookie(cookie_password);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("ERROR", "账号和密码不匹配");
                    return View(lgmodel);
                }
                else
                {
                    ModelState.AddModelError("ERROR", "服务器内部出错，请联系管理员");
                    return View(lgmodel);
                }
            }
            catch (Exception ex)
            {
                // 如果我们进行到这一步时某个地方出错，则重新显示表单
                ModelState.AddModelError("ERROR", "服务器内部出错，请联系管理员");
                return View(model);
            }
        }

        // GET: /Account/Logoff
        public ActionResult LogOff()
        {
            UserContext.Logout();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult UserPage()
        {
            //LoginModel user = new LoginModel();
            //user.UserName = UserContext.CurrentUser.UserName;
            this.lgmodel.UserName = UserContext.CurrentUser.UserName;
            return View(lgmodel);
        }

        [HttpPost]
        public ActionResult UserPage(LoginModel model)
        {
            if (model.Password == "******" || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }
            else
            {
                UserManager userManager = new UserManager();
                int userId = UserContext.CurrentUser.UserId;
                userManager.ModifyPassword(userId, model.Password);
                return LogOff();
            }

        }
    }
}

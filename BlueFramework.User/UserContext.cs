using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using BlueFramework.User.Models;

namespace BlueFramework.User
{
    public class UserContext
    {
        //private static string USER_CONTEXT_SESSION_NAME = "GUEST_VISITORID_SESSION";
        public static bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.Request.IsAuthenticated;
            }
        }

        private static string CurrentVisitorId
        {
            get
            {
                if (IsAuthenticated)
                {
                    return HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// get current visitor
        /// </summary>
        public static Visitor CurrentVisitor
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentVisitorId))
                    return null;
                else
                {
                    Visitor visitor = Session.Current.GetVisitor(CurrentVisitorId);
                    if (visitor == null)
                    {
                        if (IsAuthenticated)
                        {
                            // load user from db
                            UserInfo user = new UserManager().GetUser(CurrentVisitorId);
                            visitor = new Visitor(user);
                            Session.Current.AddVisitor(visitor);
                        }

                    }

                    return visitor;
                }
            }
        }

        /// <summary>
        /// get current user
        /// </summary>
        public static IUser Current
        {
            get
            {
                if (IsAuthenticated)
                {
                    Visitor visitor = CurrentVisitor;
                    return visitor.GetUser() ;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// get current userinfo 
        /// if user is not userinfo , will return null
        /// </summary>
        public static UserInfo CurrentUser
        {
            get
            {
                try
                {
                    UserInfo user = (UserInfo)Current;
                    return user;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool Login(string userName,string password)
        {
            bool pass = Validate(userName, password);
            if (pass)
            {
                UserInfo user = new UserManager().GetUser(userName);
                Visitor visitor = new Visitor(user);
                Session.Current.AddVisitor(visitor);
                FormsAuthentication.SetAuthCookie(userName, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Logout()
        {
            string visitorId = UserContext.CurrentVisitor.VisitorId ;
            Session.Current.RemoveVisitor(visitorId);
            FormsAuthentication.SignOut();

        }

        public static bool Validate(string userName, string password)
        {
            string encodePassword = MD5Encrypt64(password);
            UserManager userManager = new UserManager();
            bool pass = userManager.ValidatePassword(userName, encodePassword);
            return pass;
        }

        /// <summary>
        /// 64位的MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt64(string password)
        {
            string cl = password;
            MD5 md5 = MD5.Create();//实例化一个md5对象
            //加密后是一个字节类型的数组，需要注意编码的选择
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }
    }
}

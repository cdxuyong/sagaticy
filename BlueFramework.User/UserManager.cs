using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueFramework.User.Models;
using BlueFramework.User.DataAccess;
using System.Security.Cryptography;

namespace BlueFramework.User
{
    public class UserManager
    {
        UserAccess ua = new UserAccess();

        public static UserManager Instance
        {
            get
            {
                UserManager um = new UserManager();
                return um;
            }
        }

        public UserInfo GetUser(int userId)
        {
            UserAccess userDao = new UserAccess();
            return userDao.GetUserInfo(userId);
        }

        public UserInfo GetUser(string userName)
        {
            UserAccess userDao = new UserAccess();
            return userDao.GetUser(userName);
        }

        public bool ValidatePassword(string userName, string password)
        {
            UserAccess userDao = new UserAccess();
            UserInfo user = userDao.GetUserByName(userName);
            if (user.Password == password)
            {
                return true;
            }
            return false;
        }

        public List<OrgnizationInfo> GetOrgnizations()
        {
            UserAccess userDao = new UserAccess();
            return userDao.GetOrgnizations();
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddAccount(UserInfo user)
        {
            int re = -1;
            user.Password = MD5Encrypt64(user.Password);
            if (GetUserByName(user.UserName).UserName != null)
            {
                re = 3;
                return re;
            }
            if (ua.AddAccount(user))
            {
                re = 4;
            }
            else
            {
                re = -4;
            }
            return re;

        }

        /// <summary>
        /// 删除用户，返回信息
        /// </summary>
        /// <returns>0：失败，1：成功，2：不能删除当前登录用户</returns>
        public int DeleteUser(UserInfo user)
        {
            if (user.UserId == UserContext.CurrentUser.UserId)
                return -1;
            if (ua.Delete(user))
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>返回用户信息</returns>
        public UserInfo GetUserByName(string userName)
        {
            UserInfo ui = ua.GetUserByName(userName);
            return ui;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns>返回用户信息</returns>
        public UserInfo QueryUserById(int userId)
        {
            UserInfo ui = ua.QueryUserById(userId);
            return ui;
        }

        /// <summary>
        /// 获取用户信息（包括权限）
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public UserInfo GetFullUserInfo(string userName)
        {
            SysAccess sa = new SysAccess();
            UserInfo ui = ua.GetUser(userName);
            return ui;
        }

        /// <summary>
        /// 修改账号密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public bool ModifyPassword(int userId, string password)
        {
            string pwd = MD5Encrypt64(password);
            return ua.InitPwd(userId, pwd);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="oldName">原用户名</param>
        /// <returns>成功返回true</returns>
        public int UpdateAccount(UserInfo user, string oldName)
        {
            if (!string.IsNullOrEmpty(GetUserByName(user.UserName).UserName))
            {
                if (user.UserName != oldName)
                {
                    return -1;
                }
            }
            return ua.UpdateUser(user);
        }

        /// <summary>
        /// 条件查询用户列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<UserInfo> GetUsers(UserInfo user)
        {
            return ua.GetUsers(user);
        }

        public List<UserInfo> GetUserList()
        {
            return ua.GetUserList();
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

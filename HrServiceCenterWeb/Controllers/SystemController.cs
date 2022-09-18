using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueFramework.User;
using BlueFramework.User.Models;

namespace HrServiceCenterWeb.Controllers
{
    public class SystemController : BaseController
    {
        //
        // GET: System

        public ActionResult Index()
        {
            return View();
        }

        #region common

        //
        //GET：GetOrgs

        public ActionResult GetOrgs()
        {
            UserManager um = new UserManager();
            List<OrgnizationInfo> orgs = um.GetOrgnizations();
            return Json(orgs);
        }
        #endregion

        #region User

        //
        //GET: 

        public ActionResult UserManage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UsersQuery(UserInfo userquery, string userName)
        {
            userquery.UserName = userName;
            userquery.TrueName = userName;
            return Json(UserManager.Instance.GetUsers(userquery));
        }

        //
        //GET:AddUser

        public ActionResult AddUser(UserInfo user)
        {
            int result = UserManager.Instance.AddAccount(user);
            string backMsg = string.Empty;
            switch (result)
            {
                case 3:
                    backMsg = "该账户已存在";
                    break;
                case 4:
                    backMsg = "新增成功";
                    break;
                case -4:
                    backMsg = "新增失败";
                    break;
            }
            return Json(backMsg);
        }

        public ActionResult UpDateUser(UserInfo user,string oldName)
        {
            int result = UserManager.Instance.UpdateAccount(user,oldName);
            string backMsg = string.Empty;
            switch (result)
            {
                case 1:
                    backMsg = "更新成功";
                    break;
                case 0:
                    backMsg = "更新失败";
                    break;
                case -1:
                    backMsg = "账户名已存在";
                    break;
            }
            return Json(backMsg);
        }

        //
        //GET:LoadUser

        public ActionResult LoadUser(UserInfo user)
        {
            return Json(UserManager.Instance.QueryUserById(user.UserId));
        }

        //
        //GET:InitPwd

        public ActionResult InitPwd(UserInfo user)
        {
            string backMsg = string.Empty;
            if (UserManager.Instance.ModifyPassword(user.UserId, user.Password))
            {
                backMsg = "重置密码成功";
            }
            else
            {
                backMsg = "重置密码失败";
            }
            return Json(backMsg);
        }

        //
        //GET:DeleteUser

        public ActionResult DeleteUser(UserInfo user)
        {
            int result = UserManager.Instance.DeleteUser(user);
            string backMsg = string.Empty;
            switch (result)
            {
                case -1:
                    backMsg = "不能删除当前登录用户";
                    break;
                case 1:
                    backMsg = "删除成功";
                    break;
                case 0:
                    backMsg = "删除失败";
                    break;
            }
            return Json(backMsg);
        }

        #endregion

        #region Role

        //
        // GET:RoleManager

        public ActionResult RoleManage()
        {
            return View();
        }

        //
        //GET:RolesQuery

        public ActionResult RolesQuery(RoleInfo role,string roleName)
        {
            role.RoleName = roleName;
            return Json(RoleManager.Instance.GetRoleList(role));
        }

        //
        //GET:AddRole

        [HttpPost]
        public ActionResult AddRole(RoleInfo role)
        {
            int result = RoleManager.Instance.AddOnlyRole(role);
            string msg = string.Empty;
            switch (result)
            {
                case -1:
                    msg = "该角色名已存在";
                    break;
                case 1:
                    msg = "新增成功";
                    break;
                case 0:
                    msg = "新增失败";
                    break;
            }
            return Json(msg);
        }

        //
        //GET:DeleteRole

        public ActionResult DeleteRole(RoleInfo role)
        {
            string msg = string.Empty;
            if (RoleManager.Instance.DeleteRole(role))
            {
                msg = "删除成功";
            }
            else
            {
                msg = "删除失败";
            }
            return Json(msg);
        }

        //
        //GET:LoadRole

        public ActionResult LoadRole(RoleInfo role)
        {
            return Json(RoleManager.Instance.GetRoleByRoleId(role.RoleId));
        }

        //
        //GET:

        public ActionResult UpdateOnlyRole(RoleInfo role,string oldName)
        {
            int result = RoleManager.Instance.UpdateOnlyRole(role,oldName);
            string msg = string.Empty;
            switch (result)
            {
                case -1:
                    msg = "该角色名已存在";
                    break;
                case 1:
                    msg = "修改成功";
                    break;
                case 0:
                    msg = "修改失败";
                    break;
            }
            return Json(msg);
        }

        //
        //GET:LoadMenus
        [HttpPost]
        public ActionResult LoadMenus()
        {
            return Json(RightManager.Instance.getRightTree("0"));
        }

        //
        //GET:GetRoleMenus

        public ActionResult GetRoleMenus(RoleInfo role)
        {
            int[] imenu = RoleManager.Instance.GetMenuRights(role.RoleId);
            string[] smenu = imenu.Select(i => i.ToString()).ToArray();
            string strmenu = string.Join(",", smenu);
            return Json(strmenu);
        }

        //
        //POST:SaveRoleMenus

        public ActionResult SaveRoleMenus(RoleInfo role)
        {
            int[] menuArr = null;
            if (role.Menus != null && role.Menus != "")
            {
                string strMenu = role.Menus;
                string[] strArr = strMenu.Split(',');
                menuArr = new int[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    menuArr[i] = Convert.ToInt32(strArr[i]);
                }
            }
            string msg = string.Empty;
            if (RightManager.Instance.SaveMenuRights(role.RoleId, menuArr))
            {
                msg = "保存成功";
            }
            else
            {
                msg = "保存失败";
            }
            return Json(msg);
        }

        #endregion

        #region ROLE GROUPING
        //
        // GET:RoleGrouping

        public ActionResult RoleGrouping(int RoleId)
        {
            ViewBag.RoleId = RoleId;
            return View();
        }

        //
        //POST:LoadRoleUsers
        [HttpPost]
        public ActionResult LoadRoleUsers(RoleInfo role)
        {
            RoleInfo right = new RoleInfo();
            int[] iuser =RoleManager.Instance.GetGrouping(role.RoleId);
            string[] suser = iuser.Select(i => i.ToString()).ToArray();
            string struser = string.Join(",", suser);
            return Json(struser);
        }

        //
        //GET:GetUserList
        public ActionResult GetUserList()
        {
            return Json(UserManager.Instance.GetUserList());
        }

        //
        //GET:SaveRoleUsers
        public ActionResult SaveRoleUsers(RoleInfo role)
        {
            role.Users = new List<int>();
            if (role.Grouping != null && role.Grouping.Contains(','))
            {
                string[] str = role.Grouping.Split(',');
                foreach (string st in str)
                {
                    role.Users.Add(int.Parse(st));
                }
            }
            else if (role.Grouping != null && role.Grouping.Length > 0)
            {
                role.Users.Add(int.Parse(role.Grouping));
            }
            if (RoleManager.Instance.UpdateRoleUsers(role))
                return Json("分配成功");
            else
                return Json("分配失败");
        }


        #endregion
    }
}
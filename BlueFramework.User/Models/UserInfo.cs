using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    /// <summary>
    /// user infomation and user action
    /// </summary>
    public class UserInfo : IUser
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string TrueName { get; set; }

        public bool IsAdmin { get; set; }

        public string CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public int OrgId { get; set; }

        public string OrgName { get; set; }

        /// <summary>
        /// user orgnization
        /// </summary>
        public OrgnizationInfo Orgnization { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 用户角色或分组
        /// 一个用户可以分配一个或多个角色
        /// </summary>
        public List<RoleInfo> Roles { get; set; }


        public List<MenuInfo> MenuList { get; set; }

        /// <summary>
        /// 功能菜单权限
        /// 数据继承（来自）于角色分配权限
        /// </summary>
        public List<int> MenuRights { get; set; }

        /// <summary>
        /// 组织机构或地域权限
        /// 数据继承（来自）于角色分配权限
        /// </summary>
        public List<int> DataRights { get; set; }

    }
}

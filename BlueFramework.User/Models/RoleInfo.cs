using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class RoleInfo
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public int State { get; set; }

        /// <summary>
        /// 角色拥有的菜单权限
        /// </summary>
        public List<int> OwnMenus { get; set; }

        /// <summary>
        /// 角色拥有的数据权限
        /// </summary>
        public List<int> OwnDatas { get; set; }

        /// <summary>
        /// 角色拥有的用户
        /// </summary>
        public List<int> OwnUsers { get; set; }

        /// <summary>
        /// 角色拥有的菜单
        /// </summary>
        public string Menus { get; set; }

        /// <summary>
        /// 所拥有的用户
        /// </summary>
        public List<int> Users { get; set; }

        /// <summary>
        /// 角色拥有的用户
        /// </summary>
        public string Grouping { get; set; }
    }
}

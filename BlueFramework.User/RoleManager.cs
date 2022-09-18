using BlueFramework.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User
{
    /// <summary>
    /// 角色功能提供
    /// 增删改查等... ...
    /// </summary>
    public class RoleManager
    {
        protected BlueFramework.User.DataAccess.SysAccess sysAccess = new User.DataAccess.SysAccess();

        public static RoleManager Instance
        {
            get
            {
                RoleManager role = new RoleManager();
                return role;
            }
        }

        /// <summary>
        /// 获取角色所属的菜单权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public int[] GetMenuRights(int roleId)
        {
            return sysAccess.GetMenuRights(roleId);
        }

        /// <summary>
        /// 获取角色所属的数据权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public int[] GetDataRights(int roleId)
        {
            return sysAccess.GetDataRights(roleId);
        }

        /// <summary>
        /// 获取角色列表
        /// 支持对角色名称模糊查询
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public List<RoleInfo> GetRoleList(RoleInfo role)
        {
            return sysAccess.GetRoles(role);
        }

        /// <summary>
        /// 通过角色ID获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleInfo GetRoleByRoleId(int roleId)
        {
            return sysAccess.GetRoleByRoleId(roleId);
        }

        /// <summary>
        /// 通过角色姓名获取角色信息
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public RoleInfo GetRoleByRoleName(string roleName)
        {
            return sysAccess.GetRoleByRoleName(roleName);
        }

        /// <summary>
        /// 保存菜单权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool SaveMenuRights(int roleId, int[] items)
        {
            return sysAccess.SaveMenuRights(roleId,items);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public int AddRole(RoleInfo role)
        {
            return sysAccess.AddRole(role);
        }

        /// <summary>
        /// 角色添加操作（不包括分组）
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public int AddOnlyRole(RoleInfo role)
        {
            if (role.RoleName == GetRoleByRoleName(role.RoleName).RoleName)
            {
                return -1;
            }
            return sysAccess.AddOnlyRole(role);
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public bool DeleteRole(RoleInfo role)
        {
            return sysAccess.DeleteRole(role);
        }

        /// <summary>
        /// 获取角色对应的用户
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>用户ID对应的int数组</returns>
        public int[] GetGrouping(int roleId)
        {
            return sysAccess.GetGrouping(roleId);
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public int UpdateOnlyRole(RoleInfo role ,string oldName)
        {
            if (!string.IsNullOrEmpty(GetRoleByRoleName(role.RoleName).RoleName))
            {
                if (role.RoleName != oldName)
                {
                    return -1;
                }
            }
            return sysAccess.UpdateOnlyRole(role);
        }

        public bool UpdateRoleUsers(RoleInfo role)
        {
            return sysAccess.UpdateRoleUsers(role);
        }
    }
}

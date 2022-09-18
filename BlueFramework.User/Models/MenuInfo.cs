using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    public class MenuInfo
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类型：1 子系统 2 菜单或子模块 3 页面或PANEL 4 按钮 0 无
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 是否需要授权：1 授权 0未授权
        /// </summary>
        public bool IsLicense { get; set; }

        /// <summary>
        /// 是否用户权限项，是则需要管理员授权
        /// </summary>
        public bool IsUserRight { get; set; }

        /// <summary>
        /// 链接或路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 样式名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuInfo> Menus { get; set; }

        /// <summary>
        /// CSSid
        /// </summary>
        public string CssId { get; set; }

        /// <summary>
        /// 显示状态：true:显示  false：隐藏
        /// </summary>
        public bool MobDisplay { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public MenuInfo(int id, int pid, string name)
        {
            this.MenuId = id;
            this.ParentId = pid;
            this.Name = name;
        }

        public MenuInfo()
        { }
    }
}

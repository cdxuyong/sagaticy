using BlueFramework.User.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlueFramework.User
{
    public class RightManager
    {
        protected BlueFramework.User.DataAccess.SysAccess sysAccess = new User.DataAccess.SysAccess();

        public static RightManager Instance
        {
            get
            {
                RightManager role = new RightManager();
                return role;
            }
        }

        /// <summary>
        /// 获取树形菜单
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<VEasyUiTree> getRightTree(string pid)
        {
            List<VEasyUiTree> list = new List<VEasyUiTree>();
            DataTable dt = ToDataTable<MenuInfo>(GetMenuList());
            DataRow[] drs = dt.Select("ParentId = '" + pid + "'");
            if (drs.Length > 0)
            {
                foreach (DataRow dr in drs)
                {
                    list.Add(getRightTree(dr, dt));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有权限菜单
        /// </summary>
        /// <returns></returns>
        private List<MenuInfo> GetMenuList()
        {
            List<MenuInfo> lists = new List<MenuInfo>();
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "Setting/Menu/MenuConfig.xml");
            XmlNodeList mainnode = doc.SelectNodes("//Menus/Title");
            foreach (XmlNode node in mainnode)
            {
                XmlNodeList subnodes = node.SelectNodes("./Node");
                MenuInfo parentMenu = new MenuInfo(int.Parse(node.Attributes["id"].Value), 0, node.Attributes["caption"].Value);
                lists.Add(parentMenu);
                if (subnodes.Count == 0)
                    break;
                foreach (XmlNode n in subnodes)
                {
                    lists.Add(new MenuInfo(int.Parse(n.Attributes["id"].Value), parentMenu.MenuId, n.Attributes["caption"].Value));
                }
            }
            return lists;
        }

        /// <summary>
        /// 转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        private DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取树形菜单的子菜单
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private VEasyUiTree getRightTree(DataRow dr, DataTable dt)
        {
            VEasyUiTree tree = new VEasyUiTree();
            tree.text = dr["Name"].ToString();
            tree.id = dr["MenuId"].ToString();
            DataRow[] drs = dt.Select("ParentId = '" + dr["MenuId"].ToString() + "'");
            if (drs.Length > 0)
            {
                tree.children = new List<VEasyUiTree>();
                foreach (DataRow mdr in drs)
                {
                    //递归子节点
                    tree.children.Add(getRightTree(mdr, dt));
                }
            }
            return tree;
        }

        /// <summary>
        /// 保存功能权限
        /// </summary>
        /// <returns></returns>
        public bool SaveMenuRights(int roleid, int[] menuItems)
        {
            return sysAccess.SaveMenuRights(roleid, menuItems);
        }
    }
}

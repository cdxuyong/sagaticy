using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueFramework.User.Models;
using System.Xml;

namespace HrServiceCenterWeb.Controllers
{
    /// <summary>
    /// 部分试图控制器
    /// 菜单列表
    /// </summary>
    public class MenuController : BaseController
    {
        //当前登录用户得菜单权限列表
        private List<MenuInfo> menuifo;

        // GET: Menu
        public ActionResult Index()
        {
            ViewBag.User = (UserInfo)BlueFramework.User.UserContext.Current;
            ViewBag.MenuList = menuInfo;
            return PartialView("Menu");//分布试图
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        private List<MenuInfo> menuInfo
        {
            get
            {
                UserInfo onlineUser = (UserInfo)BlueFramework.User.UserContext.Current;
                if (onlineUser.MenuList != null && onlineUser.MenuList.Count > 0)
                    return onlineUser.MenuList;
                else
                {
                    menuifo = new List<MenuInfo>();
                    if (onlineUser.IsAdmin == true)
                        menuifo = getMenusForXml();
                    else
                        menuifo = getMenusForRights(onlineUser.MenuRights, getMenusForXml());
                    onlineUser.MenuList = new List<MenuInfo>();
                    onlineUser.MenuList = menuifo;
                    Visitor visitor = new Visitor(onlineUser);
                    BlueFramework.User.Session.Current.AddVisitor(visitor);
                    return menuifo;
                }
            }
        }

        /// <summary>
        /// 解析配置文件中XML（菜单）
        /// </summary>
        private List<MenuInfo> getMenusForXml()
        {
            menuifo = new List<MenuInfo>();
            XmlDocument doc = new XmlDocument();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "Setting/Menu/MenuConfig.xml";
            doc.Load(path);
            XmlNode menus = doc.SelectSingleNode("Menus");
            XmlNodeList xn = menus.ChildNodes;
            foreach (XmlNode title in xn)
            {
                XmlElement xe = (XmlElement)title;
                MenuInfo mi = new MenuInfo();
                mi.Name = xe.GetAttribute("caption");
                mi.MenuId = int.Parse(xe.GetAttribute("id"));
                mi.Type = bool.Parse(xe.GetAttribute("isroot")) == true ? 1 : 0;
                mi.ClassName = xe.GetAttribute("ic_class");
                mi.CssId = xe.GetAttribute("ic_id");
                mi.Menus = new List<MenuInfo>();
                mi.MobDisplay = bool.Parse(xe.GetAttribute("mob_display"));
                XmlNodeList nodes = title.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;
                    XmlElement nd = (XmlElement)node;
                    MenuInfo mis = new MenuInfo();
                    mis.Url = nd.GetAttribute("url");
                    mis.MenuId = int.Parse(nd.GetAttribute("id"));
                    mis.Name = nd.GetAttribute("caption");
                    mis.MobDisplay = bool.Parse(nd.GetAttribute("mob_display"));
                    mi.Menus.Add(mis);
                }
                menuifo.Add(mi);
            }
            return menuifo;
        }

        /// <summary>
        /// 根据用户权限过滤菜单列表
        /// </summary>
        /// <param name="menuRights"></param>
        /// <param name="menuInfo"></param>
        /// <returns></returns>
        private List<MenuInfo> getMenusForRights(List<int> menuRights, List<MenuInfo> menuInfo)
        {
            //过滤一级目录
            for (int i = menuInfo.Count - 1; i >= 0; i--)
            {
                bool firstRelt = false;
                for (int n = 0; n < menuRights.Count; n++)
                {
                    if (menuInfo[i].MenuId.ToString() == menuRights[n].ToString())
                    {
                        firstRelt = true;
                        break;
                    }
                }
                if (!firstRelt)
                    menuInfo.RemoveAt(i);
            }
            //过滤二级目录
            for (int i = 0; i < menuInfo.Count; i++)
            {
                for (int n = menuInfo[i].Menus.Count - 1; n >= 0; n--)
                {
                    bool secondRelt = false;
                    for (int m = 0; m < menuRights.Count; m++)
                    {
                        if (menuInfo[i].Menus[n].MenuId.ToString() == menuRights[m].ToString())
                        {
                            secondRelt = true;
                            break;
                        }
                    }
                    if (!secondRelt)
                        menuInfo[i].Menus.RemoveAt(n);
                }
            }
            return menuInfo;
        }
    }
}
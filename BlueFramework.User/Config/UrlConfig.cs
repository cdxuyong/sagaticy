using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlueFramework.User.Config
{
    public class UrlConfig
    {
        /// <summary>
        /// 访问地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// 是否为路径
        /// </summary>
        public bool IsPath { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        private Regex RegexExpress;

        /// <summary>
        /// URL模块定义
        /// </summary>
        /// <param name="url"></param>
        /// <param name="moduleID"></param>
        /// <param name="isPath"></param>
        public UrlConfig(string url, int moduleID, bool isPath)
        {
            this.Url = url;
            this.ModuleID = moduleID;
            this.IsPath = isPath;
            if (!isPath)
            {
                RegexExpress = new Regex(url, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// 测试方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool Test(string url)
        {
            if (this.RegexExpress.IsMatch(url))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

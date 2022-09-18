using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    /// <summary>
    /// 日志对象
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// 地市ID
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 请求类型（默认=1）
        /// </summary>
        public int RequestType { get; set; }

        /// <summary>
        /// 是否匿名访客，否则就是系统用户
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// 备注消息
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public double CostMillisecond { get; set; }

    }
}

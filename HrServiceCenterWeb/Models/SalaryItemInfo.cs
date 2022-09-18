using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 薪酬项目列表
    /// </summary>
    public class SalaryItemInfo
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
    }
}
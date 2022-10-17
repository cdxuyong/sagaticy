using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 月度考核概要信息
    /// </summary>
    public class CheckMonthSumVO
    {
        /// <summary>
        /// 考核月份
        /// </summary>
        public DateTime Month { get; set; }

        public string CheckMonth { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string CmpName { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int CheckDays { get; set; }

        public int LateDays { get; set; }

        public int LostDays { get; set; }

    }
}
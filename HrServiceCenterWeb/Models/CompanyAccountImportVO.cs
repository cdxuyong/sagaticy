using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 单位结算账号VO
    /// </summary>
    public class CompanyAccountImportVO
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public string ImportName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string CmpName { get; set; }

        public DateTime PayDate { get; set; }

        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 结算总金额
        /// </summary>
        public decimal Total { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 发放表详情参数
    /// </summary>
    public class PayValueInfo
    {
        public int PayId { get; set; }

        public int ItemId { get; set; }

        public int PersonId { get; set; }

        public decimal PayValue { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class PayObjectDO
    {

        public int PaymentId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string ObjectCode { get; set; }
        public int ServiceFee { get; set; } = 100;
        public string Memo { get; set; } = "";
        // 20230311 add
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }
    }
}
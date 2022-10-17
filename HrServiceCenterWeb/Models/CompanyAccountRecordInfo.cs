using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CompanyAccountRecordInfo
    {
        public int CompanyId { get; set; }

        public string CmpName { get; set; }

        public int AccountId { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal AccountBalance { get; set; }
        /// <summary>
        /// 结算项目【20221015】
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 结算月份【20221015】
        /// </summary>
        public DateTime PayDate { get; set; }
        /// <summary>
        /// 开票日期【20221015】
        /// </summary>
        public DateTime BillDate { get; set; }
        /// <summary>
        /// 到账日期【20221015】
        /// </summary>
        public DateTime EntryDate { get; set; }
        public DateTime OptDate { get; set; }

        public DateTime CreateTime { get; set; }

        public int Creator { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 标题或文件名【20221015】
        /// </summary>
        public string ImportName { get; set; }
        public string Remark { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueFramework.Common;
using Newtonsoft.Json;

namespace HrServiceCenterWeb.Models
{
    public class CompanyAccountRecordInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
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
        [JsonConverter(typeof(MonthFormat))]
        public DateTime PayDate { get; set; }
        /// <summary>
        /// 开票日期【20221015】
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime BillDate { get; set; }
        /// <summary>
        /// 到账日期【20221015】
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime EntryDate { get; set; }

        [JsonConverter(typeof(DateFormat))]
        public DateTime OptDate { get; set; }

        [JsonConverter(typeof(DateFullFormat))]
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
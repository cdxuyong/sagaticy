using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class EmployeeInfo
    {
        public int PersonId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public string PersonName { get; set; }
        public string CardId { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string JoinWorkTime { get; set; }
        public int Degree { get; set; }
        public string Polity { get; set; }
        public string Nation { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactsPerson { get; set; }
        public string ContactsPhone { get; set; }
        public string Specialty { get; set; }
        public string School { get; set; }
        public int Creator { get; set; }
        public string CreateTime { get; set; }
        public int State { get; set; }

        public string StateCaption { get; set; }

        public string LeaveTime { get; set; }

        public int PersonCode { get; set; }
        public string BankName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCard { get; set; }

        public string ContractTime { get; set; }
        public string RetireTime { get; set; }
        /// <summary>
        /// 合同到期天数
        /// </summary>
        public int ContractDays { get; set; }
        public int RetireDays { get; set; }

        public int ServiceFee { get; set; }

        public string ContractFirstTime { get; set; }
        public string ContractNO { get; set; }
        public string AgreementNO { get; set; }
        /// <summary>
        /// 用途类型：0默认全部统计 ， 1 限制统计
        /// </summary>
        public int UseType { get; set; }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 受雇类型
        /// </summary>
        public string EmployType { get; set; }
        /// <summary>
        /// 受雇日期
        /// </summary>
        public string EmployDate { get; set; }

    }
}
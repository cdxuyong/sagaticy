using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 保险详情实体
    /// </summary>
    public class InsuranceDetailInfo:InsuranceInfo
    {
       public int PersonId { get; set; }

        public string PersonName { get; set; }

        public string CardId { get; set; }

        public string PayMonth { get; set; }

        public string ItemName { get; set; }

        public int ItemId { get; set; }

        /// <summary>
        /// 个人缴存
        /// </summary>
        public decimal PersonPayValue { get; set; }

        /// <summary>
        /// 单位缴存
        /// </summary>
        public decimal CompanyPayValue { get; set; }

        public string ImportColumnName { get; set; }

        public InsuranceDetailInfo Clone()
        {
            InsuranceDetailInfo o = new InsuranceDetailInfo();
            o.ImportId = this.ImportId;
            o.PayMonth = this.PayMonth;
            o.PersonId = this.PersonId;
            return o;
        }
    }
}
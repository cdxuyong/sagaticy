using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CompanyAccountRecordInfo
    {
        public int CompanyId { get; set; }
        public int AccountId { get; set; }

        public decimal AccountBalance { get; set; }

        public DateTime CreateTime { get; set; }

        public int Creator { get; set; }

        public decimal Money { get; set; }
    }
}
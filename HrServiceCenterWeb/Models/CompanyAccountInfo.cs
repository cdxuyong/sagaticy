using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CompanyAccountInfo
    {
        public int AccountId { get; set; }

        public int CompanyId { get; set; }

        public decimal AccountBalance { get; set; }
    }
}
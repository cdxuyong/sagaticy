using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CompanyPositionSetInfo:PositionInfo
    {
        public int CompanyId { get; set; }
        public int PlanCount { get; set; }

        public string UseType { get; set; }

        public string Remark { get; set; }

        public int RealCount { get; set; }
        public int LeaveCount { get; set; }
    }
}
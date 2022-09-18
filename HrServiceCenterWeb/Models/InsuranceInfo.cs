using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 保险模板实体
    /// </summary>
    public class InsuranceInfo
    {
        public int ImportId { get; set; }

        public int ImportType { get; set; }

        public string Title { get; set; }

        public int CreatorId { get; set; }

        public string CreatorName{ get; set; }

        public string CreateTime { get; set; }
    }
}
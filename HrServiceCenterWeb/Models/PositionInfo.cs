using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 岗位信息
    /// </summary>
    public class PositionInfo
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public string PositionDesc { get; set; }


    }
}
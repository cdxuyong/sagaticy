using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueFramework.Common;
using Newtonsoft.Json;

namespace HrServiceCenterWeb.Models
{
    public class CheckMonthInfo:CheckInfo
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string CardId { get; set; }
        public string CmpName { get; set; }
        [JsonConverter(typeof(MonthFormat))]
        public DateTime CheckMonth { get; set; }
        public int CheckDays { get; set; }

        public int LateDays { get; set; }

        public int LostDays { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueFramework.Common;
using Newtonsoft.Json;

namespace HrServiceCenterWeb.Models
{
    public class CheckDayInfo:CheckInfo
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string CardId { get; set; }
        public string CmpName { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime CheckDate { get; set; }
        public string CheckAdress { get; set; }
        [JsonConverter(typeof(DateFullFormat))]
        public DateTime StartTime { get; set; }
        [JsonConverter(typeof(DateFullFormat))]
        public DateTime EndTime { get; set; }
        public decimal WorkHours { get;set; }
        public string Demo { get; set; }
    }
}
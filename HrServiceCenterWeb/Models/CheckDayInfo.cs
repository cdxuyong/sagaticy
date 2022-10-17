using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CheckDayInfo:CheckInfo
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string CardId { get; set; }
        public string CmpName { get; set; }
        public DateTime CheckDate { get; set; }
        public string CheckAdress { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal WorkHours { get;set; }
        public string Demo { get; set; }
    }
}
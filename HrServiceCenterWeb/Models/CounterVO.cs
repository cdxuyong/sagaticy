using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class CounterVO
    {
        public string[] DataAxis { get; set; }

        public decimal[] Data { get; set; }

        public string Title { get; set; }
    }
}
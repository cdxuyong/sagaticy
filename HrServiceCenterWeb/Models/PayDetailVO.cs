using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace HrServiceCenterWeb.Models
{
    public class PayDetailVO
    {
        public int total { get; set; }

        public DataTable rows { get; set; }

        public List<JObject> footer { get; set; }
    }
}
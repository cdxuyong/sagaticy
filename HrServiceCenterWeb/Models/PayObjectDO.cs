using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class PayObjectDO
    {

        public int PaymentId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string ObjectCode { get; set; }
        public int ServiceFee { get; set; } = 100;
    }
}
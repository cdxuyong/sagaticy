using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class PayItemDO
    {

        public int PaymentId { get; set; }
        public int ItemId { get; set; }
        public int ParentId { get; set; }
        public string ItemCaption { get; set; }
        public string ItemName { get; set; }
        public int Editable { get; set; }
        public bool IsLeaf { get; set; }
    }
}
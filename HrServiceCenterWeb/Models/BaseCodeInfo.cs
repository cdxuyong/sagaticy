using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class BaseCodeInfo
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public string ObjectValue { get; set; }

        public int CategoryId { get; set; }

        private bool isLeaf;
        public bool IsLeaf
        {
            get
            {
                return isLeaf;
            }
            set
            {
                isLeaf = value;
            }
        }
    }
}
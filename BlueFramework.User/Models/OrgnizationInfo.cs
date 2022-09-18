using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
     public class OrgnizationInfo
    {
        public int OrgId { get; set; }

        public int ParentId { get; set; }

        public string OrgName { get; set; }

        public string OrgCode { get; set; }
    }
}

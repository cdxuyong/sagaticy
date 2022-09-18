using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class TemplateInfo
    {
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string CompanyName { get; set; }

        public int CompanyId { get; set; }

        public int Creator { get; set; }

        public string CreatTime { get; set; }
        
        public string Representative { get; set; }

        public string UpdateTime { get; set; }

        public TemplateInfo(int companyId,string templateName,int creator,string creatTime)
        {
            this.CompanyId = companyId;
            this.TemplateName = templateName;
            this.Creator = creator;
            this.CreatTime = creatTime;
        }

        public TemplateInfo()
        { }
    }
}
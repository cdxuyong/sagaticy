using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    public class PayTableConfig
    {
        /// <summary>
        /// 字段名（对应数据库字段名）
        /// </summary>
        public string FieldCode { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        public string FieldTitle { get; set; }

        public int ItemId { get; set; }

        /// <summary>
        /// 是否末级
        /// </summary>
        public bool IsLastStage { get; set; }

        /// <summary>
        /// 父级表头
        /// </summary>
        public string ParentTitle { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEdit { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 是否需要动态显示
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<PayTableConfig> Subnode { get; set; }

        public PayTableConfig()
        {
            Subnode = new List<PayTableConfig>();
        }
    }
}
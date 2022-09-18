using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    public class VEasyUiTree
    {
        public string id { get; set; }
        public string text { get; set; }
        /// <summary>
        /// 'open' 或 'closed'，默认是 'open'。
        /// 如果为'closed'的时候，将不自动展开该节点。
        /// </summary>
        public string state { get; set; }
        public bool ischecked { get; set; }// replace checked
        public object sttributes { get; set; }
        public List<VEasyUiTree> children { get; set; }
    }
}

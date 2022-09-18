using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.Config
{
    /// <summary>
    /// query entity config
    /// </summary>
    public class SelectConfig : EntityConfig
    {
        public SelectConfig()
        {
            this.ConfigType = ConfigType.Select;
        }
        /// <summary>
        /// the type of output parameter
        /// </summary>
        public string OutputParameterType { get; set; }
    }
}

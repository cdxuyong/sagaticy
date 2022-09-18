using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.Config
{
    public enum IdentityMadeOrder
    {
        Undefine = 0,
        Befor = 1,
        Inserting = 2,
        After = 3
    }

    public class InsertConfig:EntityConfig
    {
        public InsertConfig()
        {
            this.ConfigType = ConfigType.Insert;

        }
        public string KeyProperty { get; set; }

        public string KeyPropertySql { get; set; }

        public IdentityMadeOrder KeyMadeOrder { get; set; }
    }
}

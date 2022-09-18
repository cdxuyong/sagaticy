using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueFramework.Blood.Config;

namespace BlueFramework.Blood
{
    public class SqlCommand
    {
        public SqlCommand()
        {
        }

        public object Select(string selectId, object objectId)
        {
            EntityConfig config = ConfigManagent.Configs[selectId];
            if (config == null)
            {
                return null;
            }

            return null;
        }
    }
}

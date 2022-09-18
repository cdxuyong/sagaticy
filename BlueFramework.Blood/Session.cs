using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueFramework.Blood.DataAccess;
using BlueFramework.Blood.Config;

namespace BlueFramework.Blood
{
    /// <summary>
    /// the session of database's operation
    /// </summary>
    public class Session
    {
        private static bool loaded = false;

        /// <summary>
        /// initialize configs
        /// </summary>
        public static void Init()
        {
            if (!loaded)
            {
                LoadConfigs();
                loaded = true;
            }
        }

        /// <summary>
        /// load mapping configs
        /// </summary>
        static void LoadConfigs()
        {
            Config.ConfigManagent.Init();
        }

        /// <summary>
        /// create db context
        /// </summary>
        /// <returns></returns>
        public static EntityContext CreateContext()
        {
            EntityContext context = new EntityContext();
            return context;
        }

        
    }
}

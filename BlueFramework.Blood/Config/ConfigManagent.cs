using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BlueFramework.Blood.Config
{
    /// <summary>
    /// privide method of entity config's querying
    /// </summary>
    public class ConfigManagent
    {
        public static NameConfigCollection Configs = new NameConfigCollection();

        /// <summary>
        /// load xml config and init cache
        /// </summary>
        public static void Init()
        {
            string path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/sqlMappers");
            Init(path);

        }

        /// <summary>
        /// load configs and init cache
        /// </summary>
        /// <param name="sqlMapperPath"></param>
        public static void Init(string sqlMapperPath)
        {
            ConfigFile configFile = new ConfigFile(sqlMapperPath);
            List<EntityConfig> configs = configFile.LoadConfigs();
            foreach (EntityConfig config in configs)
            {
                Configs.Add(config);
            }
        }

        

        
    }
}

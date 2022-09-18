using System;
    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueFramework.Blood.Config
{
    /// <summary>
    /// config dictionary by name
    /// </summary>
    public class NameConfigCollection
    {
        private Dictionary<string, EntityConfig> configs;

        /// <summary>
        /// configs collection
        /// get config by name
        /// </summary>
        public NameConfigCollection()
        {
            configs = new Dictionary<string, EntityConfig>();
        }

        /// <summary>
        /// add config
        /// </summary>
        /// <param name="entityConfig"></param>
        public void Add(EntityConfig entityConfig)
        {
            if(!configs.Keys.Contains(entityConfig.Id))
            {
                configs.Add(entityConfig.Id, entityConfig);
            }
        }

        /// <summary>
        /// get config by name key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EntityConfig this[string name]
        {
            get
            {
                EntityConfig ec;
                bool find = configs.TryGetValue(name, out ec);
                if (find)
                    return ec;
                else
                    return null;
            }

        }

        /// <summary>
        /// names count
        /// </summary>
        public int Count
        {
            get
            {
                return configs.Count;
            }
        }
    }
}

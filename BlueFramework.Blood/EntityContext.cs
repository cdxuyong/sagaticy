using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueFramework.Blood.Config;
using BlueFramework.Blood.DataAccess;

namespace BlueFramework.Blood
{
    /// <summary>
    /// provider the context of db commands
    /// </summary>
    public class EntityContext:IDisposable
    {
        Command command = new Command();

        /// <summary>
        /// create db context , if want save transaction , must begin transaction
        /// </summary>
        public EntityContext()
        {
        }

        public bool Save<T>(string commandId,T o)
        {
            EntityConfig config = ConfigManagent.Configs[commandId];
            bool pass = false;
            switch (config.ConfigType)
            {
                case ConfigType.Insert:
                    T result = command.Insert<T>((InsertConfig)config, o);
                    pass = result == null ? false : true;
                    break;
                case ConfigType.Update:
                    pass = command.Update<T>((UpdateConfig)config, o);
                    break;
            }
            return pass;
        }

        public bool Delete(string commandId,object objectId)
        {
            EntityConfig config = ConfigManagent.Configs[commandId];
            bool pass = command.Delete((DeleteConfig)config, objectId);
            return pass;
        }

        public bool Delete<T>(string commandId, object deleteObject)
        {
            EntityConfig config = ConfigManagent.Configs[commandId];
            bool pass = command.Delete<T>((DeleteConfig)config, deleteObject);
            return pass;
        }

        public T Selete<T>(string selectId,object objectId)
        {
            EntityConfig config = ConfigManagent.Configs[selectId];
            T o = command.Select<T>(config, objectId);
            return o;
        }

        public List<T> SelectList<T>(string selectId,object selectValue)
        {
            EntityConfig config = ConfigManagent.Configs[selectId];
            CommandParameter[] parameters = null;
            if (selectValue != null)
            {
                parameters = new CommandParameter[] { new CommandParameter("value", selectValue, selectValue.GetType()) };
            }
            List<T> ts = command.SelectList<T>(config, parameters);
            return ts;
        }

        public List<T> SelectList<T>(string selectId,params CommandParameter[] parameters)
        {
            EntityConfig config = ConfigManagent.Configs[selectId];
            List<T> ts = command.SelectList<T>(config, parameters);
            return ts;
        }

        public List<T> SelectListByTemplate<T>(string selectId, T objectValue)
        {
            EntityConfig config = ConfigManagent.Configs[selectId];
            List<T> ts = command.SelectList<T>(config, objectValue);
            return ts;
        }

        public void BeginTransaction()
        {
            command.BeginTransaction();
        }

        public void Commit()
        {
            command.CommitTransaction();
        }

        public void Rollback()
        {
            command.RollbackTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                command.Dispose();
                GC.SuppressFinalize(this);
            }
        }

    }
}

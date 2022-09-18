using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using BlueFramework.Blood.Config;
using BlueFramework.Data;
using BlueFramework.Common.Logger;
using BlueFramework.Blood.EntityFramework;

namespace BlueFramework.Blood.DataAccess
{
    public class Command:IDisposable
    {
        DatabaseProviderFactory factory;
        Database db;
        DbTransaction dbTransaction = null;
        DbConnection dbConnection = null;
        public Command()
        {
            factory = new DatabaseProviderFactory();
            db = factory.CreateDefault();
        }

        public T LoadEntity<T>(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return default(T);
            T o = System.Activator.CreateInstance<T>();
            Type type = o.GetType();
            DataRow row = dt.Rows[0];
            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length == 0)
            {
                return (T)row[0];
            }
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                string propertyName = property.Name;
                if (dt.Columns.Contains(propertyName))
                {
                    bool isNull = row[propertyName] is System.DBNull;
                    if (!isNull)
                    {
                        property.SetValue(o, row[propertyName]);
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// load entity list from datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> LoadEntities<T>(DataTable dt)
        {
            if (dt == null) return null;
            if (dt.Rows.Count == 0) return new List<T>();
            Type type = typeof(T);
            List<PropertyInfo> properties = type.GetProperties().ToList();
            // remove property where it is not exist
            for(int i = 0; i < properties.Count; i++)
            {
                bool isExist = dt.Columns.IndexOf(properties[i].Name) !=-1 ? true : false;
                if (!isExist)
                {
                    properties.RemoveAt(i);
                    i--;
                }
            }
            // freach properties
            List<T> objects = new List<T>();
            List<PropertyBehavior> behaviors = null;
            if (dt.Rows.Count > 0)
            {
                behaviors = BehaviorUtils.GetBehaviors(dt.Rows[0], properties);
            }
            foreach (DataRow dr in dt.Rows)
            {
                T o = System.Activator.CreateInstance<T>();
                foreach(PropertyBehavior behavior in behaviors)
                {
                    bool isNull = dr[behavior.Property.Name] is System.DBNull;
                    if (!isNull)
                    {
                        switch (behavior.Behavior)
                        {
                            case BehaviorType.IntToBoolean:
                                if((int)dr[behavior.Property.Name]==0)
                                    behavior.Property.SetValue(o,false );
                                else
                                    behavior.Property.SetValue(o, true);
                                break;
                            case BehaviorType.StringToDate:
                                DateTime dateTime = DateTime.Parse(dr[behavior.Property.Name].ToString());
                                behavior.Property.SetValue(o, dateTime);
                                break;
                            case BehaviorType.StringToDecimal:
                                behavior.Property.SetValue(o, decimal.Parse( dr[behavior.Property.Name].ToString()));
                                break;
                            default:
                                behavior.Property.SetValue(o, dr[behavior.Property.Name]);
                                break;
                        }
                    }

                }
                objects.Add(o);
            }
            return objects;
        }

        /// <summary>
        /// get config's object by  object value
        /// </summary>
        /// <typeparam name="T">entity template</typeparam>
        /// <param name="config">config</param>
        /// <param name="objectValue">query value or entity id</param>
        /// <returns></returns>
        public T Select<T>(EntityConfig config, object objectValue)
        {
            DbCommand dbCommand = BuildCommand(config, objectValue);
            try
            {
                T o = default(T);
                using (DataSet ds = db.ExecuteDataSet(dbCommand))
                {
                    DataTable dt = ds.Tables[0];
                    o = LoadEntity<T>(dt);
                }
                return o;
            }
            catch(Exception ex)
            {
                LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.Select<T>" ,ex);
                return default(T);
            }
        }

        /// <summary>
        /// Get config's objects by input parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> SelectList<T>(EntityConfig config, CommandParameter[] parameters)
        {
            DbCommand dbCommand = BuildCommand(config, parameters);
            try
            {
                List<T> list = new List<T>();
                using (DataSet ds = db.ExecuteDataSet(dbCommand))
                {
                    DataTable dt = ds.Tables[0];
                    list = LoadEntities<T>(dt);
                }
                return list;
            }
            catch(Exception ex)
            {
                LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.SelectList<T> :" , ex);
                return null;
            }
        }

        public List<T> SelectList<T>(EntityConfig config,T objectValue)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            CommandParameter[] parameters = BuildCommandParameters(objectValue, properties);
            List<T> list = SelectList<T>(config, parameters);
            return list;
        }

        private CommandParameter[] BuildCommandParameters(object o, PropertyInfo[] properties)
        {
            CommandParameter[] parameters = new CommandParameter[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                CommandParameter parameter = null;
                if ( property.PropertyType.Namespace != "System" )
                    parameter = new CommandParameter(property.Name,null, property.PropertyType);
                else
                    parameter = new CommandParameter(property.Name, property.GetValue(o), property.PropertyType);
                parameters[i] = parameter;
            }
            return parameters;
        }

        public T Insert<T>(InsertConfig config,T insertObject)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            CommandParameter[] parameters = BuildCommandParameters(insertObject, properties);
            DbCommand dbCommand = BuildInsertCommand(config, parameters);
            int insertCount = 0;
            if (dbTransaction == null)
            {
                try
                {

                    insertCount = db.ExecuteNonQuery(dbCommand);
                }
                catch(Exception ex)
                {
                    LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.Insert<T> :" , ex);
                    insertCount = 0;
                }
            }
            else
            {
                insertCount = db.ExecuteNonQuery(dbCommand, dbTransaction);
            }
            if (insertCount>0 && !string.IsNullOrEmpty(config.KeyProperty))
            {
                PropertyInfo propertyInfo = properties.FirstOrDefault(o => o.Name == config.KeyProperty);
                propertyInfo.SetValue(insertObject, dbCommand.Parameters[0].Value);
            }
            return insertCount > 0 ? insertObject : default(T);
        }

        public bool Update<T>(UpdateConfig config,T updateObject)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            CommandParameter[] parameters = BuildCommandParameters(updateObject, properties);
            DbCommand dbCommand = BuildCommand(config, parameters);
            bool pass = true;
            if (dbTransaction == null)
            {
                try
                {

                    db.ExecuteNonQuery(dbCommand);
                }
                catch(Exception ex)
                {
                    LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.Update<T> :" , ex);
                    pass = false;
                }
            }
            else
            {
                db.ExecuteNonQuery(dbCommand, dbTransaction);
            }
            return pass;
        }

        public bool Delete(DeleteConfig config,object objectId)
        {
            DbCommand dbCommand = BuildCommand(config, objectId);
            bool pass = true;
            if (dbTransaction == null)
            {
                try
                {
                    db.ExecuteNonQuery(dbCommand);
                }
                catch(Exception ex)
                {
                    LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.Delete :" , ex);
                    pass = false;
                }
            }
            else
            {
                db.ExecuteNonQuery(dbCommand,dbTransaction);
            }
            return pass;
        }

        public bool Delete<T>(DeleteConfig config, object deleteObject)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            CommandParameter[] parameters = BuildCommandParameters(deleteObject, properties);
            DbCommand dbCommand = BuildCommand(config, parameters);
            bool pass = true;
            if (dbTransaction == null)
            {
                try
                {

                    db.ExecuteNonQuery(dbCommand);
                }
                catch (Exception ex)
                {
                    LogHelper.Warn("BlueFramework.Blood.DataAccess.Command.Delete<T> :", ex);
                    pass = false;
                }
            }
            else
            {
                db.ExecuteNonQuery(dbCommand, dbTransaction);
            }
            return pass;
        }



        public void BeginTransaction()
        {
            dbConnection = db.CreateConnection();
            dbConnection.Open();
            dbTransaction = dbConnection.BeginTransaction();
           
        }

        public void RollbackTransaction()
        {
            dbTransaction.Rollback();
            
        }

        public void CommitTransaction()
        {
            dbTransaction.Commit();
        }

        #region build command 
        private string FormatSql(EntityConfig config,object objectId)
        {
            CommandParameter[] parameters = { new CommandParameter() { ParameterName="value",ParameterValue=objectId } };
            return FormatSql(config, parameters);
        }

        private string FormatSql(EntityConfig config, CommandParameter[] parameters)
        {
            string sql = config.Sql;
            if(parameters!=null)
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandParameter parameter = parameters[i];
                string parameterName = db.BuildParameterName(parameter.ParameterName);
                sql = sql.Replace("#{"+parameter.ParameterName+"}", parameterName);
                string parameterValue = string.Empty;
                if (parameter.ParameterValue != null)
                {
                    parameterValue = parameter.ParameterValue.ToString();
                }
                sql = sql.Replace("${"+parameter.ParameterName+"}", parameterValue);
            }
            LogHelper.Debugger("BlueFramework.Blood.DataAccess.Command.FormatSql", sql);
            return sql;
        }

        private DbCommand BuildCommand(EntityConfig config, object objectId)
        {
            string sql = FormatSql(config,objectId);
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            DbType dbType = GetDbType(objectId.GetType());
            db.AddInParameter(dbCommand, "value", dbType, objectId);
            return dbCommand;
        }

        private DbCommand BuildCommand(EntityConfig config, params CommandParameter[] parameters)
        {
            string sql = FormatSql(config, parameters);
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            if(parameters!=null)
            foreach (CommandParameter parameter in parameters)
            {
                db.AddInParameter(
                    dbCommand,
                    parameter.ParameterName, 
                    GetDbType(parameter.ParameterType),
                    parameter.ParameterValue);
            }
            return dbCommand;
        }

        private DbCommand BuildInsertCommand(InsertConfig config, params CommandParameter[] parameters)
        {
            string sql = FormatSql(config, parameters);
            if(config.KeyMadeOrder== IdentityMadeOrder.Inserting)
            {
                sql = sql + " " + config.KeyPropertySql;
            }
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            if (config.KeyMadeOrder == IdentityMadeOrder.Inserting)
            {
                foreach (CommandParameter parameter in parameters)
                {
                    if (parameter.ParameterName == config.KeyProperty)
                    {
                        db.AddOutParameter(
                            dbCommand,
                            parameter.ParameterName,
                            GetDbType(parameter.ParameterType),
                            32
                        );
                        break;
                    }
                }

            }
            foreach (CommandParameter parameter in parameters)
            {
                if (parameter.ParameterName != config.KeyProperty)
                {
                    db.AddInParameter(
                        dbCommand,
                        parameter.ParameterName,
                        GetDbType(parameter.ParameterType),
                        parameter.ParameterValue
                    );
                }
            }
            return dbCommand;
        }

        private DbType GetDbType(Type type)
        {
            DbType dbType = DbType.String;
            switch (type.ToString())
            {
                case  "System.Int32":
                    dbType = DbType.Int32;
                    break;
                case "System.Double":
                    dbType = DbType.Double;
                    break;
                case "System.DateTime":
                    dbType = DbType.DateTime;
                    break;
            }
            return dbType;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbConnection != null)
                {
                    dbConnection.Dispose();
                    dbConnection = null;
                }
                if (dbTransaction != null)
                {
                    dbTransaction.Dispose();
                    dbTransaction = null;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}

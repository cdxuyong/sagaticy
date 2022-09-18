//===============================================================================
// BlueFramework DataProvider
// author:YoungTsui
//===============================================================================
//
//
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.Common;
using BlueFramework.Data.Properties;

namespace BlueFramework.Data
{
    /// <summary>
    /// <para>the base data provider,create by <see cref="DatabaseProviderFactory"/></para>
    /// </summary>
    public class Database
    {
        static readonly string VALID_PASSWORD_TOKENS = Resources.Password;
        static readonly string VALID_USER_ID_TOKENS = Resources.UserName;
        public const int CONNECTION_TIME_OUT = 600;

        readonly string connectionString;
        readonly DbProviderFactory dbProviderFactory;

        /// <summary>
        /// <para>Gets the string used to open a database.</para>
        /// </summary>
        /// <value>
        /// <para>The string used to open a database.</para>
        /// </value>
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        /// <summary>
        /// <para>Gets the string used to open a database.</para>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbProviderFactory"></param>
        protected Database(string connectionString, DbProviderFactory dbProviderFactory)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "connectionString");
            if (dbProviderFactory == null) throw new ArgumentNullException("dbProviderFactory");

            this.connectionString = connectionString;
            this.dbProviderFactory = dbProviderFactory;
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType,
                                   object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType,
                                   string sourceColumn,
                                   DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public void AddOutParameter(DbCommand command,
                                    string name,
                                    DbType dbType,
                                    int size)
        {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        public virtual void AddParameter(DbCommand command,
                                         string name,
                                         DbType dbType,
                                         int size,
                                         ParameterDirection direction,
                                         bool nullable,
                                         byte precision,
                                         byte scale,
                                         string sourceColumn,
                                         DataRowVersion sourceVersion,
                                         object value)
        {
            if (command == null) throw new ArgumentNullException("command");

            DbParameter parameter = CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        public void AddParameter(DbCommand command,
                                 string name,
                                 DbType dbType,
                                 ParameterDirection direction,
                                 string sourceColumn,
                                 DataRowVersion sourceVersion,
                                 object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>  
        /// <returns>A newly created <see cref="DbParameter"/> fully initialized with given parameters.</returns>
        protected DbParameter CreateParameter(string name,
                                              DbType dbType,
                                              int size,
                                              ParameterDirection direction,
                                              bool nullable,
                                              byte precision,
                                              byte scale,
                                              string sourceColumn,
                                              DataRowVersion sourceVersion,
                                              object value)
        {
            DbParameter param = CreateParameter(name);
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }

        protected DbParameter CreateParameter(string name)
        {
            DbParameter param = dbProviderFactory.CreateParameter();
            param.ParameterName = BuildParameterName(name);
            return param;
        }

        public virtual string BuildParameterName(string name)
        {
            return name;
        }

        protected virtual void ConfigureParameter(DbParameter param,
                                          string name,
                                          DbType dbType,
                                          int size,
                                          ParameterDirection direction,
                                          bool nullable,
                                          byte precision,
                                          byte scale,
                                          string sourceColumn,
                                          DataRowVersion sourceVersion,
                                          object value)
        {
            param.DbType = dbType;
            param.Size = size;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }

        DbCommand CreateCommandByCommandType(CommandType commandType,
                                     string commandText)
        {
            DbCommand command = dbProviderFactory.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            return command;
        }

        /// <summary>
        /// <para>创建当前数据库的连接</para>
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        public virtual DbConnection CreateConnection()
        {
            DbConnection newConnection = dbProviderFactory.CreateConnection();
            newConnection.ConnectionString = connectionString;

            return newConnection;
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType,
                                      string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">current database command</param>
        /// <returns>return a dataset</returns>
        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>  
        public virtual DataSet ExecuteDataSet(DbCommand command,
                                      DbTransaction transaction)
        {
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }



        /// <summary>
        /// <para>Executes the <paramref name="command" /> and fill the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <param name="tableName"></param>
        public virtual void LoadDataSet(DbCommand command,
                                DataSet dataSet,
                                string tableName)
        {
            LoadDataSet(command, dataSet, new[] { tableName });
        }

        /// <summary>
        /// <para>Executes the <paramref name="command" /> and fill the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <param name="tableNames"></param>
        public virtual void LoadDataSet(DbCommand command,
                                DataSet dataSet,
                                string[] tableNames)
        {
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        public virtual void LoadDataSet(DbCommand command,
                                DataSet dataSet,
                                string tableName,
                                DbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new[] { tableName }, transaction);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        public virtual void LoadDataSet(DbCommand command,
                                DataSet dataSet,
                                string[] tableNames,
                                DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        void DoLoadDataSet(IDbCommand command,
                   DataSet dataSet,
                   string[] tableNames)
        {
            if (tableNames == null) throw new ArgumentNullException("tableNames");
            if (tableNames.Length == 0)
            {
                throw new ArgumentException(Resources.ExceptionTableNameArrayEmpty, "tableNames");
            }
            for (int i = 0; i < tableNames.Length; i++)
            {
                if (string.IsNullOrEmpty(tableNames[i])) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, string.Concat("tableNames[", i, "]"));
            }

            using (DbDataAdapter adapter = GetDataAdapter(UpdateBehavior.Standard))
            {
                ((IDbDataAdapter)adapter).SelectCommand = command;

                string systemCreatedTableNameRoot = "Table";
                for (int i = 0; i < tableNames.Length; i++)
                {
                    string systemCreatedTableName = (i == 0)
                                                        ? systemCreatedTableNameRoot
                                                        : systemCreatedTableNameRoot + i;

                    adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                }

                adapter.Fill(dataSet);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbCommand command,
                                           DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(CommandType commandType,
                                           string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction,
                                           CommandType commandType,
                                           string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }


        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(string storedProcedureName,
                                           params object[] parameterValues)
        {
            using (DbCommand command = GetStoredProcCommand(storedProcedureName, parameterValues))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction,
                                           string storedProcedureName,
                                           params object[] parameterValues)
        {
            using (DbCommand command = GetStoredProcCommand(storedProcedureName, parameterValues))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteScalar(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbCommand command,
                                            DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(CommandType commandType,
                                            string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbTransaction transaction,
                                            CommandType commandType,
                                            string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command, transaction);
            }
        }


        /// <summary>
        /// Gets a parameter value.
        /// </summary>
        /// <param name="command">The command that contains the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public virtual object GetParameterValue(DbCommand command,
                                                string name)
        {
            if (command == null) throw new ArgumentNullException("command");

            return command.Parameters[BuildParameterName(name)].Value;
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        public DbCommand GetSqlStringCommand(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "query");

            return CreateCommandByCommandType(CommandType.Text, query);
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        public virtual DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "storedProcedureName");

            return CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        public virtual DbCommand GetStoredProcCommand(string storedProcedureName,
                                                      params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "storedProcedureName");

            DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);

            //AssignParameters(command, parameterValues);

            return command;
        }

        /// <summary>
        /// Executes the query for <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> representing the query to execute.</param>
        /// <returns>The quantity of rows affected.</returns>
        protected int DoExecuteNonQuery(DbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }

        object DoExecuteScalar(IDbCommand command)
        {
            object returnValue = command.ExecuteScalar();
            return returnValue;
        }

        /// <summary>
        /// <para>set <paramref name="command"/>'s <paramref name="connection"/> </para>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        protected static void PrepareCommand(DbCommand command,
                                             DbConnection connection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (connection == null) throw new ArgumentNullException("connection");

            command.Connection = connection;
        }
        /// <summary>
        /// <para>Assigns a <paramref name="transaction"/> to the <paramref name="command"/> and discovers parameters if needed.</para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to prepare.</para></param>
        /// <param name="transaction">The transaction to assign to the command.</param>
        protected static void PrepareCommand(DbCommand command,
                                     DbTransaction transaction)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (transaction == null) throw new ArgumentNullException("transaction");

            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
        }

        protected DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior)
        {
            DbDataAdapter adapter = dbProviderFactory.CreateDataAdapter();

            if (updateBehavior == UpdateBehavior.Continue)
            {
                SetUpRowUpdatedEvent(adapter);
            }
            return adapter;
        }

        protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter) { }

        /// <summary>
        ///		Gets a "wrapped" connection that will be not be disposed if a transaction is
        ///		active (created by creating a <see cref="TransactionScope"/> instance). The
        ///		connection will be disposed when no transaction is active.
        /// </summary>
        /// <returns></returns>
        protected DatabaseConnectionWrapper GetOpenConnection()
        {
            DatabaseConnectionWrapper connection = TransactionScopeConnections.GetConnection(this);
            //DatabaseConnectionWrapper connection = null;
            return connection ?? GetWrappedConnection();
        }

        /// <summary>
        /// Gets a "wrapped" connection for use outside a transaction.
        /// </summary>
        /// <returns>The wrapped connection.</returns>
        protected virtual DatabaseConnectionWrapper GetWrappedConnection()
        {
            return new DatabaseConnectionWrapper(GetNewOpenConnection());
        }
        internal DbConnection GetNewOpenConnection()
        {
            DbConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch
            {
                if (connection != null)
                    connection.Close();

                throw;
            }

            return connection;
        }
    }
}

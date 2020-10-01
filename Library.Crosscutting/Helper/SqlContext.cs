using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Library.Crosscutting.Helper
{
    public abstract class SqlContext
    {
        private static readonly Hashtable ParamCache = Hashtable.Synchronized(new Hashtable());
        protected string ConnectionString;
        protected IDbConnection Disconnection;
        protected IDbTransaction Transaction;
        protected const int CommandTimeOutValue = 600;
        protected const string ExceptionMessage = "Application Server is not configured properly.\\nThere is no link between Application Server and Database Server.\\nPlease configure the Application Server properly or ask your MIS Officer or technical person to do so.";

        public SqlContext()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        protected virtual void OpenConnection()
        {
            if (Disconnection.IsNull())
                Disconnection = new SqlConnection(ConnectionString);
            else if (Disconnection.State == ConnectionState.Open)
                Disconnection.Close();
            if (!Disconnection.IsNotNull())
                return;
            Disconnection.Open();
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        private void BeginTransaction(IsolationLevel isolation)
        {
            if (Disconnection.IsNotNull() && (uint)Disconnection.State > 0U)
            {
                Transaction = Disconnection.BeginTransaction(isolation);
            }
            else
            {
                OpenConnection();
                if (Disconnection.IsNotNull())
                    Transaction = Disconnection.BeginTransaction(isolation);
            }
        }

        public void CommitTransaction()
        {
            if (Transaction.IsNull())
                throw new Exception("Transaction object not Initialized");
            Transaction.Commit();
            CloseConnection();
        }

        public void RollBack()
        {
            if (Transaction.IsNull())
                throw new Exception("Transaction object not Initialized");
            Transaction.Rollback();
            CloseConnection();
        }

        protected void CloseConnection()
        {
            if (Disconnection.IsNull() || Disconnection.State != ConnectionState.Open)
                return;
            Disconnection.Close();
            Disconnection.Dispose();
            Disconnection = null;
        }

        public IDataReader ExecuteReader(string cmdText, params object[] parameters)
        {
            return ExecuteReader(cmdText, CommandType.Text, CommandBehavior.CloseConnection, parameters);
        }

        public IDataReader ExecuteReader(
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
        {
            return ExecuteReader(cmdText, cmdType, CommandBehavior.CloseConnection, parameters);
        }

        public IDataReader ExecuteReader(
          string cmdText,
          CommandType cmdType,
          CommandBehavior cmdBehavior,
          params object[] parameters)
        {
            try
            {
                IsValidCommandText(cmdText);
                OpenConnection();
                IDbCommand command = Disconnection.CreateCommand();
                PrepareCommand(command, cmdText, cmdType);
                PrepareCommandParameter(command, cmdText, parameters);
                return command.ExecuteReader(cmdBehavior);
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw;
            }
        }

        public object ExecuteScalar(string cmdText, params object[] parameters)
        {
            return ExecuteScalar(false, cmdText, CommandType.Text, parameters);
        }

        public object ExecuteScalar(bool reqTrans, string cmdText, params object[] parameters)
        {
            return ExecuteScalar(reqTrans, cmdText, CommandType.Text, parameters);
        }

        public object ExecuteScalar(string cmdText, CommandType cmdType, params object[] parameters)
        {
            return ExecuteScalar(false, cmdText, cmdType, parameters);
        }

        public virtual object ExecuteScalar(
          bool reqTrans,
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
        {
            bool flag = false;
            try
            {
                IsValidCommandText(cmdText);
                PrepareTransaction(reqTrans);
                flag = true;
                IDbCommand command = Disconnection.CreateCommand();
                PrepareCommand(command, cmdText, cmdType);
                PrepareCommandParameter(command, cmdText, parameters);
                object obj = command.ExecuteScalar();
                if (reqTrans)
                    return obj;
                Transaction.Commit();
                return obj;
            }
            catch (Exception ex)
            {
                if (reqTrans)
                {
                    throw;
                }
                else
                {
                    if (flag)
                        Transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                if (reqTrans.IsFalse())
                    CloseConnection();
            }
        }

        public int ExecuteNonQuery(string cmdText, params object[] parameters)
        {
            return ExecuteNonQuery(false, cmdText, CommandType.Text, parameters);
        }

        public int ExecuteNonQuery(bool reqTrans, string cmdText, params object[] parameters)
        {
            return ExecuteNonQuery(reqTrans, cmdText, CommandType.Text, parameters);
        }

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params object[] parameters)
        {
            return ExecuteNonQuery(false, cmdText, cmdType, parameters);
        }

        protected virtual int ExecuteNonQuery(
          bool reqTrans,
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
        {
            bool flag = false;
            try
            {
                IsValidCommandText(cmdText);
                PrepareTransaction(reqTrans);
                flag = true;
                IDbCommand command = Disconnection.CreateCommand();
                PrepareCommand(command, cmdText, cmdType);
                PrepareCommandParameter(command, cmdText, parameters);
                int num = command.ExecuteNonQuery();
                if (reqTrans)
                    return num;
                Transaction.Commit();
                return num;
            }
            catch (Exception ex)
            {
                if (reqTrans)
                {
                    throw;
                }
                else
                {
                    if (flag)
                        Transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                if (!reqTrans)
                    CloseConnection();
            }
        }

        private void ExecuteTextNonQuery(
          string cmdText,
          string hashKey,
          Dictionary<string, object> parameters)
        {
            IsValidCommandText(cmdText);
            PrepareTransaction(true);
            IDbCommand command = Disconnection.CreateCommand();
            PrepareCommand(command, cmdText, CommandType.Text);
            PrepareCommandParameter(command, hashKey, parameters);
            command.ExecuteNonQuery();
        }

        private void ExecuteSPNonQuery(string cmdText, Dictionary<string, object> parameters)
        {
            IsValidCommandText(cmdText);
            PrepareTransaction(true);
            IDbCommand command = Disconnection.CreateCommand();
            PrepareCommand(command, cmdText, CommandType.StoredProcedure);
            PrepareCommandParameter(command, cmdText, parameters);
            command.ExecuteNonQuery();
        }

        protected void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            foreach (IDbDataParameter commandParameter in commandParameters)
            {
                if (commandParameter.Direction == ParameterDirection.InputOutput && commandParameter.Value.IsNull())
                    commandParameter.Value = DBNull.Value;
                command.Parameters.Add(commandParameter);
            }
        }

        protected void AssignParameterValues(
          IDbDataParameter[] commandParameters,
          object[] parameterValues)
        {
            if (commandParameters.IsNull() || parameterValues.IsNull())
                return;
            if (commandParameters.Length != parameterValues.Length)
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            int index = 0;
            for (int length = commandParameters.Length; index < length; ++index)
                commandParameters[index].Value = parameterValues[index];
        }

        protected void AssignParameterValues(
          IDbDataParameter[] commandParameters,
          Dictionary<string, object> parameterValues)
        {
            if (commandParameters.IsNull() || parameterValues.IsNull())
                return;
            if (commandParameters.Length != parameterValues.Count)
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            foreach (IDbDataParameter commandParameter in commandParameters)
            {
                string key = commandParameter.ParameterName.Replace("@", string.Empty);
                if (!parameterValues.ContainsKey(key))
                    throw new ArgumentException(string.Format("Stored procedure expects {0} parameter which was not supplied", key));
                commandParameter.Value = parameterValues[key];
            }
        }

        protected internal IDbDataParameter[] GetSpParameterSet(string spName)
        {
            return GetSpParameterSet(ConnectionString, spName, false);
        }

        protected IDbDataParameter[] GetSpParameterSet(
          string connectionString,
          string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        protected void GetSpParameterSet(IDbCommand command, object[] parameterValues)
        {
            IDbDataParameter[] dbDataParameterArray = new IDbDataParameter[parameterValues.Length];
            for (int index = 0; index < parameterValues.Length; ++index)
            {
                object parameterValue = parameterValues[index];
                dbDataParameterArray[index] = command.CreateParameter();
                dbDataParameterArray[index].DbType = MappedDbType.GetDbType(parameterValue.GetType());
                dbDataParameterArray[index].ParameterName = index.ToString();
                dbDataParameterArray[index].Value = parameterValue;
                command.Parameters.Add(dbDataParameterArray[index]);
            }
        }

        protected IDbDataParameter[] GetSpParameterSet(
          string connectionString,
          string spName,
          bool includeReturnValueParameter)
        {
            string str = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : string.Empty);
            IDbDataParameter[] originalParameters = (IDbDataParameter[])ParamCache[str];
            if (originalParameters.IsNull())
                originalParameters = (IDbDataParameter[])(ParamCache[str] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            return CloneParameters(originalParameters);
        }

        protected IDbDataParameter[] GetSpParameterSet(
          IDbCommand command,
          string hashKey,
          Dictionary<string, object> parameterValues)
        {
            IDbDataParameter[] originalParameters = (IDbDataParameter[])ParamCache[hashKey];
            if (originalParameters.IsNull())
                originalParameters = (IDbDataParameter[])(ParamCache[hashKey] = DiscoverSpParameterSet(command, parameterValues));
            return CloneParameters(originalParameters);
        }

        protected virtual IDbDataParameter[] DiscoverSpParameterSet(
          string connectionString,
          string spName,
          bool includeReturnValueParameter)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        SqlCommandBuilder.DeriveParameters(command);
                        if (includeReturnValueParameter.IsFalse())
                            command.Parameters.RemoveAt(0);
                        IDbDataParameter[] dbDataParameterArray = new IDbDataParameter[command.Parameters.Count];
                        command.Parameters.CopyTo(dbDataParameterArray, 0);
                        return dbDataParameterArray;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected virtual IDbDataParameter[] DiscoverSpParameterSet(
          IDbCommand command,
          Dictionary<string, object> parameterValues)
        {
            IDbDataParameter[] dbDataParameterArray = new IDbDataParameter[parameterValues.Count];
            int index = 0;
            foreach (KeyValuePair<string, object> parameterValue in parameterValues)
            {
                dbDataParameterArray[index] = command.CreateParameter();
                dbDataParameterArray[index].DbType = MappedDbType.GetDbType(parameterValue.Value.GetType());
                dbDataParameterArray[index].ParameterName = parameterValue.Key;
                ++index;
            }
            return dbDataParameterArray;
        }

        protected static IDbDataParameter[] CloneParameters(
          IDbDataParameter[] originalParameters)
        {
            IDbDataParameter[] dbDataParameterArray = new IDbDataParameter[originalParameters.Length];
            int index = 0;
            for (int length = originalParameters.Length; index < length; ++index)
                dbDataParameterArray[index] = (IDbDataParameter)((ICloneable)originalParameters[index]).Clone();
            return dbDataParameterArray;
        }

        protected virtual void PrepareCommandParameter(
          IDbCommand command,
          string cmdText,
          object[] parameters)
        {
            if (parameters.IsNull() || parameters.Length.IsZero())
                return;
            if (command.CommandType == CommandType.StoredProcedure)
            {
                IDbDataParameter[] spParameterSet = GetSpParameterSet(ConnectionString, cmdText);
                AssignParameterValues(spParameterSet, parameters);
                AttachParameters(command, spParameterSet);
            }
            else
                GetSpParameterSet(command, parameters);
        }

        protected virtual void PrepareCommandParameter(
          IDbCommand command,
          string cmdText,
          Dictionary<string, object> parameters)
        {
            if (parameters.IsNull() || parameters.Count.IsZero())
                return;
            IDbDataParameter[] commandParameters = command.CommandType != CommandType.StoredProcedure ? GetSpParameterSet(command, cmdText, parameters) : GetSpParameterSet(ConnectionString, cmdText);
            AssignParameterValues(commandParameters, parameters);
            AttachParameters(command, commandParameters);
        }

        private static void IsValidCommandText(string commandText)
        {
            if (commandText.IsNullOrEmpty())
                throw new Exception("Query is blank.");
        }

        private void PrepareTransaction(bool reqTrans)
        {
            if (!reqTrans)
            {
                OpenConnection();
                Transaction = Disconnection.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            else if (Transaction.IsNull())
                throw new Exception("Transaction is not initialized.");
        }

        private void PrepareCommand(IDbCommand command, string cmdText, CommandType cmdType)
        {
            command.CommandType = cmdType;
            command.CommandText = cmdText;
            command.CommandTimeout = 600;
            command.Connection = Disconnection;
            command.Transaction = Transaction;
        }

        [DebuggerStepThrough]
        public void SaveModelCollection(BaseModel model)
        {
            SaveModelCollection(false, (IEnumerable<BaseModel>)new List<BaseModel>()
      {
        model
      });
        }

        [DebuggerStepThrough]
        public void SaveModelCollection(params IEnumerable<BaseModel>[] saveItems)
        {
            SaveModelCollection(false, saveItems);
        }

        [DebuggerStepThrough]
        public virtual void SaveModelCollection(
          bool reqTrans,
          params IEnumerable<BaseModel>[] saveItems)
        {
            bool flag = false;
            try
            {
                if (saveItems.IsNull() || saveItems.Length.IsZero())
                    throw new Exception("Collection not Initialized");
                PrepareTransaction(reqTrans);
                flag = true;
                saveItems = saveItems.Reverse();
                foreach (IEnumerable<BaseModel> saveItem in saveItems)
                    BatchUpdate(saveItem.GetChanges(ModelState.Deleted));
                saveItems = saveItems.Reverse();
                foreach (IEnumerable<BaseModel> saveItem in saveItems)
                {
                    IList<BaseModel> list = saveItem as IList<BaseModel> ?? saveItem.ToList<BaseModel>();
                    BatchUpdate(list.GetChanges(ModelState.Added));
                    BatchUpdate(list.GetChanges(ModelState.Modified));
                }
                if (reqTrans)
                    return;
                Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (reqTrans)
                {
                    throw;
                }
                else
                {
                    if (flag)
                        Transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                if (!reqTrans)
                    CloseConnection();
            }
        }

        private void BatchUpdate(IEnumerable<BaseModel> modelList)
        {
            foreach (BaseModel model in modelList)
            {
                Dictionary<string, object> getParameterInfo = model.GetParameterInfo;
                if (getParameterInfo.Keys.Contains<string>("ProcedureName"))
                {
                    string str = getParameterInfo["ProcedureName"].ToString();
                    getParameterInfo.Remove("ProcedureName");
                    if (!str.IsNullOrEmpty())
                        ExecuteSPNonQuery(str, getParameterInfo);
                }
                else if (getParameterInfo.Keys.Contains<string>("TableName"))
                {
                    string str = getParameterInfo["TableName"].ToString();
                    getParameterInfo.Remove("TableName");
                    if (!str.IsNullOrEmpty())
                        ExecuteTextNonQuery(MakeSql(str, model.ModelState, getParameterInfo), string.Format("{0}:{1}:Parameters", str, model.ModelState), getParameterInfo);
                }
            }
        }

        private string MakeSql(string tableName, ModelState state, Dictionary<string, object> fields)
        {
            string str1 = string.Format("{0}:{1}:Sql", tableName, state);
            string str2 = ParamCache[str1].IsNull() ? string.Empty : ParamCache[str1].ToString();
            if (str2.IsNullOrEmpty())
            {
                switch (state)
                {
                    case ModelState.Added:
                        str2 = GetInsertedScript(tableName, fields);
                        break;
                    case ModelState.Modified:
                        str2 = GetUpdatedScript(tableName, fields);
                        break;
                    case ModelState.Deleted:
                        str2 = GetDeletedScript(tableName, fields);
                        break;
                }
                ParamCache[str1] = str2;
            }
            return str2;
        }

        private static string GetInsertedScript(string tableName, Dictionary<string, object> fields)
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder1.Append("\tINSERT INTO " + tableName + " (");
            stringBuilder2.Append("\tVALUES (");
            int num = 0;
            foreach (KeyValuePair<string, object> field in fields)
            {
                ++num;
                if (num % 5 == 0)
                {
                    stringBuilder1.Append("\n\t\t");
                    stringBuilder2.Append("\n\t\t");
                }
                stringBuilder1.Append(field.Key + ", ");
                stringBuilder2.Append("@" + field.Key + ", ");
            }
            StringBuilder stringBuilder3 = stringBuilder1.Remove(stringBuilder1.Length - 2, 2);
            StringBuilder stringBuilder4 = stringBuilder2.Remove(stringBuilder2.Length - 2, 2);
            stringBuilder3.Append(")\n");
            stringBuilder4.Append(")\n");
            stringBuilder3.Append(stringBuilder4);
            return stringBuilder3.ToString();
        }

        private string GetUpdatedScript(string tableName, Dictionary<string, object> fields)
        {
            Dictionary<string, object> dictionary = DiscoverKeyFields(ConnectionString, tableName);
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append("\tUPDATE " + tableName + "\n");
            stringBuilder1.Append("\tSET\t");
            foreach (KeyValuePair<string, object> field in fields)
            {
                if (!dictionary.ContainsKey(field.Key))
                    stringBuilder1.Append(field.Key + " = @" + field.Key + ",\n\t\t");
            }
            StringBuilder stringBuilder2 = stringBuilder1.Remove(stringBuilder1.Length - 4, 4);
            stringBuilder2.Append("\n\tWHERE\t");
            foreach (KeyValuePair<string, object> keyValuePair in dictionary)
                stringBuilder2.Append(keyValuePair.Key + " = @" + keyValuePair.Key + " AND ");
            return stringBuilder2.Remove(stringBuilder2.Length - 5, 5).ToString();
        }

        private static string GetDeletedScript(string tableName, Dictionary<string, object> fields)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\tDELETE " + tableName + "\n");
            stringBuilder.Append("\tWHERE\t");
            foreach (KeyValuePair<string, object> field in fields)
                stringBuilder.Append(field.Key + " = @" + field.Key + " AND ");
            return stringBuilder.Remove(stringBuilder.Length - 5, 5).ToString();
        }

        protected Dictionary<string, object> DiscoverKeyFields(
          string connectionString,
          string tableName)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM " + tableName, connection))
                {
                    connection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    DataTable schemaTable = sqlDataReader.GetSchemaTable();
                    if (schemaTable != null)
                    {
                        DataRow[] dataRowArray = schemaTable.Select("IsKey = true");
                        if (dataRowArray.Length == 0)
                            dataRowArray = schemaTable.Select("IsUnique = true");
                        if (dataRowArray.Length == 0)
                            dataRowArray = new DataRow[1]
                            {
                schemaTable.Rows[0]
                            };
                        foreach (DataRow dataRow in dataRowArray)
                            dictionary.Add(dataRow["ColumnName"].ToString().Replace("  ", string.Empty).Replace(" ", string.Empty), "");
                    }
                    sqlDataReader.Close();
                }
            }
            return dictionary;
        }
    }
}
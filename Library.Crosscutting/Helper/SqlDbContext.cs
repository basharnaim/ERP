using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Library.Crosscutting.Helper
{
    public class SqlDbContext : SqlContext, ISqlDbContext
    {
        public void Insert(BaseModel model)
        {
            SaveModelCollection(model);
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(string cmdText, params object[] parameters) where T : class
        {
            return LoadModel<T>(cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(
          DbUtil.OverrideModel<T> mapFunc,
          string cmdText,
          params object[] parameters)
          where T : class
        {
            return LoadModel<T>(mapFunc, cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(string cmdText, CommandType cmdType, params object[] parameters) where T : class
        {
            return LoadModel<T>(null, cmdText, cmdType, parameters);
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(
          DbUtil.OverrideModel<T> mapFunc,
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
          where T : class
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText, cmdType, parameters);
                return LoadModel<T>(reader, mapFunc);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(IDataReader reader) where T : class
        {
            return LoadModel<T>(reader, null);
        }

        [DebuggerStepThrough]
        public T LoadModel<T>(IDataReader reader, DbUtil.OverrideModel<T> mapFunc) where T : class
        {
            if (!typeof(T).IsBaseModel())
                throw new Exception("Item must be inherited from BaseModel class.");
            if (!reader.Read())
                return Activator.CreateInstance<T>();
            Dictionary<string, object> fields = GetFields(reader);
            return mapFunc.IsNotNull() ? mapFunc(fields) : CreateModel<T>(fields);
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(string cmdText, params object[] parameters) where T : class
        {
            return LoadModelCollection<T>(cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(
          DbUtil.OverrideModel<T> mapFunc,
          string cmdText,
          params object[] parameters)
          where T : class
        {
            return LoadModelCollection<T>(mapFunc, cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
          where T : class
        {
            return LoadModelCollection<T>(null, cmdText, cmdType, parameters);
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(
          DbUtil.OverrideModel<T> mapFunc,
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
          where T : class
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText, cmdType, parameters);
                return LoadModelCollection<T>(reader, mapFunc);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(IDataReader reader) where T : class
        {
            return LoadModelCollection<T>(reader, null);
        }

        [DebuggerStepThrough]
        public List<T> LoadModelCollection<T>(IDataReader reader, DbUtil.OverrideModel<T> mapFunc) where T : class
        {
            if (!typeof(T).IsBaseModel())
                throw new Exception("Item must be inherited from BaseModel class.");
            List<T> objList = new List<T>();
            while (reader.Read())
            {
                Dictionary<string, object> fields = GetFields(reader);
                objList.Add(mapFunc.IsNotNull() ? mapFunc(fields) : CreateModel<T>(fields));
            }
            return objList;
        }

        private static T CreateModel<T>(Dictionary<string, object> fields)
        {
            T instance = Activator.CreateInstance<T>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
            foreach (KeyValuePair<string, object> field in fields)
            {
                PropertyDescriptor propertyDescriptor = properties.Find(field.Key, true);
                if (!propertyDescriptor.IsNull())
                {
                    object obj = field.Value.MapField(propertyDescriptor.PropertyType);
                    propertyDescriptor.SetValue(instance, obj);
                }
            }
            PropertyDescriptor propertyDescriptor1 = properties.Find("State", true);
            if (propertyDescriptor1.IsNotNull())
                propertyDescriptor1.SetValue(instance, ModelState.Unchanged);
            return instance;
        }

        [DebuggerStepThrough]
        public GridModel LoadGridModel(GridParameter parameters, string cmdText)
        {
            IDataReader reader = null;
            try
            {
                if (parameters.ExportType.IsNotNullOrEmpty())
                    return LoadGridExport(parameters, cmdText);
                GridModel gridModel = new GridModel();
                List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
                List<string> stringList = new List<string>();
                int num = 0;
                cmdText = MakeSearchQuery(parameters, cmdText);
                if (parameters.searchBy.IsNotNullOrEmpty() && parameters.search.IsNotNullOrEmpty())
                    reader = ExecuteReader(cmdText, (object)parameters.search);
                else
                    reader = ExecuteReader(cmdText);
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    string name = reader.GetName(i);
                    if (name != "TotalRows")
                        stringList.Add(name);
                }
                if (reader.Read())
                {
                    if (parameters.ServerPagination)
                        num = Convert.ToInt32(reader["TotalRows"]);
                    dictionaryList.Add(GetFields(stringList, reader));
                }
                while (reader.Read())
                    dictionaryList.Add(GetFields(stringList, reader));
                gridModel.rows = dictionaryList;
                gridModel.total = parameters.ServerPagination ? num : dictionaryList.Count;
                return gridModel;
            }
            finally
            {
                reader.CloseReader();
            }
        }

        public DataTable LoadDataTable(string cmdText)
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText);
                return DbUtil.DataReaderToDataTable(reader);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        private GridModel LoadGridExport(GridParameter parameters, string commandText)
        {
            IDataReader reader = null;
            try
            {
                GridModel gridModel = new GridModel();
                commandText = MakeSearchQuery(parameters, commandText);
                reader = ExecuteReader(commandText);
                gridModel.source = DbUtil.DataReaderToDataSet(reader);
                return gridModel;
            }
            finally
            {
                reader.CloseReader();
            }
        }

        private static string MakeSearchQuery(GridParameter parameters, string sql)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            if (parameters.searchBy.IsNotNullOrEmpty() && parameters.search.IsNotNullOrEmpty())
                str1 = string.Format("WHERE {0} LIKE @0", parameters.searchBy);
            if (parameters.sort.IsNotNullOrEmpty() && parameters.order.IsNotNullOrEmpty())
                str2 = string.Format("ORDER BY {0} {1}", parameters.sort, parameters.order);
            if (parameters.ServerPagination)
                str3 = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", parameters.offset, parameters.Limit);
            return string.Format("SELECT *, COUNT(*) OVER () AS TotalRows\r\n                                FROM (\r\n\t                                {0}\r\n                                ) AS TAB\r\n                                {1}\r\n                                {2}\r\n                                {3}", sql, str1, str2, str3);
        }

        [DebuggerStepThrough]
        public List<ComboModel> LoadComboModel(string cmdText, string valueField)
        {
            return LoadComboModel(cmdText, valueField, valueField);
        }

        [DebuggerStepThrough]
        public List<ComboModel> LoadComboModel(
          string cmdText,
          string valueField,
          string textField)
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText);
                return LoadComboModel(reader, valueField, textField);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        [DebuggerStepThrough]
        public List<ComboModel> LoadComboModel(IDataReader reader, string valueField)
        {
            return LoadComboModel(reader, valueField, valueField);
        }

        [DebuggerStepThrough]
        public List<ComboModel> LoadComboModel(
          IDataReader reader,
          string valueField,
          string textField)
        {
            List<ComboModel> comboModelList = new List<ComboModel>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                string name = reader.GetName(i);
                if (name.Equals(valueField, StringComparison.CurrentCultureIgnoreCase))
                    dictionary["ValueField"] = name;
                if (name.Equals(textField, StringComparison.CurrentCultureIgnoreCase))
                    dictionary["TextField"] = name;
            }
            while (reader.Read())
            {
                ComboModel comboModel = new ComboModel();
                foreach (string index in dictionary.Values)
                {
                    if (index.Equals(valueField, StringComparison.CurrentCultureIgnoreCase))
                        comboModel.Value = reader[index].ToString();
                    if (index.Equals(textField, StringComparison.CurrentCultureIgnoreCase))
                        comboModel.Text = reader[index].ToString();
                }
                comboModelList.Add(comboModel);
            }
            return comboModelList;
        }

        [DebuggerStepThrough]
        public Dictionary<string, object> ModelData(
          string cmdText,
          params object[] parameters)
        {
            return ModelData(cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public Dictionary<string, object> ModelData(
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText, cmdType, parameters);
                return ModelData(reader);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        [DebuggerStepThrough]
        public Dictionary<string, object> ModelData(IDataReader reader)
        {
            List<string> stringList = new List<string>();
            for (int i = 0; i < reader.FieldCount; ++i)
                stringList.Add(reader.GetName(i));
            if (reader.Read())
                return GetFields(stringList, reader);
            return new Dictionary<string, object>();
        }

        [DebuggerStepThrough]
        public List<Dictionary<string, object>> ModelDataCollection(
          string cmdText,
          params object[] parameters)
        {
            return ModelDataCollection(cmdText, CommandType.Text, parameters);
        }

        [DebuggerStepThrough]
        public List<Dictionary<string, object>> ModelDataCollection(
          string cmdText,
          CommandType cmdType,
          params object[] parameters)
        {
            IDataReader reader = null;
            try
            {
                reader = ExecuteReader(cmdText, cmdType, parameters);
                return ModelDataCollection(reader);
            }
            finally
            {
                reader.CloseReader();
            }
        }

        [DebuggerStepThrough]
        public List<Dictionary<string, object>> ModelDataCollection(IDataReader reader)
        {
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            List<string> stringList = new List<string>();
            for (int i = 0; i < reader.FieldCount; ++i)
                stringList.Add(reader.GetName(i));
            while (reader.Read())
                dictionaryList.Add(GetFields(stringList, reader));
            return dictionaryList;
        }

        private static Dictionary<string, object> GetFields(IDataRecord reader)
        {
            int fieldCount = reader.FieldCount;
            Dictionary<string, object> dictionary = new Dictionary<string, object>(fieldCount);
            for (int i = 0; i < fieldCount; ++i)
            {
                string name = reader.GetName(i);
                int ordinal = reader.GetOrdinal(name);
                dictionary[name] = reader.GetValue(ordinal);
            }
            return dictionary;
        }

        private static Dictionary<string, object> GetFields(
          IEnumerable<string> cols,
          IDataRecord reader)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (string col in cols)
            {
                int ordinal = reader.GetOrdinal(col);
                string dataTypeName = reader.GetDataTypeName(ordinal);
                object obj = reader.GetValue(ordinal);
                if (obj.IsNullOrDbNull())
                {
                    dictionary[col] = obj;
                }
                else
                {
                    string str = dataTypeName;
                    dictionary[col] = str == "bigint" ? obj.ToString() : (str == "date" ? ((DateTime)obj).ToString(Util.ConvertedDateFormat) : (str == "datetime" ? ((DateTime)obj).ToString(Util.ConvertedDateFormat + " hh:mm:ss tt") : (str == "time" ? DateTime.MinValue.Add((TimeSpan)obj).ToString("hh:mm tt") : obj)));
                }
            }
            return dictionary;
        }

        public T GetMaxNumber<T>(string tableName, string fieldName)
        {
            return GetMaxNumber<T>(tableName, fieldName, true);
        }

        private T GetMaxNumber<T>(string tableName, string fieldName, bool reqTrans)
        {
            string cmdText = string.Format("SELECT ISNULL(MAX({1}),0) + 1 FROM {0}", tableName, fieldName);
            return (T)Convert.ChangeType(ExecuteScalar(reqTrans, cmdText), typeof(T));
        }

        public bool HasValueInDatabase(
          string tableName,
          string fieldName,
          string value,
          string conName)
        {
            return HasValueInDatabase(tableName, fieldName, value, false);
        }

        public bool HasValueInDatabase(string tableName, string fieldName, string value)
        {
            return HasValueInDatabase(tableName, fieldName, value, true);
        }

        private bool HasValueInDatabase(
          string tableName,
          string fieldName,
          string value,
          bool reqTrans)
        {
            string cmdText = string.Format("SELECT COUNT({1}) FROM {0} WHERE {1} = '{2}'", tableName, fieldName, value);
            return Convert.ToInt32(ExecuteScalar(reqTrans, cmdText)).IsNotZero();
        }

        public T GetCurrentIdent<T>(string tableName)
        {
            return GetCurrentIdent<T>(tableName, true);
        }

        private T GetCurrentIdent<T>(string tableName, bool reqTrans)
        {
            string cmdText = string.Format("Select IDENT_CURRENT('{0}')", tableName);
            object obj = ExecuteScalar(reqTrans, cmdText);
            if (obj.IsNullOrDbNull())
                obj = 0;
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
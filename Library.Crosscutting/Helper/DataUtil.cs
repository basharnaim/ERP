using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Library.Crosscutting.Helper
{
    public static class DataUtil
    {
        public static DataTable CreateDataTable<T>() where T : class
        {
            Type componentType = typeof(T);
            DataTable dataTable = new DataTable(componentType.Name);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(componentType))
            {
                Type type = property.PropertyType;
                if (CanUseType(type))
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);
                    if (type.IsEnum)
                        type = Enum.GetUnderlyingType(type);
                    dataTable.Columns.Add(property.Name, type);
                }
            }
            return dataTable;
        }

        public static DataTable ConvertToDataTable<T>(ICollection<T> collection) where T : class
        {
            DataTable dataTable = CreateDataTable<T>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (T obj in collection)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    if (CanUseType(propertyDescriptor.PropertyType))
                        row[propertyDescriptor.Name] = propertyDescriptor.GetValue(obj) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        private static bool CanUseType(Type propertyType)
        {
            return !propertyType.IsArray && (propertyType.IsValueType || !(propertyType != typeof(string)));
        }

        public static ICollection<T> ConvertToCollection<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0)
                return null;
            ICollection<T> objs = new List<T>();
            foreach (DataRow row in (InternalDataCollectionBase)dt.Rows)
            {
                T entity = ConvertDataRowToEntity<T>(row);
                objs.Add(entity);
            }
            return objs;
        }

        public static T ConvertDataRowToEntity<T>(DataRow row) where T : class, new()
        {
            Type type = typeof(T);
            T obj1 = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo property = type.GetProperty(column.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (!(property == null) && property.CanWrite)
                {
                    object obj2 = row[column.ColumnName];
                    if (obj2 == DBNull.Value)
                        obj2 = null;
                    property.SetValue(obj1, obj2, null);
                }
            }
            return obj1;
        }
    }
}
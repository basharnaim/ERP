using System.Collections.Generic;
using System.Data;

namespace Library.Crosscutting.Helper
{
    public static class DbUtil
    {
        public static DataSet DataReaderToDataSet(IDataReader reader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                int fieldCount = reader.FieldCount;
                DataTable table = new DataTable();
                for (int i = 0; i < fieldCount; ++i)
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                table.BeginLoadData();
                object[] values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();
                dataSet.Tables.Add(table);
            }
            while (reader.NextResult());
            reader.Close();
            return dataSet;
        }

        public static DataTable DataReaderToDataTable(IDataReader reader)
        {
            DataTable dataTable = new DataTable();
            do
            {
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; ++i)
                    dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                dataTable.BeginLoadData();
                object[] values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    dataTable.LoadDataRow(values, true);
                }
                dataTable.EndLoadData();
            }
            while (reader.NextResult());
            reader.Close();
            return dataTable;
        }

        public delegate T OverrideModel<out T>(Dictionary<string, object> fields);
    }
}
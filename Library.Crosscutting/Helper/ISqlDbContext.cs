using System.Collections.Generic;
using System.Data;

namespace Library.Crosscutting.Helper
{
    public interface ISqlDbContext
    {
        T LoadModel<T>(string cmdText, params object[] parameters) where T : class;

        List<T> LoadModelCollection<T>(string cmdText, params object[] parameters) where T : class;

        List<T> LoadModelCollection<T>(
            string cmdText,
            CommandType cmdType,
            params object[] parameters)
            where T : class;

        List<ComboModel> LoadComboModel(
            string cmdText,
            string valueField,
            string textField);

        GridModel LoadGridModel(GridParameter parameters, string cmdText);

        DataTable LoadDataTable(string cmdText);

        Dictionary<string, object> ModelData(string cmdText, params object[] parameters);

        List<Dictionary<string, object>> ModelDataCollection(
            string cmdText,
            params object[] parameters);
    }
}

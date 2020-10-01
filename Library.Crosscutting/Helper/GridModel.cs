using System.Collections;
using System.Data;

namespace Library.Crosscutting.Helper
{
    public class GridModel
    {
        public IEnumerable rows { get; set; }

        public int total { get; set; }

        public DataSet source { get; set; }
    }
}
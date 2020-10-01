namespace Library.Crosscutting.Helper
{
    public class GridParameter
    {
        public string TempKeyName { get; set; }

        public bool IsSession { get; set; }

        public int Limit { get; set; }

        public int offset { get; set; }

        public string order { get; set; }

        public string searchBy { get; set; }

        public string search { get; set; }

        public string sort { get; set; }

        public bool ServerPagination { get; set; }

        public string CmdText { get; set; }

        public string ExportType { get; set; }
    }
}
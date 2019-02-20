using System.Collections.Generic;
using System.Linq;

namespace Database
{
    class Table
    {
        private const string TYPE_STRING = "VARCHAR";
        private const string TYPE_BOOL = "BIT";
        private const string TYPE_DATE = "DATETIME";
        private const string TYPE_DOUBLE = "REAL";

        private List<string> ColumnNames;
        private readonly Dictionary<string, string> ColumnNameToType;
        private readonly List<string[]> Lines = new List<string[]>();
        private readonly int NrColumns = 0;

        // Create table where 
        public Table(List<string> names, Dictionary<string, string> columns)
        {
            ColumnNames = names;
            ColumnNameToType = columns;
            NrColumns = names.Count;
        }
        public List<string> GetColumnNames() { return ColumnNames; }

        public void Insert(List<string> Names, List<string> Values)
        {
            var nameValuePairs = Names.Zip(Values, (n, v) => new { Name = n, Value = v });
            string[] line = new string[NrColumns];
            foreach (var pair in nameValuePairs)
            {
                line[ColumnNames.IndexOf(pair.Name)] = pair.Value;
            }
            Lines.Add(line);
        }

        public List<string[]> Select(List<string> columns, List<string[]> lines = null)
        {
            if (lines == null)
            {
                lines = Lines;
            }
            List<string[]> selectedLines = new List<string[]>();

            foreach (var l in lines)
            {
                string[] selectedColumns = new string[columns.Count];
                int i = 0;

                foreach (var c in columns)
                {
                    selectedColumns[i++] = l[ColumnNames.IndexOf(c)];
                }

                selectedLines.Add(selectedColumns);
            }

            return selectedLines;
        }
    }
}
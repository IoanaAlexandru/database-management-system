using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private List<string[]> Lines = new List<string[]>();
        private readonly int NrColumns = 0;

        // Create table where 
        public Table(List<string> names, Dictionary<string, string> columns)
        {
            ColumnNames = names;
            ColumnNameToType = columns;
            NrColumns = names.Count;
        }
        public List<string> GetColumnNames() { return ColumnNames; }

        public void Insert(List<string> names, List<string> values)
        {
            var nameValuePairs = names.Zip(values, (n, v) => new { Name = n, Value = v });
            string[] line = new string[NrColumns];
            Parallel.ForEach(nameValuePairs, pair =>
           {
               line[ColumnNames.IndexOf(pair.Name)] = pair.Value;
           });
            Lines.Add(line);
        }

        public List<string[]> Select(List<string> columns, List<string[]> lines = null)
        {
            if (lines == null)
            {
                lines = Lines;
            }
            List<string[]> selectedLines = new List<string[]>();

            Parallel.ForEach(lines, l =>
           {
               string[] selectedColumns = new string[columns.Count];
               int i = 0;

               foreach (var c in columns)
               {
                   selectedColumns[i++] = l[ColumnNames.IndexOf(c)];
               }

               selectedLines.Add(selectedColumns);
           });

            return selectedLines;
        }

        public void Update(List<string> names, List<string> values, List<string[]> lines = null)
        {
            if (lines == null)
            {
                lines = Lines;
            }

            var nameValuePairs = names.Zip(values, (n, v) => new { Name = n, Value = v });
            Parallel.ForEach(lines, l =>
           {
               Parallel.ForEach(nameValuePairs, pair =>
              {
                  l[ColumnNames.IndexOf(pair.Name)] = pair.Value;
              });
           });
        }

        public void Delete(List<string[]> lines = null)
        {
            if (lines == null)
            {
                Lines.Clear();
            }
            else
            {
                Lines = Lines.Except(lines).ToList();
            }
        }
    }
}
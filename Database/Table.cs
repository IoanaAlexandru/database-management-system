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

        // Create table using the column names (in order) and the column types
        public Table(List<string> names, Dictionary<string, string> columns)
        {
            ColumnNames = names;
            ColumnNameToType = columns;
            NrColumns = names.Count;
        }
        public List<string> GetColumnNames() { return ColumnNames; }

        // Insert new entry in the table using the name-value pairs
        public void Insert(List<string> names, List<string> values)
        {
            var nameValuePairs = names.Zip(values, (n, v) => new { Name = n, Value = v });
            string[] line = new string[NrColumns];
            Parallel.ForEach(nameValuePairs, pair =>
           {
               int colIndex = ColumnNames.IndexOf(pair.Name);
               if (colIndex < 0) return;
               line[colIndex] = pair.Value;
           });
            Lines.Add(line);
        }

        // Select columns from the lines in the table (or from the lines parameter, if not null)
        public List<string[]> Select(ref List<string> columns, List<string[]> lines = null)
        {
            if (lines == null)
            {
                lines = Lines;
            }

            List<string[]> selectedLines = new List<string[]>();
            List<string> columnsClone = new List<string>();
            foreach (var c in columns)
                columnsClone.Add(c);
            List<string> invalidColumns = new List<string>();

            Parallel.ForEach(lines, l =>
           {
               List<string> selectedColumns = new List<string>();

               foreach (var c in columnsClone)
               {
                   int colIndex = ColumnNames.IndexOf(c);
                   if (colIndex < 0)
                   {
                       invalidColumns.Add(c);
                       continue;
                   }
                   selectedColumns.Add(l[colIndex]);
               }

               selectedLines.Add(selectedColumns.ToArray());
           });

            // Remove columns that were not found
            columns = columns.Except(invalidColumns).ToList();
            return selectedLines;
        }

        // Update the name-value pairs in the lines from table (or from the lines parameter, if not null)
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
                  int col_index = ColumnNames.IndexOf(pair.Name);
                  if (col_index < 0) return;
                  l[col_index] = pair.Value;
              });
           });
        }

        // Delete all lines from the table (or only the lines parameter, if not null)
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

        // Get all lines from the table which follow the restriction column = value
        public List<string[]> WhereEquals(string column, string value)
        {
            int colIndex = ColumnNames.IndexOf(column);
            if (colIndex < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[colIndex].Equals(value)
                         select x;
            return result.ToList();
        }

        // Get all lines from the table which follow the restriction column < value
        public List<string[]> WhereLess(string column, string value)
        {
            int colIndex = ColumnNames.IndexOf(column);
            if (colIndex < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[colIndex].CompareTo(value) < 0
                         select x;
            return result.ToList();
        }

        // Get all lines from the table which follow the restriction column > value
        public List<string[]> WhereGreater(string column, string value)
        {
            int colIndex = ColumnNames.IndexOf(column);
            if (colIndex < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[colIndex].CompareTo(value) > 0
                         select x;
            return result.ToList();
        }
    }
}
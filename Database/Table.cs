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
               int col_index = ColumnNames.IndexOf(pair.Name);
               if (col_index < 0) return;
               line[col_index] = pair.Value;
           });
            Lines.Add(line);
        }

        public List<string[]> Select(ref List<string> columns, List<string[]> lines = null)
        {
            if (lines == null)
            {
                lines = Lines;
            }

            List<string[]> selectedLines = new List<string[]>();
            List<string> columns_clone = new List<string>();
            foreach (var c in columns)
                columns_clone.Add(c);
            List<string> invalid_columns = new List<string>();

            Parallel.ForEach(lines, l =>
           {
               List<string> selectedColumns = new List<string>();

               foreach (var c in columns_clone)
               {
                   int col_index = ColumnNames.IndexOf(c);
                   if (col_index < 0)
                   {
                       invalid_columns.Add(c);
                       continue;
                   }
                   selectedColumns.Add(l[col_index]);
               }

               selectedLines.Add(selectedColumns.ToArray());
           });

            columns = columns.Except(invalid_columns).ToList();
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
                  int col_index = ColumnNames.IndexOf(pair.Name);
                  if (col_index < 0) return;
                  l[col_index] = pair.Value;
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

        public List<string[]> WhereEquals(string column, string value)
        {
            int col_index = ColumnNames.IndexOf(column);
            if (col_index < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[col_index].Equals(value)
                         select x;
            return result.ToList();
        }

        public List<string[]> WhereLess(string column, string value)
        {
            int col_index = ColumnNames.IndexOf(column);
            if (col_index < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[col_index].CompareTo(value) < 0
                         select x;
            return result.ToList();
        }

        public List<string[]> WhereGreater(string column, string value)
        {
            int col_index = ColumnNames.IndexOf(column);
            if (col_index < 0)
                return new List<string[]>();

            var result = from x in Lines
                         where x[col_index].CompareTo(value) > 0
                         select x;
            return result.ToList();
        }
    }
}
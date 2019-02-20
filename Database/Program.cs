using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly List<string[]> Lines = new List<string[]>();
        private readonly int NrColumns = 0;

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

        public List<string[]> Select(List<string> columns)
        {
            List<string[]> selectedLines = new List<string[]>();

            foreach (var l in Lines)
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

    class Program
    {
        private const string CMD_CREATE_TABLE = "CREATE TABLE";
        private const string CMD_INSERT_INTO = "INSERT INTO";
        private const string CMD_VALUES = "VALUES";
        private const string CMD_SELECT = "SELECT";
        private const string CMD_FROM = "FROM";
        private const string CMD_QUIT = "QUIT";
        private const string CMD_EXIT = "EXIT";

        static void Main(string[] args)
        {
            Dictionary<string, Table> tables = new Dictionary<string, Table>();

            string cmd = "";
            int open_bracket, close_bracket;
            while (!cmd.Equals("QUIT") && !cmd.Equals("EXIT"))
            {
                cmd = Console.ReadLine();
                if (cmd.IndexOf(CMD_CREATE_TABLE) == 0)
                {
                    cmd = cmd.Substring(cmd.IndexOf(CMD_CREATE_TABLE) + CMD_CREATE_TABLE.Length + 1);
                    // Get substring up to the first space or bracket
                    string table_name = cmd.Substring(0, Math.Min(cmd.IndexOf(" "), cmd.IndexOf("(")));

                    // Get column names and types
                    open_bracket = cmd.IndexOf("(");
                    close_bracket = cmd.LastIndexOf(")");
                    cmd = cmd.Substring(open_bracket + 1, close_bracket - open_bracket - 1);
                    string[] columns = cmd.Split(',');

                    Console.WriteLine("name = " + table_name);
                    List<string> column_names = new List<string>();
                    Dictionary<string, string> column_params = new Dictionary<string, string>();
                    foreach (string s in columns)
                    {
                        string[] column_params_string = s.Trim().Split(' ');
                        column_names.Add(column_params_string[0]);
                        column_params.Add(column_params_string[0], column_params_string[1].ToUpper());
                    }

                    tables.Add(table_name, new Table(column_names, column_params));
                }
                else if (cmd.IndexOf(CMD_INSERT_INTO) == 0)
                {
                    cmd = cmd.Substring(cmd.IndexOf(CMD_INSERT_INTO) + CMD_INSERT_INTO.Length + 1);
                    // Get substring up to the first space or bracket
                    string table_name = cmd.Substring(0, Math.Min(cmd.IndexOf(" "), cmd.IndexOf("(")));

                    Table table = tables[table_name];

                    List<string> columns = new List<string>();
                    List<string> values = new List<string>();

                    if (cmd.IndexOf("(") < cmd.IndexOf(CMD_VALUES))
                    {
                        // Command specifies columns
                        string aux_cmd = cmd.Substring(0, cmd.IndexOf(CMD_VALUES) - 1);
                        open_bracket = aux_cmd.IndexOf("(");
                        close_bracket = aux_cmd.LastIndexOf(")");
                        aux_cmd = aux_cmd.Substring(open_bracket + 1, close_bracket - open_bracket - 1);

                        string[] col_names = aux_cmd.Split(',');
                        foreach (string c in col_names)
                        {
                            columns.Add(c.TrimStart().TrimEnd());
                        }
                    }
                    else
                    {
                        // Command does not specify colums
                        columns = table.GetColumnNames();
                    }

                    cmd = cmd.Substring(cmd.IndexOf(CMD_VALUES) + CMD_VALUES.Length + 1);

                    open_bracket = cmd.IndexOf("(");
                    close_bracket = cmd.LastIndexOf(")");
                    cmd = cmd.Substring(open_bracket + 1, close_bracket - open_bracket - 1);

                    string[] vals = cmd.Split(',');
                    foreach (string v in vals)
                    {
                        values.Add(v.TrimStart().TrimEnd());
                    }

                    table.Insert(columns, values);
                }
                else if (cmd.IndexOf(CMD_SELECT) == 0)
                {
                    string cmd_aux = cmd.Substring(cmd.IndexOf(CMD_FROM));
                    string table_name = cmd_aux.Split(' ')[1];

                    Table table = tables[table_name];

                    cmd = cmd.Substring(cmd.IndexOf(CMD_SELECT) + CMD_SELECT.Length + 1);

                    cmd = cmd.Substring(0, cmd.IndexOf(CMD_FROM) - 1).TrimEnd().TrimStart();
                    List<string> columns = new List<string>();

                    if (cmd.Equals("*"))
                    {
                        columns = table.GetColumnNames();
                    }
                    else
                    {
                        string[] col_names = cmd.Split(',');
                        foreach (string c in col_names)
                        {
                            columns.Add(c.TrimStart().TrimEnd());
                        }
                    }

                    var selected = table.Select(columns);

                    foreach (var line in selected)
                    {
                        Console.Write("| ");
                        foreach (var cell in line)
                        {
                            Console.Write(cell + " | ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}

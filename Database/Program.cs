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

        private readonly Dictionary<string, string> column_name_to_type;
        private readonly List<string> lines;

        public Table(Dictionary<string, string> columns)
        {
            this.column_name_to_type = columns;
        }
    }

    class Program
    {
        private const string CMD_CREATE_TABLE = "CREATE TABLE";
        private const string CMD_QUIT = "QUIT";
        private const string CMD_EXIT = "EXIT";

        static void Main(string[] args)
        {
            Dictionary<string, Table> tables = new Dictionary<string, Table>();

            string cmd = "";
            while (!cmd.Equals("QUIT") && !cmd.Equals("EXIT"))
            {
                cmd = Console.ReadLine();
                if (cmd.IndexOf(CMD_CREATE_TABLE) == 0)
                {
                    cmd = cmd.Substring(cmd.IndexOf(CMD_CREATE_TABLE) + CMD_CREATE_TABLE.Length + 1);
                    string table_name = cmd.Substring(0, Math.Min(cmd.IndexOf(" "), cmd.IndexOf("(")));

                    int open_bracket = cmd.IndexOf("(");
                    int close_bracket = cmd.LastIndexOf(")");
                    cmd = cmd.Substring(open_bracket + 1, close_bracket - open_bracket - 1);
                    string[] columns = cmd.Substring(cmd.LastIndexOf(CMD_CREATE_TABLE) + 1).Split(',');

                    Console.WriteLine("name = " + table_name);
                    Dictionary<string, string> column_params = new Dictionary<string, string>();
                    foreach (string s in columns)
                    {
                        string[] column_params_string = s.Trim().Split(' ');
                        column_params.Add(column_params_string[0], column_params_string[1].ToUpper());
                    }

                    tables.Add(table_name, new Table(column_params));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Database
{
    class Program
    {
        // Command tokens
        private const string CMD_CREATE_TABLE = "CREATE TABLE";
        private const string CMD_INSERT_INTO = "INSERT INTO";
        private const string CMD_VALUES = "VALUES";
        private const string CMD_SELECT = "SELECT";
        private const string CMD_FROM = "FROM";
        private const string CMD_UPDATE = "UPDATE";
        private const string CMD_SET = "SET";
        private const string CMD_DELETE = "DELETE FROM";
        private const string CMD_WHERE = "WHERE";
        private const string CMD_QUIT = "QUIT";
        private const string CMD_EXIT = "EXIT";

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter each (uppercase) command on a single line, without ending it in a semicolon.");
            Console.WriteLine("Write 'QUIT' or 'EXIT' to finish.");
            Console.WriteLine("Examples:");
            Console.WriteLine("CREATE TABLE Students (Surname varchar(255), FirstName varchar(255), Birthday datetime, YearStarted real)");
            Console.WriteLine("INSERT INTO Students VALUES ('Smith', 'Edward', 1998-05-23T14:25:10, 2016)");
            Console.WriteLine("SELECT Surname, FirstName FROM Students");
            Console.WriteLine();

            Dictionary<string, Table> tables = new Dictionary<string, Table>();

            string cmd = "";
            int open_bracket, close_bracket;
            while (!cmd.Equals("QUIT") && !cmd.Equals("EXIT"))
            {
                cmd = Console.ReadLine();
                if (cmd.IndexOf(CMD_CREATE_TABLE) == 0)
                {
                    // Remove command name from string
                    cmd = cmd.Substring(cmd.IndexOf(CMD_CREATE_TABLE) + CMD_CREATE_TABLE.Length + 1);

                    // Get substring up to the first space or bracket
                    string table_name = cmd.Substring(0, Math.Min(cmd.IndexOf(" "), cmd.IndexOf("(")));

                    // Get column names and types from between brackets
                    open_bracket = cmd.IndexOf("(");
                    close_bracket = cmd.LastIndexOf(")");
                    cmd = cmd.Substring(open_bracket + 1, close_bracket - open_bracket - 1);
                    string[] columns = cmd.Split(',');

                    // Create lists for the table constructor
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
                    // Remove command name from string
                    cmd = cmd.Substring(cmd.IndexOf(CMD_INSERT_INTO) + CMD_INSERT_INTO.Length + 1);

                    // Get substring up to the first space or bracket to obtain table name
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

                    // Get values from command
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
                    // Get table name
                    string cmd_aux = cmd.Substring(cmd.IndexOf(CMD_FROM));
                    string table_name = cmd_aux.Split(' ')[1];
                    Table table = tables[table_name];

                    // Get selected columns
                    List<string> columns = new List<string>();

                    cmd = cmd.Substring(cmd.IndexOf(CMD_SELECT) + CMD_SELECT.Length + 1);

                    int where_index = cmd.IndexOf(CMD_WHERE);
                    string where_cond = "";
                    if (where_index >= 0)
                        where_cond = cmd.Substring(where_index + CMD_WHERE.Length + 1);
                    cmd = cmd.Substring(0, cmd.IndexOf(CMD_FROM) - 1).TrimEnd().TrimStart();

                    if (cmd.Equals("*"))  // Select all columns
                    {
                        columns = table.GetColumnNames();
                    }
                    else  // Select specific columns
                    {
                        string[] col_names = cmd.Split(',');
                        foreach (string c in col_names)
                        {
                            columns.Add(c.TrimStart().TrimEnd());
                        }
                    }

                    List<string[]> filtered = null;
                    if (!where_cond.Equals(""))
                    {
                        string[] separators = { ">=", "<=", ">", "<", "=" };
                        string separator = "";
                        string[] separated = null;
                        foreach (var sep in separators)
                        {
                            if (where_cond.Contains(sep))
                            {
                                separated = where_cond.Split(new string[] { sep }, StringSplitOptions.None);
                                separator = sep;
                                break;
                            }
                        }
                        if (separated != null)
                        {
                            filtered = new List<string[]>();
                            if (separator.Contains("="))
                                filtered = filtered.Union(table.WhereEquals(separated[0], separated[1])).ToList();
                            if (separator.Contains(">"))
                                filtered = filtered.Union(table.WhereGreater(separated[0], separated[1])).ToList();
                            if (separator.Contains("<"))
                                filtered = filtered.Union(table.WhereLess(separated[0], separated[1])).ToList();
                        }
                    }
                    var selected = table.Select(columns, filtered);
                    Utils.PrintTable(columns, selected);
                }
                else if (cmd.IndexOf(CMD_UPDATE) == 0)
                {
                    // Get table name
                    string table_name = cmd.Split(' ')[1];
                    Table table = tables[table_name];

                    // Get name-value pairs to be modifies
                    cmd = cmd.Substring(cmd.IndexOf(CMD_SET) + CMD_SET.Length + 1);
                    string[] pairs = cmd.Split(',');
                    List<string> names = new List<string>();
                    List<string> values = new List<string>();

                    foreach (var pair in pairs)
                    {
                        string[] p = pair.Split('=');
                        names.Add(p[0].TrimStart().TrimEnd());
                        values.Add(p[1].TrimStart().TrimEnd());
                    }

                    table.Update(names, values);
                }
                else if (cmd.IndexOf(CMD_DELETE) == 0)
                {
                    // Get table name
                    cmd = cmd.Substring(CMD_DELETE.Length + 1);
                    string table_name = cmd.Split(' ')[0];
                    Table table = tables[table_name];

                    table.Delete();
                }
            }
        }
    }
}

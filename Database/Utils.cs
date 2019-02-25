using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    class Utils
    {
        // Print table using box drawing characters
        public static void PrintTable(List<string> header, List<string[]> lines)
        {
            // Calculate lengths to print table cells centered
            int[] maxCellLen = new int[header.Count];
            int maxLineLen = 0;
            int i = 0;
            foreach (var c in header)
            {
                maxCellLen[i++] = c.Length;
            }
            foreach (var line in lines)
            {
                for (i = 0; i < header.Count; i++)
                {
                    maxCellLen[i] = Math.Max(maxCellLen[i], line[i]?.Length ?? 0);
                }
            }
            foreach (var max in maxCellLen)
            {
                maxLineLen += max;
            }

            // Print top border
            Console.Write("┌");
            foreach (var max in maxCellLen)
            {
                Console.Write(new String('─', max) + (max.Equals(maxCellLen[maxCellLen.Length - 1]) ? "┐" : "┬"));
            }
            Console.WriteLine();

            // Print header
            i = 0;
            Console.Write("│");
            foreach (var c in header)
            {
                int beginningSpaces = (maxCellLen[i] - c.Length) / 2;
                int endSpaces = maxCellLen[i++] - c.Length - beginningSpaces;
                Console.Write(new String(' ', beginningSpaces) + c + new String(' ', endSpaces) + "│");
            }
            Console.WriteLine();

            // Print separator between header and content
            Console.Write("├");
            foreach (var max in maxCellLen)
            {
                Console.Write(new String('─', max) + (max.Equals(maxCellLen[maxCellLen.Length - 1]) ? "┤" : "┼"));
            }
            Console.WriteLine();

            // Print content
            foreach (var line in lines)
            {
                Console.Write("│");
                i = 0;
                foreach (var cell in line)
                {
                    int beginningSpaces = (maxCellLen[i] - (cell?.Length ?? 0)) / 2;
                    int endSpaces = maxCellLen[i++] - (cell?.Length ?? 0) - beginningSpaces;
                    Console.Write(new String(' ', beginningSpaces) + cell + new String(' ', endSpaces) + "│");
                }
                Console.WriteLine();
            }

            // Print bottom border
            Console.Write("└");
            foreach (var max in maxCellLen)
            {
                Console.Write(new String('─', max) + (max.Equals(maxCellLen[maxCellLen.Length - 1]) ? "┘" : "┴"));
            }
            Console.WriteLine();
        }

        // Parse condition and get corresponding lines from the table
        public static List<string[]> Filter(Table table, string condition)
        {
            List<string[]> filtered = null;

            if (!condition.Equals(""))
            {
                // Basic comparison operators
                string[] separators = { ">=", "<=", ">", "<", "=" };
                string separator = "";
                string[] separated = null;
                foreach (var sep in separators)
                {
                    if (condition.Contains(sep))
                    {
                        separated = condition.Split(new string[] { sep }, StringSplitOptions.None);
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

            return filtered;
        }
    }
}

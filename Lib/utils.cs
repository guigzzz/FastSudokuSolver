using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class SudokuUtils
{
    public static int[] LoadFromFile(string filename)
    {
        var parsed = File.ReadAllText(filename)
            .Split('\n')
            .Select(
                row => row.Trim().Split(' ').Select(s => int.Parse(s))
            )
            .SelectMany(i => i)
            .ToArray();

        if (parsed.Length != 81)
            throw new Exception("Error while reading sudoku file, it does not contain 81 numbers");
        return parsed;
    }

    public static int[] ParseLine(string line)
    {
        var grid = new int[81];
        for (var i = 0; i < 81; i++)
            if (line[i] == '.')
                grid[i] = 0;
            else
                grid[i] = int.Parse(line[i].ToString());
        return grid;
    }

    public static List<int[]> LoadFromFileDotNotation(string filename)
    {
        return File.ReadLines(filename)
            .Where(i => i.Count() > 0)
            .Select(ParseLine)
            .ToList();
    }

    public static string GridToString(int[] grid)
    {
        var builder = new StringBuilder();
        builder.AppendLine("---------------------");
        for (var i = 0; i < 9; i++)
        {
            if (i % 3 == 0 && i > 0) builder.AppendLine();
            for (var j = 0; j < 9; j++)
            {
                if (j % 3 == 0 && j > 0) builder.Append("| ");
                builder.Append(string.Format("{0} ", grid[i * 9 + j]));
            }

            builder.AppendLine();
        }

        builder.AppendLine("---------------------");

        return builder.ToString();
    }

    public static bool IsValidSudokuSolution(Sudoku sdku, int[] startingGrid)
    {
        // check colums and rows
        for (var i = 0; i < 9; i++)
        {
            var rowbin = 0;
            var colbin = 0;

            for (var j = 0; j < 9; j++)
            {
                rowbin |= 1 << (sdku.Grid[i * 9 + j] - 1);
                colbin |= 1 << (sdku.Grid[j * 9 + i] - 1);
            }

            if (rowbin != 0x1FF)
            {
                Console.WriteLine("Found invalid row r{1}: {0}", Convert.ToString(rowbin & 0x1FF, 2).PadLeft(9, '0'),
                    i + 1);
                return false;
            }

            if (colbin != 0x1FF)
            {
                Console.WriteLine("Found invalid column c{1}: {0}", Convert.ToString(colbin & 0x1FF, 2).PadLeft(9, '0'),
                    i + 1);
                return false;
            }
        }

        // check houses
        for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                var housebin = 0;
                for (var k1 = i * 3; k1 < (i + 1) * 3; k1++)
                    for (var k2 = j * 3; k2 < (j + 1) * 3; k2++)
                        housebin |= 1 << (sdku.Grid[k1 * 9 + k2] - 1);
                if (housebin != 0x1FF)
                {
                    Console.WriteLine("Found invalid house {0} @ ({1},{2})",
                        Convert.ToString(housebin & 0x1FF, 2).PadLeft(9, '0'), i + 1, j + 1);
                    return false;
                }
            }
        // solution grid has all digits in all columns, rows and houses
        // need to check that the returned grid still has the non-zero values from the 
        // unsolved grid in the same positions

        return startingGrid.Zip(
            sdku.Grid,
            (first, second) => first > 0 ? first == second : true
        ).All(i => i);
    }
}

internal static class SudokuCSV
{
    private static Tuple<int[], int[]> RowToTuple(string[] row)
    {
        return new Tuple<int[], int[]>(
            row[0].Select(i => int.Parse(i.ToString())).ToArray(),
            row[1].Select(i => int.Parse(i.ToString())).ToArray()
        );
    }

    public static List<Tuple<int[], int[]>> ReadFromCSV(
        string filename, int number = 10)
    {
        var parsed = File.ReadLines(filename)
            .Skip(1).Take(number)
            .Select(
                row => RowToTuple(row.Trim().Split(','))
            ).ToList();

        return parsed;
    }
}
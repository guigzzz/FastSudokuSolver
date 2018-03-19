using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

static class SudokuUtils
{
    public static int[] loadFromFile(string file_name)
    {
        var parsed = File.ReadAllText(file_name)
                    .Split('\n')
                    .Select(
                       row => row.Trim().Split(' ').Select(s => Int32.Parse(s))
                    )
                    .SelectMany(i => i)
                    .ToArray();

        if(parsed.Length != 81)
            throw new Exception("Error while reading sudoku file, it does not contain 81 numbers");
        else
            return parsed;
    }

    public static List<int[]> loadFromFileDotNotation(string file_name)
    {
        var parsed = File.ReadLines(file_name)
                        .ToArray();
        List<int[]> grids = new List<int[]>(parsed.Length);

        foreach(var p in parsed)
        {
            var grid = new int[81];
            for(int i = 0; i < 81; i++)
            {
                if(p[i] == '.')
                    grid[i] = 0;
                else
                    grid[i] = Int32.Parse(p[i].ToString());
            }
            grids.Add(grid);
        }

        return grids;
    }

    public static string gridToString(int[] grid)
    {
        var builder = new StringBuilder();
        builder.AppendLine("---------------------");
        for(int i = 0; i < 9; i++)
        {
            if(i % 3 == 0 && i > 0) builder.AppendLine();
            for(int j = 0; j < 9; j++)
            {
                if(j % 3 == 0 && j > 0) builder.Append("| ");
                builder.Append(String.Format("{0} ", grid[i * 9 + j]));
            }
            builder.AppendLine();
        }
        builder.AppendLine("---------------------");

        return builder.ToString();
    }

    public static bool isValidSudokuSolution(Sudoku sdku)
    {
        for(int i = 0; i < 9; i++)
        {
            int rowbin = 0;
            int colbin = 0;

            for(int j = 0; j < 9; j++)
            {
                rowbin |= 1 << (sdku.grid[i * 9 + j] - 1);
                colbin |= 1 << (sdku.grid[j * 9 + i] - 1);
            }
            if(rowbin != 0x1FF) 
            {
                Console.WriteLine(String.Format(
                    "Found invalid row r{1}: {0}", Convert.ToString(rowbin & 0x1FF, 2).PadLeft(9, '0'), i+1
                ));
                return false;
            }
            else if(colbin != 0x1FF)
            {
                Console.WriteLine(String.Format(
                    "Found invalid column c{1}: {0}", Convert.ToString(colbin & 0x1FF, 2).PadLeft(9, '0'), i+1
                ));
                return false;
            }
        }

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                int housebin = 0;
                for(int k1 = i * 3; k1 < (i + 1) * 3; k1++)
                {
                    for(int k2 = j * 3; k2 < (j + 1) * 3; k2++)
                    {
                        housebin |= 1 << (sdku.grid[k1 * 9 + k2] - 1);
                    }
                }
                if(housebin != 0x1FF) 
                {
                    Console.WriteLine(String.Format(
                        "Found invalid house {0} @ ({1},{2})", 
                        Convert.ToString(housebin & 0x1FF, 2).PadLeft(9, '0'), i+1, j+1
                    ));
                    return false;
                }
            }
        }
        return true;
    }
    
}


static class SudokuCSV
{

    private static Tuple<int[], int[]> rowToTuple(string[] row)
    {
        return new Tuple<int[], int[]>(
                row[0].Select(i => Int32.Parse(i.ToString())).ToArray(),
                row[1].Select(i => Int32.Parse(i.ToString())).ToArray()
        );
    }
    public static List<Tuple<int[], int[]>> readFromCSV(
        string filename, int number = 10)
    {
        var parsed = File.ReadLines(filename)
                     .Skip(1).Take(number)
                     .Select(
                         row => rowToTuple(row.Trim().Split(','))
                     ).ToList();

        return parsed;
    }

}

using System;
using System.Linq;
using System.Collections.Generic;

class Sudoku
{
    public int[] grid = new int[9 * 9];
    int[] binarytaken = new int[9];
    public int[] BinaryTaken { get => binarytaken; }

    int[] candidates = new int[9 * 9];
    
    public int[] Candidates { get => candidates; set => candidates = value; }

    public void loadFromFile(string file_name)
    {
        var parsed = System.IO.File.ReadAllText(file_name)
                    .Split('\n')
                    .Select(
                       row => row.Split(' ').Select(s => Int32.Parse(s))
                    )
                    .SelectMany(i => i)
                    .ToArray();

        if(parsed.Length != 81)
            Console.WriteLine("Error while reading sudoku file, it does not contain 81 numbers");
        else
        {
            grid = parsed;
            initialiseBinaryRep();
        }
    }

    private void initialiseBinaryRep()
    {
        //build binary representation
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(grid[i * 9 + j] != 0)
                {
                    BinaryTaken[i] |= 1 << (grid[i * 9 + j] - 1);
                    BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (grid[i * 9 + j] + 18 - 1);
                }

                if(grid[j * 9 + i] != 0)
                    BinaryTaken[i] |= 1 << (grid[j * 9 + i] + 9 - 1);
            }
        }
    }

    public bool isSolved()
    {
        return BinaryTaken.All(i => i == 0x7FFFFFF);
    }

    public void print()
    {
        Console.WriteLine("---------------------");
        for(int i = 0; i < 9; i++)
        {
            if(i % 3 == 0 && i > 0) Console.WriteLine();
            for(int j = 0; j < 9; j++)
            {
                if(j % 3 == 0 && j > 0) Console.Write("| ");
                Console.Write(String.Format("{0} ", grid[i * 9 + j]));
            }
            Console.WriteLine();
        }
        Console.WriteLine("---------------------");
        // Console.WriteLine();
        // Console.WriteLine("-------------------- Bin --------------------");
        // foreach(var av in BinaryTaken)
        // {
        //     var s = Convert.ToString(av, 2).PadLeft(27, '0');
        //     var printstr = String.Format("Subquare: {0}, Column: {1}, Row: {2}", 
        //             s.Substring(0, 9), s.Substring(9, 9), s.Substring(18, 9));

        //     Console.WriteLine(printstr);
        // }
        // Console.WriteLine("---------------------------------------------");

        // for(int i = 0; i < 9; i++)
        // {
        //     if(i % 3 == 0 && i > 0) Console.WriteLine();
        //     for(int j = 0; j < 9; j++)
        //     {
        //         if(j % 3 == 0 && j > 0) Console.Write("| ");
        //         if(grid[i * 9 + j] == 0)
        //             Console.Write(Convert.ToString(Candidates[i * 9 + j], 2).PadLeft(9, '0') + " ");
        //         else
        //             Console.Write("--------- ");
        //     }
        //     Console.WriteLine();
        // }
    }

    public int this[int i, int j]
    {
        get => grid[i * 9 + j];
        set{ grid[i * 9 + j] = value; }
    }
}

class SudokuSolver
{
    public Sudoku solve(Sudoku sdku)
    {
        while(!sdku.isSolved())
        {
            sdku = hiddensolve(nakedsolve(sdku));
        }

        return sdku;
    }

    private Sudoku hiddensolve(Sudoku sdku)
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(sdku.grid[i * 9 + j] == 0)
                {
                    int hidden = checkForHiddenSingle(sdku, i, j);
                    if(hidden > 0)
                    {
                        sdku.grid[i * 9 + j] = hidden;
                        sdku.BinaryTaken[i] |= 1 << (hidden - 1);
                        sdku.BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (hidden + 18 - 1);
                        sdku.BinaryTaken[j] |= 1 << (hidden + 9 - 1);
                        return sdku;
                    }
                }
            }
        }
        return sdku;
    }

    private int checkForHiddenSingle(Sudoku sudoku, int i, int j)
    {
        int rowhidden = sudoku.Candidates[i * 9 + j];
        int colhidden = rowhidden;
        int househidden = rowhidden;

        // row && col
        for(int k = 0; k < 9; k++)
        {
            rowhidden &= (sudoku.grid[i * 9 + k] == 0 && k != j) ? ~sudoku.Candidates[i * 9 + k] : ~0;
            colhidden &= (sudoku.grid[k * 9 + j] == 0 && k != i) ? ~sudoku.Candidates[k * 9 + j] : ~0;
        }

        rowhidden &= 0x1FF;
        colhidden &= 0x1FF;

        if((rowhidden & rowhidden-1) == 0 && rowhidden > 0)
            return (int)Math.Log(rowhidden, 2) + 1;

        if((colhidden & colhidden-1) == 0 && colhidden > 0)
            return (int)Math.Log(colhidden, 2) + 1;

        //house
        // for(int k1 = i / 3 * 3; k1 < (i + 1) / 3 * 3; k1++)
        // {
        //     for(int k2 = j / 3 * 3; k2 < (j + 1) / 3 * 3; k2++)
        //         househidden &= (sudoku.grid[k1 * 9 + k2] == 0 && k1 != i && k2 != j) 
        //                         ? ~sudoku.Candidates[k1 + k2] : ~0;
        // }

        // househidden &= 0x1FF;

        // if((househidden & househidden-1) == 0 && househidden > 0)
        //     return (int)Math.Log(househidden, 2) + 1;

        // no hidden single
        return 0;
    }

    private Sudoku nakedsolve(Sudoku sdku)
    {
        bool change = true;
        // int min_guesses = 9;

        while(change)
        {
            change = false;
            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    if(sdku[i, j] == 0)
                    {
                        int possibleDigits = getPossibleDigits(sdku, i, j);
                        sdku.Candidates[i * 9 + j] = possibleDigits;

                        // if only single digit possible in (i, j)
                        if((possibleDigits & possibleDigits-1) == 0)
                        {
                            int digit = (int)Math.Log(possibleDigits, 2) + 1;
                            sdku[i, j] = digit;
                            sdku.BinaryTaken[i] |= 1 << (digit - 1);
                            sdku.BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (digit + 18 - 1);
                            sdku.BinaryTaken[j] |= 1 << (digit + 9 - 1);
                            change = true;
                        }
                    }
                }
            }
        }
        return sdku;
    }

    private int getPossibleDigits(Sudoku sudoku, int i, int j)
    {
        int possibleDigits = ~(
                sudoku.BinaryTaken[i]
                | sudoku.BinaryTaken[j] >> 9
                | sudoku.BinaryTaken[i / 3 * 3 + j / 3] >> 18
            ) & 0x1FF;
        return possibleDigits;
    }

    private List<int> digits_from_bits(int bits)
    {
        var ret = new List<int>(9);
        for(int i = 1; i < 10; i++) 
        {
            if((bits & 1) == 1) ret.Add(i);
            bits >>= 1;
        }
        return ret;
    }
}


static class SolutionChecker
{
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
            if(rowbin != 0x1FF || colbin != 0x1FF) return false;
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
                if(housebin != 0x1FF) return false;
            }
        }
        return true;
    }

    
}
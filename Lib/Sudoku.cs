using System;
using System.Numerics;

public struct Sudoku
{
    public int[] Grid = new int[9 * 9];

    public int[] BinaryTaken { get; } = new int[9];

    public int[] Candidates { get; set; } = new int[9 * 9];

    public Sudoku(int[] grid)
    {
        Array.Copy(grid, Grid, 81);
        UpdateBinaryRep();
    }


    public Sudoku(int[] grid, int[] binaryTaken)
    {
        Array.Copy(grid, Grid, 81);
        Array.Copy(binaryTaken, BinaryTaken, 9);
    }

    private readonly void UpdateBinaryRep()
    {
        //build binary representation
        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
            {
                if (Grid[i * 9 + j] > 0)
                {
                    BinaryTaken[i] |= 1 << (Grid[i * 9 + j] - 1);
                    BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (Grid[i * 9 + j] + 18 - 1);
                }

                if (Grid[j * 9 + i] > 0)
                    BinaryTaken[i] |= 1 << (Grid[j * 9 + i] + 9 - 1);
            }
    }

    public readonly void SetDigit(int i, int j, int digit)
    {
        Grid[i * 9 + j] = digit;
        BinaryTaken[i] |= 1 << (digit - 1);
        BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (digit + 18 - 1);
        BinaryTaken[j] |= 1 << (digit + 9 - 1);
    }

    public readonly bool IsSolved()
    {
        for (int i = 0; i < 9; i++)
        {
            if (BinaryTaken[i] != 0x7FFFFFF)
            {
                return false;
            }
        }
        return true;
    }

    public override readonly string ToString()
    {
        return SudokuUtils.GridToString(Grid);
    }
}

public class SudokuSolver
{
    public Sudoku? Solve(Sudoku sudoku)
    {
        while (!sudoku.IsSolved())
        {
            var hasChanged = TrySolve(sudoku);

            if (hasChanged == null) return null;

            if (hasChanged == false) // need to guess
            {
                var guessloc = SelectBestGuessLocation(sudoku);

                var bits = sudoku.Candidates[guessloc];

                for (var d = 1; d < 10; d++)
                {
                    var bitset = (bits & 1) == 1;
                    bits >>= 1;

                    if (!bitset)
                    {
                        continue;
                    }

                    var newSudoku = new Sudoku(sudoku.Grid, sudoku.BinaryTaken);
                    newSudoku.SetDigit(guessloc / 9, guessloc % 9, d);

                    var res = Solve(newSudoku);
                    if (res != null) return res;
                }

                return null;
            }
        }

        return sudoku;
    }

    private static bool? TrySolve(Sudoku sdku)
    {
        // single naked
        var nakedpasschange = NakedSinglePass(sdku);

        if (nakedpasschange == null) return null;
        return nakedpasschange == false ? HiddenSinglePass(sdku) : true;
    }

    private static bool? NakedSinglePass(Sudoku sdku)
    {
        var change = false;
        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
                if (sdku.Grid[i * 9 + j] == 0)
                {
                    var possibleDigits = GetPossibleDigits(sdku, i, j);

                    // invalid grid
                    if (possibleDigits == 0) return null;

                    // if only single digit possible in (i, j)
                    if ((possibleDigits & (possibleDigits - 1)) == 0)
                    {
                        var digit = (int)Math.Log(possibleDigits, 2) + 1;
                        sdku.SetDigit(i, j, digit);
                        change = true;
                    }
                }

        return change;
    }

    private static bool HiddenSinglePass(Sudoku sdku)
    {
        var change = false;
        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
                sdku.Candidates[i * 9 + j] = sdku.Grid[i * 9 + j] == 0
                    ? GetPossibleDigits(sdku, i, j)
                    : 0;

        for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
                if (sdku.Grid[i * 9 + j] == 0)
                {
                    var hidden = CheckForHiddenSingle(sdku, i, j);
                    if (hidden > 0)
                    {
                        sdku.SetDigit(i, j, hidden);
                        change = true;
                    }
                }

        return change;
    }

    private static int CheckForHiddenSingle(Sudoku sudoku, int i, int j)
    {
        var rowhidden = sudoku.Candidates[i * 9 + j];
        var colhidden = rowhidden;
        var househidden = rowhidden;

        // row && col
        for (var k = 0; k < 9; k++)
        {
            rowhidden &= k != j ? ~sudoku.Candidates[i * 9 + k] : 0x1FF;
            colhidden &= k != i ? ~sudoku.Candidates[k * 9 + j] : 0x1FF;
        }

        if ((rowhidden & (rowhidden - 1)) == 0 && rowhidden > 0)
            return (int)Math.Log(rowhidden, 2) + 1;

        if ((colhidden & (colhidden - 1)) == 0 && colhidden > 0)
            return (int)Math.Log(colhidden, 2) + 1;

        //house
        // for(int k1 = i / 3 * 3; k1 < (i + 1) / 3 * 3; k1++)
        // {
        //     for(int k2 = j / 3 * 3; k2 < (j + 1) / 3 * 3; k2++)
        //     {
        //         househidden &= (k1 != i || k2 != j) 
        //                         ? ~sudoku.Candidates[k1 * 9 + k2] : ~0;
        //     }
        // }

        // househidden &= 0x1FF;

        // if((househidden & househidden-1) == 0 && househidden > 0)
        //     return (int)Math.Log(househidden, 2) + 1;

        // no hidden single
        return 0;
    }

    private static int SelectBestGuessLocation(Sudoku sudoku)
    {
        var minimum = 9;
        var minindex = 0;

        for (var i = 0; i < 81; i++)
        {
            if (sudoku.Grid[i] > 0) continue;
            // hamming weight == number of 1 bits
            var weight = BitOperations.PopCount((uint)sudoku.Candidates[i]);
            if (weight == 2) return i;

            if (weight < minimum)
            {
                minimum = weight;
                minindex = i;
            }
        }

        return minindex;
    }

    private static int GetPossibleDigits(Sudoku sudoku, int i, int j)
    {
        var possibleDigits = ~(
            sudoku.BinaryTaken[i]
            | (sudoku.BinaryTaken[j] >> 9)
            | (sudoku.BinaryTaken[i / 3 * 3 + j / 3] >> 18)
        ) & 0x1FF;
        return possibleDigits;
    }
}
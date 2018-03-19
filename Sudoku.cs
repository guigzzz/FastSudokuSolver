using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


class Sudoku
{
    public int[] grid = new int[9 * 9];
    int[] binarytaken = new int[9];
    public int[] BinaryTaken { get => binarytaken; }

    int[] candidates = new int[9 * 9];
    
    public int[] Candidates { get => candidates; set => candidates = value; }


    public Sudoku(int[] grid)
    {
        Array.Copy(grid, this.grid, 81);
        updateBinaryRep();
    }

    public void updateBinaryRep()
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

    public override string ToString()
    {
        return SudokuUtils.gridToString(grid);
    }
    public void printPossibleDigits()
    {
        Console.WriteLine(new String('-', 93));
        for(int i = 0; i < 9; i++)
        {
            if(i % 3 == 0 && i > 0) Console.WriteLine();
            for(int j = 0; j < 9; j++)
            {
                if(j % 3 == 0 && j > 0) Console.Write("| ");

                int checker = 0x100;
                if(candidates[i * 9 + j] > 0)
                {
                    for(int k = 9; k > 0; k--)
                    {
                        if((candidates[i * 9 + j] & checker) == checker) Console.Write(k.ToString());
                        else Console.Write("-");
                        checker >>= 1;
                    }
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("    -     ");
                }

            }
            Console.WriteLine();
        }
        Console.WriteLine(new String('-', 93));
    }

    public int this[int i, int j]
    {
        get => grid[i * 9 + j];
        set{ grid[i * 9 + j] = value; }
    }
}

class SudokuSolver
{
    public Sudoku solve(int[] sdku, int depth)
    {
        var sudoku = new Sudoku(sdku);

        while(!sudoku.isSolved())
        {
            bool? hasChanged = trysolve(sudoku);
            // Console.WriteLine(hasChanged == true);
            
            if(hasChanged == null) return null;
            else if(hasChanged == false) // need to guess
            {
                int guessloc = selectBestGuessLocation(sudoku);
                List<int> digits = digits_from_bits(sudoku.Candidates[guessloc]);
                if(guessloc > 0) Console.WriteLine(guessloc + " " + depth);
                foreach(var d in digits)
                {
                    // Console.Write(d);
                    sudoku.grid[guessloc] = d;
                    Sudoku res = solve(sudoku.grid, depth + 1);
                    Console.WriteLine(sudoku.ToString());
                    if(res != null)
                    {
                        sudoku = res;
                        break;                        
                    }
                }
            }
        }
        return sudoku;
    }

    private bool? trysolve(Sudoku sdku)
    {
        bool change = false;

        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(sdku.grid[i * 9 + j] == 0)
                {
                    int possibleDigits = getPossibleDigits(sdku, i, j);

                    // invalid grid
                    if(possibleDigits == 0) return null;

                    // if only single digit possible in (i, j)
                    if((possibleDigits & possibleDigits-1) == 0)
                    {
                        int digit = (int)Math.Log(possibleDigits, 2) + 1;
                        sdku.grid[i * 9 + j] = digit;
                        sdku.BinaryTaken[i] |= 1 << (digit - 1);
                        sdku.BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (digit + 18 - 1);
                        sdku.BinaryTaken[j] |= 1 << (digit + 9 - 1);
                        change = true;
                    }
                }
            }
        }
        // sdku.updateBinaryRep();

        if(!change)
        {
            // update candidates
            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                    sdku.Candidates[i * 9 + j] = (sdku.grid[i * 9 + j] == 0) 
                                                ? getPossibleDigits(sdku, i, j) : 0;
            }

            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {

                    int hidden = checkForHiddenSingle(sdku, i, j);
                    if(hidden > 0)
                    {
                        // Console.WriteLine(String.Format(
                        //     "Hidden {0} @ r{1}c{2}", hidden, i+1, j+1 
                        // ));
                        sdku.grid[i * 9 + j] = hidden;
                        change = true;
                    }
                }
            }
            sdku.updateBinaryRep();
        }
        return change;
    }

    private int checkForHiddenSingle(Sudoku sudoku, int i, int j)
    {
        int rowhidden = sudoku.Candidates[i * 9 + j];
        int colhidden = rowhidden;
        int househidden = rowhidden;

        // row && col
        for(int k = 0; k < 9; k++)
        {
            rowhidden &= (k != j) ? ~sudoku.Candidates[i * 9 + k] : ~0;
            colhidden &= (k != i) ? ~sudoku.Candidates[k * 9 + j] : ~0;
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

    private int selectBestGuessLocation(Sudoku sudoku)
    {
        int minimum = 9;
        int minindex = 0;

        for(int i = 0; i < 81; i++)
        {
            int weight = HammingWeight(sudoku[i / 9, i % 9]);
            if(weight < minimum && sudoku.grid[i] == 0)
            {
                minimum = weight;
                minindex = i;
            }
        }
        return minindex;
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

    public int HammingWeight(int value)
    {
        value = value - ((value >> 1) & 0x55555555);
        value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
        return (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }
}

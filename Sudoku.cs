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
                if(grid[i * 9 + j] > 0)
                {
                    BinaryTaken[i] |= 1 << (grid[i * 9 + j] - 1);
                    BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (grid[i * 9 + j] + 18 - 1);
                }

                if(grid[j * 9 + i] > 0)
                    BinaryTaken[i] |= 1 << (grid[j * 9 + i] + 9 - 1);
            }
        }
    }

    public bool isSolved() => BinaryTaken.All(i => i == 0x7FFFFFF);

    public override string ToString() => SudokuUtils.gridToString(grid);

    private int getPossibleDigits(Sudoku sudoku, int i, int j)
    {
        int possibleDigits = ~(
                sudoku.BinaryTaken[i]
                | sudoku.BinaryTaken[j] >> 9
                | sudoku.BinaryTaken[i / 3 * 3 + j / 3] >> 18
            ) & 0x1FF;
        return possibleDigits;
    }

    public void printPossibleDigits()
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
                Candidates[i * 9 + j] = (grid[i * 9 + j] == 0) 
                                            ? getPossibleDigits(this, i, j) : 0;
        }
        var builder = new StringBuilder();
        builder.AppendLine(new String('-', 93));
        for(int i = 0; i < 9; i++)
        {
            if(i % 3 == 0 && i > 0) builder.AppendLine();
            for(int j = 0; j < 9; j++)
            {
                if(j % 3 == 0 && j > 0) builder.Append("| ");

                int checker = 0x100;
                if(candidates[i * 9 + j] > 0)
                {
                    for(int k = 9; k > 0; k--)
                    {
                        if((candidates[i * 9 + j] & checker) == checker) builder.Append(k.ToString());
                        else builder.Append("-");
                        checker >>= 1;
                    }
                    builder.Append(" ");
                }
                else
                    builder.Append(String.Format("    {0}     ", grid[i * 9 + j]));
            }
            builder.AppendLine();
        }
        builder.AppendLine(new String('-', 93));

        Console.WriteLine(builder);
    }
}

class SudokuSolver
{

    public Sudoku solve(int[] sdku)
    {
        var sudoku = new Sudoku(sdku);

        while(!sudoku.isSolved())
        {
            bool? hasChanged = trysolve(sudoku);
            
            if(hasChanged == null) return null;
            else if(hasChanged == false) // need to guess
            {
                int guessloc = selectBestGuessLocation(sudoku);
                List<int> digits = digitsFromBits(sudoku.Candidates[guessloc]);

                foreach(var d in digits)
                {
                    sudoku.grid[guessloc] = d;
                    Sudoku res = solve(sudoku.grid);
                    if(res != null) return res;
                }
                return null;
            }
        }
        return sudoku;
    }

    private bool? trysolve(Sudoku sdku)
    {
        bool change = false;
        sdku.updateBinaryRep();
        // single naked
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(sdku.grid[i * 9 + j] == 0)
                {
                    int possibleDigits = getPossibleDigits(sdku, i, j);

                    // invalid grid
                    if(possibleDigits == 0){
                        // Console.WriteLine(String.Format("no possible digits @ r{0}c{1}", i+1, j+1));
                        return null;
                    } 

                    // if only single digit possible in (i, j)
                    if((possibleDigits & possibleDigits-1) == 0)
                    {
                        int digit = (int)Math.Log(possibleDigits, 2) + 1;
                        // if(digit == 0)
                        //     Console.WriteLine(String.Format(
                        //         "Naked {0} @ r{1}c{2}", digit, i+1, j+1 
                        //     ));
                        sdku.grid[i * 9 + j] = digit;
                        sdku.BinaryTaken[i] |= 1 << (digit - 1);
                        sdku.BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (digit + 18 - 1);
                        sdku.BinaryTaken[j] |= 1 << (digit + 9 - 1);
                        change = true;
                    }
                }
            }
        }
        // if(change) Console.WriteLine();
        // else Console.WriteLine("no naked");
        // sdku.updateBinaryRep();

        if(!change)
        {
            // hidden naked

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
                        // if(hidden == 0)
                        //     Console.WriteLine(String.Format(
                        //         "Hidden {0} @ r{1}c{2}", hidden, i+1, j+1 
                        //     ));
                        sdku.grid[i * 9 + j] = hidden;
                        change = true;
                        sdku.BinaryTaken[i] |= 1 << (hidden - 1);
                        sdku.BinaryTaken[i / 3 * 3 + j / 3] |= 1 << (hidden + 18 - 1);
                        sdku.BinaryTaken[j] |= 1 << (hidden + 9 - 1);
                    }
                }
            }
            // if(change) Console.WriteLine();
            // else Console.WriteLine("no hidden");
            // sdku.updateBinaryRep();
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
            rowhidden &= (k != j) ? ~sudoku.Candidates[i * 9 + k] : 0x1FF;
            colhidden &= (k != i) ? ~sudoku.Candidates[k * 9 + j] : 0x1FF;
        }

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
            // hamming weight == number of 1 bits
            int weight = HammingWeight(sudoku.Candidates[i]); 
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

    private List<int> digitsFromBits(int bits)
    {
        var ret = new List<int>(9);
        for(int i = 1; i < 10; i++) 
        {
            if((bits & 1) == 1) ret.Add(i);
            bits >>= 1;
        }
        return ret;
    }

    private int HammingWeight(int value)
    {
        value = value - ((value >> 1) & 0x55555555);
        value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
        return (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }

}

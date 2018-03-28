using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class UnitTest
{
    private List<int[]> sudokus = 
        @".0............3.85..1.2.......5.7.....4...1...9.......5......73..2.1........4...9
        .......12........3..23..4....18....5.6..7.8.......9.....85.....9...4.5..47...6...
        .2..5.7..4..1....68....3...2....8..3.4..2.5.....6...1...2.9.....9......57.4...9..
        ........3..1..56...9..4..7......9.5.7.......8.5.4.2....8..2..9...35..1..6........
        12.3....435....1....4........54..2..6...7.........8.9...31..5.......9.7.....6...8
        1.......2.9.4...5...6...7...5.9.3.......7.......85..4.7.....6...3...9.8...2.....1
        .......39.....1..5..3.5.8....8.9...6.7...2...1..4.......9.8..5..2....6..4..7.....
        12.3.....4.....3....3.5......42..5......8...9.6...5.7...15..2......9..6......7..8
        ..3..6.8....1..2......7...4..9..8.6..3..4...1.7.2.....3....5.....5...6..98.....5.
        1.......9..67...2..8....4......75.3...5..2....6.3......9....8..6...4...1..25...6.
        ..9...4...7.3...2.8...6...71..8....6....1..7.....56...3....5..1.4.....9...2...7..
        ....9..5..1.....3...23..7....45...7.8.....2.......64...9..1.....8..6......54....7
        4...3.......6..8..........1....5..9..8....6...7.2........1.27..5.3....4.9........
        7.8...3.....2.1...5.........4.....263...8.......1...9..9.6....4....7.5...........
        3.7.4...........918........4.....7.....16.......25..........38..9....5...2.6.....
        ........8..3...4...9..2..6.....79.......612...6.5.2.7...8...5...1.....2.4.5.....3
        .......1.4.........2...........5.4.7..8...3....1.9....3..4..2...5.1........8.6...
        .......12....35......6...7.7.....3.....4..8..1...........12.....8.....4..5....6..
        1.......2.9.4...5...6...7...5.3.4.......6........58.4...2...6...3...9.8.7.......1
        .....1.2.3...4.5.....6....7..2.....1.8..9..3.4.....8..5....2....9..3.4....67....."
        .Split('\n').Select(i => SudokuUtils.parseLine(i.Trim())).ToList();


    [Fact]
    public void HardSudokus()
    {
        SudokuSolver solver = new SudokuSolver();

        foreach(var s in sudokus)
        {
            Sudoku solved = solver.solve(s);
            Assert.True(SudokuUtils.isValidSudokuSolution(solved, s));
        }
    }

    int[] solvedsudoku = SudokuUtils.parseLine(
        "839465712146782953752391486391824675564173829287659341628537194913248567475916238");

    [Fact]
    public void TestValidSolutionChecker()
    {
        Assert.True(SudokuUtils.isValidSudokuSolution(new Sudoku(solvedsudoku), solvedsudoku));
    }

    int[] invalidsudoku = SudokuUtils.parseLine(
        "..9.7...5..21..9..1...28....7...5..1..851.....5....3.......3..68........21.....87"
    );

    [Fact]
    public void TestInvalidGrid()
    {
        SudokuSolver solver = new SudokuSolver();
        Sudoku s = solver.solve(invalidsudoku);
        Assert.Null(s);
    }

}

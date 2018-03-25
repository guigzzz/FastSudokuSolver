# FastSudokuSolver
An attempt at writing a relatively fast sudoku solver

- The base algorithm solves the sudoku puzzle through brute-force depth first search.
- In order to prune the search tree as much as possible, this brute force approach is augmented with both Naked Single and Hidden Single search.


## Usage

### Loading Data:

To load a sudoku grid using the standard dot notation (i.e. 81-character row-major strings where empty slots are represented by dots). This returns a List of integer arrays where each array corresponds to a sudoku on a certain line of the file:
```csharp
List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("dotnotationsudokus.txt");
```

Or using an alternative notation where the sudoku is represented as a 9x9 block of numbers (with only a single grid per file):
```csharp
int[] sudoku = SudokuUtils.loadFromFile("sudoku.txt");
```

For example:

```
8 0 0 0 0 0 0 0 0
0 0 3 6 0 0 0 0 0
0 7 0 0 9 0 2 0 0
0 5 0 0 0 7 0 0 0
0 0 0 0 4 5 7 0 0
0 0 0 1 0 0 0 3 0
0 0 1 0 0 0 0 6 8
0 0 8 5 0 0 0 1 0
0 9 0 0 0 0 4 0 0
```

### Executing the Solver:

```cs
SudokuSolver solver = new SudokuSolver();
Sudoku solvedSudoku = solver.solve(sudoku);
```

### Checking the solution:

The following shows how to check the validity of the solution output by the solver (for testing purposes). This is achieved by making sure that each row, column and house has exactly one of every number 1 through 9.

```cs
if(!SudokuUtils.isValidSudokuSolution(solvedSudoku))
    throw new Exception(String.Format(
        "Failed for:\n {0}\n Got:\n{1}\n", 
        SudokuUtils.gridToString(sudoku),
        SudokuUtils.gridToString(solvedSudoku.grid)
        ));
```


## Getting data

Sample sudoku grids can be found [here](http://magictour.free.fr/sudoku.htm). In fact, the `JSolveBenchmark` and `sudoku1465Benchmark` functions in `Benchmarks.cs` rely respectively on [this file](https://github.com/attractivechaos/plb/blob/master/sudoku/sudoku.txt) and [this file](http://magictour.free.fr/top1465). 


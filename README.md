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


## How it works

In order to be efficient, this program relies on a an implicit binary representation of the rows, columns and houses in order to check the presence of a naked or hidden single. The digits in each row, column and house can be reprensented using only 9-bits. This means that the information for which digits are present in each row, column and house can fit in 9 32-bit integers (where only 3*9=27 bits in each int will be used). 

Checking for naked singles in any given slot is done by OR-ing the 9-bit representations for the associated row, column and house and checking if there is only a single 0 bit.

Checking for hidden singles can be done by first computing the potential digits for each slot in the sudoku (represented yet again with 9 bits) and AND-ing the 9-bit representations accross any given row, column or house. If there is only a single set bit, then there is a hidden single in that particular row, column or house. In practice, the code only checks for hidden singles overs rows and columns as checking over houses also did not bring a measureable increase in solving speed.

Checking for hidden singles is clearly slower than checking for naked singles. Due to this, the program will prioritise checking for naked singles, and will fallback to hidden singles only if no naked singles are found. Similarly, the program will fall-back to a brute force search if no naked or hidden singles are found. 

## Results

This program solves the [top10 hardest](http://www.aisudoku.com/en/AIwME.html) in 2.9 +/- 0.03 milliseconds. See `top10sudokus` in `Benchmark.cs` for code.

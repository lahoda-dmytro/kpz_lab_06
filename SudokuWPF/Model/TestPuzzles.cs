using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public static class TestPuzzles
    {
        // Легкий рівень
        public static CellClass[,] GetEasyPuzzle()
        {
            var cells = new CellClass[9, 9];
            int[,] puzzle = new int[,]
            {
                {5,3,0,0,7,0,0,0,0},
                {6,0,0,1,9,5,0,0,0},
                {0,9,8,0,0,0,0,6,0},
                {8,0,0,0,6,0,0,0,3},
                {4,0,0,8,0,3,0,0,1},
                {7,0,0,0,2,0,0,0,6},
                {0,6,0,0,0,0,2,8,0},
                {0,0,0,4,1,9,0,0,5},
                {0,0,0,0,8,0,0,7,9}
            };

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col] = new CellClass(col, row);
                    if (puzzle[row, col] != 0)
                    {
                        cells[row, col].Answer = puzzle[row, col];
                        cells[row, col].CellState = CellStateEnum.Answer;
                    }
                    else
                    {
                        cells[row, col].CellState = CellStateEnum.Blank;
                    }
                }
            }
            return cells;
        }

        // Середній рівень
        public static CellClass[,] GetMediumPuzzle()
        {
            var cells = new CellClass[9, 9];
            int[,] puzzle = new int[,]
            {
                {0,0,0,2,6,0,7,0,1},
                {6,8,0,0,7,0,0,9,0},
                {1,9,0,0,0,4,5,0,0},
                {8,2,0,1,0,0,0,4,0},
                {0,0,4,6,0,2,9,0,0},
                {0,5,0,0,0,3,0,2,8},
                {0,0,9,3,0,0,0,7,4},
                {0,4,0,0,5,0,0,3,6},
                {7,0,3,0,1,8,0,0,0}
            };

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col] = new CellClass(col, row);
                    if (puzzle[row, col] != 0)
                    {
                        cells[row, col].Answer = puzzle[row, col];
                        cells[row, col].CellState = CellStateEnum.Answer;
                    }
                    else
                    {
                        cells[row, col].CellState = CellStateEnum.Blank;
                    }
                }
            }
            return cells;
        }

        // Важкий рівень
        public static CellClass[,] GetHardPuzzle()
        {
            var cells = new CellClass[9, 9];
            int[,] puzzle = new int[,]
            {
                {0,0,0,6,0,0,4,0,0},
                {7,0,0,0,0,3,6,0,0},
                {0,0,0,0,9,1,0,8,0},
                {0,0,0,0,0,0,0,0,0},
                {0,5,0,1,8,0,0,0,3},
                {0,0,0,3,0,6,0,4,5},
                {0,4,0,2,0,0,0,6,0},
                {9,0,3,0,0,0,0,0,0},
                {0,2,0,0,0,0,1,0,0}
            };

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col] = new CellClass(col, row);
                    if (puzzle[row, col] != 0)
                    {
                        cells[row, col].Answer = puzzle[row, col];
                        cells[row, col].CellState = CellStateEnum.Answer;
                    }
                    else
                    {
                        cells[row, col].CellState = CellStateEnum.Blank;
                    }
                }
            }
            return cells;
        }

        public static CellClass[,] GetPuzzleByLevel(DifficultyLevels level)
        {
            switch (level)
            {
                case DifficultyLevels.Easy:
                    return GetEasyPuzzle();
                case DifficultyLevels.Medium:
                    return GetMediumPuzzle();
                case DifficultyLevels.Hard:
                    return GetHardPuzzle();
                default:
                    return GetEasyPuzzle();
            }
        }
    }
} 
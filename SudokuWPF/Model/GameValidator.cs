using System;
using System.Collections.Generic;
using System.Linq;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class GameValidator
    {
        private readonly GameModel _gameModel;
        private readonly List<CellClass> _conflictingCells;

        public GameValidator(GameModel gameModel)
        {
            _gameModel = gameModel;
            _conflictingCells = new List<CellClass>();
        }

        public bool ValidateCell(int col, int row)
        {
            _conflictingCells.Clear();
            var cell = _gameModel[col, row];
            if (cell == null || cell.CellState == CellStateEnum.Answer) return true;

            var value = cell.UserAnswer;
            if (value == 0) return true;

            // Перевірка рядка
            for (int i = 0; i < 9; i++)
            {
                if (i != col && _gameModel[i, row]?.UserAnswer == value)
                {
                    _conflictingCells.Add(_gameModel[i, row]);
                }
            }

            // Перевірка стовпця
            for (int i = 0; i < 9; i++)
            {
                if (i != row && _gameModel[col, i]?.UserAnswer == value)
                {
                    _conflictingCells.Add(_gameModel[col, i]);
                }
            }

            // Перевірка квадрата 3x3
            var region = cell.Region;
            var regionCells = _gameModel.RegionCells(region);
            foreach (var regionCell in regionCells)
            {
                if (regionCell != cell && regionCell.UserAnswer == value)
                {
                    _conflictingCells.Add(regionCell);
                }
            }

            return _conflictingCells.Count == 0;
        }

        public List<CellClass> GetConflictingCells()
        {
            return _conflictingCells;
        }

        public bool IsGameValid()
        {
            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (!ValidateCell(col, row))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<CellClass> GetHint()
        {
            var emptyCells = _gameModel.CellList
                .Where(c => c.CellState == CellStateEnum.Blank)
                .ToList();

            if (!emptyCells.Any()) return null;

            // Знаходимо клітинку з найменшою кількістю можливих значень
            var cellWithLeastOptions = emptyCells
                .OrderBy(c => c.Notes.Count(n => n.State))
                .First();

            var possibleValues = cellWithLeastOptions.Notes
                .Select((note, index) => new { Value = index + 1, IsPossible = note.State })
                .Where(n => n.IsPossible)
                .Select(n => n.Value)
                .ToList();

            if (!possibleValues.Any()) return null;

            // Повертаємо клітинку та можливі значення
            return new List<CellClass> { cellWithLeastOptions };
        }
    }
} 
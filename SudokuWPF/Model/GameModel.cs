using System;
using System.Collections.Generic;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel;

namespace SudokuWPF.Model
{
    internal class GameModel
    {
        private readonly GameValidator _validator;

        internal GameModel(CellClass[,] cells)
        {
            InitClass(cells); 
            _validator = new GameValidator(this);
        }

        private CellClass[,] _cells; 
        private List<CellClass>[] _regionList; 

      
        internal CellClass this[int col, int row]
        {
            get
            {
                if ((_cells != null) && Common.IsValidIndex(col, row)) 
                    return _cells[col, row]; 
                return null;
            }
        }

        internal bool GameComplete => EmptyCount == 0;

        internal List<CellClass> CellList { get; private set; }

        internal int EmptyCount { get; set; }

        internal void ComputeNote(int col, int row)
        {
            if (Common.IsValidIndex(col, row)) 
                GenerateNote(_cells[col, row]); 
        }

        internal void ResetPuzzle()
        {
            if (_cells != null) 
            {
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState != CellStateEnum.Answer) 
                    {                  
                        CellList[i].CellState = CellStateEnum.Blank; 
                        CellList[i].UserAnswer = 0; 
                    }
                CountEmpties();
            }
        }

        internal void ShowNotes()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState == CellStateEnum.Blank) 
                        CellList[i].CellState = CellStateEnum.Notes;
        }

        internal void HideNotes()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState == CellStateEnum.Notes) 
                        CellList[i].CellState = CellStateEnum.Blank;
        }

        internal void ShowSolution()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++) 
                    if ((CellList[i].CellState != CellStateEnum.Answer) &&
                        (CellList[i].CellState != CellStateEnum.UserInputCorrect))
                        CellList[i].CellState = CellStateEnum.Hint;
        }

        internal void HideSolution(bool showAllNotes)
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++) 
                    if (CellList[i].CellState == CellStateEnum.Hint) 
                        if (showAllNotes)
                            CellList[i].CellState = CellStateEnum.Notes; 
                        else
                            CellList[i].CellState = CellStateEnum.Blank;
        }

        internal List<CellClass> RegionCells(int index)
        {
            if (Common.IsValidIndex(index) && (_regionList != null))
                return _regionList[index]; 
            return null;
        }

        internal CellClass[,] GetCells()
        {
            return _cells;
        }

        private void InitClass(CellClass[,] cells)
        {
            if (cells != null) 
            {
                _cells = cells; 
                InitRegionList(); 
                ConvertToList(); 
                GenerateAllNotes(); 
                CountEmpties(); 
            }
        }

        private void InitRegionList()
        {
            _regionList = new List<CellClass>[9]; 
            for (var i = 0; i < 9; i++) 
                _regionList[i] = new List<CellClass>(); 
        }

        private void ConvertToList()
        {
            CellList = new List<CellClass>(_cells.Length);
            for (var col = 0; col < 9; col++) 
                for (var row = 0; row < 9; row++) 
                    AddCell(_cells[col, row]); 
        }

        private void AddCell(CellClass cell)
        {
            if (cell != null) 
            {
                CellList.Add(cell); 
                _regionList[cell.Region].Add(cell); 
            }
            else
                throw new Exception("Cell cannot be null."); 
        }

        private void GenerateAllNotes()
        {
            for (var i = 0; i < CellList.Count; i++) 
                GenerateNote(CellList[i]); 
        }

        private void GenerateNote(CellClass cell)
        {
            if (cell.CellState != CellStateEnum.Answer) 
            {
                for (var i = 0; i < 9; i++) 
                    cell.Notes[i].State = true;
                for (var i = 0; i < 9; i++) 
                {
                    ProcessNote(cell, _cells[cell.Col, i]); 
                    ProcessNote(cell, _cells[i, cell.Row]); 
                }
                foreach (var item in _regionList[cell.Region]) 
                    ProcessNote(cell, item);
            }
        }

        private void ProcessNote(CellClass targetCell, CellClass sourceCell)
        {
            if (sourceCell.CellState == CellStateEnum.Answer) 
                targetCell.Notes[sourceCell.Answer - 1].State = false; 
        }

        private void CountEmpties()
        {
            EmptyCount = 0; 
            foreach (var item in CellList)
                if (item.CellState == CellStateEnum.Blank) 
                    EmptyCount++; 
        }

        public bool ValidateCell(int col, int row)
        {
            return _validator.ValidateCell(col, row);
        }

        public List<CellClass> GetConflictingCells()
        {
            return _validator.GetConflictingCells();
        }

        public bool IsGameValid()
        {
            return _validator.IsGameValid();
        }

        public List<CellClass> GetHint()
        {
            return _validator.GetHint();
        }
    }
}
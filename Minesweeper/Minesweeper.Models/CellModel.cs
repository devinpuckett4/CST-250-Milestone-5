namespace Minesweeper.Models
{
    // Data-only model for a single cell on the board 
    public class CellModel
    {
        // location in the grid
        public int Row { get; set; } = -1;
        public int Column { get; set; } = -1;

        // state flags
        public bool IsVisited { get; set; } = false;      // found/cleared
        public bool IsBomb { get; set; } = false;         // true if this cell has a bomb
        public bool IsFlagged { get; set; } = false;      // player placed a flag

        // how many neighboring cells contain bombs, 9 used as sentinel for bombs
        public int NumberOfBombNeighbors { get; set; } = 0;

        // special reward marker 
        public bool HasSpecialReward { get; set; } = false;

        public CellModel() { }

        //  constructor to set row/column
        public CellModel(int row, int col) { Row = row; Column = col; }
    }
}
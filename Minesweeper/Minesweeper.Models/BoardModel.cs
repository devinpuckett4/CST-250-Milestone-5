using System;

namespace Minesweeper.Models
{
    // Data-only model for the game board (no logic here)
    public class BoardModel
    {
        // Board is square: Size x Size
        public int Size { get; set; }

        // Track when a game starts/ends
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // 2D grid of cells owned by the board
        public CellModel[,] Cells { get; set; }

        // Bomb placement probability
        public float DifficultyPercentage { get; set; } = 0.12f;

        // Count of rewards collected / available to use
        public int RewardsRemaining { get; set; } = 0;

        // Current game state
        public GameState GameState { get; set; } = GameState.StillPlaying;

        // Create the board and initialize every cell with its row/col
        public BoardModel(int size)
        {
            Size = size;
            Cells = new CellModel[size, size];

            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    Cells[r, c] = new CellModel(r, c);
        }
    }
}
using Minesweeper.BLL;
using Minesweeper.Models;
using Xunit;

namespace Minesweeper.Tests
{
    public class FloodFillTests
    {
        [Fact]
        public void RevealCell_OnZero_ExpandsZeros_AndStopsAtNumbers()
        {
            // Arrange: 3x3 board with a single bomb at (0,0); all other cells safe.
            var board = new BoardModel(3) { DifficultyPercentage = 0f }; // no random bombs
            var svc = new BoardService();

            board.Cells[0, 0].IsBomb = true;   // manual placement for a predictable layout
            svc.CountBombsNearby(board);       // compute numbers around bombs

            // Act: click a zero cell far from the bomb to trigger flood fill
            svc.RevealCell(board, 2, 2);

            // Assert:
            Assert.False(board.Cells[0, 0].IsVisited); // bomb should remain hidden
            Assert.True(board.Cells[2, 2].IsVisited);  // zero area opens
            Assert.True(board.Cells[1, 1].IsVisited);  // numbered border reveals
            Assert.True(board.Cells[0, 1].IsVisited);  // numbered border reveals
            Assert.True(board.Cells[1, 0].IsVisited);  // numbered border reveals
        }

        [Fact]
        public void RevealCell_OnNumber_RevealsOnlyThatCell()
        {
            // Arrange: 2x2 with bomb at (0,0), so (0,1) will be a number > 0
            var board = new BoardModel(2) { DifficultyPercentage = 0f };
            var svc = new BoardService();

            board.Cells[0, 0].IsBomb = true;
            svc.CountBombsNearby(board);

            // Act: click a numbered cell
            svc.RevealCell(board, 0, 1);

            // Assert: only that numbered cell reveals (no flood)
            Assert.True(board.Cells[0, 1].IsVisited);
            Assert.False(board.Cells[1, 1].IsVisited);
            Assert.False(board.Cells[1, 0].IsVisited);
        }

        [Fact]
        public void RevealCell_OnBomb_RevealsBomb_AndGameStateBecomesLost()
        {
            // Arrange: 1x1 board with a bomb; only possible click is the bomb
            var board = new BoardModel(1) { DifficultyPercentage = 0f };
            var svc = new BoardService();

            board.Cells[0, 0].IsBomb = true;
            svc.CountBombsNearby(board);

            // Act: click the bomb
            svc.RevealCell(board, 0, 0);

            // Assert: bomb is revealed and the game state is Lost
            Assert.True(board.Cells[0, 0].IsVisited);

            var state = svc.DetermineGameState(board);
            Assert.Equal(GameState.Lost, state);
        }
    }
}
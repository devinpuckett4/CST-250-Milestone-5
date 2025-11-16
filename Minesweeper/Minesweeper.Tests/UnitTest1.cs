using Minesweeper.Models;
using Minesweeper.BLL;
using Xunit;

namespace Minesweeper.Tests
{
    public class UnitTest1
    {
        // Existing tests if any...

        [Fact]
        public void CreateGameStat_ValidWin_ReturnsCorrectStat()
        {
            var board = new BoardModel(5) { DifficultyPercentage = 0.1f, StartTime = DateTime.UtcNow.AddSeconds(-30), EndTime = DateTime.UtcNow, GameState = GameState.Won };
            var ops = new BoardService();
            var stat = ops.CreateGameStat(board, "TestPlayer");
            Assert.Equal("TestPlayer", stat.Name);
            Assert.True(stat.Score > 0);
            Assert.Equal(30, (int)stat.GameTime.TotalSeconds); // Approx
        }
    }
}


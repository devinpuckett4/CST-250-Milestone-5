using System;

namespace Minesweeper.Models
{
    // Model for game statistics
    public class GameStat
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique ID
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
        public TimeSpan GameTime { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow; // For sorting
    }
}

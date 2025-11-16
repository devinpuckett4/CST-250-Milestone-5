using System;
using System.Linq;
using Minesweeper.Models;

namespace Minesweeper.BLL
{
    public class BoardService : IBoardOperations
    {
        private readonly Random _rng;
        public BoardService() : this(new Random()) { }
        public BoardService(Random rng) { _rng = rng; }

        public void SetupBombs(BoardModel board)
        {
            board.StartTime = DateTime.UtcNow;
            board.EndTime = null;
            board.GameState = GameState.StillPlaying;
            board.RewardsRemaining = 0;

            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.Cells[r, c];
                    cell.IsVisited = false;
                    cell.IsFlagged = false;
                    cell.HasSpecialReward = false;
                    cell.NumberOfBombNeighbors = 0;
                    cell.IsBomb = _rng.NextDouble() < board.DifficultyPercentage;
                }
            }

            TryPlaceSingleReward(board);
            CountBombsNearby(board);
        }

        private void TryPlaceSingleReward(BoardModel board)
        {
            int n = board.Size;
            var safe = new (int r, int c)[n * n];
            int k = 0;
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (!board.Cells[r, c].IsBomb)
                        safe[k++] = (r, c);
            if (k == 0) return;
            var pick = safe[_rng.Next(k)];
            board.Cells[pick.r, pick.c].HasSpecialReward = true;
            board.RewardsRemaining = 1;
        }

        public void CountBombsNearby(BoardModel board)
        {
            int n = board.Size;
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var cell = board.Cells[r, c];
                    if (cell.IsBomb)
                    {
                        cell.NumberOfBombNeighbors = 9;
                        continue;
                    }
                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            int rr = r + dr, cc = c + dc;
                            if (rr >= 0 && rr < n && cc >= 0 && cc < n && board.Cells[rr, cc].IsBomb)
                                count++;
                        }
                    cell.NumberOfBombNeighbors = count;
                }
            }
        }

        public void RevealCell(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged) return;

            bool isFirstReveal = board.Cells.Cast<CellModel>().All(cm => !cm.IsVisited);
            if (isFirstReveal && cell.IsBomb)
                EnsureFirstClickSafe(board, r, c);

            if (cell.HasSpecialReward)
            {
                cell.HasSpecialReward = false;
                board.RewardsRemaining += 1;
            }

            if (cell.IsBomb)
            {
                cell.IsVisited = true;
                return;
            }

            if (cell.NumberOfBombNeighbors > 0)
            {
                cell.IsVisited = true;
                return;
            }

            FloodFillOpening(board, r, c);
        }

        private void EnsureFirstClickSafe(BoardModel board, int r, int c)
        {
            var cell = board.Cells[r, c];
            var safeCells = new List<(int rr, int cc)>();
            for (int rr = 0; rr < board.Size; rr++)
                for (int cc = 0; cc < board.Size; cc++)
                    if (!board.Cells[rr, cc].IsBomb && !(rr == r && cc == c) && !board.Cells[rr, cc].HasSpecialReward)
                        safeCells.Add((rr, cc));

            if (safeCells.Count > 0)
            {
                var (nr, nc) = safeCells[_rng.Next(safeCells.Count)];
                board.Cells[nr, nc].IsBomb = true;
                cell.IsBomb = false;
                CountBombsNearby(board);
            }
        }

        private void FloodFillOpening(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged || cell.IsBomb) return;

            cell.IsVisited = true;
            if (cell.NumberOfBombNeighbors > 0) return;

            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    FloodFillOpening(board, r + dr, c + dc);
                }
        }

        public bool VisitCell(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return false;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged) return false;
            cell.IsVisited = true;
            if (cell.HasSpecialReward)
            {
                cell.HasSpecialReward = false;
                board.RewardsRemaining += 1;
            }
            return cell.IsBomb;
        }

        public void ToggleFlag(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited) return;
            cell.IsFlagged = !cell.IsFlagged;
        }

        public string UseRewardPeek(BoardModel board, int r, int c)
        {
            if (board.RewardsRemaining <= 0) return "No reward available.";
            if (!InBounds(board, r, c)) return "That position is out of bounds.";
            board.RewardsRemaining -= 1;
            var cell = board.Cells[r, c];
            return cell.IsBomb
                ? "Peek result: This cell IS a bomb."
                : "Peek result: This cell is safe.";
        }

        public GameState DetermineGameState(BoardModel board)
        {
            int n = board.Size;

            // Check for loss: any visited bomb
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (board.Cells[r, c].IsBomb && board.Cells[r, c].IsVisited)
                    {
                        board.GameState = GameState.Lost;
                        board.EndTime = DateTime.UtcNow;
                        return board.GameState;
                    }

            // Check for win: all non-bombs visited (bombs can be flagged or not)
            bool allSafeVisited = true;
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (!board.Cells[r, c].IsBomb && !board.Cells[r, c].IsVisited)
                    {
                        allSafeVisited = false;
                        break;
                    }
            if (allSafeVisited)
            {
                board.GameState = GameState.Won;
                board.EndTime = DateTime.UtcNow;
                return board.GameState;
            }

            // Still playing
            board.GameState = GameState.StillPlaying;
            return board.GameState;
        }

        public int DetermineFinalScore(BoardModel board)
        {
            if (board.GameState != GameState.Won || board.EndTime == null) return 0;
            double elapsed = (board.EndTime.Value - board.StartTime).TotalSeconds;
            double baseScore = board.Size * board.Size * 20;
            double difficultyBonus = board.DifficultyPercentage * 5000;
            double speedBonus = Math.Max(0, 3000 - elapsed * 10);
            return (int)(baseScore + difficultyBonus + speedBonus);
        }

        public GameStat CreateGameStat(BoardModel board, string playerName)
        {
            if (board.GameState != GameState.Won || board.EndTime == null) throw new InvalidOperationException("Game must be won to create stat.");
            var elapsed = board.EndTime.Value - board.StartTime;
            return new GameStat
            {
                Name = playerName,
                Score = DetermineFinalScore(board),
                GameTime = elapsed,
                Date = board.EndTime.Value
            };
        }

        private bool InBounds(BoardModel board, int r, int c)
            => r >= 0 && r < board.Size && c >= 0 && c < board.Size;
    }
}
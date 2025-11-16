using System;
using Minesweeper.BLL;
using Minesweeper.Models;

namespace Minesweeper.ConsoleApp
{
    internal class Program
    {
        private const int CELL_W = 3;

        static void Main(string[] args)
        {
            // Create the board and the service that runs the rules
            // Note: DifficultyPercentage controls random bomb density when using SetupBombs.
            var board = new BoardModel(size: 10) { DifficultyPercentage = 0.12f };
            IBoardOperations svc = new BoardService();

            // Set up a fresh game place bombs, reset state and compute neighbor counts.
            svc.SetupBombs(board);

            Console.WriteLine("Answer Key cheat view:");
            PrintAnswers(board);
            Console.WriteLine();
            Console.Write("Press Enter to start gameplay...");
            Console.ReadLine();
            Console.Clear();

            // Game loop flags
            bool victory = false;
            bool death = false;

            // Main input/output loop
            while (!victory && !death)
            {
                // Show the masked board each turn and prompt the user
                PrintBoard(board);
                Console.WriteLine($"Rewards available: {board.RewardsRemaining}");
                Console.WriteLine("Enter: row col action   V = Visit, F = Flag, U = Use Reward");
                Console.Write("> ");

                // Read one line of input
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("No input read. Example: 3 4 V");
                    continue;
                }

                // Split on spaces, tabs, or commas
                var parts = input.Trim().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    Console.WriteLine("Please enter three parts: row col action. Example: 3 4 V");
                    continue;
                }

                // Parse row and column
                if (!int.TryParse(parts[0], out int r) || !int.TryParse(parts[1], out int c))
                {
                    Console.WriteLine("Row and col must be whole numbers like 0 1 V");
                    continue;
                }

                // Normalize action (V/F/U)
                string actionRaw = parts[2].Trim().ToUpperInvariant();
                string action = actionRaw switch
                {
                    "V" or "VISIT" => "V",
                    "F" or "FLAG" => "F",
                    "U" or "USE" or "PEEK" => "U",
                    _ => ""
                };
                if (action == "")
                {
                    Console.WriteLine("Action must be V F or U");
                    continue;
                }

                // Quick bounds check so we can give a message instead of crashing
                if (r < 0 || r >= board.Size || c < 0 || c >= board.Size)
                {
                    Console.WriteLine("That move is out of bounds");
                    continue;
                }

                // --- Handle the action ---
                if (action == "V")
                {
                    // Remember reward and visited state before the reveal 
                    bool hadReward = board.Cells[r, c].HasSpecialReward;
                    bool wasVisited = board.Cells[r, c].IsVisited;

                    // NEW: Use the recursive entry point handles bomb, number, and zero flood fill
                    svc.RevealCell(board, r, c);

                    var cell = board.Cells[r, c];

                    // Player messages based on what happened
                    if (!wasVisited && hadReward)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("You found the reward. You can use U once to peek a cell");
                        Console.ResetColor();
                    }
                    else if (cell.IsBomb && cell.IsVisited)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You visited a bomb. Game over");
                        Console.ResetColor();
                    }
                    else if (cell.IsVisited && cell.NumberOfBombNeighbors == 0)
                    {
                        // This means flood fill ran and opened an empty area
                        Console.WriteLine("Opened an empty area");
                    }
                    else if (cell.IsVisited && cell.NumberOfBombNeighbors > 0)
                    {
                        // Numbered cell reveals only itself
                        Console.WriteLine($"Revealed a {cell.NumberOfBombNeighbors}");
                    }
                    else
                    {
                        // Rare: clicking a flagged or already visited cell
                        Console.WriteLine("Cell unchanged");
                    }
                }
                else if (action == "F")
                {
                    // Flags only apply to hidden cells
                    if (board.Cells[r, c].IsVisited)
                    {
                        Console.WriteLine("You cannot flag a cell that is already visited");
                    }
                    else
                    {
                        svc.ToggleFlag(board, r, c);
                        Console.WriteLine(board.Cells[r, c].IsFlagged ? "Flag placed" : "Flag removed");
                    }
                }
                else if (action == "U")
                {
                    // Use the one-time peek if available
                    var msg = svc.UseRewardPeek(board, r, c);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }

                // After any action, re-check game state and end if needed
                var state = svc.DetermineGameState(board);
                if (state == GameState.Won)
                {
                    victory = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nYou win");
                    Console.ResetColor();
                    PrintAnswers(board); // Final reveal
                }
                else if (state == GameState.Lost)
                {
                    death = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nYou lose");
                    Console.ResetColor();
                    PrintAnswers(board); // Final reveal
                }
                else
                {
                    Console.WriteLine(); // spacer between turns
                }
            }

            Console.WriteLine("\nDone. Press any key to exit.");
            Console.ReadKey();
        }

        // Helper: center short strings in a fixed width
        private static string Center(string s, int width)
        {
            if (string.IsNullOrEmpty(s)) s = " ";
            if (s.Length >= width) return s.Substring(0, width);
            int left = (width - s.Length) / 2;
            int right = width - s.Length - left;
            return new string(' ', left) + s + new string(' ', right);
        }

        // Helper: header with column numbers and a top border
        private static void PrintHeaderAndBorder(int n)
        {
            Console.Write("    |");
            for (int c = 0; c < n; c++)
            {
                Console.Write(Center(c.ToString(), CELL_W));
                Console.Write("|");
            }
            Console.WriteLine();

            // Top border that matches the row borders
            Console.Write("    +");
            for (int c = 0; c < n; c++) Console.Write(new string('-', CELL_W) + "+");
            Console.WriteLine();
        }

        // hidden shows ?, flags show F, revealed shows B/number/.
        private static void PrintBoard(BoardModel board)
        {
            int n = board.Size;

            PrintHeaderAndBorder(n);

            for (int r = 0; r < n; r++)
            {
                Console.Write($"{r,2}  |"); // row label and left border

                for (int c = 0; c < n; c++)
                {
                    var cell = board.Cells[r, c];
                    string content;

                    if (!cell.IsVisited)
                        content = cell.IsFlagged ? "F" : "?";
                    else if (cell.IsBomb)
                        content = "B";
                    else if (cell.NumberOfBombNeighbors > 0)
                        content = cell.NumberOfBombNeighbors.ToString();
                    else
                        content = "."; // visited zero

                    if (content == "B") Console.ForegroundColor = ConsoleColor.Red;
                    else if (content == "F") Console.ForegroundColor = ConsoleColor.Cyan;
                    else if (int.TryParse(content, out _)) Console.ForegroundColor = ConsoleColor.Yellow;
                    else Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write(Center(content, CELL_W));
                    Console.ResetColor();
                    Console.Write("|");
                }

                Console.WriteLine();

                Console.Write("    +");
                for (int c = 0; c < n; c++) Console.Write(new string('-', CELL_W) + "+");
                Console.WriteLine();
            }
        }

        // Full reveal "answer key"
        private static void PrintAnswers(BoardModel board)
        {
            int n = board.Size;

            PrintHeaderAndBorder(n);

            for (int r = 0; r < n; r++)
            {
                Console.Write($"{r,2}  |");

                for (int c = 0; c < n; c++)
                {
                    var cell = board.Cells[r, c];
                    string content = cell.IsBomb
                        ? "B"
                        : (cell.NumberOfBombNeighbors > 0 ? cell.NumberOfBombNeighbors.ToString() : ".");

                    if (content == "B") Console.ForegroundColor = ConsoleColor.Red;
                    else if (int.TryParse(content, out _)) Console.ForegroundColor = ConsoleColor.Yellow;
                    else Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write(Center(content, CELL_W));
                    Console.ResetColor();
                    Console.Write("|");
                }

                Console.WriteLine();

                Console.Write("    +");
                for (int c = 0; c < n; c++) Console.Write(new string('-', CELL_W) + "+");
                Console.WriteLine();
            }
        }
    }
}
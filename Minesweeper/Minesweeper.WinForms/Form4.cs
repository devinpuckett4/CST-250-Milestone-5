using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Minesweeper.Models;
using Newtonsoft.Json;
namespace Minesweeper.WinForms
{
    public partial class Form4 : Form
    {
        // Stores all high score entries (player name, score, time, date)
        private List<GameStat> _stats = new List<GameStat>();
        // Grid control used to display the scores on the form
        private DataGridView dgvScores;

        // Full path to the JSON file used for saving and loading scores
        private readonly string _filePath = Path.Combine(Application.StartupPath, "highscores.json");

        // Optional newStat lets us pass in the latest game result when opening this form
        public Form4(GameStat? newStat = null)
        {
            // Basic form window setup
            Text = "High Scores";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(600, 400);

            // Create main menu strip
            var menu = new MenuStrip();

            // "File" menu with Save, Load, and Exit options
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Save", null, (s, e) => SaveScores()); // manually save scores to file
            fileMenu.DropDownItems.Add("Load", null, (s, e) => LoadScores()); // manually load scores from file
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Close());      // close the high scores window

            // "Sort" menu to organize the list different ways
            var sortMenu = new ToolStripMenuItem("Sort");
            sortMenu.DropDownItems.Add("By Name", null, (s, e) => SortBy("Name"));                        // sort A-Z by player name
            sortMenu.DropDownItems.Add("By Score", null, (s, e) => SortBy("Score", descending: true));    // sort highest score first
            sortMenu.DropDownItems.Add("By Date", null, (s, e) => SortBy("Date", descending: true));      // sort newest date first

            // Add menus to the menu strip and attach to form
            menu.Items.Add(fileMenu);
            menu.Items.Add(sortMenu);
            MainMenuStrip = menu;
            Controls.Add(menu);

            // Set up the DataGridView that shows the scores
            dgvScores = new DataGridView
            {
                Dock = DockStyle.Fill,                                        // fill the remaining space
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,   // stretch columns nicely
                ReadOnly = true                                               // prevent editing directly in the grid
            };
            Controls.Add(dgvScores);

            // Load any previously saved scores when the form opens
            LoadScores();

            // If a new score was passed in, add it to the list and save
            if (newStat != null)
            {
                _stats.Add(newStat);
                SaveScores(); // auto-save so the new entry is kept
            }

            // Show the current scores in the grid
            RefreshGrid();
        }

        // Rebuilds the DataGridView display from the current _stats list
        private void RefreshGrid()
        {
            // Reset the data source so it refreshes correctly
            dgvScores.DataSource = null;

            // Project GameStat into a simple anonymous type for display
            dgvScores.DataSource = _stats.Select(s => new
            {
                Name = s.Name,                               // player name
                Score = s.Score,                             // player score
                Time = s.GameTime.ToString(@"mm\:ss"),       // formatted time (mm:ss)
                Date = s.Date.ToShortDateString()            // date in short format
            }).ToList();
        }

        // Sorts the scores list based on a selected property (Name, Score, or Date)
        private void SortBy(string property, bool descending = false)
        {
            if (descending)
            {
                // Sort in descending order when requested
                _stats = property switch
                {
                    "Name" => _stats.OrderByDescending(s => s.Name).ToList(),
                    "Score" => _stats.OrderByDescending(s => s.Score).ToList(),
                    "Date" => _stats.OrderByDescending(s => s.Date).ToList(),
                    _ => _stats
                };
            }
            else
            {
                // Default: sort in ascending order
                _stats = property switch
                {
                    "Name" => _stats.OrderBy(s => s.Name).ToList(),
                    "Score" => _stats.OrderBy(s => s.Score).ToList(),
                    "Date" => _stats.OrderBy(s => s.Date).ToList(),
                    _ => _stats
                };
            }

            // Update the grid after sorting
            RefreshGrid();
        }

        // Saves the current _stats list to the JSON file
        private void SaveScores()
        {
            try
            {
                // Convert the list to JSON and write it to disk
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_stats, Formatting.Indented));

                // Let the user know it worked
                MessageBox.Show("Scores saved!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Show any error that happens during save
                MessageBox.Show("Error saving scores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads scores from the JSON file (if it exists)
        private void LoadScores()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    // Read JSON file and convert it back into a list of GameStat
                    _stats = JsonConvert.DeserializeObject<List<GameStat>>(File.ReadAllText(_filePath))
                             ?? new List<GameStat>(); // fallback to empty list if null
                }
                else
                {
                    // No file yet, start with an empty list
                    _stats = new List<GameStat>();
                }
            }
            catch (Exception ex)
            {
                // Show any error that happens during load and reset list
                MessageBox.Show("Error loading scores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _stats = new List<GameStat>();
            }

            // Refresh the grid so it matches whatever was loaded
            RefreshGrid();
        }
    }

}
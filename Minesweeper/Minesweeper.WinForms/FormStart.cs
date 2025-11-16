using System;
using System.Drawing;
using System.Windows.Forms;
using Minesweeper.BLL;
using Minesweeper.Models;

namespace Minesweeper.WinForms
{
    public partial class FormStart : Form
    {
        private TrackBar trkSize, trkDifficulty;
        private Label lblSizeValue, lblDiffValue;

        public FormStart()
        {
            Text = "Minesweeper - Setup";
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.None;

            // Roomy, resizable window
            ClientSize = new Size(640, 360);
            MinimumSize = new Size(640, 360);
            Font = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Point);

            // Grid layout: [label] [trackbar (stretch)] [value label]
            var tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                Padding = new Padding(16)
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));

            var lblSize = new Label { Text = "Board size", AutoSize = true, Anchor = AnchorStyles.Left };
            trkSize = new TrackBar { Minimum = 6, Maximum = 24, Value = 10, TickFrequency = 2, Dock = DockStyle.Fill };
            lblSizeValue = new Label { Text = "10 x 10", AutoSize = true, Anchor = AnchorStyles.Left };

            var lblDiff = new Label { Text = "Difficulty", AutoSize = true, Anchor = AnchorStyles.Left };
            trkDifficulty = new TrackBar { Minimum = 1, Maximum = 3, Value = 1, TickFrequency = 1, Dock = DockStyle.Fill };
            lblDiffValue = new Label { Text = "1 (Easy)", AutoSize = true, Anchor = AnchorStyles.Left };

            var btnStart = new Button
            {
                Text = "Start Game",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.Left
            };
            AcceptButton = btnStart;

            // Events
            trkSize.ValueChanged += (s, e) => lblSizeValue.Text = $"{trkSize.Value} x {trkSize.Value}";
            trkDifficulty.ValueChanged += (s, e) =>
                lblDiffValue.Text = trkDifficulty.Value switch
                {
                    1 => "1 (Easy)",
                    2 => "2 (Medium)",
                    3 => "3 (Hard)",
                    _ => trkDifficulty.Value.ToString()
                };
            btnStart.Click += (s, e) =>
            {
                int size = trkSize.Value;
                int diff = trkDifficulty.Value;

                // Difficulty is a float on your model → use f-suffix
                float diffPct = diff switch
                {
                    1 => 0.12f, // easy
                    2 => 0.18f, // medium
                    3 => 0.24f, // hard
                    _ => 0.15f
                };

                // Your model requires the size in the ctor
                var board = new BoardModel(size);
                board.DifficultyPercentage = diffPct;

                IBoardOperations ops = new BoardService();
                ops.SetupBombs(board); // bombs, counts, reward, state reset

                new FormGame(ops, board).Show();
            };

            // Add controls to grid
            tlp.Controls.Add(lblSize, 0, 0);
            tlp.Controls.Add(trkSize, 1, 0);
            tlp.Controls.Add(lblSizeValue, 2, 0);

            tlp.Controls.Add(lblDiff, 0, 1);
            tlp.Controls.Add(trkDifficulty, 1, 1);
            tlp.Controls.Add(lblDiffValue, 2, 1);

            tlp.Controls.Add(btnStart, 0, 3);
            tlp.SetColumnSpan(btnStart, 3);

            Controls.Add(tlp);
        }
    }
}
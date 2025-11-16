using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper.WinForms
{
    // Name input form for winners
    public partial class Form3 : Form
    {
        private TextBox txtName;
        private Button btnOK;

        public string PlayerName { get; private set; } = string.Empty;

        public Form3()
        {
            Text = "Enter Your Name";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(300, 200); 
            Font = new Font("Segoe UI", 11f);

            var lbl = new Label { Text = "You won! Enter your name for high scores:", AutoSize = true, Location = new Point(20, 20) };
            txtName = new TextBox { Location = new Point(20, 50), Size = new Size(260, 30) };
            btnOK = new Button { Text = "OK", Location = new Point(20, 90), Size = new Size(260, 50) };
            btnOK.Click += (s, e) => { PlayerName = txtName.Text.Trim(); DialogResult = DialogResult.OK; Close(); };

            Controls.AddRange(new Control[] { lbl, txtName, btnOK });
            AcceptButton = btnOK;
            txtName.Focus();
        }
    }
}
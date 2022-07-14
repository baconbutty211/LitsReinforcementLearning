using System;
using LitsReinforcementLearning;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LitsFormsDebug
{
    public partial class EnvironmentForm : Form
    {
        private static int size = LitsReinforcementLearning.Environment.size;
        private LitsReinforcementLearning.Environment environment = new LitsReinforcementLearning.Environment();
        private Button[] board = new Button[size];

        public EnvironmentForm()
        {
            InitializeComponent();
            environment.boardChanged += Environment_BoardChanged;
        }

        private void EnvironmentForm_Load(object sender, EventArgs e)
        {
            int boardSize = (int)Math.Sqrt(size);
            for (int i = 0; i < size; i++)
            {
                board[i] = new Button();
                Size size = new Size(50,50);
                board[i].Size = size;
                int x = 3 + (size.Width * (i % boardSize));
                int y = 3 + (size.Height * (i / boardSize));
                board[i].Location = new Point(x, y);
                boardPanel.Controls.Add(board[i]);
            }
        }
        private void Environment_BoardChanged(LitsReinforcementLearning.Environment.Tile[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                string text = " "; // Default text
                Color backColour = SystemColors.Control; // Default colour
                switch (board[i])
                {
                    case LitsReinforcementLearning.Environment.Tile._:
                        break;
                    case LitsReinforcementLearning.Environment.Tile.O:
                        text = board[i].ToString();
                        break;
                    case LitsReinforcementLearning.Environment.Tile.X:
                        text = board[i].ToString();
                        break;
                    case LitsReinforcementLearning.Environment.Tile.L:
                        text = board[i].ToString();
                        backColour = Color.Blue;
                        break;
                    case LitsReinforcementLearning.Environment.Tile.I:
                        text = board[i].ToString();
                        backColour = Color.Green;
                        break;
                    case LitsReinforcementLearning.Environment.Tile.T:
                        text = board[i].ToString();
                        backColour = Color.Magenta;
                        break;
                    case LitsReinforcementLearning.Environment.Tile.S:
                        text = board[i].ToString();
                        backColour = Color.Yellow;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                this.board[i].Text = text;
                this.board[i].BackColor = backColour;
            }
        }
        
        private void ResetEnvironmentBtn_Click(object sender, EventArgs e)
        {
            environment.Reset();
            RandomActionBtn.Enabled = true;
        }
        private void RandomActionBtn_Click(object sender, EventArgs e)
        {
            LitsReinforcementLearning.Action randAct = environment.GetRandomAction();
            environment.Step(randAct);
            if (environment.isDone)
                RandomActionBtn.Enabled = false;
        }
    }
}

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

namespace LitsForms
{
    public partial class EnvironmentForm : Form
    {
        protected LitsReinforcementLearning.Environment environment = new LitsReinforcementLearning.Environment();

        private static int size = LitsReinforcementLearning.Environment.size;
        private Button[] board = new Button[size];
        private Stack<KeyValuePair<int, LitsReinforcementLearning.Environment>> previousActionsStack = new Stack<KeyValuePair<int, LitsReinforcementLearning.Environment>>(); // Keeps track of the previous environment states and the accompanying action the is applied next.

        public EnvironmentForm()
        {
            InitializeComponent();
            
            environment.boardChanged += Environment_BoardChanged;
            
            validActionsList.MouseDoubleClick += ValidActionsList_MouseDoubleClick;
            validActionsList.KeyDown += ValidActionsList_KeyDown;

            previousActionsList.MouseDoubleClick += PreviousActionsList_MouseDoubleClick;
            previousActionsList.KeyDown += PreviousActionsList_KeyDown;
        }

        private void EnvironmentForm_Load(object sender, EventArgs e)
        {
            // Initializes Board
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
            resultLab.Text = "";

            environment.Reset();
        }
        private void Environment_BoardChanged(LitsReinforcementLearning.Environment.Tile[] board)
        {
            // Updates the board buttons
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

            if (environment.isDone)
                resultLab.Text = environment.GetResult();
            else
                resultLab.Text = "";

            // Updates the valid actions ListBox
            validActionsList.Items.Clear();
            foreach (LitsReinforcementLearning.Action act in environment.validActions)
                validActionsList.Items.Add(act);

            
            RandomActionBtn.Enabled = !environment.isDone;
        }
        
        private void ResetEnvironmentBtn_Click(object sender, EventArgs e)
        {
            environment.Reset();
            previousActionsList.Items.Clear();
            previousActionsStack.Clear();
            RandomActionBtn.Enabled = true;
        }
        private void RandomActionBtn_Click(object sender, EventArgs e)
        {
            LitsReinforcementLearning.Action randAct = environment.GetRandomAction();
            ApplyAction(randAct);
        }
        
        private void ValidActionsList_MouseDoubleClick(object sender, EventArgs e)
        {
            string act = validActionsList.SelectedItem.ToString();
            int actId = int.Parse(act.Split(')')[0]);
            LitsReinforcementLearning.Action action = LitsReinforcementLearning.Action.GetAction(actId);

            ApplyAction(action);
        }
        private void ValidActionsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (validActionsList.SelectedItem != null)
                validActionsList.SelectedItem = null;
        }

        private void PreviousActionsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string act = previousActionsList.SelectedItem.ToString();
            int actId = int.Parse(act.Split(')')[0]);
            LitsReinforcementLearning.Action action = LitsReinforcementLearning.Action.GetAction(actId);

            KeyValuePair<int, LitsReinforcementLearning.Environment> state;
            do
            {
                state = previousActionsStack.Pop();
                previousActionsList.Items.RemoveAt(previousActionsList.Items.Count - 1);
            }
            while (state.Key != actId);

            environment.boardChanged -= Environment_BoardChanged;
            environment = state.Value;
            environment.boardChanged += Environment_BoardChanged;

            ApplyAction(action);
        }
        private void PreviousActionsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (previousActionsList.SelectedItem != null)
                previousActionsList.SelectedItem = null;
        }

        protected void ApplyAction(LitsReinforcementLearning.Action action) 
        {
            KeyValuePair<int, LitsReinforcementLearning.Environment> state = new KeyValuePair<int, LitsReinforcementLearning.Environment>(action.Id, environment.Clone());
            previousActionsStack.Push(state); // Stores the environment before the action is applied, so that when it is recalled the action is applied and the board will update.
            previousActionsList.Items.Add(action);

            environment.Step(action);
        }
    }
}

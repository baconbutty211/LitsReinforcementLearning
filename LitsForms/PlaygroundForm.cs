using LitsReinforcementLearning;
using System;
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
    public partial class PlaygroundForm : EnvironmentForm
    {
        Agent agent;
        public PlaygroundForm(Agent agent) : base()
        {
            InitializeComponent();
            this.agent = agent;
        }

        protected override void Environment_BoardChanged(LitsReinforcementLearning.Environment.Tile[] board)
        {
            base.Environment_BoardChanged(board);
            AiActionBtn.Enabled = !environment.isDone;
        }
        private void AiActionBtn_Click(object sender, EventArgs e)
        {
            ApplyAction(agent.Exploit(environment));
        }
    }
}

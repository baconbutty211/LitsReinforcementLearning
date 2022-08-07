using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LitsReinforcementLearning;

namespace LitsForms
{
    public partial class LitsForm : Form
    {
        public LitsForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private Agent SelectAgent()
        {
            FolderBrowserDialog agentSelectionDialog = new FolderBrowserDialog();
            agentSelectionDialog.Description = "Select an agent";
            agentSelectionDialog.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            DialogResult result = agentSelectionDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] path = agentSelectionDialog.SelectedPath.Split(Path.Slash);
                string agentName = path[path.Length - 1];

                return new DynamicProgrammingAgent(agentName);
            }
            else
                return null;
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            Hide();
            Agent agent = SelectAgent();
            if (agent == null)
            {
                Show();
                return;
            }

            PlaygroundForm playground = new PlaygroundForm(agent);
            playground.Show();
            playground.FormClosed += Environment_FormClosed;
        }
        private void Environment_FormClosed(object sender, FormClosedEventArgs e)
        {
            Show();
        }
    }
}

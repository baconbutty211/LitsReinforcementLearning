
namespace LitsForms
{
    partial class EnvironmentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boardPanel = new System.Windows.Forms.Panel();
            this.boardLab = new System.Windows.Forms.Label();
            this.RandomActionBtn = new System.Windows.Forms.Button();
            this.ResetEnvironmentBtn = new System.Windows.Forms.Button();
            this.validActionsList = new System.Windows.Forms.ListBox();
            this.validActionsLab = new System.Windows.Forms.Label();
            this.previousActionsList = new System.Windows.Forms.ListBox();
            this.PreviousActionsLab = new System.Windows.Forms.Label();
            this.resultLab = new System.Windows.Forms.Label();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // boardPanel
            // 
            this.boardPanel.AutoSize = true;
            this.boardPanel.Location = new System.Drawing.Point(635, 207);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.Size = new System.Drawing.Size(500, 500);
            this.boardPanel.TabIndex = 0;
            // 
            // boardLab
            // 
            this.boardLab.AutoSize = true;
            this.boardLab.Location = new System.Drawing.Point(635, 179);
            this.boardLab.Name = "boardLab";
            this.boardLab.Size = new System.Drawing.Size(59, 25);
            this.boardLab.TabIndex = 1;
            this.boardLab.Text = "Board";
            // 
            // RandomActionBtn
            // 
            this.RandomActionBtn.Location = new System.Drawing.Point(989, 713);
            this.RandomActionBtn.Name = "RandomActionBtn";
            this.RandomActionBtn.Size = new System.Drawing.Size(146, 49);
            this.RandomActionBtn.TabIndex = 2;
            this.RandomActionBtn.Text = "Random";
            this.RandomActionBtn.UseVisualStyleBackColor = true;
            this.RandomActionBtn.Click += new System.EventHandler(this.RandomActionBtn_Click);
            // 
            // ResetEnvironmentBtn
            // 
            this.ResetEnvironmentBtn.Location = new System.Drawing.Point(635, 713);
            this.ResetEnvironmentBtn.Name = "ResetEnvironmentBtn";
            this.ResetEnvironmentBtn.Size = new System.Drawing.Size(146, 49);
            this.ResetEnvironmentBtn.TabIndex = 3;
            this.ResetEnvironmentBtn.Text = "Reset";
            this.ResetEnvironmentBtn.UseVisualStyleBackColor = true;
            this.ResetEnvironmentBtn.Click += new System.EventHandler(this.ResetEnvironmentBtn_Click);
            // 
            // validActionsList
            // 
            this.validActionsList.FormattingEnabled = true;
            this.validActionsList.ItemHeight = 25;
            this.validActionsList.Location = new System.Drawing.Point(124, 78);
            this.validActionsList.Name = "validActionsList";
            this.validActionsList.Size = new System.Drawing.Size(394, 829);
            this.validActionsList.TabIndex = 4;
            // 
            // validActionsLab
            // 
            this.validActionsLab.AutoSize = true;
            this.validActionsLab.Location = new System.Drawing.Point(124, 50);
            this.validActionsLab.Name = "validActionsLab";
            this.validActionsLab.Size = new System.Drawing.Size(114, 25);
            this.validActionsLab.TabIndex = 5;
            this.validActionsLab.Text = "Valid Actions";
            // 
            // previousActionsList
            // 
            this.previousActionsList.FormattingEnabled = true;
            this.previousActionsList.ItemHeight = 25;
            this.previousActionsList.Location = new System.Drawing.Point(1269, 78);
            this.previousActionsList.Name = "previousActionsList";
            this.previousActionsList.Size = new System.Drawing.Size(394, 829);
            this.previousActionsList.TabIndex = 6;
            // 
            // PreviousActionsLab
            // 
            this.PreviousActionsLab.AutoSize = true;
            this.PreviousActionsLab.Location = new System.Drawing.Point(1269, 43);
            this.PreviousActionsLab.Name = "PreviousActionsLab";
            this.PreviousActionsLab.Size = new System.Drawing.Size(143, 25);
            this.PreviousActionsLab.TabIndex = 7;
            this.PreviousActionsLab.Text = "Previous Actions";
            // 
            // resultLab
            // 
            this.resultLab.AutoSize = true;
            this.resultLab.Location = new System.Drawing.Point(634, 840);
            this.resultLab.Name = "resultLab";
            this.resultLab.Size = new System.Drawing.Size(190, 25);
            this.resultLab.TabIndex = 8;
            this.resultLab.Text = "*End game result text*";
            // 
            // PlayBtn
            // 
            this.PlayBtn.Location = new System.Drawing.Point(806, 713);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(146, 49);
            this.PlayBtn.TabIndex = 9;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // EnvironmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1796, 976);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.resultLab);
            this.Controls.Add(this.PreviousActionsLab);
            this.Controls.Add(this.previousActionsList);
            this.Controls.Add(this.validActionsLab);
            this.Controls.Add(this.validActionsList);
            this.Controls.Add(this.ResetEnvironmentBtn);
            this.Controls.Add(this.RandomActionBtn);
            this.Controls.Add(this.boardLab);
            this.Controls.Add(this.boardPanel);
            this.Name = "EnvironmentForm";
            this.Text = "Environment";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EnvironmentForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel boardPanel;
        private System.Windows.Forms.Label boardLab;
        private System.Windows.Forms.Button RandomActionBtn;
        private System.Windows.Forms.Button ResetEnvironmentBtn;
        private System.Windows.Forms.ListBox validActionsList;
        private System.Windows.Forms.Label validActionsLab;
        private System.Windows.Forms.ListBox previousActionsList;
        private System.Windows.Forms.Label PreviousActionsLab;
        private System.Windows.Forms.Label resultLab;
        private System.Windows.Forms.Button PlayBtn;
    }
}
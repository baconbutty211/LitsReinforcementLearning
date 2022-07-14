
namespace LitsFormsDebug
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
            this.SuspendLayout();
            // 
            // boardPanel
            // 
            this.boardPanel.AutoSize = true;
            this.boardPanel.Location = new System.Drawing.Point(565, 207);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.Size = new System.Drawing.Size(500, 500);
            this.boardPanel.TabIndex = 0;
            // 
            // boardLab
            // 
            this.boardLab.AutoSize = true;
            this.boardLab.Location = new System.Drawing.Point(565, 179);
            this.boardLab.Name = "boardLab";
            this.boardLab.Size = new System.Drawing.Size(59, 25);
            this.boardLab.TabIndex = 1;
            this.boardLab.Text = "Board";
            // 
            // RandomActionBtn
            // 
            this.RandomActionBtn.Location = new System.Drawing.Point(919, 713);
            this.RandomActionBtn.Name = "RandomActionBtn";
            this.RandomActionBtn.Size = new System.Drawing.Size(146, 49);
            this.RandomActionBtn.TabIndex = 2;
            this.RandomActionBtn.Text = "Random";
            this.RandomActionBtn.UseVisualStyleBackColor = true;
            this.RandomActionBtn.Click += new System.EventHandler(this.RandomActionBtn_Click);
            // 
            // ResetEnvironmentBtn
            // 
            this.ResetEnvironmentBtn.Location = new System.Drawing.Point(565, 713);
            this.ResetEnvironmentBtn.Name = "ResetEnvironmentBtn";
            this.ResetEnvironmentBtn.Size = new System.Drawing.Size(146, 49);
            this.ResetEnvironmentBtn.TabIndex = 3;
            this.ResetEnvironmentBtn.Text = "Reset";
            this.ResetEnvironmentBtn.UseVisualStyleBackColor = true;
            this.ResetEnvironmentBtn.Click += new System.EventHandler(this.ResetEnvironmentBtn_Click);
            // 
            // EnvironmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1796, 976);
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
    }
}
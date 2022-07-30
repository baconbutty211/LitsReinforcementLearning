
namespace LitsForms
{
    partial class PlaygroundForm
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
            this.AiActionBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AiActionBtn
            // 
            this.AiActionBtn.Location = new System.Drawing.Point(811, 713);
            this.AiActionBtn.Name = "AiActionBtn";
            this.AiActionBtn.Size = new System.Drawing.Size(146, 49);
            this.AiActionBtn.TabIndex = 8;
            this.AiActionBtn.Text = "AI Action";
            this.AiActionBtn.UseVisualStyleBackColor = true;
            this.AiActionBtn.Click += new System.EventHandler(this.AiActionBtn_Click);
            // 
            // PlaygroundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1666, 910);
            this.Controls.Add(this.AiActionBtn);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "PlaygroundForm";
            this.Text = "PlaygroundForm";
            this.Controls.SetChildIndex(this.AiActionBtn, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AiActionBtn;
    }
}
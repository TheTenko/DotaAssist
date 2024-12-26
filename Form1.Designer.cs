namespace DotaLauncher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gameTimeLabel = new Label();
            lvlLabel = new Label();
            roshanLabel = new Label();
            neutralLabel = new Label();
            wisdomRuneLabel = new Label();
            activeRuneLabel = new Label();
            bountyRuneLabel = new Label();
            networthLabel = new Label();
            SuspendLayout();
            // 
            // gameTimeLabel
            // 
            gameTimeLabel.AutoSize = true;
            gameTimeLabel.Location = new Point(12, 55);
            gameTimeLabel.Name = "gameTimeLabel";
            gameTimeLabel.Size = new Size(38, 15);
            gameTimeLabel.TabIndex = 0;
            gameTimeLabel.Text = "label1";
            // 
            // lvlLabel
            // 
            lvlLabel.AutoSize = true;
            lvlLabel.Location = new Point(12, 80);
            lvlLabel.Name = "lvlLabel";
            lvlLabel.Size = new Size(38, 15);
            lvlLabel.TabIndex = 1;
            lvlLabel.Text = "label2";
            // 
            // roshanLabel
            // 
            roshanLabel.AutoSize = true;
            roshanLabel.Location = new Point(12, 106);
            roshanLabel.Name = "roshanLabel";
            roshanLabel.Size = new Size(38, 15);
            roshanLabel.TabIndex = 2;
            roshanLabel.Text = "label3";
            // 
            // neutralLabel
            // 
            neutralLabel.AutoSize = true;
            neutralLabel.Location = new Point(12, 132);
            neutralLabel.Name = "neutralLabel";
            neutralLabel.Size = new Size(38, 15);
            neutralLabel.TabIndex = 3;
            neutralLabel.Text = "label4";
            // 
            // wisdomRuneLabel
            // 
            wisdomRuneLabel.AutoSize = true;
            wisdomRuneLabel.Location = new Point(12, 156);
            wisdomRuneLabel.Name = "wisdomRuneLabel";
            wisdomRuneLabel.Size = new Size(38, 15);
            wisdomRuneLabel.TabIndex = 4;
            wisdomRuneLabel.Text = "label5";
            // 
            // activeRuneLabel
            // 
            activeRuneLabel.AutoSize = true;
            activeRuneLabel.Location = new Point(12, 180);
            activeRuneLabel.Name = "activeRuneLabel";
            activeRuneLabel.Size = new Size(38, 15);
            activeRuneLabel.TabIndex = 5;
            activeRuneLabel.Text = "label1";
            // 
            // bountyRuneLabel
            // 
            bountyRuneLabel.AutoSize = true;
            bountyRuneLabel.Location = new Point(12, 206);
            bountyRuneLabel.Name = "bountyRuneLabel";
            bountyRuneLabel.Size = new Size(38, 15);
            bountyRuneLabel.TabIndex = 6;
            bountyRuneLabel.Text = "label2";
            // 
            // networthLabel
            // 
            networthLabel.AutoSize = true;
            networthLabel.Location = new Point(12, 232);
            networthLabel.Name = "networthLabel";
            networthLabel.Size = new Size(38, 15);
            networthLabel.TabIndex = 7;
            networthLabel.Text = "label2";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(309, 296);
            Controls.Add(networthLabel);
            Controls.Add(bountyRuneLabel);
            Controls.Add(activeRuneLabel);
            Controls.Add(wisdomRuneLabel);
            Controls.Add(neutralLabel);
            Controls.Add(roshanLabel);
            Controls.Add(lvlLabel);
            Controls.Add(gameTimeLabel);
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            TransparencyKey = Color.Transparent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label gameTimeLabel;
        private Label lvlLabel;
        private Label roshanLabel;
        private Label neutralLabel;
        private Label wisdomRuneLabel;
        private Label activeRuneLabel;
        private Label bountyRuneLabel;
        private Label networthLabel;
    }
}

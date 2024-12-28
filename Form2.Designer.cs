namespace DotaLauncher
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            playButton = new Button();
            infoLabel = new Label();
            versionLabel = new Label();
            settingsButton = new Button();
            SuspendLayout();
            // 
            // playButton
            // 
            resources.ApplyResources(playButton, "playButton");
            playButton.Name = "playButton";
            playButton.UseVisualStyleBackColor = true;
            playButton.Click += playButton_Click;
            // 
            // infoLabel
            // 
            resources.ApplyResources(infoLabel, "infoLabel");
            infoLabel.Name = "infoLabel";
            // 
            // versionLabel
            // 
            resources.ApplyResources(versionLabel, "versionLabel");
            versionLabel.Name = "versionLabel";
            // 
            // settingsButton
            // 
            resources.ApplyResources(settingsButton, "settingsButton");
            settingsButton.Name = "settingsButton";
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += settingsButton_Click;
            // 
            // Launcher
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(settingsButton);
            Controls.Add(versionLabel);
            Controls.Add(infoLabel);
            Controls.Add(playButton);
            Name = "Launcher";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button playButton;
        private Label infoLabel;
        private Label versionLabel;
        private Button settingsButton;
    }
}
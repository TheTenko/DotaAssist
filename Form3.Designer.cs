namespace DotaLauncher
{
    partial class Settings
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
            notificationSet = new CheckBox();
            roshanSet = new CheckBox();
            bountySet = new CheckBox();
            wisdomSet = new CheckBox();
            middleSet = new CheckBox();
            netwothSet = new CheckBox();
            game_timeSet = new CheckBox();
            lvlSet = new CheckBox();
            neutralSet = new CheckBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // notificationSet
            // 
            notificationSet.AutoSize = true;
            notificationSet.Location = new Point(12, 92);
            notificationSet.Name = "notificationSet";
            notificationSet.Size = new Size(157, 19);
            notificationSet.TabIndex = 0;
            notificationSet.Text = "Включить уведомления";
            notificationSet.UseVisualStyleBackColor = true;
            notificationSet.CheckedChanged += notificationSet_CheckedChanged;
            // 
            // roshanSet
            // 
            roshanSet.AutoSize = true;
            roshanSet.Location = new Point(12, 117);
            roshanSet.Name = "roshanSet";
            roshanSet.Size = new Size(171, 19);
            roshanSet.TabIndex = 1;
            roshanSet.Text = "Отключить таймер рошана";
            roshanSet.UseVisualStyleBackColor = true;
            roshanSet.CheckedChanged += roshanSet_CheckedChanged;
            // 
            // bountySet
            // 
            bountySet.AutoSize = true;
            bountySet.Location = new Point(12, 142);
            bountySet.Name = "bountySet";
            bountySet.Size = new Size(203, 19);
            bountySet.TabIndex = 2;
            bountySet.Text = "Отключить таймер рун богатства";
            bountySet.UseVisualStyleBackColor = true;
            bountySet.CheckedChanged += bountySet_CheckedChanged;
            // 
            // wisdomSet
            // 
            wisdomSet.AutoSize = true;
            wisdomSet.Location = new Point(12, 167);
            wisdomSet.Name = "wisdomSet";
            wisdomSet.Size = new Size(203, 19);
            wisdomSet.TabIndex = 3;
            wisdomSet.Text = "Отключить таймер рун мудрости";
            wisdomSet.UseVisualStyleBackColor = true;
            wisdomSet.CheckedChanged += wisdomSet_CheckedChanged;
            // 
            // middleSet
            // 
            middleSet.AutoSize = true;
            middleSet.Location = new Point(12, 192);
            middleSet.Name = "middleSet";
            middleSet.Size = new Size(202, 19);
            middleSet.TabIndex = 4;
            middleSet.Text = "Отключить таймер активных рун";
            middleSet.UseVisualStyleBackColor = true;
            middleSet.CheckedChanged += middleSet_CheckedChanged;
            // 
            // netwothSet
            // 
            netwothSet.AutoSize = true;
            netwothSet.Location = new Point(12, 217);
            netwothSet.Name = "netwothSet";
            netwothSet.Size = new Size(181, 19);
            netwothSet.TabIndex = 5;
            netwothSet.Text = "Отключить счетчик нетворса";
            netwothSet.UseVisualStyleBackColor = true;
            netwothSet.CheckedChanged += netwothSet_CheckedChanged;
            // 
            // game_timeSet
            // 
            game_timeSet.AutoSize = true;
            game_timeSet.Location = new Point(12, 242);
            game_timeSet.Name = "game_timeSet";
            game_timeSet.Size = new Size(263, 19);
            game_timeSet.TabIndex = 6;
            game_timeSet.Text = "Отключить отображение игрового времени";
            game_timeSet.UseVisualStyleBackColor = true;
            game_timeSet.CheckedChanged += game_timeSet_CheckedChanged;
            // 
            // lvlSet
            // 
            lvlSet.AutoSize = true;
            lvlSet.Location = new Point(12, 267);
            lvlSet.Name = "lvlSet";
            lvlSet.Size = new Size(200, 19);
            lvlSet.TabIndex = 7;
            lvlSet.Text = "Отключить отображение уровня";
            lvlSet.UseVisualStyleBackColor = true;
            lvlSet.CheckedChanged += lvlSet_CheckedChanged;
            // 
            // neutralSet
            // 
            neutralSet.AutoSize = true;
            neutralSet.Location = new Point(12, 292);
            neutralSet.Name = "neutralSet";
            neutralSet.Size = new Size(325, 19);
            neutralSet.TabIndex = 8;
            neutralSet.Text = "Отключить текущий уровень нейтральных предметов";
            neutralSet.UseVisualStyleBackColor = true;
            neutralSet.CheckedChanged += neutralSet_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic", 24F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(98, 9);
            label1.Name = "label1";
            label1.Size = new Size(306, 42);
            label1.TabIndex = 9;
            label1.Text = "Настройки";
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 450);
            Controls.Add(label1);
            Controls.Add(neutralSet);
            Controls.Add(lvlSet);
            Controls.Add(game_timeSet);
            Controls.Add(netwothSet);
            Controls.Add(middleSet);
            Controls.Add(wisdomSet);
            Controls.Add(bountySet);
            Controls.Add(roshanSet);
            Controls.Add(notificationSet);
            Name = "Settings";
            Text = "Form3";
            Load += Settings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox notificationSet;
        private CheckBox roshanSet;
        private CheckBox bountySet;
        private CheckBox wisdomSet;
        private CheckBox middleSet;
        private CheckBox netwothSet;
        private CheckBox game_timeSet;
        private CheckBox lvlSet;
        private CheckBox neutralSet;
        private Label label1;
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace DotaLauncher
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();


            LoadConfig();
        }
        private string steamPath;
        public const string ConfigFileName = "config.json";
        DotaAssistant overlay = new DotaAssistant();
        Settings settings = new Settings();

        private void playButton_Click(object sender, EventArgs e)
        {
            if (!IsProgramRunning("dota2"))
            {
                if (!string.IsNullOrEmpty(steamPath) && File.Exists(steamPath))
                {
                    try
                    {
                        Process.Start(steamPath, "-applaunch 570");
                        overlay.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка запуска Dota 2: {ex.Message}", "Ошибка");
                    }
                }
                else
                {
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Steam.exe|Steam.exe";
                        openFileDialog.Title = "Выберите Steam.exe";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            steamPath = openFileDialog.FileName;
                            SaveConfig(); // Сохраняем путь в конфигурационный файл
                            MessageBox.Show("Путь к Steam установлен.", "Информация");

                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Программа уже запущена");
                overlay.Show();
            }
        }
        public static bool IsProgramRunning(string dota2)
        {
            Process[] dota = Process.GetProcesses();
            // Проверяем, есть ли процесс с именем programName
            foreach (var process in dota)
            {
                if (process.ProcessName.Equals(dota2, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Программа запущена
                }
            }

            return false; // Программа не найдена
        }
        private void SaveConfig()
        {
            try
            {
                File.WriteAllText(ConfigFileName, steamPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения конфигурации: {ex.Message}", "Ошибка");
            }
        }
        private void LoadConfig()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    steamPath = File.ReadAllText(ConfigFileName);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки конфигурации: {ex.Message}", "Ошибка");
                }
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            if(!settings.IsDisposed) settings.Show();
        }
    }
}

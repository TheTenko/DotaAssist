using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using static DotaLauncher.DotaAssistant;

namespace DotaLauncher
{
    public partial class Settings : Form
    {
        private string saveFilePath = "settings.json";
        private List<CheckBox> checkBoxes = new List<CheckBox>();


        public Settings()
        {
            InitializeComponent();
            LoadCheckboxStates(); // Загружаем состояния чекбоксов при запуске

            this.FormClosing += Settings_FormClosing;

            checkBoxes.Add(notificationSet);
            checkBoxes.Add(roshanSet);
            checkBoxes.Add(bountySet);
            checkBoxes.Add(wisdomSet);
            checkBoxes.Add(middleSet);
            checkBoxes.Add(netwothSet);
            checkBoxes.Add(game_timeSet);
            checkBoxes.Add(lvlSet);
            checkBoxes.Add(neutralSet);


        }
        private void SaveCheckboxStates()
        {
            try
            {
                var states = checkBoxes.Select(cb => cb.Checked).ToList();
                var json = JsonSerializer.Serialize(states);
                File.WriteAllText(saveFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения состояния: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeDefaultStates()
        {
            foreach (var checkBox in checkBoxes)
            {
                checkBox.Checked = false;
            }

            SaveCheckboxStates();
        }

        // Загружаем состояния чекбоксов
        private void LoadCheckboxStates()
        {
            if (File.Exists(saveFilePath))
            {
                try
                {
                    var json = File.ReadAllText(saveFilePath);

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        InitializeDefaultStates();
                        return;
                    }

                    var states = JsonSerializer.Deserialize<List<bool>>(json);

                    if (states == null || states.Count == 0)
                    {
                        InitializeDefaultStates();
                        return;
                    }

                    // Отключаем обработку событий
                    foreach (var checkBox in checkBoxes)
                    {
                        checkBox.CheckedChanged -= CheckBox_CheckedChanged;
                    }

                    for (int i = 0; i < checkBoxes.Count; i++)
                    {
                        if (i < states.Count)
                            checkBoxes[i].Checked = states[i];
                    }

                    // Включаем обработку событий
                    foreach (var checkBox in checkBoxes)
                    {
                        checkBox.CheckedChanged += CheckBox_CheckedChanged;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки состояния: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InitializeDefaultStates();
                }
            }
            else
            {
                InitializeDefaultStates();
            }

        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Отменяем закрытие окна
            e.Cancel = true;
            SaveCheckboxStates();

            // Скрываем окно
            this.Hide();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            LoadCheckboxStates();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SaveCheckboxStates();
        }

        private void notificationSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                NotificationForm.SetNotificationsEnabled(checkBox.Checked);
            }
        }

        private void roshanSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetRoshanTimerEnabled(checkBox.Checked);
            }
        }

        private void bountySet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetBountyTimeEnabled(checkBox.Checked);
            }
        }

        private void wisdomSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetWisdomTimeEnabled(checkBox.Checked);
            }
        }

        private void middleSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetMiddleTimeEnabled(checkBox.Checked);
            }
        }

        private void netwothSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetNetworthEnabled(checkBox.Checked);
            }
        }

        private void game_timeSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetGameTimeEnabled(checkBox.Checked);
            }
        }

        private void lvlSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetHeroLevelEnabled(checkBox.Checked);
            }
        }

        private void neutralSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                // Включаем или выключаем уведомления в зависимости от состояния чекбокса
                DotaAssistant.SetNeutralEnabled(checkBox.Checked);
            }
        }
    }
}

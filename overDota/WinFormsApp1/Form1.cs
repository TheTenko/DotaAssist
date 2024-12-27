using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Dota2GSI;

namespace overlayDota
{
    public partial class DotaLauncher : Form
    {
        private string steamPath;
        private string gsiPath;
        private const string ConfigFileName = "config.json";
        private Button launchDotaButton;

        public DotaLauncher()
        {
            this.Text = "Dota 2 Launcher";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Кнопка выбора пути к Steam
            var selectSteamPathButton = new Button
            {
                Text = "Выбрать путь к Steam",
                Size = new Size(400, 40),
                Location = new Point(50, 30)
            };
            selectSteamPathButton.Click += SelectSteamPathButton_Click;
            this.Controls.Add(selectSteamPathButton);

            // Поле для ввода пути к GSI
            var gsiPathLabel = new Label
            {
                Text = "Путь к GSI директории:",
                Location = new Point(50, 90),
                Size = new Size(400, 20)
            };
            this.Controls.Add(gsiPathLabel);

            var gsiPathTextBox = new TextBox
            {
                Location = new Point(50, 120),
                Size = new Size(400, 30)
            };
            this.Controls.Add(gsiPathTextBox);

            var selectGsiPathButton = new Button
            {
                Text = "Выбрать GSI директорию",
                Size = new Size(400, 40),
                Location = new Point(50, 170)
            };
            selectGsiPathButton.Click += (sender, e) =>
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        gsiPath = folderBrowserDialog.SelectedPath;
                        gsiPathTextBox.Text = gsiPath;
                        SaveConfig();
                        MessageBox.Show("Путь к GSI директории установлен.", "Информация");
                    }
                }
            };
            this.Controls.Add(selectGsiPathButton);

            // Кнопка запуска Dota 2
            launchDotaButton = new Button
            {
                Text = "Запустить Dota 2",
                Size = new Size(400, 40),
                Location = new Point(50, 220),
                Enabled = false
            };
            launchDotaButton.Click += LaunchDotaButton_Click;
            this.Controls.Add(launchDotaButton);

            LoadConfig();
        }

        private void LaunchDotaButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(steamPath) && File.Exists(steamPath))
            {
                if (!string.IsNullOrEmpty(gsiPath) && Directory.Exists(gsiPath))
                {
                    try
                    {
                        Process.Start(steamPath, "-applaunch 570");
                        CreateGsiConfigFile();
                        StartOverlay();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка запуска Dota 2: {ex.Message}", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Путь к GSI директории не задан или недействителен.", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Путь к Steam не задан или файл отсутствует.", "Ошибка");
            }
        }

        private void SelectSteamPathButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Steam.exe|Steam.exe";
                openFileDialog.Title = "Выберите Steam.exe";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    steamPath = openFileDialog.FileName;
                    SaveConfig();
                    MessageBox.Show("Путь к Steam установлен.", "Информация");
                    launchDotaButton.Enabled = true;
                }
            }
        }

        private void CreateGsiConfigFile()
        {
            string configFilePath = Path.Combine(gsiPath, "gamestate_integration_overlay.cfg");
            string configContent = @"{
            ""uri""          ""http://localhost:3001/""
            ""timeout""      ""5.0""
            ""buffer""       ""0.1""
            ""throttle""     ""0.1""
            ""heartbeat""    ""10.0""
            ""data""
            {
                ""buildings""     ""1""
                ""provider""      ""1""
                ""map""           ""1""
                ""player""        ""1""
                ""hero""          ""1""
                ""abilities""     ""1""
                ""items""         ""1""
                ""draft""         ""1""
                ""wearables""     ""1""
            }
        }";
            File.WriteAllText(configFilePath, configContent, Encoding.UTF8);
        }

        private void StartOverlay()
        {
            Thread overlayThread = new Thread(() =>
            {
                Application.Run(new DotaOverlay());
            });
            overlayThread.IsBackground = true;
            overlayThread.Start();
        }

        private void LoadConfig()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ConfigFileName));
                    steamPath = config.ContainsKey("steamPath") ? config["steamPath"] : null;
                    gsiPath = config.ContainsKey("gsiPath") ? config["gsiPath"] : null;

                    if (!string.IsNullOrEmpty(steamPath) && File.Exists(steamPath))
                    {
                        launchDotaButton.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки конфигурации: {ex.Message}", "Ошибка");
                }
            }
        }

        private void SaveConfig()
        {
            try
            {
                var config = new Dictionary<string, string>
                {
                    { "steamPath", steamPath },
                    { "gsiPath", gsiPath }
                };
                File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения конфигурации: {ex.Message}", "Ошибка");
            }
        }
    }

    public class DotaOverlay : Form
    {
        private System.Windows.Forms.Timer updateTimer;
        private HttpListener gsiListener;
        private Thread gsiThread;
        private dynamic gameState;

        // Метки для отображения данных
        private Label goldPerMinuteLabel;
        private Label gameTimeLabel;
        private Label heroLevelLabel;

        // Таймеры событий
        private Label roshanTimerLabel;
        private Label bountyRuneTimerLabel;
        private Label wisdomRuneTimerLabel;
        private Label lotusTimerLabel;
        private Label tormentorTimerLabel;
        private Label neutralItemsTierLabel;

        // GSL


        public DotaOverlay()
        {


            this.Text = "Dota 2 Overlay";
            this.Size = new Size(350, 400);
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.Opacity = 0.8;

            // Кнопка закрытия окна
            Button closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(80, 30),
                Location = new Point(10, 10)
            };

            // Делаем окно плавающим
            this.ShowInTaskbar = false;

            // Обработка нажатий клавиш
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;

            closeButton.Click += (s, e) => this.Close();
            this.Controls.Add(closeButton);

            // Метки для интерфейса
            goldPerMinuteLabel = CreateLabel("Золото в минуту: 0", new Point(20, 50));
            gameTimeLabel = CreateLabel("Игровое время: 00:00", new Point(20, 80));
            heroLevelLabel = CreateLabel("Уровень героя: 0", new Point(20, 110));

            roshanTimerLabel = CreateLabel("Рошан: 00:00", new Point(20, 140));
            bountyRuneTimerLabel = CreateLabel("Руны богатства: 00:00", new Point(20, 170));
            wisdomRuneTimerLabel = CreateLabel("Руны мудрости: 00:00", new Point(20, 200));
            lotusTimerLabel = CreateLabel("Лотус: 00:00", new Point(20, 230));
            tormentorTimerLabel = CreateLabel("Терзатель: 00:00", new Point(20, 260));
            neutralItemsTierLabel = CreateLabel("Текущий тир нейтральных предметов: 1", new Point(20, 290));

            this.Controls.Add(goldPerMinuteLabel);
            this.Controls.Add(gameTimeLabel);
            this.Controls.Add(heroLevelLabel);
            this.Controls.Add(roshanTimerLabel);
            this.Controls.Add(bountyRuneTimerLabel);
            this.Controls.Add(wisdomRuneTimerLabel);
            this.Controls.Add(lotusTimerLabel);
            this.Controls.Add(tormentorTimerLabel);
            this.Controls.Add(neutralItemsTierLabel);

            // Таймер обновления интерфейса
            updateTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            updateTimer.Tick += UpdateUI;
            updateTimer.Start();

            // Запуск GSI-сервера
            StartGsiServer();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                this.Close(); // Закрываем окно
            }
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = location,
                AutoSize = true
            };
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            if (gameState == null) return;

            try
            {
                // Золото в минуту
                if (gameState.player != null && gameState.player.gpm != null)
                {
                    goldPerMinuteLabel.Text = $"Золото в минуту: {gameState.player.gpm}";
                }

                // Игровое время
                if (gameState.map != null && gameState.map.clock_time != null)
                {
                    int clockTime = (int)gameState.map.clock_time;
                    string formattedTime = TimeSpan.FromSeconds(Math.Abs(clockTime)).ToString(@"mm\:ss");
                    gameTimeLabel.Text = clockTime >= 0
                        ? $"Игровое время: {formattedTime}"
                        : $"До начала: {formattedTime}";

                    // Таймеры событий
                    UpdateTimers(clockTime);
                }

                // Уровень героя
                if (gameState.hero != null && gameState.hero.level != null)
                {
                    heroLevelLabel.Text = $"Уровень героя: {gameState.hero.level}";
                }

                // Нейтральные предметы
                neutralItemsTierLabel.Text = $"Текущий тир нейтральных предметов: {GetNeutralItemTier((int)gameState.map.clock_time)}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления UI: {ex.Message}");
            }
        }

        private void UpdateTimers(int clockTime)
        {
            // Рошан: 8–11 минут
            TimeSpan roshanRespawn = TimeSpan.FromSeconds(Math.Max(0, 8 * 60 - clockTime));
            roshanTimerLabel.Text = $"Рошан: {roshanRespawn:mm\\:ss}";

            // Руны богатства: каждые 3 минуты
            TimeSpan bountyRune = TimeSpan.FromSeconds(Math.Max(0, 180 - (clockTime % 180)));
            bountyRuneTimerLabel.Text = $"Руны богатства: {bountyRune:mm\\:ss}";

            // Руны мудрости: каждые 7 минут
            TimeSpan wisdomRune = TimeSpan.FromSeconds(Math.Max(0, 420 - (clockTime % 420)));
            wisdomRuneTimerLabel.Text = $"Руны мудрости: {wisdomRune:mm\\:ss}";

            // Лотус: каждые 3 минуты
            TimeSpan lotus = TimeSpan.FromSeconds(Math.Max(0, 180 - (clockTime % 180)));
            lotusTimerLabel.Text = $"Лотус: {lotus:mm\\:ss}";

            // Терзатель: каждые 10 минут
            TimeSpan tormentor = TimeSpan.FromSeconds(Math.Max(0, 600 - (clockTime % 600)));
            tormentorTimerLabel.Text = $"Терзатель: {tormentor:mm\\:ss}";
        }

        private int GetNeutralItemTier(int clockTime)
        {
            if (clockTime >= 60 * 27) return 5; // 27:00 и дальше
            if (clockTime >= 60 * 17) return 4; // 17:00 – 27:00
            if (clockTime >= 60 * 7) return 3;  // 7:00 – 17:00
            if (clockTime >= 60 * 0) return 2;  // 0:00 – 7:00
            return 1;                           // До 0:00
        }

        private void StartGsiServer()
        {
            gsiListener = new HttpListener();
            gsiListener.Prefixes.Add("http://localhost:3000/");
            gsiThread = new Thread(() =>
            {
                gsiListener.Start();
                while (true)
                {
                    var context = gsiListener.GetContext();
                    var request = context.Request;
                    using (var reader = new System.IO.StreamReader(request.InputStream))
                    {
                        string json = reader.ReadToEnd();
                        gameState = JsonConvert.DeserializeObject(json);
                    }
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                }
            });
            gsiThread.IsBackground = true;
            gsiThread.Start();
        }
    }
}
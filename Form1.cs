using System.Security.Permissions;
using Dota2GSI;
using Dota2GSI.EventMessages;
using Dota2GSI.Nodes;
using Dota2GSI.Utils;
using Dota2GSI.Nodes.RoshanProvider;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Media;
using Timer = System.Windows.Forms.Timer;

namespace DotaLauncher
{
    
    public partial class DotaAssistant : Form
    {
        // GSI
        static GameStateListener gsl;

        private DateTime roshanKillTime; // Время убийства Рошана
        private TimeSpan SpawnDelay = TimeSpan.FromMinutes(11);
        private Timer updateTimer;
        bool roshanAlive = true;

        string lvl = "Ваш уровень - ";
        string roshan = "Рошан ";
        string wisdom = "Время до руны мудрости - ";
        string active = "Время до рун на реке - ";
        string bounty = "Время до руны богатства - ";
        string time = "Игровое время - ";
        string neutral = "Уровень нейтральных предметов - ";
        string networth = "Нетворс - ";

        private static bool showRoshanTimer = true;
        private static bool showBountyRuneTimers = true;
        private static bool showMiddleRuneTimers = true;
        private static bool showGameTime = true;
        private static bool showWisdomTime = true;
        private static bool showHeroLevel = true;
        private static bool showNetworth = true;
        private static bool showNeutral = true;

        public DotaAssistant()
        {
            // Listener
            gsl = new GameStateListener(4000);

            // Timer
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;

            // Creating cfg file
            if (!gsl.GenerateGSIConfigFile("Config")) MessageBox.Show("Ошибка при создании конфигурационного файла");
            if (!gsl.Start()) MessageBox.Show("Невозможно запустить чтение потока, запустите приложение от имени администратора");
            MessageBox.Show("Программа работает, можете запускать Dota 2");

            gsl.NewGameState += OnNewGameState;
            this.FormClosing += Overlay_FormClosing;
            InitializeComponent();
        }
        private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Отменяем закрытие окна
            e.Cancel = true;
            
            // Скрываем окно
            this.Hide();
        }
        public async void OnNewGameState(GameState gs)
        {
            int mMinutes = 0;
            int mSeconds = 0;
            int bMinutes = 0;
            int bSeconds = 0;
            int wMinutes = 0;
            int wSeconds = 0;
            // Networth
            if (showNetworth)
            {
                int net = gs.Hero.LocalPlayer.BuybackCost * 13 - 200 * 13;
                networthLabel.Text = networth + net;
            }
            else networthLabel.Text = "Вы выключили данную опцию";

            // Hero level
            if (showHeroLevel) lvlLabel.Text = lvl + gs.Hero.LocalPlayer.Level;
            else lvlLabel.Text = "Вы выключили данную опцию";

            // Roshan Spawn
            if (showRoshanTimer)
            {
                foreach (var game_event in gs.Events)
                {
                    if (game_event.EventType == Dota2GSI.Nodes.EventsProvider.EventType.Roshan_killed)
                    {
                        roshanAlive = false;
                        break;
                    }

                }
                if (!roshanAlive)
                {
                    roshanKillTime = DateTime.Now;
                    DateTime SpawnTime = roshanKillTime + SpawnDelay;
                    updateTimer.Start();
                }
            }
            else roshanLabel.Text = "Вы выключили данную опцию";

            // Neutral Items
            if (showNeutral)
            {
                if (gs.Map.ClockTime >= 60 * 17)
                {
                    neutralLabel.Text = neutral + 2;
                    if (gs.Map.ClockTime >= 60 * 27)
                    {
                        neutralLabel.Text = neutral + 3;
                        if (gs.Map.ClockTime >= 60 * 37)
                        {
                            neutralLabel.Text = neutral + 4;
                            if (gs.Map.ClockTime >= 60 * 60)
                            {
                                neutralLabel.Text = neutral + 5;
                            }
                        }
                    }
                }
                else neutralLabel.Text = neutral + 1;
            }
            else neutralLabel.Text = "Вы выключили данную опцию";

            // Wisdom Runes
            if (showWisdomTime)
            {
                TimeSpan wisdomTime = TimeSpan.FromSeconds(Math.Max(0, 420 - (gs.Map.ClockTime % 420)));
                wMinutes = (int)wisdomTime.TotalMinutes;
                wSeconds = wisdomTime.Seconds;
                if (wSeconds < 10) wisdomRuneLabel.Text = wisdom + wMinutes + ":0" + wSeconds;
                else wisdomRuneLabel.Text = wisdom + wMinutes + ":" + wSeconds;
            }
            else wisdomRuneLabel.Text = "Вы выключили данную опцию";


            // Middle Runes
            if (showMiddleRuneTimers)
            {
                TimeSpan middleTime = TimeSpan.FromSeconds(Math.Max(0, 120 - (gs.Map.ClockTime % 120)));
                mMinutes = (int)middleTime.TotalMinutes;
                wSeconds = middleTime.Seconds;
                if (mSeconds < 10) activeRuneLabel.Text = active + mMinutes + ":0" + mSeconds;
                else activeRuneLabel.Text = active + mMinutes + ":" + mSeconds;
            }
            else activeRuneLabel.Text = "Вы выключили данную опцию";

            // Bounty Runes
            if (showBountyRuneTimers)
            {
                TimeSpan bountyTime = TimeSpan.FromSeconds(Math.Max(0, 180 - (gs.Map.ClockTime % 180)));
                bMinutes = (int)bountyTime.TotalMinutes;
                wSeconds = bountyTime.Seconds;
                if (bSeconds < 10) bountyRuneLabel.Text = bounty + bMinutes + ":0" + bSeconds;
                else bountyRuneLabel.Text = bounty + bMinutes + ":" + bSeconds;
            }
            else bountyRuneLabel.Text = "Вы выключили данную опцию";

            // Map Time
            if (showGameTime)
            {
                var ConvertTime = TimeSpan.FromSeconds(gs.Map.ClockTime);
                int minutes = (int)ConvertTime.TotalMinutes;
                int seconds = ConvertTime.Seconds;
                if (seconds < 10) gameTimeLabel.Text = time + minutes + ":0" + seconds;
                else gameTimeLabel.Text = time + minutes + ":" + seconds;
            }
            else gameTimeLabel.Text = "Вы выключили данную опцию";

            // Notification
            if (mMinutes == 0 && mSeconds == 30)
            {
                string message = "Активная руна появится через 30 секунд!";
                NotificationForm.ShowNotification(message);
            }
            if (bMinutes == 0 && bSeconds == 30)
            {
                string message = "Руна богатства появится через 30 секунд!";
                NotificationForm.ShowNotification(message);
            }
            if (wMinutes == 0 && wSeconds == 30)
            {
                string message = "Руна мудрости появится через 30 секунд!";
                NotificationForm.ShowNotification(message);
            }
        }

        public static void SetRoshanTimerEnabled(bool enabled)
        {
            showRoshanTimer = enabled;
        }
        public static void SetBountyTimeEnabled(bool enabled)
        {
            showBountyRuneTimers = enabled;
        }
        public static void SetMiddleTimeEnabled(bool enabled)
        {
            showMiddleRuneTimers = enabled;
        }
        public static void SetGameTimeEnabled(bool enabled)
        {
            showGameTime = enabled;
        }
        public static void SetWisdomTimeEnabled(bool enabled)
        {
            showWisdomTime = enabled;
        }
        public static void SetHeroLevelEnabled(bool enabled)
        {
            showHeroLevel = enabled;
        }
        public static void SetNetworthEnabled(bool enabled)
        {
            showNetworth = enabled;
        }
        public static void SetNeutralEnabled(bool enabled)
        {
            showNeutral = enabled;
        }


        private static bool notificationsEnabled = true;
        
        // Notification Class
        public class NotificationForm : Form
        {
            private Timer closeTimer; // Таймер для автоматического закрытия
            private Label messageLabel; // Метка для текста уведомления

            public NotificationForm(string message, string soundPath = null)
            {
                if (!notificationsEnabled)
                {
                    this.Close();
                    return;
                }

                // Настройки окна
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Size = new Size(300, 100);
                this.TopMost = true; // Окно всегда поверх других

                // Настройка фона и текста
                this.BackColor = Color.FromArgb(30, 30, 30);
                this.Opacity = 0.9;

                messageLabel = new Label
                {
                    Text = message,
                    ForeColor = Color.White,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 12, FontStyle.Bold)
                };
                this.Controls.Add(messageLabel);

                // Воспроизведение звукового эффекта, если передан путь к звуку
                if (!string.IsNullOrEmpty(soundPath) && File.Exists(soundPath))
                {
                    try
                    {
                        SoundPlayer soundPlayer = new SoundPlayer(soundPath);
                        soundPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка воспроизведения звука: {ex.Message}");
                    }
                }
                else
                {
                    // Воспроизводим системный звук по умолчанию
                    SystemSounds.Beep.Play();
                }

                // Настройка таймера для автоматического закрытия
                closeTimer = new Timer();
                closeTimer.Interval = 5000; // 3 секунды
                closeTimer.Tick += CloseTimer_Tick;
                closeTimer.Start();

            }

            private void CloseTimer_Tick(object sender, EventArgs e)
            {
                closeTimer.Stop(); // Останавливаем таймер
                this.Close(); // Закрываем окно
            }

            public static void ShowNotification(string message, string soundPath = null)
            {
                if (notificationsEnabled)  // Проверяем, разрешены ли уведомления
                {
                    // Запускаем уведомление в отдельном потоке
                    NotificationForm notification = new NotificationForm(message, soundPath);
                    notification.Show();
                }
            }
            public static void SetNotificationsEnabled(bool enabled)
            {
                notificationsEnabled = enabled;
            }
        }

        // Timer Class
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (roshanKillTime == DateTime.MinValue) return;

            DateTime now = DateTime.Now;
            TimeSpan timeUntilSpawn = (roshanKillTime + SpawnDelay) - now;

            roshanLabel.Text = timeUntilSpawn.TotalSeconds > 0
                ? $"Cпавн через: {timeUntilSpawn:mm\\:ss}"
                : "Рошан жив";

            if (timeUntilSpawn.TotalSeconds <= 0)
            {
                string message = "Рошан возродился!";
                roshanAlive = true;
                NotificationForm.ShowNotification(message);
                updateTimer.Stop();
            }
        }
        public static void SetNotificationsEnabled(bool enabled)
        {
            notificationsEnabled = enabled;
        }
    }
}

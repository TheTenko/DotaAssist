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
    public partial class Form1 : Form
    {
        // GSI
        static GameStateListener gsl;

        private DateTime roshanKillTime; // ����� �������� ������
        private TimeSpan SpawnDelay = TimeSpan.FromMinutes(11);
        private Timer updateTimer;
        bool roshanAlive = true;

        public Form1()
        {
            // Listener
            gsl = new GameStateListener(4000);

            // Timer
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;

            // Creating cfg file
            if (!gsl.GenerateGSIConfigFile("Config")) MessageBox.Show("������ ��� �������� ����������������� �����");
            if (!gsl.Start()) MessageBox.Show("���������� ��������� ������ ������, ��������� ���������� �� ����� ��������������");
            MessageBox.Show("��������� ��������, ������ ��������� Dota 2");

            gsl.NewGameState += OnNewGameState;
            InitializeComponent();
        }
        public async void OnNewGameState(GameState gs)
        {
            string lvl = "��� ������� - ";
            string roshan = "����� ";
            string wisdom = "����� �� ���� �������� - ";
            string active = "����� �� ��� �� ���� - ";
            string bounty = "����� �� ���� ��������� - ";
            string time = "������� ����� - ";
            string neutral = "������� ����������� ��������� - ";
            string networth = "������� - ";


            // Networth
            /*int net = gs.Player.LocalPlayer.GoldFromIncome +
                gs.Player.LocalPlayer.GoldFromCreepKills +
                gs.Player.LocalPlayer.GoldFromHeroKills +
                gs.Player.LocalPlayer.ItemGoldSpent + 
                gs.Player.LocalPlayer.GoldFromShared +
                gs.Player.LocalPlayer.GoldReliable +
                gs.Player.LocalPlayer.GoldUnreliable +
                gs.Player.LocalPlayer.SupportGoldSpent +
                gs.Player.LocalPlayer.ConsumableGoldSpent +
                gs.Player.LocalPlayer.ItemGoldSpent +
                600;*/
            int net = gs.Hero.LocalPlayer.BuybackCost * 13 - 200 * 13;
            networthLabel.Text = networth + net;

            // Hero level
            lvlLabel.Text = lvl + gs.Hero.LocalPlayer.Level;

            // Roshan Spawn
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

            // Neutral Items
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

            // Wisdom Runes
            TimeSpan wisdomTime = TimeSpan.FromSeconds(Math.Max(0, 420 - (gs.Map.ClockTime % 420)));
            int wMinutes = (int)wisdomTime.TotalMinutes;
            int wSeconds = wisdomTime.Seconds;
            if (wSeconds < 10) wisdomRuneLabel.Text = wisdom + wMinutes + ":0" + wSeconds;
            else wisdomRuneLabel.Text = wisdom + wMinutes + ":" + wSeconds;


            // Middle Runes
            TimeSpan middleTime = TimeSpan.FromSeconds(Math.Max(0, 120 - (gs.Map.ClockTime % 120)));
            int mMinutes = (int)middleTime.TotalMinutes;
            int mSeconds = middleTime.Seconds;
            if (mSeconds < 10) activeRuneLabel.Text = active + mMinutes + ":0" + mSeconds;
            else activeRuneLabel.Text = active + mMinutes + ":" + mSeconds;

            // Bounty Runes
            TimeSpan bountyTime = TimeSpan.FromSeconds(Math.Max(0, 180 - (gs.Map.ClockTime % 180)));
            int bMinutes = (int)bountyTime.TotalMinutes;
            int bSeconds = bountyTime.Seconds;
            if (bSeconds < 10) bountyRuneLabel.Text = bounty + bMinutes + ":0" + bSeconds;
            else bountyRuneLabel.Text = bounty + bMinutes + ":" + bSeconds;

            // Map Time
            var ConvertTime = TimeSpan.FromSeconds(gs.Map.ClockTime);
            int minutes = (int)ConvertTime.TotalMinutes;
            int seconds = ConvertTime.Seconds;
            if (seconds < 10) gameTimeLabel.Text = time + minutes + ":0" + seconds;
            else gameTimeLabel.Text = time + minutes + ":" + seconds;

            // Notification
            if (mMinutes == 0 && mSeconds == 30)
            {
                string message = "�������� ���� �������� ����� 30 ������!";
                NotificationForm.ShowNotification(message);
            }
            if (bMinutes == 0 && bSeconds == 30)
            {
                string message = "���� ��������� �������� ����� 30 ������!";
                NotificationForm.ShowNotification(message);
            }
            if (wMinutes == 0 && wSeconds == 30)
            {
                string message = "���� �������� �������� ����� 30 ������!";
                NotificationForm.ShowNotification(message);
            }
        }

        // Notification Class
        public class NotificationForm : Form
        {
            private Timer closeTimer; // ������ ��� ��������������� ��������
            private Label messageLabel; // ����� ��� ������ �����������

            public NotificationForm(string message, string soundPath = null)
            {
                // ��������� ����
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Size = new Size(300, 100);
                this.TopMost = true; // ���� ������ ������ ������

                // ��������� ���� � ������
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

                // ��������������� ��������� �������, ���� ������� ���� � �����
                if (!string.IsNullOrEmpty(soundPath) && File.Exists(soundPath))
                {
                    try
                    {
                        SoundPlayer soundPlayer = new SoundPlayer(soundPath);
                        soundPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"������ ��������������� �����: {ex.Message}");
                    }
                }
                else
                {
                    // ������������� ��������� ���� �� ���������
                    SystemSounds.Beep.Play();
                }

                // ��������� ������� ��� ��������������� ��������
                closeTimer = new Timer();
                closeTimer.Interval = 5000; // 3 �������
                closeTimer.Tick += CloseTimer_Tick;
                closeTimer.Start();
            }

            private void CloseTimer_Tick(object sender, EventArgs e)
            {
                closeTimer.Stop(); // ������������� ������
                this.Close(); // ��������� ����
            }

            public static void ShowNotification(string message, string soundPath = null)
            {
                // ��������� ����������� � ��������� ������
                NotificationForm notification = new NotificationForm(message, soundPath);
                notification.Show();
            }
        }

        // Timer Class
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (roshanKillTime == DateTime.MinValue) return;

            DateTime now = DateTime.Now;
            TimeSpan timeUntilSpawn = (roshanKillTime + SpawnDelay) - now;

            roshanLabel.Text = timeUntilSpawn.TotalSeconds > 0
                ? $"C���� �����: {timeUntilSpawn:mm\\:ss}"
                : "����� ���";

            if (timeUntilSpawn.TotalSeconds <= 0)
            {
                string message = "����� ����������!";
                roshanAlive = true;
                NotificationForm.ShowNotification(message);
                updateTimer.Stop();
            }
        }
    }
}

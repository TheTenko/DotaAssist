using System.Security.Permissions;
using Dota2GSI;
using Dota2GSI.EventMessages;
using Dota2GSI.Nodes;
using Dota2GSI.Utils;
using Dota2GSI.Nodes.RoshanProvider;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DotaLauncher
{
    public partial class Form1 : Form
    {
        // GSI
        static GameStateListener gsl;

        public Form1()
        {
            // Listener
            gsl = new GameStateListener(4000);

            // Creating cfg file
            if (!gsl.GenerateGSIConfigFile("Config")) MessageBox.Show("Ошибка при создании конфигурационного файла");
            if (!gsl.Start()) MessageBox.Show("Невозможно запустить чтение потока, запустите приложение от имени администратора");
            MessageBox.Show("Программа работает, можете запускать Dota 2");

            gsl.NewGameState += OnNewGameState;
            InitializeComponent();
        }
        public async void OnNewGameState(GameState gs)
        {
            string lvl = "Ваш уровень - ";
            string roshan = "Рошан ";
            string wisdom = "Время до руны мудрости";
            string active = "Время до рун на реке - ";
            string bounty = "Время до руны богатства - ";
            string time = "Игровое время - ";
            string neutral = "Уровень нейтральных предметов - ";
            string networth = "Нетворс - ";
            bool roshanAlive = true;

            // Timer
            var start = DateTime.Now;
            var end = start.AddSeconds(600);
            var diff = TimeSpan.FromSeconds(600);

            // Networth
            int net = 0;

            net = gs.Player.LocalPlayer.GoldFromIncome +
                gs.Player.LocalPlayer.GoldFromCreepKills +
                gs.Player.LocalPlayer.GoldFromHeroKills +
                gs.Player.LocalPlayer.ItemGoldSpent;
            networthLabel.Text = networth + net;

            // Hero level
            lvlLabel.Text = lvl + gs.Hero.LocalPlayer.Level;

            // Roshan Spawn
            roshanLabel.Text = roshan + gs.Map.RoshanState+" "+gs.Roshan.PhaseTimeRemaining;

            // Neutral Items


            // Wisdom Runes

            // Active Runes

            // Bounty Runes

            // Map Time
            var ConvertTime = TimeSpan.FromSeconds(gs.Map.ClockTime);
            int minutes = (int)ConvertTime.TotalMinutes;
            int seconds = ConvertTime.Seconds;
            if (seconds < 10) gameTimeLabel.Text = time + minutes + ":0" + seconds;
            else gameTimeLabel.Text = time + minutes + ":" + seconds;
        }
    }
}

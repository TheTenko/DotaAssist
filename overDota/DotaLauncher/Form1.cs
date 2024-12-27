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
using Dota2GSI.EventMessages;

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
        static GameStateListener gsl;

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

            // GSL
            gsl = new GameStateListener(4000);
            gsl.GameEvent += OnGameEvent;
            gsl.RoshanUpdated += OnRoshanState;

            // Запуск GSI-сервера
            StartGsiServer();
        }

        private static void OnGameEvent(DotaGameEvent game_event)
        {

        }
        private static void OnRoshanState(DotaGameEvent game_event)
        {
            

            
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


namespace Dota2GSI_Example_program
{
    class Program
    {
        static GameStateListener _gsl;

        static void Main(string[] args)
        {
            _gsl = new GameStateListener(4000);

            if (!_gsl.GenerateGSIConfigFile("Example"))
            {
                Console.WriteLine("Could not generate GSI configuration file.");
            }

            // There are many callbacks that can be subscribed.
            // This example shows a few.
            // _gsl.NewGameState += OnNewGameState; // `NewGameState` can be used alongside `GameEvent`. Just not in this example.
            _gsl.GameEvent += OnGameEvent; // `GameEvent` can be used alongside `NewGameState`.
            _gsl.TimeOfDayChanged += OnTimeOfDayChanged;
            _gsl.TeamScoreChanged += OnTeamScoreChanged;
            _gsl.PauseStateChanged += OnPauseStateChanged;
            _gsl.PlayerGameplayEvent += OnPlayerGameplayEvent;
            _gsl.TeamGameplayEvent += OnTeamGameplayEvent;
            _gsl.InventoryItemAdded += OnInventoryItemAdded;
            _gsl.InventoryItemRemoved += OnInventoryItemRemoved;

            if (!_gsl.Start())
            {
                Console.WriteLine("GameStateListener could not start. Try running this program as Administrator. Exiting.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            Console.WriteLine("Listening for game integration calls...");

            Console.WriteLine("Press ESC to quit");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(1000);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static void OnTimeOfDayChanged(TimeOfDayChanged game_event)
        {
            Console.WriteLine($"Is daytime: {game_event.IsDaytime} Is Nightstalker night: {game_event.IsNightstalkerNight}");
        }
        private static void OnTeamScoreChanged(TeamScoreChanged game_event)
        {
            Console.WriteLine($"New score for {game_event.Team} is {game_event.New}");
        }

        private static void OnPauseStateChanged(PauseStateChanged game_event)
        {
            Console.WriteLine($"New pause state is {(game_event.New ? "paused" : "not paused")}");
        }

        private static void OnPlayerGameplayEvent(PlayerGameplayEvent game_event)
        {
            Console.WriteLine($"Player {game_event.Player.Details.Name} did a thing: " + game_event.Value.EventType);
        }

        private static void OnTeamGameplayEvent(TeamGameplayEvent game_event)
        {
            Console.WriteLine($"Team {game_event.Team} did a thing: " + game_event.Value.EventType);
        }

        private static void OnInventoryItemAdded(InventoryItemAdded game_event)
        {
            Console.WriteLine($"Player {game_event.Player.Details.Name} gained an item in their inventory: " + game_event.Value.Name);
        }

        private static void OnInventoryItemRemoved(InventoryItemRemoved game_event)
        {
            Console.WriteLine($"Player {game_event.Player.Details.Name} lost an item from their inventory: " + game_event.Value.Name);
        }

        private static void OnGameEvent(DotaGameEvent game_event)
        {
            if (game_event is ProviderUpdated provider)
            {
                Console.WriteLine($"Current Game version: {provider.New.Version}");
                Console.WriteLine($"Current Game time stamp: {provider.New.TimeStamp}");
            }
            else if (game_event is PlayerDetailsChanged player_details)
            {
                Console.WriteLine($"Player Name: {player_details.New.Name}");
                Console.WriteLine($"Player Account ID: {player_details.New.AccountID}");
            }
            else if (game_event is HeroDetailsChanged hero_details)
            {
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero ID: " + hero_details.New.ID);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero XP: " + hero_details.New.Experience);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero has Aghanims Shard upgrade: " + hero_details.New.HasAghanimsShardUpgrade);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Health: " + hero_details.New.Health);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Mana: " + hero_details.New.Mana);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Location: " + hero_details.New.Location);
            }
            else if (game_event is AbilityUpdated ability)
            {
                Console.WriteLine($"Player {ability.Player.Details.Name} updated their ability: " + ability.New);
            }
            else if (game_event is TowerUpdated tower_updated)
            {
                if (tower_updated.New.Health < tower_updated.Previous.Health)
                {
                    Console.WriteLine($"{tower_updated.Team} {tower_updated.Location} tower is under attack! Health: " + tower_updated.New.Health);
                }
                else if (tower_updated.New.Health > tower_updated.Previous.Health)
                {
                    Console.WriteLine($"{tower_updated.Team} {tower_updated.Location} tower is being healed! Health: " + tower_updated.New.Health);
                }
            }
            else if (game_event is TowerDestroyed tower_destroyed)
            {
                Console.WriteLine($"{tower_destroyed.Team} {tower_destroyed.Location} tower is destroyed!");
            }
            else if (game_event is RacksUpdated racks_updated)
            {
                if (racks_updated.New.Health < racks_updated.Previous.Health)
                {
                    Console.WriteLine($"{racks_updated.Team} {racks_updated.Location} {racks_updated.RacksType} racks are under attack! Health: " + racks_updated.New.Health);
                }
                else if (racks_updated.New.Health > racks_updated.Previous.Health)
                {
                    Console.WriteLine($"{racks_updated.Team} {racks_updated.Location} {racks_updated.RacksType} tower are being healed! Health: " + racks_updated.New.Health);
                }
            }
            else if (game_event is RacksDestroyed racks_destroyed)
            {
                Console.WriteLine($"{racks_destroyed.Team} {racks_destroyed.Location} {racks_destroyed.RacksType} racks is destroyed!");
            }
            else if (game_event is AncientUpdated ancient_updated)
            {
                if (ancient_updated.New.Health < ancient_updated.Previous.Health)
                {
                    Console.WriteLine($"{ancient_updated.Team} ancient is under attack! Health: " + ancient_updated.New.Health);
                }
                else if (ancient_updated.New.Health > ancient_updated.Previous.Health)
                {
                    Console.WriteLine($"{ancient_updated.Team} ancient is being healed! Health: " + ancient_updated.New.Health);
                }
            }
            else if (game_event is TeamNeutralItemsUpdated team_neutral_items_updated)
            {
                Console.WriteLine($"{team_neutral_items_updated.Team} neutral items updated: {team_neutral_items_updated.New}");
            }
            else if (game_event is CourierUpdated courier_updated)
            {
                Console.WriteLine($"Player {courier_updated.Player.Details.Name} courier updated: {courier_updated.New}");
            }
            else if (game_event is TeamDraftDetailsUpdated draft_details_updated)
            {
                Console.WriteLine($"{draft_details_updated.Team} draft details updated: {draft_details_updated.New}");
            }
            else if (game_event is TeamDefeat team_defeat)
            {
                Console.WriteLine($"{team_defeat.Team} lost the game.");
            }
            else if (game_event is TeamVictory team_victory)
            {
                Console.WriteLine($"{team_victory.Team} won the game!");
            }
        }

        // NewGameState example

        static void OnNewGameState(GameState gs)
        {
            Console.Clear();

            Console.WriteLine("Current Dota version: " + gs.Provider.Version);
            Console.WriteLine("Your steam name: " + gs.Player.LocalPlayer.Name);
            Console.WriteLine("Your dota account id: " + gs.Player.LocalPlayer.AccountID);

            Console.WriteLine("Current time as displayed by the clock (in seconds): " + gs.Map.ClockTime);

            Console.WriteLine("Radiant Score: " + gs.Map.RadiantScore);
            Console.WriteLine("Dire Score: " + gs.Map.DireScore);
            Console.WriteLine("Is game paused: " + gs.Map.IsPaused);

            Console.WriteLine("Hero ID: " + gs.Hero.LocalPlayer.ID);
            Console.WriteLine("Hero XP: " + gs.Hero.LocalPlayer.Experience);
            Console.WriteLine("Hero has Aghanims Shard upgrade: " + gs.Hero.LocalPlayer.HasAghanimsShardUpgrade);

            Console.WriteLine("Hero Health: " + gs.Hero.LocalPlayer.Health);
            for (int i = 0; i < gs.Abilities.LocalPlayer.Count; i++)
            {
                Console.WriteLine("Ability {0} = {1}", i, gs.Abilities.LocalPlayer[i].Name);
            }

            Console.WriteLine("First slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(0).Name);
            Console.WriteLine("Second slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(1).Name);
            Console.WriteLine("Third slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(2).Name);
            Console.WriteLine("Fourth slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(3).Name);
            Console.WriteLine("Fifth slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(4).Name);
            Console.WriteLine("Sixth slot inventory: " + gs.Items.LocalPlayer.GetInventoryAt(5).Name);

            Console.WriteLine("Teleport item slot: " + gs.Items.LocalPlayer.Teleport.Name);
            Console.WriteLine("Neutral item slot: " + gs.Items.LocalPlayer.Neutral.Name);

            if (gs.Items.LocalPlayer.InventoryContains("item_blink"))
            {
                Console.WriteLine("You have a blink dagger");
            }
            else
            {
                Console.WriteLine("You DO NOT have a blink dagger");
            }

            Console.WriteLine("Talent Tree:");
            for (int i = gs.Hero.LocalPlayer.TalentTree.Length - 1; i >= 0; i--)
            {
                var level = gs.Hero.LocalPlayer.TalentTree[i];
                Console.WriteLine($"{level}");
            }

            foreach (var game_event in gs.Events)
            {
                if (game_event.EventType == Dota2GSI.Nodes.EventsProvider.EventType.Bounty_rune_pickup)
                {
                    Console.WriteLine("Bounty rune was picked up!");
                    break;
                }
                else if (game_event.EventType == Dota2GSI.Nodes.EventsProvider.EventType.Roshan_killed)
                {
                    Console.WriteLine("Roshan was brutally killed!");
                    break;
                }
            }

            Console.WriteLine("Press ESC to quit");
        }
    }
}//переделай этот код взяв в основу наш старый с использованием библиотеки GSLDota
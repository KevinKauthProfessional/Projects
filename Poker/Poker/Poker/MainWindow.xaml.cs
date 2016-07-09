namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Utility;
    using Utility.Cards;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LastPlayerListFileName = "LastPlayerList.txt";
        private ObservableCollection<Player> playerList = new ObservableCollection<Player>();
        public StringBuilder LogBuilder = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(LastPlayerListFileName))
            {
                using (StreamReader reader = new StreamReader(LastPlayerListFileName))
                {
                    while (!reader.EndOfStream)
                    {
                        Player player = Player.FromString(reader.ReadLine());
                        if (player != null)
                        {
                            this.playerList.Add(player);
                        }
                    }
                }
            }

            this.DataGrid_PlayerList.ItemsSource = this.playerList;
        }

        private void Button_AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            float chipCount;
            if (!float.TryParse(this.TextBox_ChipCount.Text, out chipCount))
            {
                this.LogLine("Could not parse {0} check chip count value", this.TextBox_ChipCount.Text);
                return;
            }

            Player insertPlayer = new Player(this.TextBox_PlayerName.Text, chipCount);

            foreach (Player player in playerList)
            {
                if (string.Compare(player.PlayerName, insertPlayer.PlayerName, StringComparison.OrdinalIgnoreCase) ==  0)
                {
                    this.LogLine("Already have player with name: {0}", insertPlayer.PlayerName);
                    return;
                }
            }

            int selectedIndex = this.DataGrid_PlayerList.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < this.playerList.Count - 1)
            {
                Player previousPlayer = this.playerList[selectedIndex];
                this.playerList.Insert(selectedIndex + 1, insertPlayer);

                for (int i = selectedIndex + 1; i < this.playerList.Count; ++i)
                {
                    // Cascade update player position values
                    this.playerList[i].Position = previousPlayer.Position + 1;
                    previousPlayer = this.playerList[i];
                }
            }
            else
            {
                Player previousPlayer = this.playerList.LastOrDefault();
                if (previousPlayer != default(Player))
                {
                    insertPlayer.Position = previousPlayer.Position + 1;
                }
                else
                {
                    insertPlayer.Position = 1;
                }

                // Add to end
                this.playerList.Add(insertPlayer);
            }

            // Update the UI
            this.DataGrid_PlayerList.ItemsSource = this.playerList;
        }

        private void LogLine(string formatString, params object[] args)
        {
            this.LogLine(string.Format(formatString, args));
        }

        private void LogLine(string message)
        {
            this.LogBuilder.AppendLine(string.Format("{0}: {1}", DateTime.Now, message));
            this.InvokeUIChange(() => this.TextBlock_Log.Text = this.LogBuilder.ToString());
        }

        private void InvokeUIChange(Action change)
        {
            Dispatcher.Invoke(change);
        }

        private void Button_RemoveAllPlayers_Click(object sender, RoutedEventArgs e)
        {
            this.playerList.Clear();
            this.DataGrid_PlayerList.ItemsSource = this.playerList;
        }

        private void Button_DealNewHand_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.DataGrid_PlayerList.SelectedIndex;
            if (selectedIndex == -1)
            {
                this.LogLine("Please select the player in the dealer position");
                return;
            }

            int assignPosition = 1;
            for (int i = selectedIndex + 1; i < this.playerList.Count; ++i)
            {
                this.playerList[i].Position = assignPosition++;
                this.playerList[i].Folded = false;
            }

            for (int i = 0; i <= selectedIndex; ++i)
            {
                this.playerList[i].Position = assignPosition++;
                this.playerList[i].Folded = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(LastPlayerListFileName))
            {
                foreach (Player player in this.playerList)
                {
                    writer.WriteLine(player.ToString());
                }
            }
        }

        private void Image_CommunityCard1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_CommunityCard1.Source = chosenCard.CardImage);
        }

        private void Image_CommunityCard2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_CommunityCard2.Source = chosenCard.CardImage);
        }

        private void Image_CommunityCard3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_CommunityCard3.Source = chosenCard.CardImage);
        }

        private void Image_CommunityCard4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_CommunityCard4.Source = chosenCard.CardImage);
        }

        private void Image_CommunityCard5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_CommunityCard5.Source = chosenCard.CardImage);
        }

        private void Image_PrivateCard1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_PrivateCard1.Source = chosenCard.CardImage);
        }

        private void Image_PrivateCard2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Card chosenCard;
            if (!ChooseCard.TryShowChooseCardDialog(out chosenCard))
            {
                return;
            }

            this.InvokeUIChange(() => this.Image_PrivateCard2.Source = chosenCard.CardImage);
        }
    }
}

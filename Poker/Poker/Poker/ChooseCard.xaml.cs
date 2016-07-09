namespace Poker
{
    using System;
    using System.Collections.Generic;
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
    using System.Windows.Shapes;
    using Utility.Cards;

    /// <summary>
    /// Interaction logic for ChooseCard.xaml
    /// </summary>
    public partial class ChooseCard : Window
    {
        public ChooseCard()
        {
            InitializeComponent();

            CardSuit[] suits = (CardSuit[])Enum.GetValues(typeof(CardSuit));
            foreach (CardSuit suit in suits)
            {
                if (suit == CardSuit.None)
                {
                    continue;
                }

                ListBoxItem item = new ListBoxItem();
                item.Content = suit.ToString();
                this.ListBox_Suit.Items.Add(item);
            }

            CardValue[] values = (CardValue[])Enum.GetValues(typeof(CardValue));
            foreach (CardValue value in values)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = value.ToString();
                this.ListBox_Values.Items.Add(item);
            }
        }

        public bool Canceled { get; private set; }

        public Card SelectedCard
        {
            get
            {
                if (this.ListBox_Suit.SelectedIndex == -1 ||
                    this.ListBox_Values.SelectedIndex == -1)
                {
                    return null;
                }

                CardSuit suit = (CardSuit)this.ListBox_Suit.SelectedIndex;
                CardValue value = (CardValue)this.ListBox_Values.SelectedIndex;
                return new Card(suit, value);
            }
        }

        public static bool TryShowChooseCardDialog(out Card chosenCard)
        {
            ChooseCard window = new ChooseCard();
            window.InitializeComponent();
            window.ShowDialog();

            chosenCard = window.SelectedCard;

            if (window.Canceled)
            {
                return false;
            }

            return chosenCard != null;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Canceled = true;
            this.Close();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

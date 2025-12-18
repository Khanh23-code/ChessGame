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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUI.Views.BoardMenu
{
    public class TwoPlayerSettings
    {
        public string WhiteName { get; set; }
        public string BlackName { get; set; }
        public TimeSpan TimeLimit { get; set; }
    }
    public partial class TwoPlayerSetup : UserControl
    {
        public event EventHandler<TwoPlayerSettings> OnStartGameClicked;

        public TwoPlayerSetup()
        {
            InitializeComponent();
            StartTwoPlayerButton.Click += StartTwoPlayerButton_Click;
        }
        private void SwapSideButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = WhitePlayerName.Text;
            WhitePlayerName.Text = BlackPlayerName.Text;
            BlackPlayerName.Text = temp;
        }
        private void StartTwoPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan selectedTime = TimeSpan.FromMinutes(10);
            if (TimeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string content = selectedItem.Content.ToString();
                if (content.Contains("1 phút")) selectedTime = TimeSpan.FromMinutes(1);
                else if (content.Contains("3 phút")) selectedTime = TimeSpan.FromMinutes(3);
                else if (content.Contains("10 phút")) selectedTime = TimeSpan.FromMinutes(10);
                else if (content.Contains("30 phút")) selectedTime = TimeSpan.FromMinutes(30);
                else if (content.Contains("No limts")) selectedTime = TimeSpan.FromHours(24);
            }
            string wName = WhitePlayerName.Text;
            string bName = BlackPlayerName.Text;
            if (string.IsNullOrWhiteSpace(wName))
            {
                wName = "Player 1";
            }

            if (string.IsNullOrWhiteSpace(bName))
            {
                bName = "Player 2";
            }
            var settings = new TwoPlayerSettings
            {
                WhiteName = wName,
                BlackName = bName,
                TimeLimit = selectedTime
            };

            OnStartGameClicked?.Invoke(this, settings);
        }
    }
}

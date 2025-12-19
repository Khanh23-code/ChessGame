using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ChessUI.Views.BoardMenu
{
    public partial class CountDownView : UserControl
    {
        public CountDownView()
        {
            InitializeComponent();
        }

        private void PlayAnimation()
        {
            Storyboard storyboard = (Storyboard)this.Resources["TextPopAnim"];
            storyboard.Begin();
        }

        public async Task StartCountDownAsync()
        {
            CountText.Text = "3";
            PlayAnimation();
            await Task.Delay(1000);

            CountText.Text = "2";
            PlayAnimation();
            await Task.Delay(1000);

            CountText.Text = "1";
            PlayAnimation();
            await Task.Delay(1000);

            CountText.Text = "START!";
            CountText.Foreground = System.Windows.Media.Brushes.GreenYellow;
            CountText.FontSize = 100; 
            PlayAnimation();

            await Task.Delay(600); 
        }
    }
}
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

namespace ChessUI.Views.GameMenus
{
    public enum ResumeOption
    {
        Resume,
        NewGame
    }
    public partial class ResumeMenu : UserControl
    {
        public event Action<ResumeOption> OptionSelected;
        public ResumeMenu()
        {
            InitializeComponent();
        }
        private void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(ResumeOption.Resume);
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(ResumeOption.NewGame);
        }
    }
}

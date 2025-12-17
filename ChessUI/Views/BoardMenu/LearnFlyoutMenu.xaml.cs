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
    /// <summary>
    /// Interaction logic for LearnFlyoutMenu.xaml
    /// </summary>
    public partial class LearnFlyoutMenu : UserControl
    {
        public event EventHandler ChessMovesLessonClicked;
        public LearnFlyoutMenu()
        {
            InitializeComponent();
        }

        private void btnChessMovesLesson_Click(object sender, RoutedEventArgs e)
        {
            ChessMovesLessonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnStartLearning_Click(object sender, RoutedEventArgs e)
        {
            ChessMovesLessonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}

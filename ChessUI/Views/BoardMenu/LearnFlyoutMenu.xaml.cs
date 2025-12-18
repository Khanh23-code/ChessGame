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
        public event EventHandler<int> LessonSelected;
        public LearnFlyoutMenu()
        {
            InitializeComponent();
        }

        private void rbtnChessLesson_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                if (int.TryParse(rb.Tag.ToString(), out int lessonId))
                {
                    LessonSelected?.Invoke(this, lessonId);
                }
            }
        }
        private void btnStartLearning_Click(object sender, RoutedEventArgs e)
        {
            LessonSelected?.Invoke(this, 1);
            rbtnChessMovesLesson.IsChecked = true;
        }
    }
}

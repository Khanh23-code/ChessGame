using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class PuzzleFlyoutMenu : UserControl
    {
        public event EventHandler NormalPuzzleClicked;
        public event EventHandler DailyPuzzleClicked;

        public PuzzleFlyoutMenu()
        {
            InitializeComponent();
        }
        private void NormalPuzzle_Click(object sender, RoutedEventArgs e)
        {
            NormalPuzzleClicked?.Invoke(this, EventArgs.Empty);
        }
        private void DailyPuzzle_Click(object sender, RoutedEventArgs e)
        {
            DailyPuzzleClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class PuzzleInfoView : UserControl
    {
        public ObservableCollection<PuzzleItem> PuzzleList { get; set; } = new ObservableCollection<PuzzleItem>();
        public event Action<string> OnPuzzleSelected;
        public PuzzleInfoView()
        {
            InitializeComponent();
            this.DataContext = this;

            txtDate.Text = DateTime.Now.ToString("MMM dd");
            txtDayOfWeek.Text = DateTime.Now.DayOfWeek.ToString();
        }

        private bool _isDailyMode;
        public bool IsDailyMode
        {
            get { return _isDailyMode; }
            set
            {
                _isDailyMode = value;
                UpdateMode();
            }
        }
        private void UpdateMode()
        {
            if (_isDailyMode)
            {
                // Chế độ Daily
                NormalHeader.Visibility = Visibility.Collapsed;
                DailyHeader.Visibility = Visibility.Visible;
                ActionBtn.Content = "Back to Menu"; // Daily chỉ có 1 bài

                PuzzleList.Add(new PuzzleItem
                {
                    Id = 9999,
                    Title = "Daily Challenge",
                    Rating = 1500,
                    Difficulty = "Hard",
                    FenString = "r1bqkb1r/pppp1ppp/2n2n2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 4 4 600:600"
                });
            }
            else
            {
                // Chế độ Normal
                NormalHeader.Visibility = Visibility.Visible;
                DailyHeader.Visibility = Visibility.Collapsed;
                ActionBtn.Content = "Next Puzzle";

                LoadNormalPuzzles();
            }
        }
        private void LoadNormalPuzzles()
        {
            PuzzleList.Add(new PuzzleItem
            {
                Title = "Scholar's Mate",
                Rating = 800,
                Difficulty = "Easy",
                FenString = "r1bqkb1r/pppp1ppp/2n2n2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 4 4 600:600"
            });
            PuzzleList.Add(new PuzzleItem
            {
                Title = "Ruy Lopez Start",
                Rating = 1200,
                Difficulty = "Normal",
                FenString = "r1bqkbnr/1ppp1ppp/p1n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4 600:600"
            });
            for (int i = 3; i <= 10; i++)
            {
                PuzzleList.Add(new PuzzleItem
                {
                    Title = $"Tactics #{i:000}",
                    Rating = 1200 + (i * 10),
                    Difficulty = "Hard",
                    FenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 600:600"
                });
            }
        }

        private void PuzzleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is PuzzleItem selectedPuzzle)
            {
                string fenToLoad = selectedPuzzle.FenString;
                OnPuzzleSelected?.Invoke(fenToLoad);
                listBox.SelectedIndex = -1;
            }
        }
    }
    public class PuzzleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }    
        public int Rating { get; set; }      
        public string Difficulty { get; set; }
        public string FenString { get; set; } 
    }
}
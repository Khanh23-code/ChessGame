using ChessUI.Views.GameMenus;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class PuzzleInfoView : UserControl
    {
        public ObservableCollection<PuzzleItem> PuzzleList { get; set; } = new ObservableCollection<PuzzleItem>();
        public event Action<PuzzleItem> OnPuzzleSelected;
        public event Action OnHintRequested;      
        public event Action OnRetryRequested;         
        public event Action OnBackToMenu;            
        private PuzzleItem _currentPuzzle;           

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
            PuzzleList.Clear();
            FeedbackBox.Visibility = Visibility.Collapsed;

            List<PuzzleItem> allPuzzles = GenerateAllPuzzles();

            if (_isDailyMode)
            {
                // --- CHẾ ĐỘ DAILY ---
                NormalHeader.Visibility = Visibility.Collapsed;
                DailyHeader.Visibility = Visibility.Visible;
                ActionBtn.Content = "Back to Menu";

                if (allPuzzles.Count > 0)
                {
                    int dayOfYear = DateTime.Now.DayOfYear;
                    int dailyIndex = dayOfYear % allPuzzles.Count;

                    var dailyPuzzle = allPuzzles[dailyIndex];

                    dailyPuzzle.Title = $"Daily #{DateTime.Now:dd/MM}: {dailyPuzzle.Title}";

                    PuzzleList.Add(dailyPuzzle);
                    SelectPuzzle(dailyPuzzle);
                }
            }
            else
            {
                // --- CHẾ ĐỘ NORMAL ---
                NormalHeader.Visibility = Visibility.Visible;
                DailyHeader.Visibility = Visibility.Collapsed;
                ActionBtn.Content = "Next Puzzle";

                foreach (var p in allPuzzles)
                {
                    PuzzleList.Add(p);
                }
               if (PuzzleList.Count > 0) SelectPuzzle(PuzzleList[0]);
            }
        }

        private List<PuzzleItem> GenerateAllPuzzles()
        {
            var list = new List<PuzzleItem>();

            list.Add(new PuzzleItem { Id = 1, Title = "Scholar's Mate", Rating = 400, Difficulty = "Easy", FenString = "r1bqkb1r/pppp1ppp/2n2n2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 4 4 600:600", Solution = new List<string> { "h5f7" } });
            list.Add(new PuzzleItem { Id = 2, Title = "Back Rank Mate", Rating = 600, Difficulty = "Easy", FenString = "6k1/5ppp/8/8/8/8/5PPP/4R1K1 w - - 0 1 600:600", Solution = new List<string> { "e1e8" } });
            list.Add(new PuzzleItem { Id = 3, Title = "Winning the Queen", Rating = 800, Difficulty = "Easy", FenString = "r1b1kbnr/pppp1ppp/8/4q3/2B1P3/8/PPPP1PPP/RNBQK2R w KQkq - 0 1 600:600", Solution = new List<string> { "d2d4" } });
            list.Add(new PuzzleItem { Id = 11, Title = "Arabian Mate", Rating = 800, Difficulty = "Easy", FenString = "7k/2R5/5N2/8/8/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "c7h7" } });

            list.Add(new PuzzleItem { Id = 4, Title = "Boden's Mate", Rating = 1100, Difficulty = "Normal", FenString = "2kr3r/pp1n1ppp/2p1b3/8/1b2P3/2N1BB2/PPP2PPP/R3K2R w KQ - 0 1 600:600", Solution = new List<string> { "f3a6" } });
            list.Add(new PuzzleItem { Id = 13, Title = "Smothered Mate (Basic)", Rating = 1100, Difficulty = "Normal", FenString = "6rk/5Npp/8/8/8/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "f7h6" } });
            list.Add(new PuzzleItem { Id = 5, Title = "Royal Fork", Rating = 1250, Difficulty = "Normal", FenString = "r3k2r/ppp2ppp/2n5/3p4/3P4/2N2N2/PPP2PPP/R3KB1R w KQkq - 0 1 600:600", Solution = new List<string> { "c3b5", "e8d8", "b5c7" } });
            list.Add(new PuzzleItem { Id = 19, Title = "Swallow's Tail", Rating = 1250, Difficulty = "Normal", FenString = "r2qkb1r/5ppp/8/8/3Q4/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "d4e4", "f8e7", "e4e7" } });
            list.Add(new PuzzleItem { Id = 12, Title = "Opera Game Finish", Rating = 1200, Difficulty = "Normal", FenString = "4kb1r/p2n1ppp/4q3/4p1B1/4P3/1Q6/PPP2PPP/2KR4 w k - 1 0 600:600", Solution = new List<string> { "b3b8", "d7b8", "d1d8" } });
            list.Add(new PuzzleItem { Id = 14, Title = "Damiano's Mate", Rating = 1300, Difficulty = "Normal", FenString = "6k1/5p1p/6pP/3Q4/8/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "d5a8" } });
            list.Add(new PuzzleItem { Id = 6, Title = "Hook Mate", Rating = 1350, Difficulty = "Normal", FenString = "3r4/p5kp/1p4p1/8/5P2/1P4P1/P5BP/4R1K1 w - - 0 1 600:600", Solution = new List<string> { "e1e7", "g7f6", "e7h7" } });
            list.Add(new PuzzleItem { Id = 15, Title = "Legal's Mate Trap", Rating = 1400, Difficulty = "Normal", FenString = "r2qkb1r/ppp2ppp/2np1n2/4p3/2B1P1b1/2N2N2/PPPP1PPP/R1BQK2R w KQkq - 0 1 600:600", Solution = new List<string> { "f3e5", "g4d1", "c4f7", "e8e7", "c3d5" } });

            list.Add(new PuzzleItem { Id = 17, Title = "Greco's Mate", Rating = 1500, Difficulty = "Hard", FenString = "5rk1/5ppp/8/8/2B5/8/8/3R3K w - - 0 1 600:600", Solution = new List<string> { "c4f7", "f8f7", "d1d8", "f7f8", "d8f8" } });
            list.Add(new PuzzleItem { Id = 7, Title = "Anastasia's Mate", Rating = 1600, Difficulty = "Hard", FenString = "5rk1/1p3ppp/8/8/4N3/8/5PPP/5RK1 w - - 0 1 600:600", Solution = new List<string> { "e4f6", "g7f6", "f1d1" } });
            list.Add(new PuzzleItem { Id = 16, Title = "Blind Swine Mate", Rating = 1600, Difficulty = "Hard", FenString = "k7/pp6/8/8/8/8/1R3PPP/6K1 w - - 0 1 600:600", Solution = new List<string> { "b2e2", "a8b8", "e2e8" } });
            list.Add(new PuzzleItem { Id = 18, Title = "Lolli's Mate", Rating = 1700, Difficulty = "Hard", FenString = "6k1/5p2/5P2/3Q4/8/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "d5g5", "g8f8", "g5g7", "f8e8", "g7g8" } });
            list.Add(new PuzzleItem { Id = 8, Title = "Smothered Mate (Full)", Rating = 1800, Difficulty = "Hard", FenString = "r1b2rk1/pp1p1ppp/2n1p3/8/4N3/8/PPP1QPPP/R3KB1R w KQ - 0 1 600:600", Solution = new List<string> { "e4f6", "g8h8", "e2g4" } });

            list.Add(new PuzzleItem { Id = 20, Title = "Corner Mate", Rating = 1900, Difficulty = "Expert", FenString = "7k/R7/5N2/8/8/8/8/7K w - - 0 1 600:600", Solution = new List<string> { "a7h7" } });
            list.Add(new PuzzleItem { Id = 9, Title = "Underpromotion Win", Rating = 2100, Difficulty = "Expert", FenString = "8/5P1k/8/8/8/8/8/6K1 w - - 0 1 600:600", Solution = new List<string> { "f7f8n" } });
            list.Add(new PuzzleItem { Id = 10, Title = "The Opposition", Rating = 2300, Difficulty = "Expert", FenString = "8/8/4k3/8/8/8/3K4/8 w - - 0 1 600:600", Solution = new List<string> { "d2d3" } });

            return list;
        }

        // Xử lý khi click vào ListBox
        private void PuzzleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is PuzzleItem selectedPuzzle)
            {
                SelectPuzzle(selectedPuzzle);
            }
        }

        private void SelectPuzzle(PuzzleItem puzzle)
        {
            _currentPuzzle = puzzle;
            FeedbackBox.Visibility = Visibility.Collapsed;
            OnPuzzleSelected?.Invoke(puzzle); 
        }

        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isDailyMode)
            {
                OnBackToMenu?.Invoke();
            }
            else
            {
                LoadNextPuzzle();
            }
        }

        // hint button
        private void BtnHint_Click(object sender, RoutedEventArgs e)
        {
            OnHintRequested?.Invoke();
        }
        // retry button
        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            FeedbackBox.Visibility = Visibility.Collapsed;
            if (_currentPuzzle != null)
            {
                OnRetryRequested?.Invoke();
            }
        }

        private void LoadNextPuzzle()
        {
            if (_currentPuzzle == null || PuzzleList.Count == 0) return;
            int currentIndex = PuzzleList.IndexOf(_currentPuzzle);

            if (currentIndex >= 0 && currentIndex < PuzzleList.Count - 1)
            {
                var nextPuzzle = PuzzleList[currentIndex + 1];
                PuzzleListBox.SelectedItem = nextPuzzle; 
                PuzzleListBox.ScrollIntoView(nextPuzzle);
                SelectPuzzle(nextPuzzle);
            }
            else
            {
                CustomMessageBox.Show("Đã hoàn thành tất cả bài tập!", "Chúc mừng");
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
        public System.Collections.Generic.List<string> Solution { get; set; }
    }
}
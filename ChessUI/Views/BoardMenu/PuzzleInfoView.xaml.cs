using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class PuzzleInfoView : UserControl
    {
        public PuzzleInfoView()
        {
            InitializeComponent();

            // Tự động cập nhật ngày tháng hiện tại khi mở
            txtDate.Text = DateTime.Now.ToString("MMM dd");
            txtDayOfWeek.Text = DateTime.Now.DayOfWeek.ToString();
        }

        // Property để chuyển đổi chế độ
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
            }
            else
            {
                // Chế độ Normal
                NormalHeader.Visibility = Visibility.Visible;
                DailyHeader.Visibility = Visibility.Collapsed;
                ActionBtn.Content = "Next Puzzle";
            }
        }
    }
}
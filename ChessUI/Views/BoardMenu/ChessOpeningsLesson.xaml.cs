using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChessUI.Views.BoardMenu
{
    public class OpeningVideo
    {
        public string Title { get; set; }           
        public string Description { get; set; }     
        public string Duration { get; set; }      
        public string ThumbnailPath { get; set; } 
        public string VideoPath { get; set; }     
    }
    public partial class ChessOpeningsLesson : UserControl
    {
        private List<OpeningVideo> _allLessons;
        private DispatcherTimer _timer; 
        private bool _isDragging = false; 

        public ChessOpeningsLesson()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500); 
            _timer.Tick += Timer_Tick;
            LoadLessons();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isDragging && MainPlayer.Source != null && MainPlayer.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Value = MainPlayer.Position.TotalSeconds;
                lblCurrentTime.Text = MainPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void LoadLessons()
        {
            _allLessons = new List<OpeningVideo>()
            {
                new OpeningVideo
                {
                    Title = "1. The Queen's Gambit",
                    Description = "Dominate the center with this solid strategic choice.",
                    Duration = "21:09",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningQueen'sGambit.mp4"
                },
                new OpeningVideo
                {
                    Title = "2. Nimzo-Indian Defense",
                    Description = "A dynamic defense creating imbalance for Black.",
                    Duration = "37:08",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningNimzoIndianDefense.mp4"
                },
                new OpeningVideo
                {
                    Title = "3. The Sicilian Defense",
                    Description = "The most aggressive and sharp counter-attack to 1.e4.",
                    Duration = "19:21",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningSicilianDefense.mp4"
                },
                new OpeningVideo
                {
                    Title = "4. Ruy Lopez",
                    Description = "Master the board with this classic positional squeeze.",
                    Duration = "24:21",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningRuyLopez.mp4"
                },
                new OpeningVideo
                {
                    Title = "5. Two Knights Defense", 
                    Description = "Sacrifice material for active piece play.",
                    Duration = "14:18",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningTwoKnight.mp4"
                },
                new OpeningVideo
                {
                    Title = "6. Italian Game", 
                    Description = "Rapid development targeting the weak f7 square.",
                    Duration = "23:38",
                    ThumbnailPath = "",
                    VideoPath = @"Assets\Videos\ChessOpeningItalian.mp4"
                },
            };
            VideoList.ItemsSource = _allLessons;
            if (_allLessons.Count > 0)
                VideoList.SelectedIndex = 0;
        }
        #region Event Handlers Video Player and Controls
        private void VideoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoList.SelectedItem is OpeningVideo selectedVideo)
            {
                txtMainTitle.Text = selectedVideo.Title;
                txtDescription.Text = selectedVideo.Description;
                try
                {
                    if (!string.IsNullOrEmpty(selectedVideo.VideoPath))
                    {
                        MainPlayer.Source = new Uri(selectedVideo.VideoPath, UriKind.RelativeOrAbsolute);
                        BtnBigPlay.Visibility = Visibility.Visible;
                        MainPlayer.Pause();
                    }
                }
                catch { }
            }
        }
        private void MainPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MainPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = MainPlayer.NaturalDuration.TimeSpan;
                progressSlider.Maximum = ts.TotalSeconds;
                progressSlider.SmallChange = 1;
                progressSlider.LargeChange = Math.Min(10, ts.Seconds / 10);
                lblTotalTime.Text = ts.ToString(@"mm\:ss");
            }
            _timer.Start();
        }
        private void TogglePlay_Click(object sender, RoutedEventArgs e)
        {
            if (BtnBigPlay.Visibility == Visibility.Visible)
            {
                MainPlayer.Play();
                BtnBigPlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainPlayer.Pause();
                BtnBigPlay.Visibility = Visibility.Visible;
            }
        }
        private void MainPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MainPlayer.Stop();
            BtnBigPlay.Visibility = Visibility.Visible;
        }
        private void progressSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }
        private void progressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            MainPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
        }
        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblCurrentTime.Text = TimeSpan.FromSeconds(progressSlider.Value).ToString(@"mm\:ss");
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || _allLessons == null) return;

            string filter = textBox.Text.ToLower();
            var filteredList = _allLessons.Where(v => v.Title.ToLower().Contains(filter)).ToList();
            VideoList.ItemsSource = filteredList;
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI.Views.BoardMenu
{
    public partial class CustomBoardPieces : Window
    { 
        public class GameTheme
        {
            public string Name { get; set; }
            public int AssetId { get; set; } 
            public string BoardIconPath { get; set; }
            public string BoardFullPath { get; set; }
            public string PiecePreviewPath { get; set; } 
        }

        public GameTheme SelectedTheme { get; private set; }

        public CustomBoardPieces()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var themes = new List<GameTheme>
            {
                new GameTheme
                {
                    Name = "Cổ điển",
                    AssetId = 1,
                    BoardIconPath = "/Assets/Asset1/BoardPreview.png", 
                    BoardFullPath = "/Assets/Asset1/Board.png",
                    PiecePreviewPath = "/Assets/Asset1/KnightW.png"
                },
                new GameTheme
                {
                    Name = "Hiện đại",
                    AssetId = 2,
                    BoardIconPath = "/Assets/Asset2/BoardPreview.png",
                    BoardFullPath = "/Assets/Asset2/Board.png",
                    PiecePreviewPath = "/Assets/Asset2/KnightW.png"
                },
                new GameTheme
                {
                    Name = "Đơn giản",
                    AssetId = 3,
                    BoardIconPath = "/Assets/Asset3/BoardPreview.png",
                    BoardFullPath = "/Assets/Asset3/Board.png",
                    PiecePreviewPath = "/Assets/Asset3/KnightW.png"
                },
                new GameTheme
                {
                    Name = "Nhà thờ phương Tây",
                    AssetId = 4,
                    BoardIconPath = "/Assets/Asset4/BoardPreview.png",
                    BoardFullPath = "/Assets/Asset4/Board.png",
                    PiecePreviewPath = "/Assets/Asset4/KnightW.png"
                },
                new GameTheme
                {
                    Name = "Chess.com Style",
                    AssetId = 5,
                    BoardIconPath = "/Assets/Asset5/BoardPreview.png",
                    BoardFullPath = "/Assets/Asset5/Board.png",
                    PiecePreviewPath = "/Assets/Asset5/KnightW.png"
                },
                new GameTheme
                {
                    Name = "Custom La Mã",
                    AssetId = 6,
                    BoardIconPath = "/Assets/Asset6/BoardPreview.png",
                    BoardFullPath = "/Assets/Asset6/Board.png",
                    PiecePreviewPath = "/Assets/Asset6/KnightW.png"
                },
            };

            ThemeList.ItemsSource = themes;
            ThemeList.SelectedIndex = 0;
        }

        private void ThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeList.SelectedItem is GameTheme theme)
            {
                SelectedTheme = theme;
                UpdatePreview(theme);
            }
        }

        private void UpdatePreview(GameTheme theme)
        {
            try
            {
                PreviewBoardImage.Source = new BitmapImage(new Uri(theme.BoardFullPath, UriKind.RelativeOrAbsolute)); 
                PreviewPiecesGrid.Children.Clear();

                string assetFolder = $"pack://application:,,,/Assets/Asset{theme.AssetId}";
                string[] backRank = { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" };
                for (int col = 0; col < 8; col++)
                {
                    AddPreviewPiece($"{assetFolder}/{backRank[col]}B.png", 0, col);
                    AddPreviewPiece($"{assetFolder}/PawnB.png", 1, col);
                }
                for (int col = 0; col < 8; col++)
                {
                    AddPreviewPiece($"{assetFolder}/PawnW.png", 6, col);
                    AddPreviewPiece($"{assetFolder}/{backRank[col]}W.png", 7, col);
                }
            }
            catch { }
        }

        private void AddPreviewPiece(string path, int r, int c)
        {
            try
            {
                //Image img = new Image { Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)) };
                Image img = new Image { Source = new BitmapImage(new Uri(path, UriKind.Absolute)) };
                Grid.SetRow(img, r);
                Grid.SetColumn(img, c);
                PreviewPiecesGrid.Children.Add(img);
            }
            catch { }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
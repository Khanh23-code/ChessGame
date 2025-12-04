using ChessLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessUI.Views.BoardMenu
{
    public partial class ChessMovesLesson : UserControl, INotifyPropertyChanged
    {
        public double CurrentRotation
        {
            get { return (double)GetValue(CurrentRotationProperty); }
            set { SetValue(CurrentRotationProperty, value); }
        }
        public static readonly DependencyProperty CurrentRotationProperty =
            DependencyProperty.Register("CurrentRotation", typeof(double), typeof(ChessMovesLesson), new PropertyMetadata(0.0));
        public ObservableCollection<ChessPiece> Pieces { get; set; }

        private ChessPiece _selectedPiece;
        public ChessPiece SelectedPiece
        {
            get { return _selectedPiece; }
            set
            {
                _selectedPiece = value;
                OnPropertyChanged("SelectedPiece");
            }
        }

        public ChessMovesLesson()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData();
            CurrentRotation = Math.PI / 2;
        }

        private void LoadData()
        {
            Pieces = new ObservableCollection<ChessPiece>()
            {
                new ChessPiece { Name = "KING", 
                                ImagePath = "/Assets/Asset1/KingW.png", 
                                IconPath = "/Assets/Asset1/KingW.png", 
                                Title = "How to Move the King", 
                                Description = "The King is the most important piece on the board, but he moves cautiously. He can step exactly one square in any direction—forward, backward, sideways, or diagonally. He captures enemy pieces the same way he moves, by landing on their square.However, the King has one golden rule: Safety First. He is never allowed to move to a square where an enemy piece can capture him (this is called moving into 'Check').There is one special exception to his slow movement called Castling. If the King and a Rook haven't moved yet, they can team up. The King slides two squares toward the Rook, and the Rook hops over him to stand guard by his side. This is the only time the King can move more than one square at once!" },
                new ChessPiece { Name = "QUEEN", 
                                ImagePath = "/Assets/Asset1/QueenW.png", 
                                IconPath = "/Assets/Asset1/QueenW.png", 
                                Title = "How to Move the Queen", 
                                Description = "The Queen is the most powerful piece on the chess board. You can think of her as a combination of a Rook and a Bishop. She can move in any straight line—forward, backward, sideways, or diagonally.\r\n\r\nShe can slide as many squares as she wants in a single turn, zooming across the whole board if she needs to! However, she cannot jump over other pieces. She must stop before she hits a piece of her own color, or she can stop on an enemy piece to capture it." },
                new ChessPiece { Name = "ROOK", 
                                ImagePath = "/Assets/Asset1/RookW.png", 
                                IconPath = "/Assets/Asset1/RookW.png", 
                                Title = "How to Move the Rook", 
                                Description = "The Rook looks like a castle tower, and it moves in a straight cross shape. It can go forward, backward, left, or right.You can slide the Rook as many squares as you want—it can rush across the entire board in a single turn! However, it cannot move diagonally, and it cannot jump over other pieces. It must stop if a friendly piece is in the way, or it can land on an enemy piece to capture it.Note: The Rook is also the only piece that helps the King perform the special 'Castling' move." },
                new ChessPiece { Name = "BISHOP", 
                                ImagePath = "/Assets/Asset1/BishopW.png", 
                                IconPath = "/Assets/Asset1/BishopW.png", 
                                Title = "How to Move the Bishop", 
                                Description = "Tượng đi chéo..." },
                new ChessPiece { Name = "KNIGHT", ImagePath = "/Assets/Asset1/KnightW.png", IconPath = "/Assets/Asset1/KnightW.png", Title = "How to Move the Knight", Description = "Mã đi hình chữ L..." },
                new ChessPiece { Name = "PAWN", ImagePath = "/Assets/Asset1/PawnW.png", IconPath = "/Assets/Asset1/PawnW.png", Title = "How to Move the Pawn", Description = "Tốt đi thẳng ăn chéo..." },
            };
            SelectedPiece = Pieces[0];
        }
        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element.DataContext is ChessPiece clickedPiece)
            {
                SelectedPiece = clickedPiece;
                int index = Pieces.IndexOf(clickedPiece);
                RotateToItem(index);
            }
        }

        private void RotateToItem(int index)
        {
            int totalItems = Pieces.Count;
            if (totalItems == 0) return;

            double step = 0.65;
            double currentItemAngle = CurrentRotation + (index * step);
            double targetPosition = Math.PI / 2;

            double delta = targetPosition - currentItemAngle;

            while (delta > Math.PI) delta -= 2 * Math.PI;
            while (delta < -Math.PI) delta += 2 * Math.PI;

            double targetRotation = CurrentRotation + delta;

            DoubleAnimation animation = new DoubleAnimation();
            animation.To = targetRotation;
            animation.Duration = TimeSpan.FromSeconds(0.1);
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

            this.BeginAnimation(CurrentRotationProperty, animation);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class ChessPiece
    {
        public string Name { get; set; }
        public string ImagePath { get; set; } 
        public string IconPath { get; set; }  
        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class CarouselPanelLearning : Panel
    {
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(CarouselPanelLearning),
                new PropertyMetadata(150.0, OnPropertyChanged));

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(CarouselPanelLearning),
                new PropertyMetadata(40.0, OnPropertyChanged));

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(CarouselPanelLearning),
                new PropertyMetadata(0.0, OnPropertyChanged));

        public double RadiusX { get { return (double)GetValue(RadiusXProperty); } set { SetValue(RadiusXProperty, value); } }
        public double RadiusY { get { return (double)GetValue(RadiusYProperty); } set { SetValue(RadiusYProperty, value); } }
        public double RotationAngle { get { return (double)GetValue(RotationAngleProperty); } set { SetValue(RotationAngleProperty, value); } }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CarouselPanelLearning).InvalidateArrange();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = InternalChildren.Count;
            if (count == 0) return finalSize;

            double centerX = finalSize.Width / 2;
            double centerY = finalSize.Height / 2;
            double step = 0.65;

            for (int i = 0; i < count; i++)
            {
                UIElement child = InternalChildren[i];

                double angle = RotationAngle + (i * step);
                double x = centerX + Math.Cos(angle) * RadiusX;
                double y = centerY + Math.Sin(angle) * RadiusY;
                double val = (Math.Sin(angle) + 1) / 2;

                double scaleVal = 0.6 + (Math.Pow(val, 2) * 0.8);
                double opacityVal = 0.2 + (Math.Pow(val, 2) * 0.8);

                child.RenderTransformOrigin = new Point(0.5, 0.5);
                child.RenderTransform = new ScaleTransform(scaleVal, scaleVal);
                child.Opacity = opacityVal;

                int zIndex = (int)(scaleVal * 100);
                Panel.SetZIndex(child, zIndex);

                double childWidth = child.DesiredSize.Width;
                double childHeight = child.DesiredSize.Height;
                child.Arrange(new Rect(x - childWidth / 2, y - childHeight / 2, childWidth, childHeight));
            }

            return finalSize;
        }
    }

}

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

        private string _currentColor = "W";
        private void LoadData(string color = "W")
        {
            _currentColor = color;
            var tempPieces = new ObservableCollection<ChessPiece>()
            {
                new ChessPiece { Name = "KING", 
                                ImagePath = $"/Assets/Asset1/King{color}.png", 
                                IconPath = $"/Assets/Asset1/King{color}.png", 
                                Title = "How to Move the King", 
                                Description = "The King is the most important piece on the board, but he moves cautiously. He can step exactly one square in any direction - forward, backward, sideways, or diagonally. He captures enemy pieces the same way he moves, by landing on their square.However, the King has one golden rule: Safety First. He is never allowed to move to a square where an enemy piece can capture him (this is called moving into 'Check').There is one special exception to his slow movement called Castling. If the King and a Rook haven't moved yet, they can team up. The King slides two squares toward the Rook, and the Rook hops over him to stand guard by his side. This is the only time the King can move more than one square at once!" },
                new ChessPiece { Name = "QUEEN", 
                                ImagePath = $"/Assets/Asset1/Queen{color}.png", 
                                IconPath = $"/Assets/Asset1/Queen{color}.png", 
                                Title = "How to Move the Queen", 
                                Description = "The Queen is the most powerful piece on the chess board. You can think of her as a combination of a Rook and a Bishop. She can move in any straight line - forward, backward, sideways, or diagonally.\r\n\r\nShe can slide as many squares as she wants in a single turn, zooming across the whole board if she needs to! However, she cannot jump over other pieces. She must stop before she hits a piece of her own color, or she can stop on an enemy piece to capture it." },
                new ChessPiece { Name = "ROOK", 
                                ImagePath = $"/Assets/Asset1/Rook{color}.png", 
                                IconPath = $"/Assets/Asset1/Rook{color}.png", 
                                Title = "How to Move the Rook", 
                                Description = "The Rook looks like a castle tower, and it moves in a straight cross shape. It can go forward, backward, left, or right.You can slide the Rook as many squares as you want - it can rush across the entire board in a single turn! However, it cannot move diagonally, and it cannot jump over other pieces. It must stop if a friendly piece is in the way, or it can land on an enemy piece to capture it.Note: The Rook is also the only piece that helps the King perform the special 'Castling' move." },
                new ChessPiece { Name = "BISHOP", 
                                ImagePath = $"/Assets/Asset1/Bishop{color}.png", 
                                IconPath = $"/Assets/Asset1/Bishop{color}.png", 
                                Title = "How to Move the Bishop", 
                                Description = "The Bishop is the master of diagonals. It can only move in an 'X' shape - diagonally forward or diagonally backward.Each player starts with two Bishops: one that sits on a white square and one on a black square. A Bishop is stuck on its starting color forever! The 'white - squared' Bishop can only ever go on white squares, and the 'black - squared' Bishop can only ever go on black squares.Like the Rook and Queen, the Bishop can slide as far as it wants in one turn, as long as it doesn't jump over anyone." },
                new ChessPiece { Name = "KNIGHT", 
                                ImagePath = $"/Assets/Asset1/Knight{color}.png", 
                                IconPath = $"/Assets/Asset1/Knight{color}.png", 
                                Title = "How to Move the Knight", 
                                Description = "The Knight has the most unique move in the game. It moves in an 'L' shape: two squares in one straight direction (up, down, left, or right), and then one square to the side.The Knight has a special superpower: it is the only piece that can jump. It doesn't matter if there are pieces in its way; the Knight hops right over them to get to its destination.A cool trick to remember: The Knight always lands on a square of the opposite color. If it starts on white, it finishes on black!" },
                new ChessPiece { Name = "PAWN", 
                                ImagePath = $"/Assets/Asset1/Pawn{color}.png", 
                                IconPath = $"/Assets/Asset1/Pawn{color}.png", 
                                Title = "How to Move the Pawn", 
                                Description = "Pawns are the foot soldiers of chess. They are brave and can only move forward — never backward.The March: Usually, a Pawn moves just one square forward. However, on its very first move of the game, it has the option to charge two squares forward instead.The Attack: Pawns are unique because they don't capture the same way they move. A Pawn captures enemy pieces diagonally forward (one square). If an enemy is standing directly in front of a Pawn, the Pawn is blocked and cannot move or capture.The Reward: If a Pawn makes it all the way to the other side of the board, it gets a promotion! It instantly transforms into any other piece you want (usually a Queen)." },
            };
            Pieces = tempPieces;
            OnPropertyChanged("Pieces");
            SelectedPiece = Pieces[0];
        }
        private void BtnWhite_Click (object sender, MouseButtonEventArgs e)
        {
            if (_currentColor != "W")
            {
                LoadData("W");
                BtnWhite.StrokeThickness = 3;
                BtnBlack.StrokeThickness = 0.5;
            }
        }
        private void BtnBlack_Click(object sender, MouseButtonEventArgs e)
        {
            if (_currentColor != "B")
            {
                LoadData("B");
                BtnWhite.StrokeThickness = 0.5;
                BtnBlack.StrokeThickness = 3;
            }
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

            double step = 1.05;
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
            double step = 1.05;

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

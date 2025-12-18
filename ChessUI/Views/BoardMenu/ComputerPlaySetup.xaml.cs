using ChessLogic;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChessUI.Views.BoardMenu
{
    public class ComputerMatchSettings
    {
        public int AiDepth { get; set; }
        public Player PlayerSide { get; set; }
    }

    public partial class ComputerPlaySetup : UserControl
    {
        public event EventHandler<ComputerMatchSettings> OnStartGameClicked;

        #region CurrentRotation 
        public double CurrentRotation
        {
            get { return (double)GetValue(CurrentRotationProperty); }
            set { SetValue(CurrentRotationProperty, value); }
        }
        public static readonly DependencyProperty CurrentRotationProperty =
            DependencyProperty.Register("CurrentRotation", typeof(double), typeof(ComputerPlaySetup), new PropertyMetadata(0.0));

        public string SelectedSide { get; private set; } = "white";

        public ComputerPlaySetup()
        {
            InitializeComponent();
            LoadSideOptions();
            CurrentRotation = Math.PI / 2;
        }

        // ... (Giữ nguyên các hàm LoadSideOptions, Side_MouseLeftButtonDown, RotateToItem...) ...

        private void LoadSideOptions()
        {
            var options = new List<SideOption>
            {
                new SideOption { Name = "Trắng", ImagePath = "/Assets/Asset1/KingW.png", Id = "white" },
                new SideOption { Name = "Ngẫu nhiên", ImagePath = "/Assets/MenuAssets/randomBox.png", Id = "random" },
                new SideOption { Name = "Đen", ImagePath = "/Assets/Asset1/KingB.png", Id = "black" }
            };
            SideSelectionCarousel.ItemsSource = options;
        }

        private void Side_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as FrameworkElement;
            if (grid.DataContext is SideOption selectedOpt)
            {
                SelectedSide = selectedOpt.Id;
                int index = SideSelectionCarousel.Items.IndexOf(selectedOpt);
                RotateToItem(index);
            }
        }

        private void RotateToItem(int index)
        {
            int totalItems = SideSelectionCarousel.Items.Count;
            if (totalItems == 0) return;

            double step = (2 * Math.PI) / totalItems;
            double currentItemAngle = CurrentRotation + (index * step);
            double targetPosition = Math.PI / 2;
            double delta = targetPosition - currentItemAngle;

            while (delta > Math.PI) delta -= 2 * Math.PI;
            while (delta < -Math.PI) delta += 2 * Math.PI;

            double targetRotation = CurrentRotation + delta;

            DoubleAnimation animation = new DoubleAnimation();
            animation.To = targetRotation;
            animation.Duration = TimeSpan.FromSeconds(0.5);
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

            this.BeginAnimation(CurrentRotationProperty, animation);
        }
        #endregion

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            int depth = 1;
            if (rbBeginner.IsChecked == true) depth = int.Parse(rbBeginner.Tag.ToString());
            else if (rbLiem.IsChecked == true) depth = int.Parse(rbLiem.Tag.ToString());
            else if (rbHikaru.IsChecked == true) depth = int.Parse(rbHikaru.Tag.ToString());
            else if (rbGarry.IsChecked == true) depth = int.Parse(rbGarry.Tag.ToString());
            else if (rbMagnus.IsChecked == true) depth = int.Parse(rbMagnus.Tag.ToString());

            Player playerSide = Player.White;

            if (SelectedSide == "white") playerSide = Player.White;
            else if (SelectedSide == "black") playerSide = Player.Black;
            else playerSide = (new Random().Next(0, 2) == 0) ? Player.White : Player.Black;

            var settings = new ComputerMatchSettings
            {
                AiDepth = depth,
                PlayerSide = playerSide
            };

            OnStartGameClicked?.Invoke(this, settings);
        }
    } 

    #region Option Data Model
    public class SideOption
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Id { get; set; }
    }
    #endregion

    #region Carousel Algorithm
    public class CarouselPanel : Panel
    {
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(CarouselPanel),
                new PropertyMetadata(150.0, OnPropertyChanged));

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(CarouselPanel),
                new PropertyMetadata(40.0, OnPropertyChanged));

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(CarouselPanel),
                new PropertyMetadata(0.0, OnPropertyChanged));

        public double RadiusX { get { return (double)GetValue(RadiusXProperty); } set { SetValue(RadiusXProperty, value); } }
        public double RadiusY { get { return (double)GetValue(RadiusYProperty); } set { SetValue(RadiusYProperty, value); } }
        public double RotationAngle { get { return (double)GetValue(RotationAngleProperty); } set { SetValue(RotationAngleProperty, value); } }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CarouselPanel).InvalidateArrange();
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
            double step = (2 * Math.PI) / count;

            for (int i = 0; i < count; i++)
            {
                UIElement child = InternalChildren[i];
                double angle = RotationAngle + (i * step);

                double x = centerX + Math.Cos(angle) * RadiusX;
                double y = centerY + Math.Sin(angle) * RadiusY;

                double scaleVal = 0.5 + ((Math.Sin(angle) + 1) * 0.35);

                child.RenderTransformOrigin = new Point(0.5, 0.5);
                child.RenderTransform = new ScaleTransform(scaleVal, scaleVal);
                child.Opacity = 0.6 + ((Math.Sin(angle) + 1) * 0.2);

                int zIndex = (int)(scaleVal * 100);
                Panel.SetZIndex(child, zIndex);

                double childWidth = child.DesiredSize.Width;
                double childHeight = child.DesiredSize.Height;
                child.Arrange(new Rect(x - childWidth / 2, y - childHeight / 2, childWidth, childHeight));
            }
            return finalSize;
        }
    }
    #endregion

} 
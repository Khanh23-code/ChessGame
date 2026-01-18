using ChessLogic;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI.Views.BoardMenu
{
    public partial class UserProfileView : UserControl
    {
        // Sự kiện để báo cho MainWindow biết cần đóng Overlay
        public event EventHandler CloseRequested;

        public UserProfileView(UserData user)
        {
            InitializeComponent();
            LoadUserData(user);
        }

        private void LoadUserData(UserData user)
        {
            if (user == null) return;

            txtUsername.Text = user.UserName;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
            txtAge.Text = user.Age.ToString();
            txtLevel.Text = $"Rank: {user.Level}";
            txtJoinDate.Text = user.CreatedAt;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
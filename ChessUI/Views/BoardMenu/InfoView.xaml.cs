using ChessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUI.Views.BoardMenu
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public event Action OnPauseRequested;
        public event Action<Option> OptionSelected;

        private int count = 1;
        private string whiteData = "";
        private MoveData moveData;

        public InfoView(UserData user)
        {
            InitializeComponent();
            txtUserNameInfoView.Text = user.UserName;
            lvMovementInfo.Items.Clear();
        }

        public void ShowData(string data, Player current) 
        {
            if (current == Player.White)
            {
                moveData = new MoveData(count, data, "");
                lvMovementInfo.Items.Add(moveData);
                whiteData = data;
                lvMovementInfo.ScrollIntoView(moveData);
            }
            else if (current == Player.Black)
            {
                if (lvMovementInfo.Items.Count > 0)
                {
                    lvMovementInfo.Items.RemoveAt(lvMovementInfo.Items.Count - 1);
                }

                moveData = new MoveData(count, whiteData, data);
                lvMovementInfo.Items.Add(moveData);
                count++;

                lvMovementInfo.ScrollIntoView(moveData);
            }
        }

        public void ClearData()
        {
            lvMovementInfo.Items.Clear();
            count = 1;
            whiteData = "";
        }
        private void BtnNav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string action = btn.Tag.ToString();
                if (action == "Pause")
                {
                    OnPauseRequested?.Invoke();
                    return;
                }
                if (lvMovementInfo.Items.Count == 0) return;

                int currentIndex = lvMovementInfo.SelectedIndex;

                switch (action)
                {
                    case "First":
                        lvMovementInfo.SelectedIndex = 0;
                        break;

                    case "Prev":
                        if (currentIndex > 0)
                        {
                            lvMovementInfo.SelectedIndex = currentIndex - 1;
                        }
                        else if (currentIndex == -1)
                        {
                            lvMovementInfo.SelectedIndex = 0;
                        }
                        break;

                    case "Next":
                        if (currentIndex < lvMovementInfo.Items.Count - 1)
                        {
                            lvMovementInfo.SelectedIndex = currentIndex + 1;
                        }
                        break;

                    case "Last":
                        lvMovementInfo.SelectedIndex = lvMovementInfo.Items.Count - 1;
                        break;
                }
                if (lvMovementInfo.SelectedItem != null)
                {
                    lvMovementInfo.ScrollIntoView(lvMovementInfo.SelectedItem);
                }
            }
        }
        private void ButtonResign_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Resign);
        }

        private void ButtonOfferDraw_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Draw);
        }
    }
}

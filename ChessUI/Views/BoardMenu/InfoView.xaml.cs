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
        private int count = 1;
        private string whiteData = "";
        private MoveData moveData;

        public InfoView()
        {
            InitializeComponent();

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
                lvMovementInfo.Items.RemoveAt(lvMovementInfo.Items.Count - 1);

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
    }
}

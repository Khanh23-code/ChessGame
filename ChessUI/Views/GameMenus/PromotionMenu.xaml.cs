using ChessLogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for PromotionMenu.xaml
    /// </summary>
    public partial class PromotionMenu : UserControl
    {
        #region Properties / Constructors
        public event Action<PieceType> PieceSelected;

        public PromotionMenu(Player player)
        {
            InitializeComponent();

            imgQueen.Source = Images.GetImage(player, PieceType.Queen);
            imgRook.Source = Images.GetImage(player, PieceType.Rook);
            imgKnight.Source = Images.GetImage(player, PieceType.Knight);
            imgBishop.Source = Images.GetImage(player, PieceType.Bishop);
        }
        #endregion

        #region Events
        private void imgQueen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Queen);
        }

        private void imgRook_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Rook);
        }

        private void imgKnight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Knight);
        }

        private void imgBishop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Bishop);
        }
        #endregion
    }
}

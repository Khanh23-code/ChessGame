﻿using ChessLogic;
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

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for GameOverMenu.xaml
    /// </summary>
    public partial class GameOverMenu : UserControl
    {
        public event Action<Option> OptionSelected;

        public GameOverMenu(GameState gameState) 
        {
            InitializeComponent();

            Result result = gameState.Result;
            txtWinner.Text = GetWinnerText(result.Winner);
            txtReason.Text = GetReasonText(result.Reason, gameState.CurrentPlayer);

        }

        private static string GetWinnerText(Player winner)
        {
            switch(winner)
            {
                case Player.White: return "WHITE WINS!!!";
                case Player.Black: return "BLACK WINS!!!";
                default: return "IT'S A DRAW!!!";
            }
        }

        private static string PlayerString(Player player)
        {
            switch(player)
            {
                case Player.White: return "WHITE";
                case Player.Black: return "BLACK";
                default: return "";
            }
        }

        private static string GetReasonText(EndReason reason, Player currentPlayer)
        {
            return reason switch
            {
                EndReason.Stalemate => $"STALEMATE - {PlayerString(currentPlayer)} CAN'T MOVE",
                EndReason.Checkmate => $"CHECKMATE - {PlayerString(currentPlayer)} CAN'T MOVE",
                EndReason.FiftyMoveRule => "FIFTY MOVE RULE",
                EndReason.InsufficentMaterial => "INSUFFICENT MATERIAL",
                EndReason.ThreefoldRepetition => "THREE FOLD RÊPTITION",
                _ => ""
            };
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }
    }
}

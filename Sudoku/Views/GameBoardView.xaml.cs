using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Sudoku.ViewModels;

namespace Sudoku.Views
{
    public partial class GameBoardView : UserControl
    {
        public GameBoardView()
        {
            InitializeComponent();
        }

        private GameBoardViewModel _gameBoard;
        public GameBoardViewModel GameBoard
        {
            get { return _gameBoard; }
            set
            {
                _gameBoard = value;
                BindBoard();
            }
        }

        #region Private Methods
        private void ChildBoxClicked(object sender, EventArgs e)
        {
            SquareViewModel inputSquare = (SquareViewModel)((SquareView)sender).DataContext;
            if (GameBoard.SelectedBox != null)
            {
                GameBoard.SelectedBox.IsSelected = false;
            }

            if (GameBoard.SelectedBox == inputSquare || !inputSquare.IsEditable)
            {
                GameBoard.SelectedBox = null;
            }
            else
            {
                GameBoard.SelectedBox = inputSquare;
                GameBoard.SelectedBox.IsSelected = true;
            }
        }

        private void BindBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    SquareViewModel square = GameBoard.GameArray[i, j];
                    SquareView uiSquare = new SquareView(square);
                    LayoutRoot.Children.Add(uiSquare);
                    uiSquare.BoxClicked += new EventHandler(ChildBoxClicked);

                    Grid.SetRow(uiSquare, i);
                    Grid.SetColumn(uiSquare, j);

                    uiSquare.DataContext = square;
                }
            }
        }

        #endregion
    }

}

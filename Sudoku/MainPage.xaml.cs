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
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Sudoku.ViewModels;

namespace Sudoku
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;
            InputControl.SendInput += new EventHandler(InputControl_SendInput);
        }

        void InputControl_SendInput(object sender, EventArgs e)
        {
            MainBoard.GameBoard.SendInput((int)sender);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GameBoardViewModel board = GameBoardViewModel.LoadFromDisk();
            if (board == null)
                board = GameBoardViewModel.LoadNewPuzzle();

            MainBoard.GameBoard = board;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MainBoard.GameBoard.SaveToDisk();
            base.OnNavigatedFrom(e);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            switch (e.Orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                case PageOrientation.LandscapeRight:
                    TitlePanel.Visibility = Visibility.Collapsed;
                    Grid.SetColumn(InputControl, 1);
                    Grid.SetRow(InputControl, 0);
                    InputControl.RotateVertical();
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                case PageOrientation.PortraitDown:
                    TitlePanel.Visibility = Visibility.Visible;
                    Grid.SetColumn(InputControl, 0);
                    Grid.SetRow(InputControl, 1);
                    InputControl.RotateHorizontal();
                    break;
                default:
                    break;
            }
            base.OnOrientationChanged(e);
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            MainBoard.GameBoard = GameBoardViewModel.LoadNewPuzzle();
        }

        private void Solve_Click(object sender, EventArgs e)
        {
            MainBoard.GameBoard.Solve();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            MainBoard.GameBoard.Clear();
        }
    }
}

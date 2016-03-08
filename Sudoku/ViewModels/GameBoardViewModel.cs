using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Sudoku.Utility;
using System.Text;

namespace Sudoku.ViewModels
{
    public class GameBoardViewModel : ViewModelBase
    {
        const string FileName = "gameboard.dat";

        public SquareViewModel SelectedBox { get; set; }
        public SquareViewModel[,] GameArray { get; set; }

        public void SendInput(int inputValue)
        {
            if (SelectedBox != null)
            {
                SelectedBox.Value = inputValue;
                ValidateBoard();
                SelectedBox.IsSelected = false;
                SelectedBox = null;
            }
        }

        public void SaveToDisk()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(FileName))
                {
                    store.DeleteFile(FileName);
                }

                using (IsolatedStorageFileStream stream = store.CreateFile(FileName))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        List<SquareViewModel> s = new List<SquareViewModel>();
                        foreach (SquareViewModel item in GameArray)
                            s.Add(item);

                        XmlSerializer serializer = new XmlSerializer(s.GetType());
                        serializer.Serialize(writer, s);
                    }
                }
            }
        }

        public static GameBoardViewModel LoadFromDisk()
        {
            GameBoardViewModel result = null;

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(FileName))
                {
                    using (IsolatedStorageFileStream stream = store.OpenFile(FileName, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            List<SquareViewModel> s = new List<SquareViewModel>();
                            XmlSerializer serializer = new XmlSerializer(s.GetType());
                            s = (List<SquareViewModel>)serializer.Deserialize(new StringReader(reader.ReadToEnd()));

                            result = new GameBoardViewModel();
                            result.GameArray = LoadFromSquareList(s);
                        }
                    }
                }
            }

            return result;
        }

        public static GameBoardViewModel LoadNewPuzzle()
        {
            GameBoardViewModel result = new GameBoardViewModel();

            Random random = new Random();
            string easyPuzzle = SavedBoards.EasyGames[random.Next(0, SavedBoards.EasyGames.Length - 1)];
            List<SquareViewModel> squares = new List<SquareViewModel>();
            foreach (char s in easyPuzzle.ToCharArray())
            {
                SquareViewModel square = new SquareViewModel();
                if (s != '.')
                {
                    square.Value = int.Parse(s.ToString());
                    square.IsEditable = false;
                }
                squares.Add(square);
            }

            result.GameArray = LoadFromSquareList(squares);
            return result;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (SquareViewModel item in GameArray)
            {
                result.Append(item.Value.ToString());
            }
            return result.ToString();
        }

        public void Solve()
        {
            SendInput(0);//deselect item
            Clear();
            int x = 0;
            int y = 0;
            Dictionary<string, string> solved = LinqSudokuSolver.parse_grid(this.ToString());
            LinqSudokuSolver.print_board(solved);
            foreach (KeyValuePair<string, string> item in solved)
            {
                string key = item.Key;
                string value = item.Value;
                GameArray[y, x].Value = int.Parse(item.Value);

                x++;
                if (x == 9)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public void Clear()
        {
            foreach (SquareViewModel item in GameArray)
            {
                if (item.IsEditable)
                {
                    item.Value = 0;
                    item.IsValid = true;
                }
            }
        }

        private void ValidateBoard()
        {
            SelectedBox.IsValid = true;
            if (SelectedBox.Value > 0)
            {
                ValidateRow(SelectedBox.Row);
                ValidateColumn(SelectedBox.Col);
                ValidateGroup(SelectedBox.Row, SelectedBox.Col);
            }
        }

        private void ValidateRow(int rowNumber)
        {
            for (int i = 0; i < 9; i++)
            {
                if (GameArray[rowNumber, i] != SelectedBox)
                {
                    if (SelectedBox.Value == (GameArray[rowNumber, i].Value))
                    {
                        SelectedBox.IsValid = false;
                    }
                }
            }
        }

        private void ValidateColumn(int columnNumber)
        {
            for (int i = 0; i < 9; i++)
            {
                if (GameArray[i, columnNumber] != SelectedBox)
                {
                    if (SelectedBox.Value == (GameArray[i, columnNumber].Value))
                    {
                        SelectedBox.IsValid = false;
                    }
                }
            }
        }

        /// <summary>
        /// assumes row and column have already been validated, so it doesn't check those squares
        /// </summary>
        private void ValidateGroup(int rowNumber, int columnNumber)
        {
            int row1 = 0;
            int row2 = 0;
            int column1 = 0;
            int column2 = 0;

            switch (rowNumber % 3)
            {
                case 0:
                    row1 = rowNumber + 1;
                    row2 = rowNumber + 2;
                    break;
                case 1:
                    row1 = rowNumber - 1;
                    row2 = rowNumber + 1;
                    break;
                case 2:
                    row1 = rowNumber - 2;
                    row2 = rowNumber - 1;
                    break;
            }
            switch (columnNumber % 3)
            {
                case 0:
                    column1 = columnNumber + 1;
                    column2 = columnNumber + 2;
                    break;
                case 1:
                    column1 = columnNumber - 1;
                    column2 = columnNumber + 1;
                    break;
                case 2:
                    column1 = columnNumber - 2;
                    column2 = columnNumber - 1;
                    break;
            }

            if (GameArray[row1, column1].Value == SelectedBox.Value)
                SelectedBox.IsValid = false;
            else if (GameArray[row1, column2].Value == SelectedBox.Value)
                SelectedBox.IsValid = false;
            else if (GameArray[row2, column1].Value == SelectedBox.Value)
                SelectedBox.IsValid = false;
            else if (GameArray[row2, column2].Value == SelectedBox.Value)
                SelectedBox.IsValid = false;
        }

        private static SquareViewModel[,] LoadFromSquareList(List<SquareViewModel> list)
        {
            SquareViewModel[,] result = new SquareViewModel[9, 9];
            int counter = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = list[counter];
                    result[i, j].Row = i;
                    result[i, j].Col = j;
                    counter += 1;
                }
            }

            return result;
        }
    }
}

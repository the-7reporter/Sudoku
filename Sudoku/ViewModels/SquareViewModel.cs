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
using Sudoku.Utility;


namespace Sudoku.ViewModels
{
    public class SquareViewModel : ViewModelBase
    {
        #region Properties
        private int _value;
        public int Value {
            get
            {
                return _value;
            }
            set
            {
                if (IsEditable)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                    NotifyPropertyChanged("StringValue");
                    UpdateState();
                }
            }
        }

        public string StringValue 
        { 
            get 
            { 
                return (_value < 1 || _value > 9) ? "" : _value.ToString(); 
            } 
        }

        public int Row { get; set; }
        public int Col { get; set; }

        private bool _isValid = true;
        public bool IsValid 
        {
            get
            { 
                return _isValid; 
            }
            set
            {
                _isValid = value;
                UpdateState();
            }
        }

        private bool _isEditable = true;
        public bool IsEditable 
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                UpdateState();
            }
        }

        private bool _isSelected;
        public bool IsSelected 
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                UpdateState();
            }
        }

        private int _currentBoxState = BoxStates.Default;
        public int CurrentBoxState 
        {
            get
            {
                return _currentBoxState;
            }
            set
            {
                _currentBoxState = value;
                NotifyPropertyChanged("CurrentBoxState");
            }
        }
        #endregion

        private void UpdateState()
        {
            if (_isEditable)
            {
                if (_isSelected)
                    CurrentBoxState = BoxStates.Selected;
                else if (!_isValid)
                    CurrentBoxState = BoxStates.Invalid;
                else
                    CurrentBoxState = BoxStates.Default;
            }
            else
            {
                CurrentBoxState = BoxStates.UnEditable;
            }
        }

    }
}

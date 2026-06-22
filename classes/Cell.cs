using System.ComponentModel;
using System.Globalization;

namespace table_editor.classes
{
    public class Cell : INotifyPropertyChanged
    {
        private string _value = string.Empty;
        private string _displayValue = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Value
        {
            get => _value;
            set
            {
                _value = value ?? string.Empty;
                DetectType();
                if (Type != CellType.Formula)
                    _displayValue = _value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        /// <summary>
        /// Value shown in the grid cell. For formula cells this is the computed result;
        /// for all other cells it mirrors <see cref="Value"/>.
        /// </summary>
        public string DisplayValue
        {
            get => _displayValue;
            set
            {
                _displayValue = value ?? string.Empty;
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public CellType Type { get; private set; }

        public Cell(string value = "")
        {
            Value = value;
        }

        public override string ToString() => Value;

        private void DetectType()
        {
            if (string.IsNullOrEmpty(_value))
            {
                Type = CellType.String;
                return;
            }

            if (_value[0] == '=')
            {
                Type = CellType.Formula;
                return;
            }

            if (int.TryParse(_value, out _))
            {
                Type = CellType.Integer;
                return;
            }

            if (double.TryParse(_value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                Type = CellType.Float;
                return;
            }

            Type = CellType.String;
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

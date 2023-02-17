using Kameleon2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kameleon2.ViewModel
{
    public class KameleonField : ViewModelBase
    {
        private bool _isLocked;
        private string _player;
        private string _color;


        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Player
        {
            get { return _player; }
            set
            {
                if (_player != value)
                {
                    _player = value;
                    OnPropertyChanged();
                }


            }
        }


        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Position { get; set; }

        public DelegateCommand? StepCommand { get; set; }
    }
}

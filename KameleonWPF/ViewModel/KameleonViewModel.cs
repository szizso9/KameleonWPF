using Kameleon2.Model;
using Kameleon2.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Drawing;
using System.Windows.Media;

namespace Kameleon2.ViewModel
{
    public class KameleonViewModel : ViewModelBase
    {
        private KameleonGameModel _model;


        public DelegateCommand NewGameCommand { get; private set; }


        public DelegateCommand LoadGameCommand { get; private set; }



        public DelegateCommand SaveGameCommand { get; private set; }


        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<KameleonField> Fields { get; set; }



        public string WhosRound { get { return _model.Player == Player.Green ? "Zöld" : "Piros"; } }

        public int getSize
        {
            get
            {
                switch (_model.fMapType)
                {
                    case MapType.Little:
                        return 3;

                    case MapType.Medium:
                        return 5;

                    case MapType.Large:
                        return 7;

                }
                return 5;
            }
        }

        public bool IsMapLittle
        {
            get { return _model.fMapType == MapType.Little; }
            set
            {
                if (_model.fMapType == MapType.Little)
                    return;

                _model.fMapType = MapType.Little;

                OnPropertyChanged(nameof(IsMapLittle));
                OnPropertyChanged(nameof(IsMapMedium));
                OnPropertyChanged(nameof(IsMapLarge));
            }
        }
        public bool IsMapMedium
        {
            get { return _model.fMapType == MapType.Medium; }
            set
            {
                if (_model.fMapType == MapType.Medium)
                    return;

                _model.fMapType = MapType.Medium;

                OnPropertyChanged(nameof(IsMapLittle));
                OnPropertyChanged(nameof(IsMapMedium));
                OnPropertyChanged(nameof(IsMapLarge));
            }
        }
        public bool IsMapLarge
        {
            get { return _model.fMapType == MapType.Large; }
            set
            {
                if (_model.fMapType == MapType.Large)
                    return;

                _model.fMapType = MapType.Large;

                OnPropertyChanged(nameof(IsMapLittle));
                OnPropertyChanged(nameof(IsMapMedium));
                OnPropertyChanged(nameof(IsMapLarge));
            }
        }


        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;


        public event EventHandler? SaveGame;


        public event EventHandler? ExitGame;

        private Point p1;
        private Point p2;

        public KameleonViewModel(KameleonGameModel model)
        {

            p1 = new Point(-1, -1);
            p2 = new Point(-1, -1);

            _model = model;
            _model.GameAdvanced += new EventHandler<KameleonEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<KameleonEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<KameleonEventArgs>(Model_GameCreated);
            _model.Change += new EventHandler<ChangeEventArgs>(Model_Change);
            _model.SuccessStep += new EventHandler<TwoPlayerArgs>(Model_SuccessStep);
            _model.FailureStep += new EventHandler<KameleonEventArgs>(Model_FailureStep);

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            OnPropertyChanged(nameof(getSize));


            Fields = new ObservableCollection<KameleonField>();

            for (int i = 0; i < _model.Map.MapSize; i++)
            {
                for (int j = 0; j < _model.Map.MapSize; j++)
                {
                    string a = "";
                    if (_model.Map.getFieldsColor(i, j) == Kameleon2.Persistence.Color.Red)
                    {
                        a = "Red";
                    }
                    else if (_model.Map.getFieldsColor(i, j) == Kameleon2.Persistence.Color.Green)
                    {
                        a = "Green";
                    }
                    else
                    {
                        a = "White";
                    }

                    Fields.Add(new KameleonField
                    {
                        IsLocked = false,
                        Color = a,
                        X = i,
                        Y = j,
                        Position = i * _model.Map.MapSize + j,

                        StepCommand = new DelegateCommand(param => Choose(Convert.ToInt32(param)))

                    });
                }
            }

            RefreshTable();
        }

        private void Choose(int x)
        {
            if (p1.X == -1)
            {
                KameleonField f = Fields[x];
                if (_model.Map.isEmpty(new Point(f.X, f.Y))) return;
                p1.X = f.X;
                p1.Y = f.Y;
            }
            else
            {
                KameleonField fa = Fields[x];
                p2.X = fa.X;
                p2.Y = fa.Y;

                if (p1 != p2)
                {
                    StepGame(p1, p2);
                }

                p1 = new Point(-1, -1);
                p2 = new Point(-1, -1);

            }
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshTable()
        {
            foreach (KameleonField field in Fields)
            {
                if (_model.Map.getFieldsPlayer(field.X, field.Y) == Kameleon2.Persistence.Color.Red)
                {
                    
                    field.Player ="Piros";
                }
                else if (_model.Map.getFieldsPlayer(field.X, field.Y) == Kameleon2.Persistence.Color.Green)
                {
                    field.Player = "Zöld";
                }
                else
                {
                    field.Player = String.Empty;
                }
            }

        }

        private void StepGame(Point x, Point y)
        {
            _model.Step(x, y);

        }

        private void Model_FailureStep(object? sender, KameleonEventArgs e)
        {

        }

        private void Model_SuccessStep(object? sender, TwoPlayerArgs e)
        {
            _model.Advance();
        }

        private void Model_Change(object? sender, ChangeEventArgs e)
        {
            RefreshTable();
        }

        private void Model_GameCreated(object? sender, KameleonEventArgs e)
        {

            OnPropertyChanged(nameof(getSize));

            Fields.Clear();

            for (int i = 0; i < _model.Map.MapSize; i++)
            {
                for (int j = 0; j < _model.Map.MapSize; j++)
                {
                    string a = "";
                    if (_model.Map.getFieldsColor(i, j) == Kameleon2.Persistence.Color.Red)
                    {
                        a = "Red";
                    }
                    else if (_model.Map.getFieldsColor(i, j) == Kameleon2.Persistence.Color.Green)
                    {
                        a = "Green";
                    }
                    else
                    {
                        a = "White";
                    }

                    Fields.Add(new KameleonField
                    {
                        IsLocked = false,
                        Player = a,
                        Color = a,
                        X = i,
                        Y = j,
                        Position = i * _model.Map.MapSize + j,

                        StepCommand = new DelegateCommand(param => Choose(Convert.ToInt32(param)))

                    });
                }
            }

            OnPropertyChanged(nameof(WhosRound));
            RefreshTable();

        }

        private void Model_GameOver(object? sender, KameleonEventArgs e)
        {
            foreach (KameleonField field in Fields)
            {
                field.IsLocked = true;
            }
        }

        private void Model_GameAdvanced(object? sender, KameleonEventArgs e)
        {
            RefreshTable();
            OnPropertyChanged(nameof(WhosRound));
        }
    }

}

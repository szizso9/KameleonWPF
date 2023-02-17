using System;
using Kameleon2.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace Kameleon2.Model
{
    
    public enum MapType { Little, Medium, Large }

    public class KameleonGameModel
    {
        
        

        private IKameleonDataAccess _dataAccess;
        private KameleonMap _map;
        private Player _player;
        private MapType _mapType;
        private Dictionary<Point, int> _enemyT;
        


        public MapType fMapType { get { return _mapType; } set { _mapType = value; } }
 
        public KameleonMap Map { get { return _map; } set { _map = value; } }

        public Player Player { get { return _player; }}

        public event EventHandler<KameleonEventArgs>? GameAdvanced;
        public event EventHandler<KameleonEventArgs>? GameOver;
        public event EventHandler<ChangeEventArgs>? Change;
        public event EventHandler<TwoPlayerArgs>? SuccessStep;
        public event EventHandler<KameleonEventArgs>? FailureStep;
        public event EventHandler<KameleonEventArgs>? GameCreated;

        public KameleonGameModel(IKameleonDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _mapType = MapType.Medium;
            _player = Player.Green;
            _map = new KameleonMap(5);
            _enemyT = new Dictionary<Point, int>();
            GenerateMap(5);
            
            
        }

        public async Task SaveGameAsync(string path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _map);
        }

        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            _map = await _dataAccess.LoadAsync(path);


            switch (_map.MapSize)
            {
                case 3:
                    _mapType = MapType.Little;
                    break;
                case 5:
                    _mapType = MapType.Medium;
                    break;
                case 7:
                    _mapType = MapType.Large;
                    break;
            }

            OnGameCreated();

        }

        private void GotChanged(Point a)
        {
            _map.setFieldsPlayer(a.X, a.Y, _map.getFieldsColor(a.X, a.Y));
            _enemyT.Remove(a);

            
            Change?.Invoke(this, new ChangeEventArgs(a));

        }

        public void Advance()
        {

            for (int i = 0; i < _enemyT.Count; i++)
            {
                var key = _enemyT.ElementAt(i).Key;
                _enemyT[key]++;

                if (_enemyT[key] > 2)
                {
                    GotChanged(key);
                }    
            }

            if (_map.isOver())
                OnGameOver();

            _player = _player == Player.Red ? Player.Green : Player.Red; 
            OnGameAdvanced();


        }
        public void Step(Point a, Point b)
        {
            if (a.X < 0 || a.X >= _map.MapSize)
                throw new ArgumentOutOfRangeException("a.X", "The X coordinate is out of range.");
            if (a.Y < 0 || a.Y >= _map.MapSize)
                throw new ArgumentOutOfRangeException("a.Y", "The Y coordinate is out of range.");
            if (b.X < 0 || b.X >= _map.MapSize)
                throw new ArgumentOutOfRangeException("b.X", "The X coordinate is out of range.");
            if (b.Y < 0 || b.Y >= _map.MapSize)
                throw new ArgumentOutOfRangeException("b.Y", "The Y coordinate is out of range.");


            if (isPlayer(a.X, a.Y) && CanMove(a,b))
            {

                _map.setFieldsPlayer(b.X, b.Y, _map.getFieldsPlayer(a.X, a.Y));
                _map.setFieldsPlayer(a.X, a.Y, Persistence.Color.Empty);

                if(_enemyT.ContainsKey(a) && _map.getFieldsPlayer(b.X,b.Y) != _map.getFieldsColor(b.X,b.Y) && _map.getFieldsColor(b.X, b.Y) != Persistence.Color.Empty)
                {
                    _enemyT.Add(b, 3);
                }

                else if(_map.getFieldsColor(b.X,b.Y) != _map.getFieldsPlayer(b.X,b.Y) && _map.getFieldsColor(b.X,b.Y) != Persistence.Color.Empty)
                {
                    _enemyT.Add(b, 0);
                }
                RemEnemyTerr(a);


                SuccessStep?.Invoke(this, new TwoPlayerArgs(a, b));
                
            }
            else
                FailureStep?.Invoke(this,new KameleonEventArgs(_player, false));


        }

        public void fNewGame()
        {
            switch (_mapType)
            {
                case MapType.Little:
                    _map = new KameleonMap(3);
                    GenerateMap(3);
                    break;
                case MapType.Medium:
                    _map = new KameleonMap(5);
                    GenerateMap(5);
                    break;
                case MapType.Large:
                    _map = new KameleonMap(7); 
                    GenerateMap(7);
                    break;
                
            }

            _player = Player.Green;
            _enemyT = new Dictionary<Point, int>();
            OnGameCreated();

        }

        private void GenerateMap(int n)
        {
            int count = n;
            int value = -n;
            int sum = -1;

            do
            {
                value = -1 * value / n;

                for (int i = 0; i < count; i++)
                {
                    sum += value;
                    _map.setFieldsColor(sum / n, sum % n, Persistence.Color.Red);
                }

                value *= n;
                count--;

                for (int i = 0; i < count-1; i++)
                {
                    sum += value;
                    _map.setFieldsColor(sum / n, sum % n, Persistence.Color.Red);
                }
                count--;

            } while (count > 0);

            _map.setFieldsColor(n / 2, n / 2, Persistence.Color.Empty);

        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new KameleonEventArgs(Player.Green, false));
        }
        private void OnGameAdvanced()
        {
            
                if(_player == Player.Red)
                GameAdvanced?.Invoke(this, new KameleonEventArgs(Player.Red, false));
            else GameAdvanced?.Invoke(this, new KameleonEventArgs(Player.Green, false));
        }

        private void OnGameOver()
        {
            
                GameOver?.Invoke(this, new KameleonEventArgs(_map.whoWins(), true));
        }

        private bool isPlayer(int x, int y)
        {
            if (_map.getFieldsPlayer(x, y) == Persistence.Color.Red && _player == Player.Red)
                return true;
            else if (_map.getFieldsPlayer(x, y) == Persistence.Color.Green && _player == Player.Green)
                return true;
            else return false;
            
        }

        private void Kill(Point a, Point b)
        {
            Point curr = Between(a, b);
            _map.setFieldsPlayer(curr.X, curr.Y, Persistence.Color.Empty);
            RemEnemyTerr(curr);

            if (Change != null)
                Change(this, new ChangeEventArgs(curr));
        }

        private void RemEnemyTerr(Point a)
        {
            if(_enemyT.ContainsKey(a))
            {
                _enemyT.Remove(a);
            }
        }

        private int Distance(Point a, Point b)
        {
            return Math.Abs((a.X + a.Y) - (b.X + b.Y));
        }

        private Point Between(Point a, Point b)
        {

            if (a.X == b.X + 2)
            {

                return new Point(a.X - 1, a.Y);
            }

            else if (a.Y == b.Y + 2)
            {
                return new Point(a.X, a.Y - 1);
            }

            else if (a.X == b.X - 2)
            {
                return new Point(a.X + 1, a.Y);
            }

            else if (a.Y == b.Y - 2)
            {
                return new Point(a.X, a.Y + 1);
            }
            else return new Point(0, 0);


        }

        private bool CanMove(Point a, Point b)
        {
            Point c = Between(a, b);
            if (_map.isEmpty(b) && !_map.isEmpty(a) && (a.X == b.X || a.Y == b.Y))
            {
                if (Distance(a, b) == 1)
                {
                    return true;
                }
                else if(!_map.isEmpty(c) && _map.getFieldsPlayer(c.X,c.Y) != _map.getFieldsPlayer(a.X,a.Y) && Distance(a,b) == 2)
                {
                    Kill(a, b); 
                    return true;
                }
            }
            return false;
        }


    }
}

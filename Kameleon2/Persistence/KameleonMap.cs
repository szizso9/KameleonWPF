using Kameleon2.Model;
using System;
using System.Drawing;

namespace Kameleon2.Persistence
{
    public enum Color { Green, Red, Empty}



    public class KameleonMap
    {
        private int _mapSize;
        private Color[,] _fieldsColor;
        private Color[,] _fieldsPlayer;
        

        public int MapSize { get { return _mapSize; } } 


        public Color getFieldsColor (int x, int y)
        {
            if (x < 0 || x >= _fieldsColor.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldsColor.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return _fieldsColor[x, y];  
        }

        public void setFieldsColor(int x, int y, Color color)
        {
            if (x < 0 || x >= _fieldsColor.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldsColor.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            _fieldsColor[x, y] = color;
            _fieldsPlayer[x, y] = color;
        }

        public Color getFieldsPlayer(int x, int y)
        {
            if (x < 0 || x >= _fieldsPlayer.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldsPlayer.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return _fieldsPlayer[x, y];

        }

        public void setFieldsPlayer(int x, int y, Color color)
        {
            if (x < 0 || x >= _fieldsPlayer.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldsPlayer.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            _fieldsPlayer[x, y] = color;
            
        }

        

        public KameleonMap(int mapSize)
        {
            if (mapSize < 0)
                throw new ArgumentOutOfRangeException("The table size is less than 0.", "tableSize");

            _mapSize = mapSize;
            _fieldsPlayer = new Color[mapSize, mapSize];
            _fieldsColor = new Color[mapSize, mapSize];

             
        }

        

        public bool isOver()
        {
            int red = 0;
            int green = 0;

            foreach (var item in _fieldsPlayer)
            {
                if (item == Color.Red) red++;
                else if(item == Color.Green) green++;
            }

            if(red == 0 || green ==0)
                return true;
            return false;
        }

        public Player whoWins()
        {
            int green = 0;
            foreach (var item in _fieldsPlayer)
            {
                if (item == Color.Green)
                    green++;
            }

            if (green > 0) return Player.Green;
            else return Player.Red;
        }

       



        public bool isEmpty(Point a)
        {
            if (a.X < 0 || a.X >= _fieldsPlayer.GetLength(0))
                throw new ArgumentOutOfRangeException("a.X", "The X coordinate is out of range.");
            if (a.Y < 0 || a.Y >= _fieldsPlayer.GetLength(1))
                throw new ArgumentOutOfRangeException("a.Y", "The Y coordinate is out of range.");
           


            if (_fieldsPlayer[a.X, a.Y] == Color.Empty)
                return true;
            return false;
        }

       

    }
}

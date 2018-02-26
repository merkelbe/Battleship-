using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleship_
{
    internal static class m
    {
        internal static T[,] Get2DArrayWithDefaultValues<T>(int _width, int _height, T _defaultValue)
        {
            T[,] result = new T[_height, _width];
            for(int x = 0; x < _width; ++x)
            {
                for(int y = 0; y < _height; ++y)
                {
                    result[y, x] = _defaultValue;
                }
            }
            return result;
        }

        internal static T[] GetRowFrom2DArray<T>(T[,] _array, int _row)
        {
            T[] row = new T[_array.GetLength(1)];
            for(int col = 0; col < _array.GetLength(1); ++col)
            {
                row[col] = _array[_row, col];
            }
            return row;
        }

        internal static T GetRandomObject<T>(List<T> _objects)
        {
            return _objects[m.Random.Next(_objects.Count)];
        }

        internal static T GetRandomObject<T>(T[] _objects)
        {
            return _objects[m.Random.Next(_objects.Length)];
        }
        
        internal static Random Random = new Random();
    }
}

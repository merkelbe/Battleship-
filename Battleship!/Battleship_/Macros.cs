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

        internal static Random Random = new Random();
    }
}

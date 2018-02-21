using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleship_
{
    // Holds all the game logic.  All drawing and presentation should key off of this object.
    class AbstractGame
    {
        internal int Width;
        internal int Height;

        List<Ship> allShips;

        // Below boards need above 3 variables to be up-to-date to build correctly.  Boards are built when 'resetBoard' function is called.
        internal int[,] Board; // -1 mean missed shot, 0 means unknown space, 1 means shot that hit
        internal int[,] ShipPlacement; // 0 means no ship and 1 means ship

        bool GameOver { get { return allShips.All(x => x.Sunk); } }

        internal AbstractGame(int _boardWidth, int _boardHeight, List<Ship> _allShips)
        {
            Width = _boardWidth;
            Height = _boardHeight;

            allShips = new List<Ship>(_allShips);

            ResetGame();
        }
        
        internal void ResetGame()
        {
            Board = m.Get2DArrayWithDefaultValues<int>(Width, Height, 0);
            ShipPlacement = m.Get2DArrayWithDefaultValues<int>(Width, Height, 0);
            
            foreach(Ship ship in allShips)
            {
                ship.Reset();

                // NOTE: The user can set up an impossible situation here if the board size is too small or the ship sizes are too large.
                placeShip(ship);
            }
        }

        void placeShip(Ship _ship)
        {
            bool shipPlaced = false;

            while (!shipPlaced)
            {
                // Tries random orientations of ship when placing (horizontal or vertical)
                if (m.Random.Next(2) == 0)
                {
                    // Flips orientation of ship
                    int temp = _ship.Width;
                    _ship.Width = _ship.Height;
                    _ship.Height = temp;
                }
                int randomStartingX = m.Random.Next(Width - _ship.Width + 1);
                int randomStartingY = m.Random.Next(Height - _ship.Height + 1);

                bool spaceFree = true;
                for (int xOffset = 0; xOffset < _ship.Width; ++xOffset)
                {
                    for (int yOffset = 0; yOffset < _ship.Height; ++yOffset)
                    {
                        if (ShipPlacement[randomStartingY + yOffset, randomStartingX + xOffset] > 0)
                        {
                            spaceFree = false;
                            break;
                        }
                    }
                }
                if (spaceFree)
                {
                    for (int xOffset = 0; xOffset < _ship.Width; ++xOffset)
                    {
                        for (int yOffset = 0; yOffset < _ship.Height; ++yOffset)
                        {
                            int x = randomStartingX + xOffset;
                            int y = randomStartingY + yOffset;
                            ShipPlacement[y, x] = 1;
                            _ship.AddSpot(new ShipSpot(x, y));
                        }
                    }

                    shipPlaced = true;
                }
            }
        }

        internal void GuessSpot(int _x, int _y, out List<Ship> _destroyedShips)
        {
            _destroyedShips = new List<Ship>();

            // If this spot hasn't been guessed yet
            if (Board[_y,_x] == 0)
            {
                // If a ship is there.
                if(ShipPlacement[_y,_x] > 0)
                {
                    Board[_y, _x] = 1;
                    foreach(Ship ship in allShips)
                    {
                        bool alreadySunk = ship.Sunk;
                        ship.RegisterHit(_x, _y);
                        // Registers sunken ship on this shot
                        if(!alreadySunk && ship.Sunk)
                        {
                            _destroyedShips.Add(ship);
                        }
                    }
                }
                else
                {
                    // Register miss
                    Board[_y, _x] = -1;
                }
            }
        }
    }


    internal class Ship
    {
        internal string Name { get; private set; }
        internal int Width;
        internal int Height;

        internal bool Sunk;

        List<ShipSpot> shipSpots;

        internal Ship(string _name, int _width, int _height)
        {
            Name = _name;
            Width = _width;
            Height = _height;

            Sunk = false;
            shipSpots = new List<ShipSpot>();
        }

        internal void Reset()
        {
            shipSpots = new List<ShipSpot>();
        }

        internal void AddSpot(ShipSpot _spot)
        {
            if (!shipSpots.Contains(_spot))
            {
                shipSpots.Add(_spot);
            }
        }

        internal void RegisterHit(int _x, int _y)
        {
            if (!Sunk)
            {
                Sunk = true;
                foreach (ShipSpot spot in shipSpots)
                {
                    if (spot.X == _x && spot.Y == _y)
                    {
                        spot.Hit = true;
                    }
                    if (!spot.Hit)
                    {
                        Sunk = false;
                    }
                }
            }
        }
        
    }

    class ShipSpot : IEquatable<ShipSpot>
    {
        internal int X { get; private set; }
        internal int Y { get; private set; }

        internal bool Hit;

        internal ShipSpot(int _x, int _y)
        {
            X = _x;
            Y = _y;
            Hit = false;
        }

        public bool Equals(ShipSpot _other)
        {
            return this.X == _other.X && this.Y == _other.Y;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Battleship_
{
    internal class GameStatsGenerator
    {
        AbstractGame abstractGame;
        Serializer serializer;
        List<Point> allPickOptions;
        PositionGuesser positionGuesser;

        internal GameStatsGenerator()
        {
            // Sets up game configurations
            Ship destroyer = new Ship("Destroyer", 2, 1);
            Ship submarine = new Ship("Submarine", 3, 1);
            Ship cruiser = new Ship("Cruiser", 3, 1);
            Ship battleship = new Ship("Battleship", 4, 1);
            Ship carrier = new Ship("Carrier", 5, 1);

            List<Ship> allShips = new List<Ship>() { carrier, battleship, cruiser, submarine, destroyer };

            abstractGame = new AbstractGame(10, 10, allShips);
            serializer = new Serializer();

            positionGuesser = new RandomPositionGuesserWithHunting(abstractGame);
            //positionGuesser = new RandomPositionGuesser(abstractGame);
        }

        internal void Run(int _gameSimCount)
        {
            int count = -1;
            while (++count < _gameSimCount)
            {
                abstractGame.NewGame();
                positionGuesser.Reset();
                
                while (!abstractGame.GameOver)
                {
                    positionGuesser.MakeGuess();
                }

                serializer.WriteBattleshipGuessesToTextFile(abstractGame);
            }
        }
    }

    internal abstract class PositionGuesser
    {
        protected AbstractGame abstractGame;

        internal PositionGuesser(AbstractGame _abstractGame)
        {
            abstractGame = _abstractGame;
        }

        // Expects guesser to guess exactly one position that has not been guessed yet on the abstract game passed in its constructor.
        internal abstract void MakeGuess();

        // Called once at the beginning of a game.  Expects class to reset itself to a state able to play a brand new game.
        internal abstract void Reset();
    }

    internal class RandomPositionGuesser : PositionGuesser
    {
        List<Point> allPositions;
        List<Point> currentPositionsLeft;

        internal RandomPositionGuesser(AbstractGame _abstractGame):base(_abstractGame)
        {
            allPositions = new List<Point>();
            for(int x = 0; x < _abstractGame.Board.GetLength(1); ++x)
            {
                for(int y = 0; y < _abstractGame.Board.GetLength(0); ++y)
                {
                    allPositions.Add(new Point(x, y));
                }
            }
        }

        internal override void Reset()
        {
            currentPositionsLeft = new List<Point>(allPositions);
        }

        internal override void MakeGuess()
        {
            Point randomPoint = m.GetRandomObject(currentPositionsLeft);
            abstractGame.GuessSpot(randomPoint.X, randomPoint.Y);
            currentPositionsLeft.Remove(randomPoint);
        }
    }
    
    internal class RandomPositionGuesserWithHunting : PositionGuesser
    {
        bool inHuntMode;
        Point huntStartPoint;
        Direction huntDirection;

        List<Point> allPositions;
        List<Point> currentPositionsLeft;

        enum Direction { North, East, West, South }
        List<Direction> allDirections;
        List<Direction> currentDirectionsLeft;

        internal RandomPositionGuesserWithHunting(AbstractGame _abstractGame): base(_abstractGame)
        {
            // Assumes sourceHuntPoint and huntDirection will be set in 'MakeGuess' function
            inHuntMode = false;

            allPositions = new List<Point>();
            for (int x = 0; x < _abstractGame.Board.GetLength(1); ++x)
            {
                for(int y = 0; y < _abstractGame.Board.GetLength(0); ++y)
                {
                    allPositions.Add(new Point(x, y));
                }
            }

            allDirections = new List<Direction>() { Direction.North, Direction.East, Direction.West, Direction.South };
            huntStartPoint = new Point();
        }

        internal override void Reset()
        {
            currentPositionsLeft = new List<Point>(allPositions);
        }

        internal override void MakeGuess()
        {
            if (inHuntMode)
            {
                Point pointToGuess;
                while(!canFindGuessPointInDirection(huntStartPoint,abstractGame.Board,huntDirection,out pointToGuess))
                {
                    // Get a new direction to search in, trying the opposite direction if possible.
                    if (currentDirectionsLeft.Contains(opposite(huntDirection)))
                    {
                        huntDirection = opposite(huntDirection);
                        currentDirectionsLeft.Remove(huntDirection);
                    }
                    else
                    {
                        huntDirection = m.GetRandomObject(currentDirectionsLeft);
                        currentDirectionsLeft.Remove(huntDirection);
                    }
                }

                GuessResult guessResult = abstractGame.GuessSpot(pointToGuess.X, pointToGuess.Y);
                currentPositionsLeft.Remove(pointToGuess);

                if (guessResult.ShipDestroyed)
                {
                    // Hunt is over.  Note it is possible you can destroy a different ship than what you started with.  This doesn't really matter.  You'll eventually hit a random point on the starting ship and (probably) finish it off then.
                    inHuntMode = false;        
                }
                else if (!guessResult.Hit)
                {
                    // No hit.  Need to change directions.
                    huntDirection = m.GetRandomObject(currentDirectionsLeft);
                    currentDirectionsLeft.Remove(huntDirection);
                }
            }
            else
            {
                // Make random guess
                Point randomPoint = m.GetRandomObject(currentPositionsLeft);
                GuessResult result = abstractGame.GuessSpot(randomPoint.X, randomPoint.Y);
                currentPositionsLeft.Remove(randomPoint);

                if (result.Hit && !result.ShipDestroyed)
                {
                    // Initialize the hunt!
                    currentDirectionsLeft = new List<Direction>(allDirections);
                    huntDirection = m.GetRandomObject(currentDirectionsLeft);
                    currentDirectionsLeft.Remove(huntDirection);

                    huntStartPoint.X = randomPoint.X;
                    huntStartPoint.Y = randomPoint.Y;

                    inHuntMode = true;
                }
            }
        }

        Direction opposite(Direction _inputDirection)
        {
            switch (_inputDirection)
            {
                case Direction.South:
                    return Direction.North;
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.West:
                    return Direction.East;
                default: throw new Exception("Unrecognized direction type: " + _inputDirection);
            }
        }

        
        bool canFindGuessPointInDirection(Point _huntStartPoint, int[,] _board, Direction _direction, out Point _pointToGuess)
        {
            Point currentPoint = new Point(_huntStartPoint.X, _huntStartPoint.Y);

            bool stillInBounds = true;
            do
            {
                incrementPoint(ref currentPoint, _direction);
                stillInBounds = currentPoint.X >= 0 && currentPoint.Y >= 0 && currentPoint.X < _board.GetLength(1) && currentPoint.Y < _board.GetLength(0);
            } while (stillInBounds && _board[currentPoint.Y,currentPoint.X] == 1);


            if (stillInBounds && _board[currentPoint.Y,currentPoint.X] == 0)
            {
                // Reached a spot we haven't guessed yet.  Should return true.
                _pointToGuess = new Point(currentPoint.X, currentPoint.Y);
                return true;
            }
            else
            {
                // Cannot guess in this direction
                _pointToGuess = new Point();
                return false;
            }
        }

        // Moves the point in the desired direction.
        // Assumes y-axis is North/South and x-axis is East/West
        void incrementPoint (ref Point _currentPoint, Direction _direction)
        {
            switch (_direction)
            {
                case Direction.North:
                    {
                        ++_currentPoint.Y;
                        break;
                    }
                case Direction.South:
                    {
                        --_currentPoint.Y;
                        break;
                    }
                case Direction.West:
                    {
                        --_currentPoint.X;
                        break;
                    }
                case Direction.East:
                    {
                        ++_currentPoint.X;
                        break;
                    }
                default:
                    {
                        throw new Exception("Unrecognized direction: " + _direction);
                    }
            }
        }
    }
}

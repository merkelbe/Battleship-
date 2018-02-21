using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship_
{
    internal class VisualGame
    {
        AbstractGame abstractGame;

        Rectangle areaToDrawTo;

        Sprite UnknownSquare;
        Sprite MissOverlay;
        Sprite HitOverlay;
        Sprite ShipOverlay;
        Sprite ShipHitOverlay;

        internal bool ShowShips;

        internal VisualGame(AbstractGame _abstractGame, Rectangle _areaToDrawTo, VisualGameAssets _visualGameAssets)
        {
            abstractGame = _abstractGame;
            areaToDrawTo = _areaToDrawTo;

            UnknownSquare = _visualGameAssets.UnknownSquare;
            MissOverlay = _visualGameAssets.MissOverlay;
            HitOverlay = _visualGameAssets.HitOverlay;
            ShipOverlay = _visualGameAssets.ShipOverlay;
            ShipHitOverlay = _visualGameAssets.ShipHitOverlay;

            ShowShips = true;
        }


        internal void Draw(SpriteBatch _spriteBatch)
        {
            int squareWidth = areaToDrawTo.Width / abstractGame.Width;
            int squareHeight = areaToDrawTo.Height / abstractGame.Height;
            Rectangle destinationRectange = new Rectangle(areaToDrawTo.X, areaToDrawTo.Y, squareWidth, squareHeight);
            for (int y = 0; y < abstractGame.Height; ++y)
            {
                for(int x = 0; x < abstractGame.Width; ++x)
                {
                    // Updates coordinates of where we're drawing
                    destinationRectange.X = areaToDrawTo.X + x * squareWidth;
                    destinationRectange.Y = areaToDrawTo.Y + y * squareHeight;

                    // Draws base image
                    _spriteBatch.Draw(UnknownSquare.EntireImage, destinationRectange, Color.White);
                    
                    switch (abstractGame.Board[y, x])
                    {
                        case -1:
                            {
                                _spriteBatch.Draw(MissOverlay.EntireImage, destinationRectange, Color.White);
                                break;
                            }
                        case 1:
                            {
                                _spriteBatch.Draw(HitOverlay.EntireImage, destinationRectange, Color.White);
                                if (ShowShips)
                                {
                                    _spriteBatch.Draw(ShipHitOverlay.EntireImage, destinationRectange, Color.White);
                                }
                                break;
                            }
                        case 0:
                            {
                                if(ShowShips && abstractGame.ShipPlacement[y,x] > 0)
                                {
                                    _spriteBatch.Draw(ShipOverlay.EntireImage, destinationRectange, Color.White);
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception("Unsupported board state: " + abstractGame.Board[y, x]);
                            }
                    }

                }
            }

        }
    }

    struct VisualGameAssets
    {
        internal Sprite UnknownSquare;
        internal Sprite MissOverlay;
        internal Sprite HitOverlay;
        internal Sprite ShipOverlay;
        internal Sprite ShipHitOverlay;
    }
}

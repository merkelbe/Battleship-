using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Battleship_
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseState mouseState;
        MouseFilter mouseFilter;
        List<ClickInfo> currentClickInfo;

        KeyboardState keyboardState;
        KeyFilter keyboardFilter;
        List<Keys> pressedKeys;

        int WINDOW_WIDTH = 1200;
        int WINDOW_HEIGHT = 700;

        AbstractGame abstractGame;
        VisualGame visualGame;
        VisualGameAssets assets;

        SpriteFont spriteFont;

        const int updateTimeInMilliseconds = 16;
        int cumulativeElapsedTime;

        Serializer serializer = new Serializer();

        public XNAGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            mouseFilter = new MouseFilter();
            currentClickInfo = new List<ClickInfo>();

            keyboardFilter = new KeyFilter();
            pressedKeys = new List<Keys>();

            cumulativeElapsedTime = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            IsMouseVisible = true;

            Ship destroyer = new Ship("Destroyer", 2, 1);
            Ship submarine = new Ship("Submarine", 3, 1);
            Ship cruiser = new Ship("Cruiser", 3, 1);
            Ship battleship = new Ship("Battleship", 4, 1);
            Ship carrier = new Ship("Carrier", 5, 1);

            List<Ship> allShips = new List<Ship>() { carrier, battleship, cruiser, submarine, destroyer };

            abstractGame = new AbstractGame(10, 10, allShips);
            Rectangle areaToDrawTo = new Rectangle(300, 50, 600, 600);

            visualGame = new VisualGame(abstractGame, areaToDrawTo, assets);
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Image loading built into the sprite class
            assets = new VisualGameAssets();
            assets.UnknownSquare = new Sprite(Content, "unknown");
            assets.MissOverlay = new Sprite(Content, "missOverlay");
            assets.HitOverlay = new Sprite(Content, "hitOverlay");
            assets.ShipOverlay = new Sprite(Content, "shipOverlay");
            assets.ShipHitOverlay = new Sprite(Content, "shipHitOverlay");

            spriteFont = Content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            cumulativeElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            // NOTE: Could get overflow exception here if update is called less frequently than updateTimeInMilliseconds
            if (cumulativeElapsedTime > updateTimeInMilliseconds)
            {
                cumulativeElapsedTime -= updateTimeInMilliseconds;
                
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                mouseState = Mouse.GetState();
                mouseFilter.Update(mouseState, ref currentClickInfo);
                if (currentClickInfo.Count > 0)
                {
                    visualGame.RegisterClick(currentClickInfo[0]); // Registers one click at a time.  currentClickInfo is like a queue.
                    currentClickInfo.RemoveAt(0);
                }

                keyboardState = Keyboard.GetState();
                pressedKeys = keyboardFilter.GetPressedKeys(keyboardState);
                foreach(Keys key in pressedKeys)
                {
                    switch (key)
                    {
                        case Keys.R:
                            {
                                abstractGame.ResetGame();
                                break;
                            }
                        case Keys.N:
                            {
                                abstractGame.NewGame();
                                break;
                            }
                        case Keys.C:
                            {
                                visualGame.ShowShips = !visualGame.ShowShips;
                                break;
                            }
                        case Keys.S:
                            {
                                if (abstractGame.GameOver)
                                {
                                    serializer.WriteBattleshipGuessesToTextFile(abstractGame);
                                }
                                break;
                            }

                    }
                }


            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            visualGame.Draw(spriteBatch);

            Color textColor = Color.White;
            
            spriteBatch.DrawString(spriteFont, "Controls:", new Vector2(20, 20), textColor);
            spriteBatch.DrawString(spriteFont, "C - Cheat (show/hide ships)", new Vector2(20, 60), textColor);
            spriteBatch.DrawString(spriteFont, "N - New puzzle", new Vector2(20, 100), textColor);
            spriteBatch.DrawString(spriteFont, "R - Reset puzzle", new Vector2(20, 140), textColor);
            spriteBatch.DrawString(spriteFont, "S - Save moves", new Vector2(20, 180), textColor);
            spriteBatch.DrawString(spriteFont, "Number of Guesses: " + abstractGame.NumberOfGuesses, new Vector2(20, 220), textColor);
            if (abstractGame.GameOver)
            {
                spriteBatch.DrawString(spriteFont, "Game over!", new Vector2(20, 260), textColor);
            }

            spriteBatch.DrawString(spriteFont, "Mouse X: " + mouseState.X, new Vector2(20, 300), textColor);
            spriteBatch.DrawString(spriteFont, "Mouse Y: " + mouseState.Y, new Vector2(20, 320), textColor);

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

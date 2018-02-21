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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseState mouseState;
        MouseFilter mouseFilter;

        KeyboardState keyboardState;
        KeyFilter keyboardFilter;
        List<ClickInfo> currentClickInfo;

        int WINDOW_WIDTH = 1200;
        int WINDOW_HEIGHT = 700;

        AbstractGame abstractGame;
        VisualGame visualGame;
        VisualGameAssets assets;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            keyboardFilter = new KeyFilter();
            mouseFilter = new MouseFilter();

            currentClickInfo = new List<ClickInfo>();
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

            Ship destroyer = new Ship("Destroyer", 2, 1);
            Ship submarine = new Ship("Submarine", 3, 1);
            Ship cruiser = new Ship("Cruiser", 3, 1);
            Ship battleship = new Ship("Battleship", 4, 1);
            Ship carrier = new Ship("Carrier", 5, 1);

            List<Ship> allShips = new List<Ship>() { carrier, battleship, cruiser, submarine, destroyer };

            AbstractGame abstractGame = new AbstractGame(10, 10, allShips);
            Rectangle areaToDrawTo = new Rectangle(300, 50, 850, 600);

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

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

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

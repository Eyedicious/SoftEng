using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CapitalismFrontend
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Capitalism : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MathEngine.GameState gameState;
        Texture2D background;
        Rectangle gameWindowSize;

        String backgroundImageName = "background_map.png";

        public Capitalism()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Left = 0;
            form.Top = 0;

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            gameState = MathEngine.initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>(backgroundImageName);
            MathEngine.ImageToBitMap(pathForImage("background_map.png"));
            gameWindowSize = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            // TODO: use this.Content to load your game content here
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

            gameState = MathEngine.Update((float)gameTime.ElapsedGameTime.TotalSeconds, gameState);
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
            spriteBatch.Draw(background, gameWindowSize, Color.White);
            foreach(var drawable in MathEngine.drawState(gameState))
            {
                spriteBatch.Draw(Content.Load<Texture2D>(drawable.Image), drawable.Position, Color.White);
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        static void Main(string[] args)
        {
            using (var game = new Capitalism())
            {
                game.Run();
            }
        }


        /// <summary>
        /// Constructs path to output folder
        /// Combines output path with requested image from content folder.
        /// Turns the path into a usable path and returns this.
        /// </summary>
        /// <param name="imageNameAndExtension">Image to get the path for with extensions ("smelly_pants.jpg")</param>
        /// <returns>Local path of the image</returns>
        public String pathForImage(String imageNameAndExtension)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var logoimage = Path.Combine(outPutDirectory, "Content\\" + imageNameAndExtension);
            string relPath = new Uri(logoimage).LocalPath;

            return relPath;
        }
    }
}

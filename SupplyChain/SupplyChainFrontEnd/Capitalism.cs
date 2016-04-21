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
        MathEngine.GameState engineState;
        Texture2D background, menuBackground, title;
        Rectangle gameWindowSize, backgroundRec;
        float backgroundScale;

        enum Game {inMENU, isRunning, isPAUSED};
        Game gameState;
        String backgroundImageName = "background_map.png";
        String menuBackgroundImageName = "menu_background.png";
        String titleImageName = "title_banner.png";
        String GuiDirectory = "GUI/";
        String ObjectsDirectory = "Objects/";

        Rectangle[] menu_button_rectangle = new Rectangle[2];
        Texture2D[] button_texture = new Texture2D[2];
        MouseState cMouseState, lMouseState;

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

            gameState = Game.inMENU;

            
            IsMouseVisible = true;
            cMouseState = Mouse.GetState();
            lMouseState = Mouse.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameWindowSize = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); 
            //screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);
            menu_button_rectangle[0] = new Rectangle(gameWindowSize.Center.X - 199, gameWindowSize.Center.Y, 398, 100);
            menu_button_rectangle[1] = new Rectangle(gameWindowSize.Center.X - 199, gameWindowSize.Center.Y + 150, 398, 100);
            //menu_button_rectangle[1] = new Rectangle(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2 + 200, 398, 100);



            menuBackground = Content.Load<Texture2D>(GuiDirectory + menuBackgroundImageName);
            title = Content.Load<Texture2D>(GuiDirectory + titleImageName);
            button_texture[0] = Content.Load<Texture2D>(@"GUI/Start_Button.png");
            button_texture[1] = Content.Load<Texture2D>(@"GUI/Exit_Button.png");

            background = Content.Load<Texture2D>(GuiDirectory + backgroundImageName);
            backgroundScale = (float)GraphicsDevice.Viewport.Height / (float)background.Bounds.Height;
            backgroundRec = new Rectangle(gameWindowSize.Center.X - (int)((background.Bounds.Width * backgroundScale) / 2), gameWindowSize.Top, background.Bounds.Width, background.Bounds.Height);
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
            lMouseState = cMouseState;
            cMouseState = Mouse.GetState();

            if (gameState == Game.isRunning)
            {
                engineState = MathEngine.Update((float)gameTime.ElapsedGameTime.TotalSeconds, engineState);
            }

            if (gameState == Game.inMENU)
            {
                if (cMouseState.LeftButton == ButtonState.Released && lMouseState.LeftButton == ButtonState.Pressed)
                {
                    var mouseLocation = new Point(cMouseState.X, cMouseState.Y);
                    if (menu_button_rectangle[0].Contains(mouseLocation))
                    {
                        startTheEngine();
                        GraphicsDevice.Clear(Color.CornflowerBlue);
                    }
                    if (menu_button_rectangle[1].Contains(mouseLocation))
                    {
                        this.Exit();
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
            if (gameState == Game.inMENU)
            {
                spriteBatch.Draw(menuBackground, gameWindowSize, Color.White);
                spriteBatch.Draw(title, new Vector2(gameWindowSize.Center.X, gameWindowSize.Center.Y), null, Color.White, 0f, new Vector2(menuBackground.Width / 2, menuBackground.Height / 2), 1f, SpriteEffects.None, 1f);
                
                spriteBatch.Draw(button_texture[0], menu_button_rectangle[0], null, Color.White, 0f, new Vector2(), SpriteEffects.None, 1f);
                spriteBatch.Draw(button_texture[1], menu_button_rectangle[1], null, Color.White, 0f, new Vector2(), SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(background, new Vector2(backgroundRec.X, backgroundRec.Y), null, Color.White, 0f, new Vector2(0, 0), backgroundScale, SpriteEffects.None, 1f);
                foreach (var drawable in MathEngine.drawState(engineState))
                {
                    Texture2D sprite = Content.Load<Texture2D>(ObjectsDirectory + drawable.Image);
                    spriteBatch.Draw(sprite, PositionOnMap(drawable.Position, backgroundScale), null, Color.White, 0f, new Vector2(sprite.Bounds.Center.X, sprite.Bounds.Center.Y), 1f, SpriteEffects.None, 1f);
                }
            }
            
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// Converts a position to its adjacent position on the map (due to map being left-offset and not taking scaling in consideration)
        /// </summary>
        /// <param name="objectPosition">Vector2 Position to convert</param>
        /// <param name="backgroundScale">Appplied background scale 0f< to >1f</param>
        /// <returns>Converted Position</returns>
        private Vector2 PositionOnMap(Vector2 objectPosition, float backgroundScale)
        {
            float x = (backgroundRec.X) + objectPosition.X * backgroundScale;
            float y = (backgroundRec.Y) + objectPosition.Y * backgroundScale;
            return new Vector2(x, y);
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

        private void startTheEngine()
        {
            gameState = Game.isRunning;
            engineState = MathEngine.setupEngineState();
            MathEngine.setBitMap(pathForImage(GuiDirectory + backgroundImageName));
        }
    }
}



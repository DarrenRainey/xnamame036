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
using System.Threading;
using System.Collections;

namespace xnamame036
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        mame.Mame mame036 = new mame.Mame();
        Thread emulthread;
        Rectangle safeArea;
        Rectangle drawArea;
        int _centerX, _centerY;
        Effect effect;
        bool useHQ2x = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        public Rectangle GetTitleSafeArea(float percent)
        {
            Rectangle retval = new Rectangle(graphics.GraphicsDevice.Viewport.X,
                graphics.GraphicsDevice.Viewport.Y,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
#if XBOX
            // Find Title Safe area of Xbox 360.
            float border = (1 - percent) / 2;
            retval.X = (int)(border * retval.Width);
            retval.Y = (int)(border * retval.Height);
            retval.Width = (int)(percent * retval.Width);
            retval.Height = (int)(percent * retval.Height);
            return retval;
#else
            return retval;
#endif
        }
        protected override void Initialize()
        {
            safeArea = GetTitleSafeArea(.8f);
            _centerX = safeArea.Width / 2;
            _centerY = safeArea.Height / 2;

            emulthread = new Thread(new ThreadStart(mame036.Run));
            emulthread.Name = "Mame MainThread";
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //effect = Content.Load<Effect>("HQ2x");
            //var viewport = GraphicsDevice.Viewport;
            //var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            //var halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            //effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            //effect.Parameters["TextureSize"].SetValue(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        static bool threadStarted = false;
        ArrayList lastKeys = new ArrayList();
        GamePadState previousGamePadState;
        protected override void Update(GameTime gameTime)
        {
            if (!threadStarted)
            {
                threadStarted = true;
                emulthread.Start();
            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateInput();

            base.Update(gameTime);
        }
        void UpdateInput()
        {
            ArrayList pressedKeys = new ArrayList(Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys());
            //if (pressedKeys.Contains((Keys)Keys.Escape) && !MAME.isPaused)
            //{
            //    StopEmulator();
            //    return;
            //}
            //if (mame.ScreenBitmap != null)//|| rom_selector.isActive)
            {
                if (pressedKeys.Contains((Keys)Keys.LeftControl) && pressedKeys.Contains((Keys)Keys.RightControl))
                {
                    //mame.isPaused = true;
                    //cSNES.Show_cSNES_Screen = true; // Show rom selector screen
                }

                foreach (Keys k in lastKeys)
                {
                    if (!pressedKeys.Contains(k))
                    {
                        //if (rom_selector.isActive)
                        //    rom_selector.HandleKey(k);
                        //if (MAME.isPaused) // We are in rom selector. Process keys there
                        //{
                        //    cSNES.HandleKey(k);
                        //    if (!cSNES.Show_cSNES_Screen && !SNES.snes.cartridge.loaded()) // Exit from rom selector, with no cart loaded -> end emulator
                        //        StopEmulator();
                        //}
                        //else
                        mame036.keyReleased(k);
                    }
                }
                //if (!mame.isPaused)
                {
                    foreach (Keys k in pressedKeys)
                    {
                        if (!lastKeys.Contains(k))
                        {
                            mame036.keyPressed(k);

                        }
                    }
                }
            }
            lastKeys = pressedKeys;

            // 360 pad handling. This has to be veryfied by someone having one. Also, there must be a better way to do this.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                mame036.UpdateGamepad(currentState,0);
            }
                previousGamePadState = currentState;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            graphics.GraphicsDevice.Textures[0] = null;
            if (mame.Mame.win_video_window != null)
            {
                lock (mame.Mame.win_video_window)
                {
                    var wr = (double)safeArea.Width / (double)mame.Mame.win_video_window.Width;
                    var hr = (double)safeArea.Height / (double)mame.Mame.win_video_window.Height;
                    double Width = 0, Height = 0;
                    if (mame.Mame.win_video_window.Height * wr > graphics.PreferredBackBufferHeight)
                    {
                        Width = mame.Mame.win_video_window.Width * hr;
                        Height = mame.Mame.win_video_window.Height * hr;
                    }
                    else
                    {
                        Width = mame.Mame.win_video_window.Width * wr;
                        Height = mame.Mame.win_video_window.Height * wr;
                    }
                    drawArea = new Rectangle((int)(_centerX - Width / 2), (int)(_centerY - Height / 2), (int)Width, (int)Height);
                    if (!useHQ2x)
                        spriteBatch.Begin();
                    else
                        spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, effect);
#if XBOX
                    spriteBatch.Draw(mame.Mame.win_video_window, safeArea, Color.White);
#else
                    //spriteBatch.Draw(mame.Mame.win_video_window, new Rectangle(20, 20, 640, 480), Color.White);
                    //spriteBatch.Draw(mame.Mame.win_video_window, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
                    spriteBatch.Draw(mame.Mame.win_video_window, drawArea, null, Color.White);
#endif
                    spriteBatch.End();
                }
            }
            base.Draw(gameTime);
        }
    }
}

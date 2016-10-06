// <copyright file="FlybyGame.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Game
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using XnaFlyby.Game.Entities;
    using XnaFlyby.Game.Entities.Actors;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FlybyGame : Game
    {
#if DEBUG
        private int _frames = 0, _time = 0;
#endif
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Scene scene;
        private Hud Hud;
        private Jet localPlayer;

        private KeyboardState lastKeyboardState = new KeyboardState(null);
        private MouseState lastMouseState = new MouseState();

        /// <summary>
        /// Initializes a new instance of the <see cref="FlybyGame"/> class.
        /// </summary>
        public FlybyGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

#if WINDOWS
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += delegate
            {
                graphics.PreferredBackBufferWidth = (Window.ClientBounds.Width > 0 ? Window.ClientBounds.Width : 400);
                graphics.PreferredBackBufferHeight = (Window.ClientBounds.Height > 0 ? Window.ClientBounds.Height : 300);
            };
            this.Window.Title = "XNA Flyby Game";
#endif
#if WINDOWS_PHONE
            graphics.IsFullScreen = true;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var camera = new Camera()
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Target = Vector3.Zero,
                Up = Vector3.Backward,
                Rotation = Quaternion.Identity,
                Listener = new AudioListener()
            };

            this.scene = new Scene(camera);
            Scene.SunPosition = new Vector3(164.0f, -228.8f, 400.0f);

            // lets the XNA Framework to proceed with the initialization of other components
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Hud = new Hud(
                this.spriteBatch,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height);
            this.Hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Square"));
            this.Hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Alien Regular"));
#if DEBUG
            this.Hud.AdapterName = this.GraphicsDevice.Adapter.Description;
            this.Hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Vera Sans Mono"));
#endif

            var ocean = new Ocean(
                this.GraphicsDevice,
                this.Content.Load<Effect>("Shaders/SM20/WaterReflection"),
                this.Content.Load<Texture2D>("Textures/WaterNormal"),
                this.Content.Load<SoundEffect>("Sounds/OceanLoop"));
            //Ocean ocean = new Ocean(GraphicsDevice, Content.Load<Effect>("Shaders/SM20/WaterCubeMap"), Content.Load<TextureCube>("Textures/SkyCubeMap"), Content.Load<Texture2D>("Textures/WaterNormal"), Content.Load<SoundEffect>("Sounds/OceanLoop"));
            ocean.TesselateNormalTextured();
            ocean.Initialize();

            var skySphere = new Sky(this.GraphicsDevice, 16, 16, this.Content.Load<Texture2D>("Textures/SkySphere"))
            {
                Effect = this.Content.Load<Effect>("Shaders/SM20/Base")
            };
            skySphere.TesselateNormalTextured();

            var island = new Island(
                this.GraphicsDevice,
                new Vector3(-512.0f, -512.0f, -0.8f),
                8.0f,
                this.Content.Load<Texture2D>("Textures/test_heightmap"),
                this.Content.Load<Texture2D>("Textures/test_diffuse"))
            {
                Effect = this.Content.Load<Effect>("Shaders/SM20/BaseLighting")
            };
            island.TesselateNormalTextured();

            this.localPlayer = new Jet(
                this.GraphicsDevice,
                this,
                this.Content.Load<Model>("Models/MiG-21bis/MiG-21bis.xna"),
                this.Content.Load<Texture2D>("Models/MiG-21bis/Default"),
                this.Content.Load<Texture2D>("Models/MiG-21bis/MiG-21bis_N0"),
                this.Content.Load<Texture2D>("Models/MiG-21bis/MiG-21bis_S0"),
                this.Content.Load<SoundEffect>("Sounds/MiG-21/Mig-21_EngineLoop"),
                this.Content.Load<SoundEffect>("Sounds/MiG-21/Mig-17_IdleLoop"))
            {
                Effect = this.Content.Load<Effect>("Shaders/SM20/BaseNormalMapping"),
                SplashSound = this.Content.Load<SoundEffect>("Sounds/HeavySplashSound"),
                ExplosionSound = this.Content.Load<SoundEffect>("Sounds/ExplosionSound")
            };
            this.localPlayer.CollidableGridStrips.Add(island);

            this.Reset();

            this.scene.Renderables.Add(skySphere);
            this.scene.Renderables.Add(island);

            this.scene.Updateables.Add(this.localPlayer);

            ocean.ReflectedEntities.Add(skySphere);
            ocean.ReflectedEntities.Add(island);
            ocean.ReflectedEntities.Add(this.localPlayer);
            this.scene.Renderables.Add(ocean);
        }

        /// <summary>
        /// Resets the game state.
        /// </summary>
        public void Reset()
        {
            this.localPlayer.Initialize();
            this.localPlayer.EngineAudioListeners.Add(this.scene.Camera.Listener);
            this.localPlayer.Position = new Vector3(0.0f, 0.0f, 128.0f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all content.
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
            // handling the camera with the mouse
            var currentMouseState = Mouse.GetState();
            var currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || currentGamePadState.Buttons.Start == ButtonState.Pressed)
            {
                this.Reset();
            }

            this.Hud.IsHelpEnabled = false;

            // TODO: repair this
            //if (Keyboard.GetState().IsKeyDown(Keys.F1) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            //{
            //    this.Hud.IsHelpEnabled = true;
            //}

#if DEBUG
            // toggles the drawing of the bounding boxes
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && !this.lastKeyboardState.IsKeyDown(Keys.F2))
            {
                this.localPlayer.IsBoundingBoxDrawingRequired = !this.localPlayer.IsBoundingBoxDrawingRequired;
            }
#endif

            float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            float acceleration = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 600.0f;
            float turningAngleX = 0.0f;
            float pitchAngleY = 0.0f;
            float throttle = 0.0f;

            turningAngleX += (currentGamePadState.ThumbSticks.Left.X * 2.0f) * turningSpeed;
            pitchAngleY += (currentGamePadState.ThumbSticks.Left.Y * -2.0f) * turningSpeed;
            throttle = (-1.0f * currentGamePadState.Triggers.Left) + currentGamePadState.Triggers.Right;

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) { pitchAngleY -= turningSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) { pitchAngleY += turningSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) { turningAngleX -= turningSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) { turningAngleX += turningSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { throttle = 1.0f; }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) || Keyboard.GetState().IsKeyDown(Keys.Y)) { throttle = -1.0f; } // (to handle US and German style keyboards, too)

            var jetDeltaRotation = Quaternion.CreateFromAxisAngle(new Vector3(-1.0f, 0.0f, 0.0f), turningAngleX) * Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), pitchAngleY);
            this.localPlayer.Control(jetDeltaRotation, throttle * acceleration);

            this.Hud.Width = this.GraphicsDevice.Viewport.Width;
            this.Hud.Height = this.GraphicsDevice.Viewport.Height;

#if DEBUG
            this.Hud.TerrainHeight = this.localPlayer.CurrentTerrainHeight;
#endif

            this.Hud.Altitude = this.localPlayer.Position.Z;
            this.Hud.Velocity = this.localPlayer.Velocity;

            var cameraPosition = new Vector3(12.0f, 0.0f, 2.8f);
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateFromQuaternion(this.scene.Camera.Rotation));
            cameraPosition += this.localPlayer.Position;

            var cameraUp = new Vector3(0.0f, 0.0f, 1.0f);
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromQuaternion(this.scene.Camera.Rotation));

            this.scene.Camera.Rotation = Quaternion.Lerp(this.scene.Camera.Rotation, this.localPlayer.Rotation, 0.1f);
            this.scene.Camera.Position = cameraPosition;
            this.scene.Camera.Target = this.localPlayer.Position;
            this.scene.Camera.Up = cameraUp;

            this.scene.Camera.Listener.Position = cameraPosition;
            this.scene.Camera.Listener.Up = cameraUp;
            this.scene.Camera.Listener.Forward = this.scene.Camera.Direction;
            this.scene.Camera.Listener.Velocity = this.scene.Camera.Direction * this.localPlayer.Velocity;

            this.scene.Update(gameTime);

            base.Update(gameTime);

            this.lastMouseState = currentMouseState;
            //this.lastGamePadState = currentGamePadState;
            this.lastKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var ocean = (Ocean)this.scene.Renderables.LastOrDefault();
            if (ocean != null)
            {
                ocean.DrawReflection(this.scene.Camera, (float)gameTime.TotalGameTime.TotalSeconds);
            }

            this.GraphicsDevice.Clear(Color.Chartreuse);

            this.scene.Camera.AspectRatio = this.GraphicsDevice.Viewport.AspectRatio;
            this.scene.Draw(gameTime);
            this.Hud.Draw(gameTime);
            base.Draw(gameTime);

#if DEBUG && WINDOWS
            this._frames++;
            int seconds = DateTime.Now.Second;
            if (seconds != this._time)
            {
                this.Window.Title = string.Format("XNA Flyby Game ({0} FPS)", this._frames);
                this.Hud.FramesPerSecond = this._frames;
                this._frames = 0;
                this._time = seconds;
            }
#endif
        }
    }
}

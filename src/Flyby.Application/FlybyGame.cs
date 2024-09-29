// <copyright file="FlybyGame.cs" company=".">
// See LICENSE for details.
// </copyright>

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaFlyby.Game.Entities;
using XnaFlyby.Game.Entities.Actors;
using XnaFlyby.Library.Components;

namespace Flyby.Application
{
    public class FlybyGame : Game
    {
#if DEBUG
        private int _frames = 0, _time = 0;
#endif
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Scene scene;
        private Hud hud;
        private Jet localPlayer;

        private KeyboardState lastKeyboardState = new KeyboardState(null);
        private MouseState lastMouseState = new MouseState();

        public FlybyGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += delegate
            {
                graphics.PreferredBackBufferWidth = (Window.ClientBounds.Width > 0 ? Window.ClientBounds.Width : 400);
                graphics.PreferredBackBufferHeight = (Window.ClientBounds.Height > 0 ? Window.ClientBounds.Height : 300);
            };

            this.Window.Title = "Flyby Game";
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

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.hud = new Hud(
                this.spriteBatch,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height);
#if DEBUG
            this.hud.AdapterName = this.GraphicsDevice.Adapter.Description;
#endif

#if MSXNA
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/SquareFont.xna"));
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Alien Encounters.xna"));
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Bitstream Vera Sans Mono.xna"));
#else
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/SquareFont"));
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Alien Encounters"));
            this.hud.SpriteFonts.Add(this.Content.Load<SpriteFont>("Fonts/Bitstream Vera Sans Mono"));
#endif

            var ocean = new Ocean(
                this.GraphicsDevice,
                this.Content.Load<Effect>("Shaders/SM20/WaterReflection"),
                this.Content.Load<Texture2D>("Textures/WaterNormal"),
                this.Content.Load<SoundEffect>("Audio/SFX/OceanLoop"));
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
#if MSXNA
                this.Content.Load<Model>("Models/MiG-21bis/MiG-21bis.xnacompat"),
#else
                this.Content.Load<Model>("Models/MiG-21bis/MiG-21bis"),
#endif
                this.Content.Load<Texture2D>("Models/MiG-21bis/Default"),
                this.Content.Load<Texture2D>("Models/MiG-21bis/MiG-21bis_N0"),
                this.Content.Load<Texture2D>("Models/MiG-21bis/MiG-21bis_S0"),
                this.Content.Load<SoundEffect>("Audio/SFX/MiG-21/Mig-21_EngineLoop"),
                this.Content.Load<SoundEffect>("Audio/SFX/MiG-21/Mig-17_IdleLoop"))
            {
                Effect = this.Content.Load<Effect>("Shaders/SM20/BaseLighting"),
                SplashSound = this.Content.Load<SoundEffect>("Audio/SFX/HeavySplashSound"),
                ExplosionSound = this.Content.Load<SoundEffect>("Audio/SFX/ExplosionSound")
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

        protected override void UnloadContent()
        {
            this.spriteBatch.Dispose();
        }

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

            this.hud.IsHelpEnabled = false;

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

            this.hud.Width = this.GraphicsDevice.Viewport.Width;
            this.hud.Height = this.GraphicsDevice.Viewport.Height;

#if DEBUG
            this.hud.TerrainHeight = this.localPlayer.CurrentTerrainHeight;
#endif

            this.hud.Altitude = this.localPlayer.Position.Z;
            this.hud.Velocity = this.localPlayer.Velocity;

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
            this.lastKeyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var ocean = (Ocean)this.scene.Renderables.LastOrDefault();
            ocean.DrawReflection(this.scene.Camera, (float)gameTime.TotalGameTime.TotalSeconds);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.scene.Camera.AspectRatio = this.GraphicsDevice.Viewport.AspectRatio;
            this.scene.Draw(gameTime);
            this.hud.Draw(gameTime);

#if DEBUG
            this._frames++;
            int seconds = DateTime.Now.Second;
            if (seconds != this._time)
            {
                this.Window.Title = string.Format("Flyby Game ({0} FPS)", this._frames);
                this.hud.FramesPerSecond = this._frames;
                this._frames = 0;
                this._time = seconds;
            }
#endif

            base.Draw(gameTime);
        }
    }
}
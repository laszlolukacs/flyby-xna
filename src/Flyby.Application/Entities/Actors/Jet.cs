// <copyright file="Jet.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Game.Entities.Actors
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;
    using XnaFlyby.Library.Entities.Actors;
    using XnaFlyby.Library.Entities.Primitives;

    /// <summary>
    /// Represents the Jet actor.
    /// </summary>
    public class Jet : ActorBase
    {
        /// <summary>
        /// Gets or sets the 3D position of the jet.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the 3D direction of the jet.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets the 3D rotation quaternion of the jet.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the velocity of the jet.
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// Gets or sets the altitude of the jet.
        /// </summary>
        public float Altitude { get; set; }

        /// <summary>
        /// Gets or sets the diffuse texture.
        /// </summary>
        public Texture2D Diffuse { get; set; }

        /// <summary>
        /// Gets or sets the normal map texture.
        /// </summary>
        public Texture2D NormalMap { get; set; }

        /// <summary>
        /// Gets or sets the specular map texture.
        /// </summary>
        public Texture2D SpecularMap { get; set; }

        public SoundEffectInstance EngineSoundLoop { get; set; }
        public SoundEffectInstance IdleSoundLoop { get; set; }
        public AudioEmitter EngineAudioEmitter { get; set; }
        public IList<AudioListener> EngineAudioListeners { get; set; }
        public SoundEffect ExplosionSound { get; set; }
        public SoundEffect SplashSound { get; set; }

        /// <summary>
        /// Gets or sets a collection which contains references to the collidable terrain elements.
        /// </summary>
        public IList<GridStripBase> CollidableGridStrips { get; set; }

        /// <summary>
        /// Gets or sets the height of the terrain at the position of the jet.
        /// </summary>
        public float CurrentTerrainHeight { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jet"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="game">XNA's game class</param>
        public Jet(GraphicsDevice device, Game game)
            : base(device, game)
        {
            this.EngineAudioListeners = new List<AudioListener>();
            this.CollidableGridStrips = new List<GridStripBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jet"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="game">XNA's game class</param>
        /// <param name="model">3D model</param>
        public Jet(GraphicsDevice device, Game game, Model model)
            : base(device, game, model)
        {
            this.EngineAudioListeners = new List<AudioListener>();
            this.CollidableGridStrips = new List<GridStripBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jet"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="game">XNA's game class</param>
        /// <param name="model">3D model</param>
        /// <param name="diffuse">Diffuse texture</param>
        /// <param name="normalMap">Normal map</param>
        /// <param name="specularMap">Specular map</param>
        /// <param name="engineLoop">Engine loop sound</param>
        /// <param name="idleLoop">Idle loop sound</param>
        public Jet(
            GraphicsDevice device,
            Game game,
            Model model,
            Texture2D diffuse,
            Texture2D normalMap,
            Texture2D specularMap,
            SoundEffect engineLoop,
            SoundEffect idleLoop)
            : base(device, game, model)
        {
            this.Diffuse = diffuse;
            this.NormalMap = normalMap;
            this.SpecularMap = specularMap;

            this.EngineSoundLoop = engineLoop.CreateInstance();
            this.IdleSoundLoop = idleLoop.CreateInstance();

            this.EngineAudioListeners = new List<AudioListener>();
            this.CollidableGridStrips = new List<GridStripBase>();
        }

        /// <summary>
        /// Initializes this actor instance.
        /// </summary>
        public override void Initialize()
        {
            this.Position = new Vector3(8.0f, -3.0f, 1.0f);
            this.Rotation = Quaternion.Identity;
            this.Velocity = 0.0f;

            this.World = Matrix.CreateScale(10.0f) * Matrix.CreateRotationZ(-1.0f * MathHelper.PiOver2);

            // shader clipping plane options
            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(false);
            this.Effect.Parameters["ClipPlane"].SetValue(new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

            if (!this.EngineSoundLoop.IsLooped)
            {
                this.EngineSoundLoop.IsLooped = true;
            }

            if (!this.IdleSoundLoop.IsLooped)
            {
                this.IdleSoundLoop.IsLooped = true;
            }

            this.EngineSoundLoop.Volume = 1.0f;
            this.IdleSoundLoop.Volume = 1.0f;

            this.EngineAudioEmitter = new AudioEmitter();
            this.EngineAudioListeners = new List<AudioListener>();

            base.Initialize();
        }

        /// <summary>
        /// Controls the movement of the jet.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="velocity">The velocity.</param>
        public void Control(Quaternion rotation, float velocity)
        {
            if (this.IsAlive)
            {
                this.Rotation *= rotation;
                this.Velocity += velocity;
                if (this.Velocity > 3.0f)
                {
                    this.Velocity = 3.0f;
                }

                if (this.Velocity < 0.0f)
                {
                    this.Velocity = 0.0f;
                }

                this.Direction = Vector3.Transform(new Vector3(-1.0f, 0.0f, 0.0f), this.Rotation);
                this.Position += this.Direction * this.Velocity;
            }
        }

        /// <summary>
        /// Performs the behavior of this element.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            this.World = Matrix.CreateScale(10.0f) * Matrix.CreateRotationZ(-1.0f * MathHelper.PiOver2) * Matrix.CreateFromQuaternion(this.Rotation) * Matrix.CreateTranslation(this.Position);

            if (this.IsAlive)
            {
                foreach (var grid in this.CollidableGridStrips)
                {
                    if (grid.IsPositionOnStrip(this.Position))
                    {
                        this.CurrentTerrainHeight = grid.GetHeight(this.Position);
                        if (this.CurrentTerrainHeight > this.Position.Z)
                        {
                            this.ExplosionSound.Play();
                            this.IsAlive = false;
                        }
                    }
                }

                // plays positional audio
                this.EmitSound();

                // when the plane crashes into the ocean
                if (this.Position.Z < -1.00f)
                {
                    this.SplashSound.Play();
                    this.ExplosionSound.Play();
                    this.IsAlive = false;
                    this.Velocity = 0.0f;
                }

                // slows down the jet -- this should be the drag force -- now it is linear
                float slowness = (1.0f - (gameTime.ElapsedGameTime.Milliseconds / 2000.0f));
                this.Velocity *= slowness;

                // applies the gravity
                Vector3 gravity = (new Vector3(Gravity.X, Gravity.Y, Gravity.Z)) * gameTime.ElapsedGameTime.Milliseconds / 300.0f;
                if (this.Position.Z > -2.00f)
                {
                    this.Position += gravity;
                }

                Debug.WriteLine("{0}, {1}, {2}, {3}", slowness, this.Velocity, gravity, this.Position);
            }
            else
            {
                // when the plane is dead
                if (this.EngineSoundLoop.State == SoundState.Playing
                    || this.IdleSoundLoop.State == SoundState.Playing)
                {
                    this.EngineSoundLoop.Stop();
                    this.IdleSoundLoop.Stop();
                }
            }

            base.Update(gameTime);
        }

        public override void InitializeEffectParameters()
        {
            this.Effect.Parameters["CameraPosition"].SetValue(Vector3.Zero);

            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(false);
            this.Effect.Parameters["ClipPlane"].SetValue(Vector4.Zero);

            this.Effect.Parameters["AmbientLightColor"].SetValue(Vector3.One * 0.1f);
            this.Effect.Parameters["DiffuseColor"].SetValue(Vector3.One);

            this.Effect.Parameters["LightIntensity"].SetValue(Vector3.One * 0.9f);
        }


        /// <summary>
        /// Performs the rendering of the jet.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="deltaT">The delta T.</param>
        public override void Draw(Camera camera, float deltaT)
        {
            if (this.IsAlive)
            {
                this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.CullCounterClockwiseFace };

                foreach (var mesh in this.Model.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        var tag = (MeshTag)meshPart.Tag;
                        var worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * this.World));

                        this.InitializeEffectParameters();

                        this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(this.ClippingPlane.HasValue);
                        this.Effect.Parameters["ClipPlane"].SetValue(this.ClippingPlane.GetValueOrDefault());

                        //this.Effect.Parameters["AmbientLightColor"].SetValue(Vector3.One * 0.08f);
                        //this.Effect.Parameters["DiffuseColor"].SetValue(Vector3.One);
                        //this.Effect.Parameters["SpecularPower"].SetValue(64.0f);

                        //this.Effect.Parameters["LightIntensity"].SetValue(Vector3.One);
                        //this.Effect.Parameters["LightDirection"].SetValue(Scene.SunPosition);

                        this.Effect.Parameters["World"].SetValue(this.World * mesh.ParentBone.Transform);
                        //this.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        this.Effect.Parameters["View"].SetValue(camera.View);
                        this.Effect.Parameters["Projection"].SetValue(camera.Projection);
                        this.Effect.Parameters["CameraPosition"].SetValue(camera.Position);

                        this.Effect.Parameters["DiffuseColor"].SetValue(tag.Color);
                        this.Effect.Parameters["SpecularPower"].SetValue(tag.SpecularPower);

                        // sets the texture property
                        this.Effect.Parameters["DiffuseTexture"].SetValue(this.Diffuse);
                        //this.Effect.Parameters["NormalMap"].SetValue(this.NormalMap);
                        //this.Effect.Parameters["SpecularMap"].SetValue(this.SpecularMap);

                        meshPart.Effect = this.Effect;
                    }

                    mesh.Draw();
                }

                base.Draw(camera, deltaT);
            }
        }

        /// <summary>
        /// Sets the clipping plane.
        /// </summary>
        /// <param name="clippingPlane">The clipping plane.</param>
        public override void SetClippingPlane(Vector4? clippingPlane)
        {
            this.ClippingPlane = clippingPlane;

            //foreach (var mesh in this.Model.Meshes)
            //{
            //    foreach (var meshPart in mesh.MeshParts)
            //    {
            //        if (meshPart.Effect.Parameters["IsClippingPlaneEnabled"] != null)
            //        {
            //            meshPart.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(clippingPlane.HasValue);
            //        }

            //        if (clippingPlane.HasValue
            //            && meshPart.Effect.Parameters["ClipPlane"] != null)
            //        {
            //            meshPart.Effect.Parameters["ClipPlane"].SetValue(clippingPlane.Value);
            //        }
            //    }
            //}
        }

        private void EmitSound()
        {
            // 3D positional sound updates
            this.EngineAudioEmitter.Position = this.Position;
            this.EngineAudioEmitter.Up = Vector3.Transform(new Vector3(0.0f, 0.0f, 1.0f), Matrix.CreateFromQuaternion(this.Rotation));
            this.EngineAudioEmitter.Forward = -1.0f * this.Direction;
            this.EngineAudioEmitter.Velocity = -1.0f * this.Direction * this.Velocity;

            this.EngineSoundLoop.Apply3D(this.EngineAudioListeners.ToArray(), this.EngineAudioEmitter);
            this.EngineSoundLoop.Volume = this.Velocity / 3.0f;
            this.EngineSoundLoop.Pitch = this.Velocity / 6.0f;
            this.EngineSoundLoop.Play();

            this.IdleSoundLoop.Volume = 1.0f - (this.Velocity / 3.0f);
            this.IdleSoundLoop.Apply3D(this.EngineAudioListeners.ToArray(), this.EngineAudioEmitter);
            this.IdleSoundLoop.Play();
        }
    }
}

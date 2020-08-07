// <copyright file="Ocean.cs" company=".">
// See license.md for details.
// </copyright>

using System;

namespace XnaFlyby.Game.Entities
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;
    using XnaFlyby.Library.Entities;
    using XnaFlyby.Library.Entities.Primitives;

    /// <summary>
    /// Represents the ocean surface.
    /// </summary>
    public class Ocean : GridStripBase
    {
        /// <summary>
        /// The reflection target
        /// </summary>
        private readonly RenderTarget2D reflectionTarget;

        /// <summary>
        /// Gets or sets the normal map.
        /// </summary>
        /// <value>
        /// The normal map.
        /// </value>
        public Texture2D NormalMap { get; set; }

        /// <summary>
        /// Gets or sets the sky cube map.
        /// </summary>
        /// <value>
        /// The sky cube map.
        /// </value>
        public TextureCube SkyCubeMap { get; set; }

        public IList<RenderableBase> ReflectedEntities { get; set; }

        public SoundEffectInstance WaveLoop { get; set; }

        /// <summary>
        /// Initializes a new instance of the ocean class which uses a full scene reflection shader.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="waterEffect">Full scene reflection shader</param>
        /// <param name="normalMap">Normal map</param>
        /// <param name="backgroundLoop">Looping sound effect</param>
        public Ocean(GraphicsDevice device, Effect waterEffect, Texture2D normalMap, SoundEffect backgroundLoop)
            : base(device, new Vector3(-2560.0f, -2560.0f, 0.0f), 40.0f, 144, 144)
        {
            this.Effect = waterEffect;
            this.NormalMap = normalMap;
            this.WaveLoop = backgroundLoop.CreateInstance();

            this.ReflectedEntities = new List<RenderableBase>();

            // the reflection rendertarget will be a 512px texture
            const int renderTargetSize = 512;
            this.reflectionTarget = new RenderTarget2D(device, renderTargetSize, renderTargetSize, false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            this.Effect.Parameters["ViewportWidth"].SetValue((float)this.reflectionTarget.Width);   //device.Viewport.Width);
            this.Effect.Parameters["ViewportHeight"].SetValue((float)this.reflectionTarget.Height);  //device.Viewport.Height);

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the ocean class which uses a cube map reflection shader.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="waterEffect">Cube map reflection shader</param>
        /// <param name="skyCubeMap">Cube map</param>
        /// <param name="normalMap">Normal map</param>
        /// <param name="loop">Looping sound effect</param>
        public Ocean(GraphicsDevice device, Effect waterEffect, TextureCube skyCubeMap, Texture2D normalMap, SoundEffect loop)
            : base(device, new Vector3(-2560.0f, -2560.0f, 0.0f), 40.0f, 128, 128)
        {
            this.Effect = waterEffect;
            this.NormalMap = normalMap;
            this.SkyCubeMap = skyCubeMap;
            this.WaveLoop = loop.CreateInstance();

            this.ReflectedEntities = new List<RenderableBase>();

            this.Effect.Parameters["ReflectionMap"].SetValue(this.SkyCubeMap);

            this.Initialize();
        }

        public override void Initialize()
        {
            this.Effect.Parameters["LightDirection"].SetValue(Scene.SunPosition);
            this.Effect.Parameters["NormalMap"].SetValue(this.NormalMap);

            this.Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.2f, 0.28f, 0.78f));
            this.Effect.Parameters["DiffuseColorAmount"].SetValue(0.28f);
            this.Effect.Parameters["Opacity"].SetValue(0.8f);

            this.Effect.Parameters["WaveLength"].SetValue(0.48f);
            this.Effect.Parameters["WaveHeight"].SetValue(0.1f);
            this.Effect.Parameters["WaveSpeed"].SetValue(0.028f);

            base.Initialize();
        }

        protected override VertexPositionNormalTexture CreateVertex(int i, int j)
        {
            return new VertexPositionNormalTexture(
                new Vector3(i, j, 0.0f),                                        // position
                new Vector3(0.0f, 0.0f, 1.0f),                                  // normal
                new Vector2((float)i / (this.Width - 1), (float)j / (this.Height - 1))    // texture
            );
        }

        public override bool IsPositionOnStrip(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override float GetHeight(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void DrawReflection(Camera camera, float deltaT)
        {
            // Reflect the camera's properties across the water plane
            Vector3 reflectedCameraPosition = camera.Position;
            reflectedCameraPosition.Z = -reflectedCameraPosition.Z;

            Vector3 reflectedCameraTarget = camera.Target;
            reflectedCameraTarget.Z = -reflectedCameraTarget.Z;

            Vector3 reflectedUp = camera.Up;
            reflectedUp.Z = -reflectedUp.Z;

            // creates a temporary camera to render the reflected scene
            var reflectedCamera = new Camera
            {
                AspectRatio = camera.AspectRatio,
                Position = reflectedCameraPosition,
                Target = reflectedCameraTarget,
                Up = reflectedUp
            };

            this.Effect.Parameters["ReflectedView"].SetValue(reflectedCamera.View);

            Vector4 clipPlane = new Vector4(0.0f, 0.0f, 1.0f, -0.00001f);

            this.Device.SetRenderTarget(this.reflectionTarget);
            this.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            this.Device.BlendState = BlendState.NonPremultiplied;
            this.Device.Clear(Color.LightCoral);

            foreach (var entity in this.ReflectedEntities)
            {
                entity.SetClippingPlane(clipPlane);
                entity.Draw(reflectedCamera, deltaT);
                entity.SetClippingPlane(null);
            }

            this.Device.SetRenderTarget(null);
            // Set the reflected scene to its effect parameter in the water effect
            this.Effect.Parameters["ReflectionMap"].SetValue(this.reflectionTarget);
        }

        public override void Draw(Camera camera, float deltaT)
        {
            this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.CullClockwiseFace };

            this.Effect.Parameters["World"].SetValue(Matrix.CreateScale(this.Scale) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, this.Position.Z));
            this.Effect.Parameters["View"].SetValue(camera.View);
            this.Effect.Parameters["Projection"].SetValue(camera.Projection);
            this.Effect.Parameters["CameraPosition"].SetValue(camera.Position);

            this.Effect.Parameters["Time"].SetValue(deltaT);

            this.Effect.CurrentTechnique.Passes[0].Apply();

            base.Draw(camera, deltaT);
        }
    }
}

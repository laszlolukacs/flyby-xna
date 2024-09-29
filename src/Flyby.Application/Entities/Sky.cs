// <copyright file="Sky.cs" company=".">
// See LICENSE for details.
// </copyright>

namespace XnaFlyby.Game.Entities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;
    using XnaFlyby.Library.Entities.Primitives;

    /// <summary>
    /// Represents the sky sphere.
    /// </summary>
    public class Sky : Sphere
    {
        /// <summary>
        /// Gets or sets the diffuse texture of the sky.
        /// </summary>
        /// <value>
        /// The diffuse texture.
        /// </value>
        public Texture2D Diffuse { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sky"/> class.
        /// </summary>
        /// <param name="device">The graphics device</param>
        /// <param name="slices">The slices of the sky sphere.</param>
        /// <param name="stacks">The stacks of the sky sphere.</param>
        /// <param name="diffuse">The diffuse texture.</param>
        public Sky(GraphicsDevice device, int slices, int stacks, Texture2D diffuse)
            : base(device, slices, stacks)
        {
            this.Diffuse = diffuse;
            this.World = Matrix.CreateScale(500.0f) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.Zero);
        }

        /// <summary>
        /// Renders the sky.
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="deltaT">Elapsed time since last draw, in seconds</param>
        public override void Draw(Camera camera, float deltaT)
        {
            this.World = Matrix.CreateScale(1000.0f) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(camera.Position);

            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(false);
            this.Effect.Parameters["ClipPlane"].SetValue(Vector4.Zero);

            this.Effect.Parameters["DiffuseColor"].SetValue(Vector3.One);

            this.Effect.Parameters["IsTexturingEnabled"].SetValue(true);
            this.Effect.Parameters["DiffuseTexture"].SetValue(this.Diffuse);

            base.Draw(camera, deltaT);
        }
    }
}

// <copyright file="RenderableBase.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Entities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// Abstract base class for the drawable elements of the scene.
    /// </summary>
    public abstract class RenderableBase
    {
        /// <summary>
        /// Provides context for drawing.
        /// </summary>
        protected readonly GraphicsDevice Device;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderableBase"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        public RenderableBase(GraphicsDevice device)
        {
            this.Device = device;
        }

        /// <summary>
        /// Gets or sets the applied shader effect.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// Gets or sets the World transform matrix of the renderable.
        /// </summary>
        public Matrix World { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="view">The view matrix.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="deltaT">The delta T.</param>
        public virtual void Draw(Matrix view, Matrix projection, float deltaT)
        {
        }

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="deltaT">The delta T.</param>
        public virtual void Draw(Camera camera, float deltaT)
        {
            this.Draw(camera.View, camera.Projection, deltaT);
        }

        /// <summary>
        /// Sets the clipping plane.
        /// </summary>
        /// <param name="clippingPlane">The clipping plane.</param>
        public virtual void SetClippingPlane(Vector4? clippingPlane)
        {
        }
    }
}

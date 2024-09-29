// <copyright file="IUpdateableAndRenderable.cs" company=".">
// See LICENSE for details.
// </copyright>

namespace XnaFlyby.Library.Entities
{
    using Microsoft.Xna.Framework;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// An element of the scene which is capable of interactions and is renderable.
    /// </summary>
    public interface IUpdateableAndRenderable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IUpdateableAndRenderable"/> is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Performs the behavior of this element.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Performs the rendering of the element.
        /// </summary>
        /// <param name="view">The view matrix.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="deltaT">The delta T.</param>
        void Draw(Matrix view, Matrix projection, float deltaT);

        /// <summary>
        /// Performs the rendering of the element.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="deltaT">The delta T.</param>
        void Draw(Camera camera, float deltaT);

        /// <summary>
        /// Sets the clipping plane.
        /// </summary>
        /// <param name="clippingPlane">The clipping plane.</param>
        void SetClippingPlane(Vector4? clippingPlane);
    }
}

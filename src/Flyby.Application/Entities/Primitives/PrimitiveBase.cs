// <copyright file="PrimitiveBase.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Entities.Primitives
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A renderable element which has indices and vertices hand can be tesselated to have normal mapping.
    /// </summary>
    public abstract class PrimitiveBase : RenderableBase
    {
        /// <summary>
        /// Represents a list of 3D vertices to be streamed to the graphics device.
        /// </summary>
        protected VertexBuffer vertexBuffer;

        /// <summary>
        /// Describes the rendering order of the vertices in a vertex buffer.
        /// </summary>
        protected IndexBuffer indexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveBase"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        public PrimitiveBase(GraphicsDevice device)
            : base(device)
        {
        }

        /// <summary>
        /// Tesselates the primitive resulting PositionNormalTexture vertices in the vertex and index buffers.
        /// </summary>
        public virtual void TesselateNormalTextured()
        {
        }
    }
}

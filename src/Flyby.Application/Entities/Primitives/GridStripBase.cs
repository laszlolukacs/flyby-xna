// <copyright file="GridStripBase.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Entities.Primitives
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// An abstract grid strip.
    /// </summary>
    public abstract class GridStripBase : PrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridStripBase"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GridStripBase(GraphicsDevice device, Vector3 position, float scale, int width, int height)
            : base(device)
        {
            this.Position = position;
            this.Scale = scale;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets the width of the grid strip.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the grid strip.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the scale of the grid strip.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Gets or sets the position of the grid strip.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Tesselates the grid strip primitive resulting PositionNormalTexture vertices in the vertex and index buffers.
        /// </summary>
        public override void TesselateNormalTextured()
        {
            // creates the vertices
            var vertices = new VertexPositionNormalTexture[this.Width * this.Height];

            int i = 0, j = 0;
            for (i = 0; i < this.Width; i++)
            {
                for (j = 0; j < this.Height; j++)
                {
                    vertices[i * this.Height + j] = this.CreateVertex(i, j);
                }
            }

            // creates the indices
            var indices = new ushort[(this.Width * 2 + 1) * (this.Height - 1)];

            int index = 0, direction = 1;

            for (j = 1; j < this.Height; j++)
            {
                int k = direction > 0 ? 0 : this.Width - 1;
                for (; k >= 0 && k < this.Width; k += direction)
                {
                    indices[index++] = (ushort)((j - 1) * this.Width + k);
                    indices[index++] = (ushort)(j * this.Width + k);
                }

                indices[index++] = (ushort)((j - 1) * this.Width + k - direction);
                direction = -direction;
            }

            // sets up the vertex buffer
            this.vertexBuffer = new VertexBuffer(this.Device, typeof(VertexPositionNormalTexture), this.Width * this.Height, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(vertices);

            // sets up the index buffer
            this.indexBuffer = new IndexBuffer(this.Device, typeof(ushort), (this.Width * 2 + 1) * (this.Height - 1), BufferUsage.WriteOnly);
            this.indexBuffer.SetData<ushort>(indices);
        }

        /// <summary>
        /// Determines whether the specified 2D position is on the grid strip.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <returns>
        ///   <c>true</c> if the specified position is on the grid strip; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsPositionOnStrip(Vector3 position);

        /// <summary>
        /// Gets the height of the strip at the specified 2D position.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <returns>The height of the grid strip at the specified position.</returns>
        public abstract float GetHeight(Vector3 position);

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="deltaT">The delta T.</param>
        public override void Draw(Camera camera, float deltaT)
        {
            this.Device.SetVertexBuffer(this.vertexBuffer);
            this.Device.Indices = this.indexBuffer;
#if MSXNA
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, this.Width * this.Height, 0, (this.Width * 2 + 1) * (this.Height - 1) - 2);
#else
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, (this.Width * 2 + 1) * (this.Height - 1) - 2);
#endif
        }

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="view">The view matrix.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="deltaT">The delta T.</param>
        public override void Draw(Matrix view, Matrix projection, float deltaT)
        {
            this.Device.SetVertexBuffer(this.vertexBuffer);
            this.Device.Indices = this.indexBuffer;
#if MSXNA
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, this.Width * this.Height, 0, (this.Width * 2 + 1) * (this.Height - 1) - 2);
#else
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, (this.Width * 2 + 1) * (this.Height - 1) - 2);
#endif
        }

        /// <summary>
        /// Creates a vertex with position, normal direction and texture coordinates.
        /// </summary>
        /// <param name="i">The i coordinate of the height map.</param>
        /// <param name="j">The j coordinate of the height map.</param>
        /// <returns>A vertex on the specified coordinate in the grid strip.</returns>
        protected abstract VertexPositionNormalTexture CreateVertex(int i, int j);
    }
}

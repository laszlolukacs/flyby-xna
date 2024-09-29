// <copyright file="Sphere.cs" company=".">
// See LICENSE for details.
// </copyright>

namespace XnaFlyby.Library.Entities.Primitives
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// A drawable sphere.
    /// </summary>
    public class Sphere : PrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sphere"/> class.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="slices">Number of slices which the sphere is composed of</param>
        /// <param name="stacks">Number of stacks which the sphere is composed of</param>
        public Sphere(GraphicsDevice device, int slices, int stacks)
            : base(device)
        {
            this.Slices = slices;
            this.Stacks = stacks;
            this.World = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.Zero);
        }

        /// <summary>
        /// Gets or sets the number of slices which the sphere is composed of.
        /// </summary>
        public int Slices { get; set; }

        /// <summary>
        /// Gets or sets the number of stacks which the sphere is composed of.
        /// </summary>
        public int Stacks { get; set; }

        /// <summary>
        /// Tesselates the sphere primitive resulting PositionNormalTexture vertices in the vertex and index buffers.
        /// </summary>
        public override void TesselateNormalTextured()
        {
            // creates the vertices
            var vertices = new VertexPositionNormalTexture[(this.Slices + 1) * (this.Stacks + 1)];

            float phi, theta;
            float dPhi = MathHelper.Pi / this.Stacks;
            float dTheta = MathHelper.TwoPi / this.Slices;

            float x, y, z, sc;
            int index = 0;

            for (int stack = 0; stack <= this.Stacks; stack++)
            {
                phi = MathHelper.PiOver2 - stack * dPhi;
                y = (float)Math.Sin(phi);
                sc = -(float)Math.Cos(phi);

                for (int slice = 0; slice <= this.Slices; slice++)
                {
                    theta = slice * dTheta;

                    x = sc * (float)Math.Sin(theta);
                    z = sc * (float)Math.Cos(theta);

                    vertices[index++] = new VertexPositionNormalTexture(
                        new Vector3(x, y, z),
                        new Vector3(x, y, z),
                        new Vector2((float)slice / (float)this.Slices, (float)stack / (float)this.Stacks));
                }
            }

            // creates the indices
            var indices = new ushort[this.Slices * this.Stacks * 6];

            int k = this.Slices + 1;
            index = 0;

            for (int stack = 0; stack < this.Stacks; stack++)
            {
                for (int slice = 0; slice < this.Slices; slice++)
                {
                    indices[index++] = (ushort)((stack + 0) * k + slice);
                    indices[index++] = (ushort)((stack + 1) * k + slice);
                    indices[index++] = (ushort)((stack + 0) * k + slice + 1);
                    indices[index++] = (ushort)((stack + 0) * k + slice + 1);
                    indices[index++] = (ushort)((stack + 1) * k + slice);
                    indices[index++] = (ushort)((stack + 1) * k + slice + 1);
                }
            }

            // sets up the vertex buffer
            this.vertexBuffer = new VertexBuffer(this.Device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(vertices);

            // sets up the index buffer
            this.indexBuffer = new IndexBuffer(this.Device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            this.indexBuffer.SetData(indices);
        }

        /// <summary>
        /// Draws the sphere.
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="deltaT">Elapsed time since last draw, in seconds</param>
        public override void Draw(Camera camera, float deltaT)
        {
            this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.CullCounterClockwiseFace };
            this.Device.SetVertexBuffer(this.vertexBuffer);
            this.Device.Indices = this.indexBuffer;

            this.Effect.Parameters["World"].SetValue(this.World);
            this.Effect.Parameters["View"].SetValue(camera.View);
            this.Effect.Parameters["Projection"].SetValue(camera.Projection);
            this.Effect.CurrentTechnique.Passes[0].Apply();

#if MSXNA
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertexBuffer.VertexCount, 0, this.indexBuffer.IndexCount / 3);
#else
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.indexBuffer.IndexCount / 3);
#endif

        }

        /// <summary>
        /// Draws the sphere.
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        /// <param name="deltaT">Elapsed time since last draw, in seconds</param>
        public override void Draw(Matrix view, Matrix projection, float deltaT)
        {
            this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.CullCounterClockwiseFace };
            this.Device.SetVertexBuffer(this.vertexBuffer);
            this.Device.Indices = this.indexBuffer;

            this.Effect.Parameters["World"].SetValue(this.World);
            this.Effect.Parameters["View"].SetValue(view);
            this.Effect.Parameters["Projection"].SetValue(projection);
            this.Effect.CurrentTechnique.Passes[0].Apply();

#if MSXNA
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertexBuffer.VertexCount, 0, this.indexBuffer.IndexCount / 3);
#else
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.indexBuffer.IndexCount / 3);
#endif
        }

        /// <summary>
        /// Sets the clipping plane.
        /// </summary>
        /// <param name="clippingPlane">The clipping plane.</param>
        public override void SetClippingPlane(Vector4? clippingPlane)
        {
            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(clippingPlane.HasValue);
            if (clippingPlane.HasValue)
            {
                this.Effect.Parameters["ClipPlane"].SetValue(clippingPlane.Value);
            }
        }
    }
}

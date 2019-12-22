// <copyright file="Island.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Game.Entities
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;
    using XnaFlyby.Library.Entities.Primitives;

    /// <summary>
    /// Represents the height map based island.
    /// </summary>
    public class Island : GridStripBase
    {
        /// <summary>
        /// The heights from the heightmap in bytes.
        /// </summary>
        private byte[,] heights;

        /// <summary>
        /// The calculated heights used by the vertices in floats.
        /// </summary>
        private float[,] calculatedHeights;

        /// <summary>
        /// Gets or sets the texture containing the height information.
        /// </summary>
        public Texture2D HeightMap { get; set; }

        /// <summary>
        /// Gets or sets the diffuse texture of the island.
        /// </summary>
        public Texture2D Diffuse { get; set; }

        /// <summary>
        /// Initializes a new instance of the island grid strip.
        /// The parameters may be set via properties.
        /// </summary>
        /// <param name="device">Graphics device</param>
        public Island(GraphicsDevice device)
            : base(device, new Vector3(), 1.0f, 1, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the island grid strip.
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="heightMap">Height map</param>
        /// <param name="diffuse">Diffuse texture</param>
        public Island(GraphicsDevice device, Vector3 position, float scale, Texture2D heightMap, Texture2D diffuse)
            : base(device, position, scale, heightMap.Width, heightMap.Height)
        {
            this.HeightMap = heightMap;
            this.Diffuse = diffuse;
            this.Initialize();
        }

        /// <summary>
        /// Initializes the island by processing the height map and the diffuse texture.
        /// </summary>
        public override void Initialize()
        {
            uint[] colors = new uint[this.HeightMap.Width * this.HeightMap.Height];

            this.HeightMap.GetData(colors);

            this.heights = new byte[this.HeightMap.Width, this.HeightMap.Height];

            this.calculatedHeights = new float[this.HeightMap.Width, this.HeightMap.Height];

            for (int i = 0; i < this.HeightMap.Width; i++)
            {
                for (int j = 0; j < this.HeightMap.Height; j++)
                {
                    this.heights[i, j] = (byte)colors[j * this.HeightMap.Width + i];
                }
            }
        }

        /// <summary>
        /// Creates a vertex with position, normal direction and texture coordinates.
        /// </summary>
        /// <param name="i">The i coordinate of the height map.</param>
        /// <param name="j">The j coordinate of the height map.</param>
        /// <returns></returns>
        protected override VertexPositionNormalTexture CreateVertex(int i, int j)
        {
            this.calculatedHeights[i, j] = (this.heights[i, j] - 9) / 10.0f;
            return new VertexPositionNormalTexture(
                new Vector3(i, j, (this.heights[i, j] - 9) / 10.0f), // position
                i == 0 || i == (this.Width - 1) || j == 0 || j == (this.Height - 1) ? // on the edges the normal is pointing upwards
                new Vector3(0.0f, 0.0f, 1.0f) : // normal
                Vector3.Normalize(              // normal
                    Vector3.Cross(
                        new Vector3(2.0f, 0.0f, (this.heights[i + 1, j] - this.heights[i - 1, j]) / 10.0f),
                        new Vector3(0.0f, 2.0f, (this.heights[i, j + 1] - this.heights[i, j - 1]) / 10.0f)
                    )
                ),
                new Vector2((float)i / (this.Width - 1), (float)j / (this.Height - 1)) // texture
            );
        }

        /// <summary>
        /// Determines whether the specified 2D position is on the grid strip.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <returns>
        ///   <c>true</c> if the specified position is on the grid strip; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsPositionOnStrip(Vector3 position)
        {
            // determines the input vector's location on the heightmap
            Vector3 positionOnHeightmap = position - this.Position;

            // checks if the input vector is inside the boundaries of the heightmap-based terrain
            if (positionOnHeightmap.X > 0.0f && positionOnHeightmap.X < this.Width * this.Scale)
            {
                if (positionOnHeightmap.Y > 0.0f && positionOnHeightmap.Y < this.Height * this.Scale)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the height of the strip at the specified 2D position.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <returns>The height of the grid strip at the specified position.</returns>
        public override float GetHeight(Vector3 position)
        {
            // determines the input vector's location on the heightmap
            Vector3 positionOnHeightmap = position - this.Position;

            // gets the height map's array indices for the input vectors's location
            int left, top;
            left = (int)positionOnHeightmap.X / (int)this.Scale;
            top = (int)positionOnHeightmap.Y / (int)this.Scale;

            // calculates modulus to determine the input vector's location between the current and the next discrete indices of the height map
            float xNormalized = (positionOnHeightmap.X % this.Scale) / this.Scale;
            float yNormalized = (positionOnHeightmap.Y % this.Scale) / this.Scale;

            // using interpolation calculates the exact height at the input vector's position
            // interpolates between the neighbouring vertices of the heightmap.
            try
            {
                float topHeight = MathHelper.Lerp(this.calculatedHeights[left, top], this.calculatedHeights[left + 1, top],
                    xNormalized);

                float bottomHeight = MathHelper.Lerp(this.calculatedHeights[left, top + 1], this.calculatedHeights[left + 1, top + 1],
                    xNormalized);

                return MathHelper.Lerp(topHeight, bottomHeight, yNormalized) * this.Scale;
            }
            // if the height map array's indices are broken then the height map's translation height is returned
            catch (Exception)
            {
                return (this.Scale * this.Position.Z);
            }
        }

        public override void InitializeEffectParameters()
        {
            this.Effect.Parameters["CameraPosition"].SetValue(Vector3.Zero);

            this.Effect.Parameters["AmbientLightColor"].SetValue(Vector3.One * 0.1f);
            this.Effect.Parameters["DiffuseColor"].SetValue(Vector3.One);

            this.Effect.Parameters["LightIntensity"].SetValue(Vector3.One * 0.9f);
        }

        /// <summary>
        /// Draws the island.
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="deltaT">Elapsed time since last draw, in seconds</param>
        public override void Draw(Camera camera, float deltaT)
        {
            this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.CullClockwiseFace };

            this.InitializeEffectParameters();

            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(this.ClippingPlane.HasValue);
            this.Effect.Parameters["ClipPlane"].SetValue(this.ClippingPlane.GetValueOrDefault());

            this.Effect.Parameters["World"].SetValue(Matrix.CreateScale(this.Scale) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, this.Position.Z));
            this.Effect.Parameters["View"].SetValue(camera.View);
            this.Effect.Parameters["Projection"].SetValue(camera.Projection);

            this.Effect.Parameters["LightDirection"].SetValue(Scene.SunPosition);

            this.Effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.22f, 0.2f));
            this.Effect.Parameters["SpecularPower"].SetValue(5.0f);

            this.Effect.Parameters["IsTexturingEnabled"].SetValue(true);
            this.Effect.Parameters["DiffuseTexture"].SetValue(this.Diffuse);

            this.Effect.CurrentTechnique.Passes[0].Apply();

            base.Draw(camera, deltaT);
        }

        /// <summary>
        /// Draws the island.
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        /// <param name="deltaT">Elapsed time since last draw, in seconds</param>
        public override void Draw(Matrix view, Matrix projection, float deltaT)
        {
            this.Device.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.CullClockwiseFace };

            this.InitializeEffectParameters();

            this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(this.ClippingPlane.HasValue);
            this.Effect.Parameters["ClipPlane"].SetValue(this.ClippingPlane.GetValueOrDefault());

            this.Effect.Parameters["World"].SetValue(Matrix.CreateTranslation(-64.0f, -64.0f, -2.0f));
            this.Effect.Parameters["View"].SetValue(view);
            this.Effect.Parameters["Projection"].SetValue(projection);

            this.Effect.Parameters["LightDirection"].SetValue(Scene.SunPosition);

            this.Effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.22f, 0.2f));
            this.Effect.Parameters["SpecularPower"].SetValue(5.0f);

            this.Effect.Parameters["IsTexturingEnabled"].SetValue(true);
            this.Effect.Parameters["BaseTexture"].SetValue(this.Diffuse);

            this.Effect.CurrentTechnique.Passes[0].Apply();

            base.Draw(view, projection, deltaT);
        }

        /// <summary>
        /// Sets the clipping plane.
        /// </summary>
        /// <param name="clippingPlane">The clipping plane.</param>
        public override void SetClippingPlane(Vector4? clippingPlane)
        {
            this.ClippingPlane = clippingPlane;

            //this.Effect.Parameters["IsClippingPlaneEnabled"].SetValue(clippingPlane.HasValue);
            //if (clippingPlane.HasValue)
            //    this.Effect.Parameters["ClipPlane"].SetValue(clippingPlane.Value);
        }
    }
}

// <copyright file="MeshTag.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Components
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Provides way to access the properties (such as diffuse color, texture 
    /// or specular intensity) of a <see cref="ModelMesh"/>.
    /// </summary>
    public class MeshTag
    {
        /// <summary>
        /// Gets or sets the diffuse color of the mesh.
        /// </summary>
        public Vector3 Color { get; set; }

        /// <summary>
        /// Gets or sets the texture of the mesh.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the specular intensity of the mesh.
        /// </summary>
        public float SpecularPower { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshTag"/> class.
        /// </summary>
        /// <param name="color">Diffuse color of the mesh</param>
        /// <param name="texture">Texture of the mesh</param>
        /// <param name="specularPower">Specular intensity of the mesh</param>
        public MeshTag(Vector3 color, Texture2D texture, float specularPower)
        {
            this.Color = color;
            this.Texture = texture;
            this.SpecularPower = specularPower;
        }
    }
}

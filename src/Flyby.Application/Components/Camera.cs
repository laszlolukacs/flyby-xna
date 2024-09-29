// <copyright file="Camera.cs" company=".">
// See LICENSE for details.
// </copyright>

namespace XnaFlyby.Library.Components
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// An implementation of a 3D perspective view camera.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the aspect ratio of the perspective view camera.
        /// </summary>
        public float AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the camera's position vector.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the camera's target vector.
        /// </summary>
        public Vector3 Target { get; set; }

        /// <summary>
        /// Gets or sets the camera's up direction vector.
        /// </summary>
        public Vector3 Up { get; set; }

        /// <summary>
        /// Gets or sets the camera's rotation quaternion.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the XNA's AudioListener of the camera (for positional 3D audio).
        /// </summary>
        public AudioListener Listener { get; set; }

        /// <summary>
        /// Gets the camera's view direction vector;
        /// </summary>
        public Vector3 Direction
        {
            get { return this.Target - this.Position; }
        }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix View
        {
            get { return Matrix.CreateLookAt(this.Position, this.Target, this.Up); }
        }

        /// <summary>
        /// Gets the projection matrix of the camera.
        /// </summary>
        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.AspectRatio, 1.0f, 2400.0f); }
        }
    }
}

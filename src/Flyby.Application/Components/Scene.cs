// <copyright file="Scene.cs" company=".">
// See LICENSE for details.
// </copyright>

namespace XnaFlyby.Library.Components
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using XnaFlyby.Library.Entities;

    /// <summary>
    /// A fully fledged 3D scene which can have multiple objects which will be rendered.
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public Scene(Camera camera)
        {
            this.Updateables = new List<IUpdateableAndRenderable>();
            this.Renderables = new List<RenderableBase>();
            this.Camera = camera;
        }

        /// <summary>
        /// Gets or sets the sun's position.
        /// </summary>
        public static Vector3 SunPosition { get; set; }

        /// <summary>
        /// Gets or sets the 3D perspective camera.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the collection of updateable elements.
        /// </summary>
        public IList<IUpdateableAndRenderable> Updateables { get; set; }

        /// <summary>
        /// Gets or sets the collection of renderable elements.
        /// </summary>
        public IList<RenderableBase> Renderables { get; set; }

        /// <summary>
        /// Builds this scene.
        /// </summary>
        public virtual void Build()
        {
        }

        /// <summary>
        /// Steps in the game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (var element in this.Updateables)
            {
                element.Update(gameTime);
            }
        }

        /// <summary>
        /// Performs the rendering of the scene by invoking the render methods of the contained renderable elements.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Draw(GameTime gameTime)
        {
            foreach (var element in this.Renderables)
            {
                element.Draw(this.Camera, (float)gameTime.TotalGameTime.TotalSeconds);
            }

            foreach (var element in this.Updateables)
            {
                element.Draw(this.Camera, (float)gameTime.TotalGameTime.TotalSeconds);
            }
        }
    }
}

// <copyright file="Hud.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Game.Entities
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents the Heads-up Display.
    /// </summary>
    public class Hud
    {
        /// <summary>
        /// Gets or sets the sprite batch.
        /// </summary>
        protected SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        /// Gets or sets the collection of the HUD's sprite fonts.
        /// </summary>
        public IList<SpriteFont> SpriteFonts { get; set; }

#if DEBUG
        public string AdapterName { get; set; }
        public int FramesPerSecond { get; set; }
#endif
        public int Width { get; set; }
        public int Height { get; set; }

        public float Altitude { get; set; }
        public float TerrainHeight { get; set; }
        public float Velocity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the overlay help is displayed.
        /// </summary>
        public bool IsHelpEnabled { get; set; }

        /// <summary>
        /// Gets or sets the help overlay texture.
        /// </summary>
        public Texture2D HelpOverlay { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hud"/> class.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="width">The width of the screen.</param>
        /// <param name="height">The height of the screen.</param>
        public Hud(SpriteBatch spriteBatch, int width, int height)
        {
            this.SpriteBatch = spriteBatch;
            this.SpriteFonts = new List<SpriteFont>();

            this.Width = width;
            this.Height = height;

            this.IsHelpEnabled = false;

            //this.HelpOverlay = helpOverlay; // TEXTURE2D
        }

        /// <summary>
        /// Draws the heads-up display.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Begin();

            this.SpriteBatch.DrawString(this.SpriteFonts[0], "Altitude", new Vector2(this.Width - 120, this.Height - 140), Color.LightGreen);
            this.SpriteBatch.DrawString(this.SpriteFonts[1], this.Altitude.ToString("00.00"), new Vector2(this.Width - 200, this.Height - 110), Color.LightGreen);

#if DEBUG
            this.SpriteBatch.DrawString(this.SpriteFonts[0], "Height", new Vector2(this.Width - 120, this.Height - 70), Color.LightGreen);
            this.SpriteBatch.DrawString(this.SpriteFonts[1], this.TerrainHeight.ToString("00.00"), new Vector2(this.Width - 200, this.Height - 40), Color.LightGreen);
#else
            SpriteBatch.DrawString(SpriteFonts[0], "Velocity", new Vector2(Width - 120, Height - 70), Color.LightGreen);
            SpriteBatch.DrawString(SpriteFonts[1], Velocity.ToString("00.00"), new Vector2(Width - 200, Height - 40), Color.LightGreen);
#endif

            // TODO: fix this
            //if (this.IsHelpEnabled)
            //{
            //    this.SpriteBatch.Draw(this.HelpOverlay, new Rectangle(0, (this.Height - this.Width) / 2, this.Width, this.Width), Color.White);
            //}

#if DEBUG
            this.SpriteBatch.DrawString(this.SpriteFonts[2], string.Format("{00} FPS", this.FramesPerSecond), new Vector2(0, 0), Color.Red);
            this.SpriteBatch.DrawString(this.SpriteFonts[2], string.Format("Rendering on {0}", this.AdapterName), new Vector2(0, 20), Color.Red);
#endif

            this.SpriteBatch.End();
        }
    }
}

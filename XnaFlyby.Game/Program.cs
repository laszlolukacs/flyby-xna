// <copyright file="Program.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Game
{
#if WINDOWS || XBOX
    /// <summary>
    /// Class containing the entry point for the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            using (var game = new FlybyGame())
            {
                game.Run();
            }
        }
    }
#endif
}
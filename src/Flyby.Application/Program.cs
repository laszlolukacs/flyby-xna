// <copyright file="Program.cs" company=".">
// See LICENSE for details.
// </copyright>

using System;

namespace Flyby.Application
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new FlybyGame())
                game.Run();
        }
    }
}

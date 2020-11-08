using System;

namespace Flyby.Application
{
    public static class Program
    {
#if WINDOWS
        [STAThread]
#endif
        static void Main()
        {
            using (var game = new FlybyGame())
                game.Run();
        }
    }
}

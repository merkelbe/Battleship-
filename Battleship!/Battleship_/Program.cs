using System;

namespace Battleship_
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XNAGame game = new XNAGame())
            {
                game.Run();
            }
        }
    }
#endif
}


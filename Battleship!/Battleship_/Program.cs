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
            // XNA version of the game you can play yourself
            using (XNAGame game = new XNAGame())
            {
                game.Run();
            }

            // Picks randomly 
            //GameStatsGenerator gameStatsGenerator = new GameStatsGenerator();
            //gameStatsGenerator.Run(1000);
        }
    }
#endif
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Battleship_
{
    // Point of this class is to serialize a game to a text file to be handed off to some smart AI program that I have to figure out how to write.
    class Serializer
    {
        internal Serializer()
        {

        }

        internal void WriteBattleshipGuessesToTextFile(AbstractGame _abstractGame)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string resultsDirectory = currentDirectory + "\\..\\..\\..\\Results";
            if(!Directory.Exists(resultsDirectory))
            {
                Directory.CreateDirectory(resultsDirectory);
            }
            string gameDirectory = resultsDirectory + "\\" + _abstractGame.NumberOfGuesses;
            if (!Directory.Exists(gameDirectory))
            {
                Directory.CreateDirectory(gameDirectory);
            }
            int gameNumber = 0;
            string filePath;
            
            do
            {
                filePath = gameDirectory + "\\gameRecord" + ++gameNumber + ".txt";
            } while (File.Exists(filePath));

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                int[,] board = _abstractGame.Board;
                for(int row = 0; row < board.GetLength(0); ++row)
                {
                    sw.WriteLine(String.Join(",",m.GetRowFrom2DArray(board, row)));
                }
            }
        }


    }
}

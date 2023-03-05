using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using CS_Game_Of_Life_Console;

namespace Game_Of_Life {
    public class Program {
        public static void Main(string[] args) {
            GameOfLife game = new GameOfLife(0, 0);
            game.InsertRandomLives();
            Console.WriteLine(game.DrawGeneration());

        loop:
            {
                game.NewGeneration();
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine(game.DrawGeneration());

                if (!false)
                    goto loop;
            }
        }
    }
}
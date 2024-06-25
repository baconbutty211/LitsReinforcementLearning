using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public static class Tester
    {
        static Environment environment = new Environment();

        public static async void PlayGame(Agent agent1, Agent agent2, Verbosity verbosity = Verbosity.High)
        {
            if (verbosity >= Verbosity.High)
                DisplayBoard(environment);

            while (!environment.isDone)
            {
                Agent agent = environment.stepCount % 2 == 0 ? agent1 : agent2;
             
                Action action = agent.Exploit(environment);
                environment.Step(action);

                if (verbosity >= Verbosity.High)
                    DisplayBoard(environment, action);
            } // Play Game

            if (verbosity >= Verbosity.Mid)
                Console.WriteLine(environment.GetResult());

            environment.Reset();
        }

        static string prevStr = "";
        static string route = "Route: ";
        static void DisplayBoard(Environment environment, Action action = null, int sleep = 2000)
        {
            string stateStr = environment.ToString();

            Console.Clear();
            for (int i = 0; i < prevStr.Length; i++)
            {
                char piece = (stateStr[i] != prevStr[i]) ? '#' : stateStr[i];
                WritePieceColour(piece);
            }
            if (action != null)
            {
                route += $"{action.Id}, ";
                Console.WriteLine(route);
            }
            prevStr = stateStr;
            Thread.Sleep(sleep);
        }
        static void WritePieceColour(char pieceChar)
        {
            ConsoleColor colour = ConsoleColor.White;
            switch (pieceChar)
            {
                case '#':
                    colour = ConsoleColor.Red;
                    break;
                case 'L':
                    colour = ConsoleColor.Blue;
                    break;
                case 'I':
                    colour = ConsoleColor.Green;
                    break;
                case 'T':
                    colour = ConsoleColor.DarkMagenta;
                    break;
                case 'S':
                    colour = ConsoleColor.DarkYellow;
                    break;
                default:
                    break;
            }
            WriteColour(pieceChar, colour);
        }
        static void WriteColour(char text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }
        static void WriteColour(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}

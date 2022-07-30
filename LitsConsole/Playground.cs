using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    public static class Playground
    {
        static Environment environment = new Environment();

        public static void PlayGame(Agent agent)
        {
            while (!environment.isDone)
            {
                bool isFirstPlayer = environment.stepCount % 2 == 0;
                Action action = null;
                while (action == null)
                    action = isFirstPlayer ? agent.Exploit(environment) : GetUserInputAction(environment.validActions);
                
                environment.Step(action);
                DisplayBoard(environment, action);
            }
            Console.WriteLine(environment.GetResult());

            environment.Reset();
        }
        //static Action GetUserInputAction(Action[] validActions)
        //{
        //    int count = 0;
        //    foreach (ActionType act in Enum.GetValues(typeof(ActionType)))
        //    {
        //        Console.WriteLine($"{count++}) {act}");
        //        Console.WriteLine($"{Action.GetString(act)}");
        //    }
        //    Console.Write("Enter the piece type:");
        //    ActionType type = (ActionType)4 + int.Parse(Console.ReadLine());

        //    count = 0;
        //    foreach (RotationType rot in Enum.GetValues(typeof(RotationType)))
        //    {
        //        Console.WriteLine($"{count++}) {rot}");
        //        Console.WriteLine($"{Action.GetString(type, rot)}");
        //    }
        //    Console.Write("Enter the piece rotation:");
        //    RotationType rotation = (RotationType)int.Parse(Console.ReadLine());

        //    count = 0;
        //    foreach (FlipType reflct in Enum.GetValues(typeof(FlipType)))
        //    {
        //        Console.WriteLine($"{count++}) {reflct}");
        //        Console.WriteLine($"{Action.GetString(type, rotation, reflct)}");
        //    }
        //    Console.Write("Enter the piece reflection:");
        //    FlipType flip = (FlipType)int.Parse(Console.ReadLine());

        //    Console.Write("Enter the topLeft position (0 - 88):");
        //    int topLeft = int.Parse(Console.ReadLine());

        //    foreach (Action action in validActions)
        //        if (action.Equals(topLeft, type, rotation, flip)) //User action matches
        //            return action;
        //    return null;
        //}
        static Action GetUserInputAction(Action[] validActions)
        {
            int[] userAction = new int[4];
            while (true)
            {
                Console.WriteLine($"Enter the 4 positions of the tiles you want to place you're piece on (w,x,y,z):");
                string input = Console.ReadLine();
                string[] strPositions = input.Split(',');

                if (strPositions.Length != 4)
                    continue;

                for (int i = 0; i < strPositions.Length; i++)
                {
                    if (int.TryParse(strPositions[i], out int act))
                        if (act < 100 && act > 0)
                            userAction[i] = act;
                    continue;
                }
            }

            foreach (Action action in validActions)
                if (action.Equals(userAction))
                    return action;
            return null;
        }

        static string prevStr = environment.ToString();
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
                    colour = ConsoleColor.DarkMagenta;
                    break;
                case 'L':
                    colour = ConsoleColor.Red;
                    break;
                case 'I':
                    colour = ConsoleColor.Yellow;
                    break;
                case 'T':
                    colour = ConsoleColor.Green;
                    break;
                case 'S':
                    colour = ConsoleColor.Blue;
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

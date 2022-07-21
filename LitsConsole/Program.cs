using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        static List<int> optimumPath = new List<int>();

        static void Main(string[] args)
        {
            bool isUserPlaying = bool.Parse(args[0]);

            Log.Clear();
            Environment environment = new Environment();
            Observation initial = environment.Reset();
            prevStr = environment.ToString();

            Agent powers = new Agent(AgentType.DynamicProgramming, initial, environment.features, true);
            //Agent powers = new Agent(AgentType.DynamicProgramming, "Powers", true);
            //Agent drEvil = new Agent(AgentType.DynamicProgramming, "Powers", false);
            while (!environment.isDone)
            {
                Action action = powers.Exploit(environment);
                environment.Step(action);
                DisplayBoard(environment, action, 50);

                //if (environment.isDone)
                //    break;

                //if (isUserPlaying)
                //{
                //    Action counterAction;
                //    do
                //    {
                //        counterAction = GetUserInputAction(environment.validActions);
                //    } while (counterAction == null);

                //    environment.Step(counterAction);
                //    DisplayBoard(environment, counterAction);
                //}
                //else
                //{
                //    Action counterAction = drEvil.Exploit(environment);
                //    environment.Step(counterAction);
                //    DisplayBoard(environment, counterAction);
                //}
            }
            //environment.Reset();
            //powers.Reset();
            //drEvil.Reset();
            
            powers.Save("Powers");
        }

        static Action GetUserInputAction(Action[] validActions) 
        {
            int count = 0;
            foreach (ActionType act in Enum.GetValues(typeof(ActionType)))
            {
                Console.WriteLine($"{count++}) {act}");
                Console.WriteLine($"{Action.GetString(act)}");
            }
            Console.Write("Enter the piece type:");
            ActionType type = (ActionType)4+int.Parse(Console.ReadLine());

            count = 0;
            foreach (RotationType rot in Enum.GetValues(typeof(RotationType)))
            {
                Console.WriteLine($"{count++}) {rot}");
                Console.WriteLine($"{Action.GetString(type, rot)}");
            }
            Console.Write("Enter the piece rotation:");
            RotationType rotation = (RotationType)int.Parse(Console.ReadLine());

            count = 0;
            foreach (FlipType reflct in Enum.GetValues(typeof(FlipType)))
            {
                Console.WriteLine($"{count++}) {reflct}");
                Console.WriteLine($"{Action.GetString(type, rotation, reflct)}");
            }
            Console.Write("Enter the piece reflection:");
            FlipType flip = (FlipType)int.Parse(Console.ReadLine());

            Console.Write("Enter the topLeft position (0 - 88):");
            int topLeft = int.Parse(Console.ReadLine());

            foreach (Action action in validActions)
                if (action.Equals(topLeft, type, rotation, flip)) //User action matches
                    return action;
            return null;
        }

        static string prevStr = "";
        static string route = "Route: ";
        static void DisplayBoard(Environment environment, Action action, int sleep = 2000)
        {
            string stateStr = environment.ToString();

            Console.Clear();
            for (int i = 0; i < prevStr.Length; i++)
            {
                char piece = (stateStr[i] != prevStr[i]) ? '#' : stateStr[i];
                WritePieceColour(piece);
            }
            route += $"{action.Id}, ";
            Console.WriteLine(route);
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

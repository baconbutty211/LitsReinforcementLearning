using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    public static class Tester
    {
        static Environment environment = new Environment();


        public static void PlayGame(Agent agent1, Agent agent2, Verbosity verbosity = Verbosity.High)
        {
            if (verbosity >= Verbosity.High)
                DisplayBoard(environment);

            while (!environment.isDone)
            {
                Agent agent = environment.stepCount % 2 == 0 ? agent1 : agent2;
             
                Action action = agent.Exploit(environment);
                Log.Write($"Applying action {action}...");
                environment.Step(action);

                if (verbosity >= Verbosity.High)
                    DisplayBoard(environment, action);
            } // Play Game

            if (verbosity >= Verbosity.Mid)
                Console.WriteLine(environment.GetResult());

            environment.Reset();
        }

        public static void PlayUser(Agent subject1)
        {
            while (!environment.isDone)
            {
                Action action = subject1.Exploit(environment);
                environment.Step(action);
                DisplayBoard(environment, action, 0);

                if (environment.isDone)
                    break;

                Action counterAction;
                do
                {
                    counterAction = GetUserInputAction(environment.validActions);
                } while (counterAction == null);

                environment.Step(counterAction);
                DisplayBoard(environment, counterAction);
            }
            Console.WriteLine(environment.GetResult());
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
            ActionType type = (ActionType)4 + int.Parse(Console.ReadLine());

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

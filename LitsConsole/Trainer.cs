using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    public static class Trainer
    {
        public enum Verbosity { None, Low, Mid, High }
        static Environment environment = new Environment();

        public static Agent CreateNewAgent(string name) 
        {
            Observation initial = environment.Reset();
            Agent newAgent = new Agent(AgentType.DynamicProgramming, initial, environment.features);
            newAgent.Save(name);
            return newAgent;
        }

        public static void PlaySolo(Agent subject1, Verbosity verbosity) 
        {
            while (!environment.isDone)
            {
                Action action = subject1.Exploit(environment);
                environment.Step(action);
                if (verbosity >= Verbosity.High)
                    DisplayBoard(environment, action);
            } // Play Game
            if(verbosity >= Verbosity.Mid)
                Console.WriteLine(GetResult(environment.GetResult()));
            environment.Reset();
            subject1.Reset();
        }
        public static void PlayAI(Agent subject1, Agent subject2, Verbosity verbosity) 
        {
            while (!environment.isDone)
            {
                Action action = subject1.Exploit(environment);
                environment.Step(action);
                if(verbosity >= Verbosity.High)
                    DisplayBoard(environment, action);

                if (environment.isDone)
                    break;

                Action counterAction = subject2.Exploit(environment);
                environment.Step(counterAction);
                if (verbosity >= Verbosity.High)
                    DisplayBoard(environment, counterAction);
            } // Play Game
            if(verbosity >= Verbosity.Mid)
                Console.WriteLine(GetResult(environment.GetResult()));
            environment.Reset();
            subject1.Reset();
            subject2.Reset();
        }
        public static void TrainSolo(Agent subject1, int episodes, Verbosity verbosity) 
        {
            for (int i = 0; i < episodes; i++)
            {
                PlaySolo(subject1, verbosity);
                if (verbosity >= Verbosity.Low)
                    Console.WriteLine($"Completed {i} games of training");
            }
        }
        public static void TrainAI(Agent subject1, Agent subject2, int episodes, Verbosity verbosity)
        {
            for (int i = 0; i < episodes; i++)
            {
                PlayAI(subject1, subject2, verbosity);
                if (verbosity >= Verbosity.Low)
                    Console.WriteLine($"Completed {i} games of training");
            }
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
            Console.WriteLine(GetResult(environment.GetResult()));
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
        static string GetResult(Environment.End result)
        {
            switch (result)
            {
                case Environment.End.Win:
                    return "User wins. \nScore is User:{xFilled} > Comp:{oFilled}";
                case Environment.End.Lose:
                    return "Computer wins. \nScore is User:{xFilled} < Comp:{oFilled}";
                case Environment.End.Draw:
                    return "Draw. \nScore is User:{xFilled} = Comp:{oFilled}";
                default:
                    throw new NotImplementedException();
            }
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

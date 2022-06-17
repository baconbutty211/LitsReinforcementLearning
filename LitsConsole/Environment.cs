using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLitsMadeSimpler
{
    public class Environment
    {
        private enum Tile { _, O, X }
        public const int size = 100;
        private Random rnd = new Random();

        private Action[] validActions 
        {
            get 
            {
                List<Action> actions = new List<Action>();
                foreach (Action action in Action.actionSpace)
                    if (IsValid(action))
                        actions.Add(action);
                return actions.ToArray();
            }
        }
        public int stepCount { get; private set; }
        public bool isDone
        {
            get
            {
                if (validActions.Length == 0)
                    return true;
                for (int i = 0; i < size; i++)
                    if (board[i] == Tile.X)
                        if (!state[i])
                            return false;
                return true;
            }
        }
        
        bool[] state;

        static Tile[] board = SetBoard();
        private static Tile[] SetBoard() 
        {
            Tile[] board = new Tile[size];
            for (int i = 0; i < size; i++)
            {
                if (i / 10 == 0 || i / 10 == 2 || i / 10 == 4)
                    if (i % 10 <= 4)
                        board[i] = Tile.X;
                    else
                        board[i] = Tile.O;
                else if (i / 10 == 5 || i / 10 == 7 || i / 10 == 9)
                    if (i % 10 <= 4)
                        board[i] = Tile.O;
                    else
                        board[i] = Tile.X;
                else
                    board[i] = Tile._;
            } //Define initial board
            return board;
        }

        public Environment()
        {
            state = new bool[size];
        }

        public Observation Reset()
        {
            //stepCount = 0;
            for (int i = 0; i < size; i++)
                state[i] = false;
            return new Observation(state.Clone() as bool[], 0, false);
        }
        public void Reset(Observation observation) 
        {
            Reset(observation.state);
        }
        /// <summary>
        /// Sets the environment to the given state.
        /// </summary>
        public void Reset(bool[] state) 
        {
            this.state = state.Clone() as bool[];
        }
        public Observation Step(Action action)
        {
            if (isDone)
                throw new IndexOutOfRangeException($"Already reached the end state ({state}). Don't ask for a new action.");

            float reward = 0;
            foreach (int pos in action.action)
            {
                if (!state[pos])
                    state[pos] = true;
                else
                    throw new IndexOutOfRangeException($"Action has already been taken.");

                switch (board[pos])
                {
                    case Tile._:
                        reward -= 1;
                        break;
                    case Tile.O:
                        reward -= 2;
                        break;
                    case Tile.X:
                        reward += 2;
                        break;
                    default:
                        throw new NotImplementedException();
                } // Set reward
            }
            stepCount++;
            return new Observation(state.Clone() as bool[], reward, isDone);
        }

        public Action GetRandomAction() 
        {
            return validActions[rnd.Next(validActions.Length)];
        }
        public IEnumerable<Action> GetValidActions() 
        {
            foreach (Action action in Action.actionSpace)
                if (IsValid(action))
                    yield return action;
        }
        private bool IsValid(Action action)
        {
            return IsValid(action.actionPreShift, action.topLeft);
        }
        private bool IsValid(int[] action, int topLeft)
        {
            for (int i = 0; i < action.Length; i++)
            {
                int actPos = action[i] + topLeft; //Position of new tile about to be placed / checked if valid

                //Check tile doesn't fall off board
                if (actPos >= size)
                    return false;
                //Check tile doesn't wrap around the board ???
                if (((action[i] + topLeft) % 10) - (action[i] % 10) < 0)
                    return false;

                //Check if a tile has already been placed in that position
                if (state[actPos])
                    return false;
            }
            return true;
        }

        public static string ToString(bool[] state)
        {
            string boardStr = "  ";
            for (int i = 0; i < 10; i++)
                boardStr += $"{i},";
            for (int i = 0; i < size; i++)
            {
                if (i % 10 == 0)
                    boardStr += $"\n{i / 10} ";
                if (state[i])
                    boardStr += $"#,";
                else
                    boardStr += $"{board[i]},";
            }
            return boardStr + '\n';
        }
    }

    public struct Observation
    {
        //public static Observation initial { get { return new Observation(Environment.initialState, 0, false); } }

        public bool[] state { get; private set; }
        public float reward { get; private set; }
        public bool isDone { get; private set; }

        public Observation(bool[] state, float reward, bool isDone)
        {
            this.state = state;
            this.reward = reward;
            this.isDone = isDone;
        }
        /// <summary>
        /// Used only for cloning
        /// </summary>
        private Observation(Observation observation) 
        {
            this.state = observation.state.Clone() as bool[];
            this.reward = observation.reward;
            this.isDone = observation.isDone;
        }
        public Observation Clone() 
        {
            return new Observation(this);
        }
    }

    public enum ActionType
    {
        // Tile, TopLeft(None), TopLeft(Quarter), TopLeft(Half), TopLeft(3 Quarters)
        L = 4,  // #            #   L               # L L           # L L
                // L                L                   L           L
                // L L L        L L L                   L           L

        I = 5,  // #            # I I I I
                // I
                // I
                // I
                // I

        T = 6,  // # T T        #                   # T             #   T
                //   T          T T T                 T             T T T
                //   T          T                   T T T               T

        S = 7,  // # S S        # 
                //   S          S S S
                // S S              S
    }
    public enum RotationType
    {
        None,
        Quarter,
        Half,
        ThreeQuarters
    } // Rotations are applied anti-clockwise
    public class Action
    {
        static int size = Environment.size;
        public static Action[] actionSpace = GetActionSpace();

        private static int id = 0;
        public int Id;
        public ActionType type;
        private RotationType rotation;
        public int topLeft;
        public int[] actionPreShift 
        {
            get
            {
                int[] action;
                switch (type)
                {
                    case ActionType.L:
                        switch (rotation)
                        {
                            case RotationType.None:
                                action = new int[] { 0, 10, 20, 21, 22 };
                                break;
                            case RotationType.Quarter:
                                action = new int[] { 20, 21, 22, 12, 2 };
                                break;
                            case RotationType.Half:
                                action = new int[] { 22, 12, 2, 1, 0 };
                                break;
                            case RotationType.ThreeQuarters:
                                action = new int[] { 2, 1, 0, 10, 20 };
                                break;
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate L tile
                        break;
                    case ActionType.I:
                        switch (rotation)
                        {
                            case RotationType.None:
                                action = new int[] { 0, 10, 20, 30, 40 };
                                break;
                            case RotationType.Quarter:
                                action = new int[] { 0, 1, 2, 3, 4 };
                                break;
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate I tile
                        break;
                    case ActionType.T:
                        switch (rotation)
                        {
                            case RotationType.None:
                                action = new int[] { 0, 1, 2, 11, 21 };
                                break;
                            case RotationType.Quarter:
                                action = new int[] { 0, 10, 20, 11, 12 };
                                break;
                            case RotationType.Half:
                                action = new int[] { 20, 21, 22, 11, 1 };
                                break;
                            case RotationType.ThreeQuarters:
                                action = new int[] { 22, 12, 2, 11, 10 };
                                break;
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate T tile
                        break;
                    case ActionType.S:
                        switch (rotation)
                        {
                            case RotationType.None:
                                action = new int[] { 1, 2, 11, 20, 21 };
                                break;
                            case RotationType.Quarter:
                                action = new int[] { 10, 0, 11, 22, 12 };
                                break;
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate S tile
                        break;
                    default:
                        throw new NotImplementedException($"No case block of type {type}");
                } //Create tile/action
                return action;
            }
        }
        public int[] action
        {
            get 
            {
                return ShiftActionToTopLeft(actionPreShift, topLeft);
            } 
        }

        private Action(int topLeft, ActionType action, RotationType rotation) 
        {
            this.Id = id++;
            this.topLeft = topLeft;
            this.type = action;
            this.rotation = rotation;
        }
        private static Action[] GetActionSpace()
        {
            List<Action> actions = new List<Action>();
            for (int topLeft = 0; topLeft < size; topLeft++)
                foreach (ActionType act in Enum.GetValues(typeof(ActionType)))
                    foreach (RotationType rot in Enum.GetValues(typeof(RotationType)))
                        if (act == ActionType.I || act == ActionType.S)
                            if (rot == RotationType.Half || rot == RotationType.ThreeQuarters)
                                continue; // These are just duplicates, as actions I & S have rotational symetry order 2.
                        else
                            actions.Add(new Action(topLeft, act, rot));
            return actions.ToArray();
        } // Sets the whole action space

        private static int[] ShiftActionToTopLeft(int[] action, int topLeft)
        {
            for (int i = 0; i < action.Length; i++)
                action[i] += topLeft;
            return action;
        }
    }
}
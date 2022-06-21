using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLitsMadeSimpler
{
    public class Environment
    {
        private enum Tile { _, O, X, L, I, T, S }
        private Dictionary<Tile, int> availableActions;
        public const int size = 100;
        private Random rnd = new Random();

        public Action[] validActions 
        {
            get 
            {
                List<Action> actions = new List<Action>();
                foreach (Action action in Action.GetActions())
                    if(IsValid(action))
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
            Reset();
        }

        public Observation Reset()
        {
            stepCount = 0;
            availableActions = new Dictionary<Tile, int>() { { Tile.L, 5 }, { Tile.I, 5 }, { Tile.T, 5 }, { Tile.S, 5 } };
            state = new bool[size];
            for (int i = 0; i < size; i++)
                state[i] = false;
            return new Observation(state, 0, false);
        }
        //public void Reset(Observation observation) 
        //{
        //    Reset(observation.state);
        //}
        ///// <summary>
        ///// Sets the environment to the given state.
        ///// </summary>
        //public void Reset(bool[] state) 
        //{
        //    this.state = state.Clone() as bool[];
        //}
        public Observation Step(Action action)
        {
            if (isDone)
                throw new IndexOutOfRangeException($"Already reached the end state ({state}). Don't ask for a new action.");

            float reward = 0;
            foreach (int pos in action.action)
            {
                if (!state[pos])
                {
                    state[pos] = true;
                    board[pos] = ActionTypeToTile(action.type); // Set board position to the new action type
                }
                else
                    throw new IndexOutOfRangeException($"Action has already been taken.");

                switch (board[pos])
                {
                    case Tile.O:
                        reward -= 2;
                        break;
                    case Tile.X:
                        reward += 2;
                        break;
                    default: // Tile is Empty (_) of filled (L, I, T, S)
                        reward -= 1;
                        break;
                } // Set reward
            }
            availableActions[ActionTypeToTile(action.type)]--;
            stepCount++;
            return new Observation(state, reward, isDone);
        }

        #region Validation
        private bool IsValid(Action action)
        {
            if (stepCount == 0) //All actions are valid on the first move
                return true;

            if (IsOverlapingOtherAction(action)) //Check if a tile has already been placed in that position
                return false;
            if (IsAdjacentToSameActionType(action)) // Check if the new piece is adjacent to another piece
                return false;
            if (!IsAdjacentToOtherAction(action)) // Check if the new piece is adjacent to another piece of the same type;
                return false;
            if (Is2By2Filled(action)) // Checks if a 2*2 area on the board is filled
                return false;
            if (!IsActionTypeAvailable(action))
                return false;
            else
                return true;
        } // Checks whether the given action can be placed on the board. (Called before the action is applied).
        private bool IsOverlapingOtherAction(Action action) 
        {
            foreach (int act in action)
                if (state[act]) //Check if a tile has already been placed in that position
                    return true;
            return false;
        } //Check if a tile has already been placed in that position
        private bool IsAdjacentToOtherAction(Action action) 
        {
            foreach (int act in action)
            {
                List<int> adjTiles = new List<int>(); // Set the position of adjTiles that fit on the board
                if (act >= 10)
                    adjTiles.Add(act - 10);
                if (act < size - 10) // Size is 100, but indexing at 0 means largest index is 99.
                    adjTiles.Add(act + 10);
                if (act % 10 != 0) // So adjacent tile doesn't wrap around side of board
                    adjTiles.Add(act - 1);
                if (act % 10 != 9) // So adjacent tile doesn't wrap around side of board
                    adjTiles.Add(act + 1);

                foreach (int adjPos in adjTiles)
                    if (!TileIsEmpty(adjPos)) // Check if the new piece is adjacent to another piece
                        return true;
            }
            return false;
        } // Check if the new piece is adjacent to another piece
        private bool IsAdjacentToSameActionType(Action action) 
        {
            Tile actionType = ActionTypeToTile(action.type);
            foreach (int act in action)
            {
                List<int> adjTiles = new List<int>(); // Set the position of adjTiles that fit on the board
                if (act >= 10)
                    adjTiles.Add(act - 10);
                if (act < size - 10) // Size is 100, but indexing at 0 means largest index is 99.
                    adjTiles.Add(act + 10);
                if (act % 10 != 0) // So adjacent tile doesn't wrap around side of board
                    adjTiles.Add(act - 1);
                if (act % 10 != 9) // So adjacent tile doesn't wrap around side of board
                    adjTiles.Add(act + 1);

                foreach (int adjPos in adjTiles)
                    if (actionType == board[adjPos]) // Check if the new piece is adjacent to another piece of the same type;
                        return true;
            }
            return false;
        } // Check if the new piece is adjacent to another piece of the same type;
        private bool Is2By2Filled(Action action) 
        {
            bool[] stateClone = state.Clone() as bool[];
            foreach (int act in action)
                stateClone[act] = true;

            for(int i = 0; i < stateClone.Length; i++) 
                if (i >= 89 || i == 9) // 2*2 Area will be out of range of the board
                    continue;
                else if (stateClone[i] && stateClone[i + 1] && stateClone[i + 10] && stateClone[i + 11]) // Checks if a 2*2 area on the board is filled
                    return true;
            
            return false;
        } // Checks if a 2*2 area on the board is filled
        private bool IsActionTypeAvailable(Action action) 
        {
            int actionsRemaining = availableActions[ActionTypeToTile(action.type)];
            if (actionsRemaining == 0)
                return false;
            else if (actionsRemaining > 0)
                return true;
            else
                throw new ArgumentOutOfRangeException($"{actionsRemaining} should never get to less than 0");
        }
        #endregion

        public Action GetRandomAction()
        {
            Action[] actions = validActions;
            return actions[rnd.Next(actions.Length)];
        }

        private Tile ActionTypeToTile(ActionType type) 
        {
            switch (type)
            {
                case ActionType.L:
                    return Tile.L;
                case ActionType.I:
                    return Tile.I;
                case ActionType.T:
                    return Tile.T;
                case ActionType.S:
                    return Tile.S;
                default:
                    throw new NotImplementedException();
            }
        }
        private bool TileIsEmpty(int pos) 
        {
            return (int)board[pos] < 3; 
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
            this.state = state.Clone() as bool[];
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
        L = 4,  // #            #                   # L L           # L L
                // L                L                   L           L
                // L L          L L L                   L           
//Flipped       
                // #  L         #                   # L             # L L
                //    L         L                   L                   L
                //  L L         L L L               L                   

        I = 5,  // #            # I I I
                // I
                // I
                // I
//Flipped       
               
        T = 6,  // # T T        #                   #               #   T
                //   T          T T                   T               T T
                //              T                   T T T               T
//Flipped       
                 
        S = 7,  // # S S        # 
                // S S          S S
                //                S
//Flipped       
                // # S          #   S                            
                //   S S          S S                            
                //                S                                    
    }
    public enum RotationType { None, Quarter, Half, ThreeQuarters } // Rotations are applied anti-clockwise
    public enum FlipType { None, Flipped } //Flipped in y-axis (No other axes are nescessary as they should be covered by this flip and a rotation)
    public class Action
    {
        static int size = Environment.size;
        static Action[] actionSpace = GetActionSpace();
        public static int actionSpaceSize = actionSpace.Length;

        private static int id = 0;
        public int Id;
        public ActionType type;
        private RotationType rotation;
        private FlipType flip;
        private int topLeft;
        private int[] actionPreShift 
        {
            get
            {
                switch (type)
                {
                    case ActionType.L:
                        switch (rotation)
                        {
                            case RotationType.None:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 0, 10, 20, 21 };
                                    case FlipType.Flipped:
                                        return new int[] { 2, 12, 22, 21 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            case RotationType.Quarter:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 20, 21, 22, 12 };
                                    case FlipType.Flipped:
                                        return new int[] { 22, 21, 20, 10 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            case RotationType.Half:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 22, 12, 2, 1 };
                                    case FlipType.Flipped:
                                        return new int[] { 20, 10, 0, 1 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            case RotationType.ThreeQuarters:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 2, 1, 0, 10 };
                                    case FlipType.Flipped:
                                        return new int[] { 0, 1, 2, 12 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate L tile
                    case ActionType.I:
                        switch (rotation)
                        {
                            case RotationType.None:
                                return new int[] { 0, 10, 20, 30 };
                            case RotationType.Quarter:
                                return new int[] { 0, 1, 2, 3 };
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate I tile
                    case ActionType.T:
                        switch (rotation)
                        {
                            case RotationType.None:
                                return new int[] { 0, 1, 2, 11 };
                            case RotationType.Quarter:
                                return new int[] { 0, 10, 20, 11 };
                            case RotationType.Half:
                                return new int[] { 20, 21, 22, 11 };
                            case RotationType.ThreeQuarters:
                                return new int[] { 22, 12, 2, 11 };
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate T tile
                    case ActionType.S:
                        switch (rotation)
                        {
                            case RotationType.None:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 1, 2, 10, 11 };
                                    case FlipType.Flipped:
                                        return new int[] { 1, 0, 12, 11 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            case RotationType.Quarter:
                                switch (flip)
                                {
                                    case FlipType.None:
                                        return new int[] { 10, 0, 21, 11 };
                                    case FlipType.Flipped:
                                        return new int[] { 12, 2, 21, 11 };
                                    default:
                                        throw new NotImplementedException($"No case block of type {flip}");
                                }
                            default:
                                throw new NotImplementedException($"No case block of type {rotation}");
                        } // Rotate S tile
                    default:
                        throw new NotImplementedException($"No case block of type {type}");
                } //Create tile/action
            }
        }
        public int[] action;
        private Action(int topLeft, ActionType action, RotationType rotation, FlipType flip) 
        {
            this.Id = id++;
            this.topLeft = topLeft;
            this.type = action;
            this.rotation = rotation;
            this.flip = flip;
        }
        public IEnumerator<int> GetEnumerator()
        {
            foreach (int act in action)
                yield return act;
        }
        private static Action[] GetActionSpace()
        {
            List<Action> actions = new List<Action>();
            for (int topLeft = 0; topLeft < size; topLeft++)
                foreach (ActionType act in Enum.GetValues(typeof(ActionType)))
                    foreach (RotationType rot in Enum.GetValues(typeof(RotationType)))
                        foreach (FlipType flip in Enum.GetValues(typeof(FlipType)))
                        {
                            if (act == ActionType.I)
                            {
                                if (flip == FlipType.Flipped)
                                    continue;
                                if (rot == RotationType.Half || rot == RotationType.ThreeQuarters)
                                    continue; // These are just duplicates, as actions I & S have rotational symetry order 2.
                            }
                            else if (act == ActionType.S)
                            {
                                if (rot == RotationType.Half || rot == RotationType.ThreeQuarters)
                                    continue; // These are just duplicates, as actions I & S have rotational symetry order 2.
                            }
                            else if (act == ActionType.T)
                            {
                                if(flip == FlipType.Flipped)
                                    continue; // These are just duplicates, as Flipping T is the same as rotating by 180.
                            }

                            Action action = new Action(topLeft, act, rot, flip);
                            if (action.IsValid())
                            {
                                action.ShiftActionToTopLeft();
                                actions.Add(action);
                            }
                            else
                                id--;
                        }
            return actions.ToArray();
        } // Sets the whole action space
        public static IEnumerable<Action> GetActions() 
        {
            foreach (Action a in actionSpace)
                yield return a;
        }
        public static Action GetAction(int index) 
        {
            return actionSpace[index];
        }

        private void ShiftActionToTopLeft()
        {
            action = actionPreShift;
            for (int i = 0; i < action.Length; i++)
                action[i] += topLeft;
        } // Called when the action space is set. No need to worry about outside of this class.
        private bool IsValid()
        {
            int[] action = actionPreShift;
            for (int i = 0; i < action.Length; i++)
            {
                // action[i] + topLeft  (Position of new tile about to be placed / checked if valid)

                //Check tile doesn't fall off board
                if (action[i] + topLeft >= size)
                    return false;
                //Check tile doesn't wrap around the board ???
                if (((action[i] + topLeft) % 10) - (action[i] % 10) < 0)
                    return false;
            }
            return true;
        } // Called when the action space is set. No need to worry about outside of this class.
    }
}
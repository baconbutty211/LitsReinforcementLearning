﻿using System;
using System.Collections.Generic;
using System.Linq;
using Python.Runtime;
using Python;
using Keras;
using Numpy;

namespace LitsReinforcementLearning
{
    public class Environment
    {
        public enum End { None, XWin, Draw, OWin }
        public enum Tile { _, O, X, L, I, T, S }
        public enum Board { Diagonals, Stripes, Boxes, Monogram, Random, Custom }
        public const int size = 100;
        private Random rnd = new Random();

        private bool calculatedValidActions = true;
        public Action[] validActions; // This is updated after every step.
        public int stepCount { get; private set; }
        public bool isDone { get { return validActions.Length == 0; } }
        private End Result
        {
            get
            {
                if (!isDone)
                    return End.None;

                if (xFilled > oFilled)
                    return End.XWin;
                else if (xFilled < oFilled)
                    return End.OWin;
                else
                    return End.Draw;
            }
        }

        private Dictionary<Tile, int> availableActions;

        public int Id = -1;
        private static int IdMaker = 0;
        static bool[] initialState
        {
            get
            {
                bool[] state = new bool[size];
                for (int i = 0; i < state.Length; i++)
                    state[i] = false;
                return state;
            }
        }
        private bool[] state;
        private static Tile[] SetBoard(Board boardType)
        {
            Tile[] board = new Tile[size];
            string strBoard = "";
            switch (boardType)
            {
                case Board.Diagonals:
                    strBoard = "0222001010102220010111022100101110211001011101110000222022202002210222020021102220200111020202001110";
                    break;
                case Board.Stripes:
                    strBoard = "2222211111000000000022222111110000000000222221111111111222220000000000111112222200000000001111122222";
                    break;
                case Board.Boxes:
                    strBoard = "2221112221100000000110212121011010000202202012010220102102022020000101101212120110000000011222111222";
                    break;
                case Board.Monogram:
                    strBoard = "1111011110100021222210002120021011212002100121110210222120021001212202100121000211112100020222202222";
                    break;
                case Board.Random:
                    Random rnd = new Random();
                    int Xs = 0;
                    int Os = 0;
                    for (int i = 0; i < size; i++)
                    {
                        int tile = 0;
                        do 
                        {
                            tile = rnd.Next(0, 3); 
                        } while ((Xs < 30 && tile == 1) || (Os < 30 && tile == 2));

                        strBoard += tile.ToString();
                    }
                    break;
                default:
                    throw new NotImplementedException($"Case block for board type {boardType} has not been implemented.");
            }
            for (int i = 0; i < size; i++)
                board[i] = (Tile)int.Parse(strBoard[i].ToString());
            return board;
        } //Define initial board
        static Tile[] initialBoard = SetBoard(Board.Stripes);
        private Tile[] board;
        public event System.Action<Tile[]> boardChanged;

        public int xFilled = 0;
        public int oFilled = 0;
        private int _Filled = 0;

        public float[] features;
        //public NDarray features;
        
        /// <summary>
        /// Initializes the environment
        /// </summary>
        public Environment()
        {
            Id = IdMaker++;
            Reset();
        }
        /// <summary>
        /// Only to be used for cloning
        /// </summary>
        private Environment(Environment original)
        {
            Id = IdMaker++;

            stepCount = original.stepCount;

            board = original.board.Clone() as Tile[];
            state = original.state.Clone() as bool[];

            calculatedValidActions = original.calculatedValidActions;
            validActions = original.validActions.Clone() as Action[];
            availableActions = original.availableActions.ToDictionary(entry => entry.Key, entry => entry.Value);

            features = original.features.Clone() as float[];
            //features = np.array(featuresArr);

            xFilled = original.xFilled;
            oFilled = original.oFilled;
            _Filled = original._Filled;
        }

        public Observation Reset()
        {
            stepCount = 0;
            availableActions = new Dictionary<Tile, int>() { { Tile.L, 5 }, { Tile.I, 5 }, { Tile.T, 5 }, { Tile.S, 5 } };

            board = initialBoard.Clone() as Tile[];

            xFilled = 0;
            oFilled = 0;
            _Filled = 0;

            state = initialState;

            calculatedValidActions = true;
            validActions = Action.GetActions().ToArray();

            features = Features();
            //features = np.array(featuresArr);

            boardChanged?.Invoke(board);
            
            return new Observation(-1, 0, false);
        }
        public Observation Step(Action action, bool calculateValidActions = true)
        {
            //if (!calculatedValidActions)
            //    throw new AccessViolationException($"On the previous step calculateValidActions was set to {calculatedValidActions}. You should not stepping through the environment because not all valid actions have been calculated. (Consider calculating valid actions here.)");

            foreach (int pos in action.action)
            {
                switch (board[pos])
                {
                    case Tile.O:
                        oFilled += 1;
                        break;
                    case Tile.X:
                        xFilled += 1;
                        break;
                    default: // Tile is Empty (_) or filled with (L, I, T, S)
                        _Filled += 1;
                        break;
                } // Increments X/O/_ filled counts

                if (!state[pos])
                {
                    board[pos] = ActionTypeToTile(action.type); // Set board position to the new action type
                    state[pos] = true;
                }
                else
                {
                    if (IsValid(action))
                        throw new Exception("IsValid() logic is wrong.");
                    else
                        throw new IndexOutOfRangeException($"Action {action} has already been taken.");
                }
            }// Applies action to the environment
            availableActions[ActionTypeToTile(action.type)]--;
            stepCount++;
            
            List<Action> validActions = new List<Action>();
            foreach (Action a in Action.GetActions())
            {
                if (IsValid(a))
                {
                    validActions.Add(a);
                    if (!calculateValidActions)
                        break;
                }
            } // Calculates the valid actions that can be applied.
            this.validActions = validActions.ToArray();
            this.calculatedValidActions = calculateValidActions;

            features = Features(); // Set features
            //features = np.array(features);

            boardChanged?.Invoke(board);
            return new Observation(action.Id, Reward(), isDone);
        }
        private float Reward()
        {
            float reward = xFilled - oFilled;
            if (isDone) // Adds a reward for the end state of the game
            {
                switch (Result)
                {
                    case End.XWin:
                        reward += 100;
                        break;
                    case End.OWin:
                        reward += -100;
                        break;
                    case End.Draw:
                        reward += 0;
                        break;
                    default:
                        break;
                }
            }
            return reward;
        }
        private float[] Features() 
        {
            List<float> featsLst = new List<float>();
            foreach (Tile tile in board)
            {
                switch (tile)
                {
                    case Tile.X:
                        featsLst.Add(1);
                        break;
                    case Tile.O:
                        featsLst.Add(-1);
                        break;
                    default:
                        featsLst.Add(0);
                        break;
                }
            } // Sets features
            return featsLst.ToArray();
        }

        public string GetResult()
        {
            switch (Result)
            {
                case End.XWin:
                    return $"X wins. \nScore is X:{xFilled} > O:{oFilled}";
                case End.OWin:
                    return $"O wins. \nScore is X:{xFilled} < O:{oFilled}";
                case End.Draw:
                    return $"{(stepCount % 2 == 0 ? "X loses" : "O loses")}. \nScore is X:{xFilled} = O:{oFilled}";
                default:
                    throw new NotImplementedException($"No case statement for {Result}");
            }
        }

        #region Validation
        private bool IsValid(Action action)
        {
            if (stepCount == 0) //All actions are valid on the first move
                return true;

            if (!IsAdjacentToOtherAction(action)) // Check if the new piece is adjacent to another piece of the same type;
                return false;
            if (IsOverlapingOtherAction(action)) //Check if a tile has already been placed in that position
                return false;
            if (IsAdjacentToSameActionType(action)) // Check if the new piece is adjacent to another piece
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
                    if (state[adjPos]) // Check if the new piece is adjacent to another piece
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
                if (i >= 89 || i%10 == 9) // 2*2 Area will be out of range of the board
                    continue;
                else if (stateClone[i] && stateClone[i+1] && stateClone[i+10] && stateClone[i+11]) // Checks if a 2*2 area on the board is filled
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
            return validActions[rnd.Next(validActions.Length)];
        }
        public bool ContainsValidAction(int actionId) 
        {
            foreach (Action action in validActions)
                if (action.Id == actionId)
                    return true;
            return false;
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
        private bool TileIsEmpty(int pos) { return (int)board[pos] < 3; }
        public override string ToString()
        {
            string boardStr = "  ";
            for (int i = 0; i < 10; i++)
                boardStr += $"{i},";
            for (int i = 0; i < size; i++)
            {
                if (i % 10 == 0)
                    boardStr += $"\n{i / 10} ";
                //if (state[i])
                //    boardStr += $"#,";
                //else
                    boardStr += $"{board[i]},";
            }
            return boardStr + '\n';
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
                    boardStr += $"{initialBoard[i]},";
            }
            return boardStr + '\n';
        }
    
        public Environment Clone() 
        {
            return new Environment(this);
        }
}

public struct Observation
    {
        public int previousActionId { get; private set; }
        public float reward { get; private set; }
        public bool isDone { get; private set; }

        public Observation(int prevActionId, float reward, bool isDone)
        {
            this.previousActionId = prevActionId;
            this.reward = reward;
            this.isDone = isDone;
        }
        /// <summary>
        /// Used only for cloning
        /// </summary>
        private Observation(Observation observation) 
        {
            this.previousActionId = observation.previousActionId;
            this.reward = observation.reward;
            this.isDone = observation.isDone;
        }

        public void SetCustomReward(float newReward) 
        {
            this.reward = newReward;
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

        #region Front-End methods
        public bool Equals(int topLeft, ActionType type, RotationType rotation, FlipType flip)
        {
            return (topLeft == this.topLeft) && (type == this.type) && (rotation == this.rotation) && (flip == this.flip);
        }
        public bool Equals(int[] action)
        {
            int[] sortArr1 = Sort(action);
            int[] sortArr2 = Sort(this.action);
            return (sortArr1[0] == sortArr2[0]) && (sortArr1[1] == sortArr2[1]) && (sortArr1[2] == sortArr2[2]) && (sortArr1[3] == sortArr2[3]);
        }
        private int[] Sort(int[] array)
        {
            int[] sorted = array.Clone() as int[];
            Array.Sort(sorted);
            return sorted;
        }
        public static string GetString(ActionType type, RotationType rotation = RotationType.None, FlipType flip = FlipType.None, int topLeft = 0)
        {
            int[] action;
            try
            {
                action = new Action(topLeft, type, rotation, flip).actionPreShift;
            }
            catch (NotImplementedException)
            {
                return "Action not available.";
            }
            char[,] actionStr = new char[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    actionStr[i, j] = ' ';

            foreach (int act in action)
                actionStr[act / 10, act % 10] = '#';

            string str = "";
            for (int i = 0; i < 4; i++, str += '\n')
                for (int j = 0; j < 4; j++)
                    str += actionStr[i, j];
            return str;
        }
        #endregion
        public override string ToString()
        {
            return $"{Id}) {topLeft}, {type}, {rotation}, {flip}";
        }
    }
}
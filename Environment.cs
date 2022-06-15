using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitsEnvironment
{
    public interface IEnvironment
    {
        public Observation Step(Action action);
        public void Reset();
    }
    public class Environment
    {
        public const int Size = 100;
        protected int[] initialState;
        protected int[] state;
        protected int stepCount;
        protected bool isDone;

        public class EnvironmentStepWhenDoneException : Exception
        {
            
        }
    }
    public struct Observation
    {
        public int[] state;
        public float reward;
        public bool isDone;

        public Observation(int[] state, float reward, bool isDone)
        {
            this.state = state;
            this.reward = reward;
            this.isDone = isDone;
        }
    }
    //public class State
    //{
    //    public const int stateSize = 100;
    //    public static State Initial = new State();

    //    public int Count { get { return state.Length; } }
    //    public int[] state = new int[stateSize];
    //    private State()
    //    {
    //        for (int i = 0; i < stateSize; i++)
    //            state[i] = 0;
    //    }

    //    /// <summary>
    //    /// Only used for copying/cloning
    //    /// </summary>
    //    /// <param name="other"> The original object that's being copied/cloned. </param>
    //    private State(State other) 
    //    {
    //        this.state = other.state.Clone() as int[];
    //    }
    //    public State Clone() 
    //    {
    //        return new State(this);
    //    }

    //    /// <summary>
    //    /// Applies action to the state. Checks that the action doesn't overlap with any filled tiles.
    //    /// Reward:
    //    ///     +1 for X tiles
    //    ///     -1 for O tiles
    //    /// Done: 
    //    ///     true if no X tiles
    //    /// </summary>
    //    /// <param name="action"></param>
    //    /// <returns>An observation of the new state. The reward gained. Whether the environment is done.</returns>
    //    public Observation Apply(Action action) 
    //    {
    //        if (Count != action.Count)
    //            throw new Action.ActionNotValidException($"Action size and state size is mismatch {Count} != {action.Count}");
    //        int reward = -1;
    //        State clone = Clone();
    //        for (int i = 0; i < Count; i++)
    //        {
    //            if (action.action[i])
    //            {
    //                if (state[i] == (int)TileType.X)
    //                    reward += 1;
    //                else if (state[i] == (int)TileType.O)
    //                    reward -= 1;
    //                else if (state[i] == (int)TileType.Filled)
    //                    throw new Action.ActionNotValidException($"Action cannot lay tile over an already filled tile (index = {i}). State {state}. Action {action}");
    //                clone.state[i] = (int)TileType.Filled;
    //            }
    //        }
    //        bool done = clone.state.Contains((int)TileType.X);
    //        return new Observation(clone, reward, done);
    //    }
    //}
    public class Action : IEnumerable<int>
    {
        public ActionType type;
        private RotationType rotation; //Rotation is applied anti-clockwise
        int[] action;
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
        public Action(ActionType type, RotationType rotation, int topLeft)
        {
            this.type = type;
            this.rotation = rotation;
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
                            action = new int[] { 2, 1, 0, 10, 20};
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


            IsValid(ref action, topLeft);
        }
        private void IsValid(ref int[] action, int topLeft)  
        {
            for (int i = 0; i < action.Length; i++) 
            {
                //Check tile doesn't fall off board
                if (action[i] + topLeft >= Environment.Size)
                    throw new IndexOutOfRangeException($"Cannot create an action that will place a tile outside the board.");
                //Check tile doesn't wrap around the board
                if (((action[i] + topLeft) % 10) - (action[i] % 10) < 0)
                    throw new ActionNotValidException($"Cannot create an action with tiles {action} and offset (top left) {topLeft}. Specifically {i} -> {i + topLeft}");

                //Add top left to tile
                action[i] += topLeft;
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (int pos in action)
                yield return pos;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public class ActionNotValidException : Exception
        {
            public ActionNotValidException(string msg) : base(msg) { }
            public ActionNotValidException() : base() { }
            public ActionNotValidException(Action action, int[] state)
            {
                new ActionNotValidException($"Cannot apply action {action} to environment in state {state}");
            }
        }
    }
}

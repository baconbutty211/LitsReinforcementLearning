using System;

namespace LitsEnvironment
{
    public enum TileType 
    { 
        Empty = 0, 
        X = 1, 
        O = 2,
        Filled = 4,
    }
    //public struct Tile 
    //{
    //    public TileType type;

    //    public Tile(int type) 
    //    {
    //        this.type = (TileType)type;
    //    }
    //    public Tile(TileType type) 
    //    {
    //        this.type = type;
    //    }

    //    public char ToChar() 
    //    {
    //        switch (type)
    //        {
    //            case TileType.Empty:
    //                return '_';
    //            case TileType.Filled:
    //                return '#';
    //            case TileType.X:
    //                return 'X';
    //            case TileType.O:
    //                return 'O';
    //            default:
    //                throw new NotImplementedException($"No case block for type {type}.");
    //        }
    //    }
    //    public override string ToString()
    //    {
    //        return ToChar().ToString();
    //    }
    //}

    public class Lits : Environment, IEnvironment
    {
        public Lits() 
        {
            for (int i = 0; i < Size; i++)
            {
                if (i / 10 == 0 || i / 10 == 2 || i / 10 == 4)
                    if (i % 10 <= 4)
                        initialState[i] = (int)TileType.X;
                    else
                        initialState[i] = (int)TileType.O;
                else if (i / 10 == 5 || i / 10 == 7 || i / 10 == 7)
                    if (i % 10 <= 4)
                        initialState[i] = (int)TileType.O;
                    else
                        initialState[i] = (int)TileType.X;
                else
                    initialState[i] = (int)TileType.Empty;
            } // Sets initial state of the board
            Reset();
        }

        public void Reset()
        {
            state = initialState.Clone() as int[];
            isDone = false;
            stepCount = 0;
        }

        
        public Observation Step(Action action)
        {
            if (isDone)
                throw new EnvironmentStepWhenDoneException();

            float reward = -1;
            int[] newState = state.Clone() as int[];
            foreach(int pos in action) // foreach tile position to be placed in the action.
            {
                if (state[pos] == (int)TileType.X) // Good result,
                    reward += 1;                   // Increment reward
                else if (state[pos] == (int)TileType.O) // Bad result,
                    reward -= 1;                        // Decrement reward

                if (IsFilled(newState[pos])) // This position has already had a tile
                    throw new Action.ActionNotValidException($"Action cannot lay tile over an already filled tile (index = {pos}). State {state}. Action {action}");
                else                         // Apply action
                    newState[pos] = (int)action.type;
            }
            isDone = newState.GetValue((int)TileType.X) == null;

            IsValid(newState, state, action);
            state = newState.Clone() as int[];
            stepCount++;
            return new Observation(newState, reward, isDone);
        }
        private void IsValid(int[] newState, int[] oldState, Action action)
        {
            // Checks for 2*2 filled
            // i #
            // # #
            int leng = newState.Length;          // 100
            int sqrtLeng = (int)Math.Sqrt(leng); // 10
            for(int i = 0; i < newState.Length; i++)
                if (i + 1 >= sqrtLeng)
                    continue;
                else if (i + sqrtLeng >= leng)
                    continue;
                else
                    if (IsFilled(newState[i]) || IsFilled(i + 1) || IsFilled(newState[i + sqrtLeng]) || IsFilled(i + sqrtLeng + 1)) //Checks for 2*2 filled
                        throw new Action.ActionNotValidException($"Creates a filled 2*2. (Top left is {i})");
            
            //Checks if the new tile/action shares an edge with another tile/action of the same type.
            if (SharesEdge(state, action))
                throw new Action.ActionNotValidException($"New tiles share edge with congruent tiles.");    
        }
        private bool IsFilled(int tile)
        {
            return (tile & (int)TileType.Filled) == (int)TileType.Filled;
        }
        private bool SharesEdge(int[] currState, Action action)
        {
            foreach (int pos in action)
            {
                try
                {
                    if (currState[pos - 10] == (int)action.type)
                        return true;
                }
                catch (IndexOutOfRangeException) { }
                try
                {
                    if (currState[pos - 1] == (int)action.type)
                        return true;
                }
                catch (IndexOutOfRangeException) { }
                try
                {
                    if (currState[pos + 1] == (int)action.type)
                        return true;
                }
                catch (IndexOutOfRangeException) { }
                try
                {
                    if (currState[pos + 10] == (int)action.type)
                        return true;
                }
                catch (IndexOutOfRangeException) { }
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Numpy;

namespace LitsReinforcementLearning
{
    public class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        public bool isFirstPlayer;

        public Agent(bool isFirstPlayer) 
        {
            this.isFirstPlayer = isFirstPlayer;
        } // Creates a new agent.
        public Agent(string agentName) 
        {
            Load(agentName);
        } // Loads an agent from a save file.
        
        #region Save/Load
        protected virtual void Load(string agentName)
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";

            isFirstPlayer = bool.Parse(File.ReadAllText($"{path}{Path.Slash}IsFirstPlayer.txt"));
        }
        public virtual void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";

            File.WriteAllText($"{path}{Path.Slash}IsFirstPlayer.txt", isFirstPlayer.ToString());
        }
        #endregion


        public void Explore(Environment env, Verbosity verbosity = Verbosity.High) 
        {
            
        }
        /// <summary>
        /// Evaluates the current state of the environment/board.
        /// </summary>
        /// <returns>The best valid action according to the neural network</returns>
        public Action Exploit(Environment env)
        {
            throw new NotImplementedException();
        }

        #region Helpers
        protected float MaxValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float maxVal = float.MinValue;
            for (int i = 0; i < env.validActions.Length; i++)
                if (values[i] > maxVal)
                    maxVal = values[i];
            return maxVal;
        }
        protected int MaxValidActionId(Environment env, float[] values)
        {
            return MaxValidAction(env, values).Id;
        }
        protected Action MaxValidAction(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            Action maxAction = null;
            float maxVal = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > maxVal)
                {
                    maxVal = values[i];
                    maxAction = env.validActions[i];
                }
            }
            return maxAction;
        }

        protected float MinValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float minVal = float.MaxValue;
            for (int i = 0; i < env.validActions.Length; i++)
                if (values[i] < minVal)
                    minVal = values[i];
            return minVal;
        }
        protected int MinValidActionId(Environment env, float[] values)
        {
            return MinValidAction(env, values).Id;
        }
        protected Action MinValidAction(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            Action minAction = null;
            float minVal = float.MaxValue;
            for (int i = 0; i < env.validActions.Length; i++)
            {
                if (values[i] < minVal)
                {
                    minVal = values[i];
                    minAction = env.validActions[i];
                }
            }
            return minAction;
        }


        protected int Argmax(float[] values) 
        {
            if (values.Length == 0)
                return -1;

            int maxArg = 0;
            for (int i = 0; i < values.Length; i++)
                if (values[i] >= values[maxArg])
                    maxArg = i;
            return maxArg;
        }
        protected int Argmin(float[] values) 
        {
            if (values.Length == 0)
                return -1;

            int minArg = 0;
            for (int i = 0; i < values.Length; i++)
                if (values[i] <= values[minArg])
                    minArg = i;
            return minArg;
        }

        /// <summary>
        /// The action values -> actionId mapping unwrapped looks like { Id1 : val1, val2, val3, ..., valN }, { Id2 : valN+1, valN+2, valN+3, ..., valN+M }, ...
        /// The action values -> actionId mapping wrapped (the way it is input in this function) looks like { Id1 : 0 }, { Id2: N }, { Id3 : M }, ...
        /// This function takes an index (example: N+2) and maps N+2 -> valN+2 as follows:
        ///     is (index == N+2) >= 0:  YES
        ///         actionId = Id1; 
        ///         index -= N;
        ///     is (index == 2) >= 0:  YES
        ///         actionId = Id2;
        ///         index -= M
        ///     is 2-M >= 0:  NO
        ///         return (actionId == Id2)
        ///     
        ///     So returns the Id where the value N+2 belongs (can be seen in the unwrapped data)
        /// </summary>
        /// <returns>
        /// ActionId at the given index
        /// </returns>
        protected int IndexToActionId(int index, List<int> indices, List<int> actionIds)
        {
            int actionId = -1;
            int i = 0;
            while(index >= 0)
            {
                actionId = actionIds[i];
                index -= indices[i];
                i++;
            }
            return actionId;
        }
        #endregion
    }
}

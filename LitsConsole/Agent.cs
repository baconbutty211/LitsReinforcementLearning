using System;
using System.IO;
using Numpy;

namespace LitsReinforcementLearning
{
    //public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public abstract class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        public bool isFirstPlayer;
        protected KerasNet model;

        public Agent(bool isFirstPlayer, int inputSize, int outputSize, params int[] hiddenSizes) 
        {
            this.isFirstPlayer = isFirstPlayer;
            model = new KerasNet(inputSize, outputSize, hiddenSizes);  // Initializes new neural network
        } // Creates a new agent.
        public Agent(string agentName) 
        {
            Load(agentName);
        } // Loads an agent from a save file.
        
        #region Save/Load
        protected virtual void Load(string agentName)
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model = KerasNet.Load(path);

            isFirstPlayer = bool.Parse(File.ReadAllText($"{path}{Path.Slash}IsFirstPlayer.txt"));
        }
        public virtual void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model.Save(path);

            File.WriteAllText($"{path}{Path.Slash}IsFirstPlayer.txt", isFirstPlayer.ToString());
        }
        #endregion

        /// <summary>
        /// Trains the neural network model on the current state and best future state.
        /// </summary>
        public abstract void Explore(Environment env, Verbosity verbosity = Verbosity.High);
        /// <summary>
        /// Evaluates the current state of the environment/board.
        /// </summary>
        /// <returns>The best valid action according to the neural network</returns>
        public abstract Action Exploit(Environment env);

        #region Helpers
        protected float MaxValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float maxVal = float.MinValue;
            foreach (Action action in env.validActions)
                if (values[action.Id] > maxVal)
                    maxVal = values[action.Id];
            return maxVal;
        }
        protected int MaxValidActionId(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            int maxId = -1;
            float maxVal = float.MinValue;
            foreach (Action action in env.validActions)
                if (values[action.Id] > maxVal)
                {
                    maxVal = values[action.Id];
                    maxId = action.Id;
                }
            return maxId;
        }
        protected float MinValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float minVal = float.MaxValue;
            foreach (Action action in env.validActions)
                if (values[action.Id] < minVal)
                    minVal = values[action.Id];
            return minVal;
        }
        protected int MinValidActionId(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            int minId = -1;
            float minVal = float.MaxValue;
            foreach (Action action in env.validActions)
                if (values[action.Id] < minVal)
                {
                    minVal = values[action.Id];
                    minId = action.Id;
                }
            return minId;
        }
        #endregion
    }



    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public class DynamicProgrammingAgent : Agent
    {
        public DynamicProgrammingAgent(int inputSize, bool isFirstPlayer) : base(isFirstPlayer, inputSize, Action.actionSpaceSize)
        {

        }
        public DynamicProgrammingAgent(string agentName) : base(agentName)
        {

        }

        public override void Explore(Environment env, Verbosity verbosity = Verbosity.High)
        {
            bool isFirstPlayer = env.stepCount % 2 == 0;

            //float bestFutureVal = float.MinValue;
            float[] actionValueArr = new float[Action.actionSpaceSize];
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);

                NDarray futureValues = model.Predict(future.features);
                float futureValue = isFirstPlayer ? MaxValidActionValue(env, futureValues.GetData<float>()) : MinValidActionValue(env, futureValues.GetData<float>());
                futureValue *= discount;
                futureValue += isFirstPlayer ? obs.reward : -obs.reward;
                actionValueArr[action.Id] = futureValue;

                //if (isFirstPlayer ? futureValue >= bestFutureVal : futureValue <= bestFutureVal)
                //    bestFutureVal = futureValue;
            }

            NDarray truth = np.array(actionValueArr);
            model.Train(env.features, truth, verbosity);
        }
        public override Action Exploit(Environment env)
        {
            bool isFirstPlayer = env.stepCount % 2 == 0;

            NDarray values = model.Predict(env.features);
            int actionId = isFirstPlayer ? MaxValidActionId(env, values.GetData<float>()) : MinValidActionId(env, values.GetData<float>());
            return Action.GetAction(actionId);
        }
    }
}

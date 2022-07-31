﻿using System;
using System.IO;
using Numpy;

namespace LitsReinforcementLearning
{
    public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public partial class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        private AgentType type;
        public bool isFirstPlayer;
        KerasNet model;

        public Agent(AgentType type, int inputSize, bool isFirstPlayer) 
        {
            this.type = type;
            this.isFirstPlayer = isFirstPlayer;
            model = new KerasNet(inputSize, Action.actionSpaceSize, 10);  // Initializes new neural network
        } // Creates a new agent.
        public Agent(AgentType type, string agentName) 
        {
            this.type = type;
            Load(agentName);
        } // Loads an agent from a save file.
        
        #region Save/Load
        private void Load(string agentName)
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model = KerasNet.Load(path);

            this.isFirstPlayer = bool.Parse(File.ReadAllText($"{path}{Path.Slash}IsFirstPlayer.txt"));
            this.type = (AgentType)int.Parse(File.ReadAllText($"{path}{Path.Slash}AgentType.txt"));
        }
        public void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model.Save(path);

            File.WriteAllText($"{path}{Path.Slash}IsFirstPlayer.txt", isFirstPlayer.ToString());
            File.WriteAllText($"{path}{Path.Slash}AgentType.txt", ((int)type).ToString());
        }
        #endregion

        /// <summary>
        /// Trains the neural network model on the current state and best future state.
        /// </summary>
        public void Explore(Environment env, Agent enemy, Verbosity verbosity = Verbosity.High) 
        {
            switch (type)
            {
                case AgentType.Abstract:
                    break;

                case AgentType.MonteCarlo:
                    throw new NotImplementedException();

                case AgentType.DynamicProgramming:
                    ExploreDynamicProgramming(env, enemy, verbosity);
                    break;

                case AgentType.ExhaustiveSearch:
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// Evaluates the current state of the environment/board.
        /// </summary>
        /// <returns>The best valid action according to the neural network</returns>
        public Action Exploit(Environment env) 
        {
            NDarray values = model.Predict(env.features);
            int actionId = MaxValidActionId(env, values.GetData<float>());
            return Action.GetAction(actionId);
        }

        #region Helpers
        private float MaxValidActionValue(Environment env, float[] values)
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
        private int MaxValidActionId(Environment env, float[] values)
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
        private float MinValidActionValue(Environment env, float[] values)
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
        private int MinValidActionId(Environment env, float[] values)
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
    public partial class Agent
    {
        private void ExploreDynamicProgramming(Environment env, Agent enemy, Verbosity verbosity = Verbosity.High)
        {
            int bestChildId = -1;
            float bestChildVal = float.MinValue;
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);

                NDarray futureValues = model.Predict(future.features);
                float futureValue = MaxValidActionValue(env, futureValues.GetData<float>());
                futureValue *= discount;
                futureValue += isFirstPlayer ? obs.reward : -obs.reward;

                if (futureValue >= bestChildVal)
                {
                    bestChildId = action.Id;
                    bestChildVal = futureValue;
                }
            }

            float[] bestActionArr = new float[Action.actionSpaceSize];
            for (int i = 0; i < Action.actionSpaceSize; i++)
                bestActionArr[i] = i == bestChildId ? 1 : 0;
            NDarray truth = np.array(bestActionArr);
            model.Train(env.features, truth, verbosity);
        }
    }
}

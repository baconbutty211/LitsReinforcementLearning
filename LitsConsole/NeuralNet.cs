using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    /// <summary>
    /// All the ML stuff
    /// </summary>
    public class NeuralNet
    {
        private const float learnRate = 0.1f;

        static Normal normal = new Normal(0, 1);

        float fitness;

        //Weights
        Matrix<float> W1;
        Matrix<float> W2;

        Vector<float> Z1;
        Vector<float> A1; //Output
        Vector<float> Z2;
        Vector<float> A2; //Output

        //Errors
        Matrix<float> dZ1;
        Matrix<float> dW1;
        Matrix<float> dZ2;
        Matrix<float> dW2;

        public NeuralNet(int featuresSize)
        {
            Initilize(featuresSize);
        }

        private void Initilize(int featuresSize)
        {
            int inputSize = featuresSize;
            int outputSize = Action.actionSpaceSize;
            W1 = Matrix<float>.Build.Random(outputSize, inputSize, normal);   // Weights (inputSize, outputSize, distribution)
        }

        /// <summary>
        /// Forward propagates the feature vector through a neural network.
        /// </summary>
        /// <param name="A0">Features vector</param>
        /// <returns>The value of each action. Choose the action with the highest value, that is still valid</returns>
        public Vector<float> Evaluate(Vector<float> A0)
        {
            //Forward propagate
            Z1 = W1 * A0;
            A1 = Sigmoid(Z1);
            //A1 = Softmax(Z1);

            if (float.IsNaN(A1[0]))
                Log.Rotate();
            return A1;

            Z2 = W2 * A1;
            A2 = Softmax(Z2);
            return A2;
        }
        /// <summary>
        /// Backward propagates the errors, to narrow the neural network weights towards optimal values.
        /// </summary>
        public void Update(float reward, Vector<float> childValues, Vector<float> currentValues, Vector<float> features)
        {
            Z1 = (Vector<float>.Build.Dense(Action.actionSpaceSize, reward) + (Agent.discount * childValues)) - currentValues; // Errors

            //Calculate output layer errors
            dW1 = Z1.ToColumnMatrix() * features.ToRowMatrix();

            // Learn
            W1 += learnRate * dW1;
            if (float.IsNaN(W1[0, 0]))
                Log.Rotate();
        }
        #region Helpers
        public static Matrix<float> Sigmoid(Matrix<float> input)
        {
            Matrix<float> output = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

            for (int i = 0; i < input.RowCount; i++)
                for (int j = 0; j < input.ColumnCount; j++)
                    output[i, j] = Sigmoid(input[i, j]);
            return output;
        }
        public static Vector<float> Sigmoid(Vector<float> input)
        {
            Vector<float> output = Vector<float>.Build.Dense(input.Count);
            for (int i = 0; i < input.Count; i++)
                output[i] = Sigmoid(input[i]);
            return output;
        }
        private static float Sigmoid(float input)
        {
            return 1 / (1 + MathF.Exp(-input));
        }

        public static Matrix<float> SigmoidPrime(Matrix<float> input)
        {
            Matrix<float> output = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

            for (int i = 0; i < input.RowCount; i++)
                for (int j = 0; j < input.ColumnCount; j++)
                    output[i, j] = SigmoidPrime(input[i, j]);
            return output;
        }
        public static Vector<float> SigmoidPrime(Vector<float> input)
        {
            Vector<float> output = Vector<float>.Build.Dense(input.Count);
            for (int i = 0; i < input.Count; i++)
                output[i] = SigmoidPrime(input[i]);
            return output;
        }
        private static float SigmoidPrime(float input)
        {
            return Sigmoid(input) * (1 - Sigmoid(input));
        }

        public static Matrix<float> ReLU(Matrix<float> input)
        {
            Matrix<float> output = input.Clone();

            output.PointwiseMaximum(0, input);
            output.PointwiseMinimum(1, input);
            return output;
        }
        public static Matrix<float> ReLUPrime(Matrix<float> input)
        {
            Matrix<float> output = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

            for (int i = 0; i < input.RowCount; i++)
                for (int j = 0; j < input.ColumnCount; j++)
                    if (input[i, j] <= 0)
                        output[i, j] = 0;
                    else
                        output[i, j] = 1;
            return output;
        }

        public static Matrix<float> Softmax(Matrix<float> input)
        {
            Matrix<float> output = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);
            
            for (int j = 0; j < input.ColumnCount; j++)
            {
                float sum = 0;
                for (int i = 0; i < input.RowCount; i++)
                    sum += (float)Math.Exp(input[i, j]);

                for (int i = 0; i < input.RowCount; i++)
                    output[i, j] = (float)Math.Exp(input[i, j]) / sum;
            }
            return output;
        }
        public static Vector<float> Softmax(Vector<float> input)
        {
            Vector<float> output = Vector<float>.Build.Dense(input.Count);

            float sum = 0;
            for (int i = 0; i < input.Count; i++)
                sum += (float)Math.Exp(input[i]);

            for (int i = 0; i < input.Count; i++)
                output[i] = (float)Math.Exp(input[i]) / sum;
            return output;
        }

        public static int Argmax(Vector<float> input, int pos = 0)
        {
            if (pos > input.Count)
            {
                Log.RotateError();
                throw new IndexOutOfRangeException($"There are not {pos} elements to select the {pos}th highest value.");
            }

            Vector<float> temp = input.Clone();
            for (int i = 0; i < pos; i++)
                temp[temp.AbsoluteMaximumIndex()] = float.NaN;
            return temp.AbsoluteMaximumIndex();
        }
        public static float Max(Vector<float> input, int pos = 0)
        {
            return input[Argmax(input, pos)];
        }
        #endregion
    }
}

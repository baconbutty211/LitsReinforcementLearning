using Keras.Datasets;
using Keras.Optimizers;
using Keras.Layers;
using Keras.Models;
using Keras.Utils;
using Keras;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LitsReinforcementLearning
{
    class KerasNet
    {
        BaseModel model;
        public KerasNet(int inputSize, int outputSize) 
        {
            model = new Sequential();
            ((Sequential)model).Add(new Input(shape: new Shape(inputSize)));
            ((Sequential)model).Add(new Dense(units: 10, input_dim: inputSize, activation: "sigmoid"));
            ((Sequential)model).Add(new Dense(units: outputSize, input_dim: 10, activation: "sigmoid"));
            model.Compile(optimizer: new Adam(), loss: "mean_squared_error");
        }
        private KerasNet(string path)
        {
            model = Sequential.LoadModel($"{path}{Path.Slash}Model");
        }
        public void Train(NDarray input, NDarray truth) 
        {
            model.Fit(input.reshape(-1, input.len), truth.reshape(-1, truth.len));
        }
        public NDarray Predict(NDarray input)
        {
            return model.Predict(input.reshape(-1, input.len), verbose: 0);
        }

        #region Save/Load
        public void Save(string path) 
        {
            model.Save($"{path}{Path.Slash}Model");
        }
        public static KerasNet Load(string path) 
        {
            return new KerasNet(path);
        }
        #endregion
    }
}

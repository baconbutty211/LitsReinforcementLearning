using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LitsReinforcementLearning
{

    public class Vector
    {
        float[] values;
        public int Count { get { return values.Length; } }
        public float this[int index]
        {
            get { return values[index]; } 
            set { values[index] = value; }
        }

        public Vector(float[] values) 
        {
            this.values = values;
        }
        public Vector(List<float> values) 
        {
            this.values = values.ToArray();
        }
        public static Vector InitializeRandom(int size) 
        {
            Random rnd = new Random();
            float[] values = new float[size];
            for (int i = 0; i < size; i++)
                values[i] = Convert.ToSingle(rnd.NextDouble());
            return new Vector(values);
        }

        public static Vector operator -(Vector a, Vector b) 
        {
            Vector sum = a.Clone();
            for (int i = 0; i < sum.Count; i++)
                sum[i] = a[i] - b[i];
            return sum;
        }
        public static Vector operator +(Vector a, Vector b) 
        {
            Vector sum = a.Clone();
            for (int i = 0; i < sum.Count; i++)
                sum[i] = a[i] + b[i];
            return sum;
        }
        public static Vector operator *(float a, Vector b)
        {
            Vector product = b.Clone();
            for (int i = 0; i < b.Count; i++)
                product[i] *= a;
            return product;
        }

        public static float Dot(Vector a, Vector b)
        {
            float dotProduct = 0;
            for (int i = 0; i < a.values.Length; i++)
                dotProduct += a[i] * b[i];
            return dotProduct;
        }

        #region Save/Load
        public static void SaveJson(Vector v, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                v.WriteVector(writer);
                File.WriteAllText($"{path}{Path.Slash}Weights.json", sb.ToString());
            }
        }
        private void WriteVector(JsonWriter writer) 
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            writer.WritePropertyName("Values");
            writer.WriteStartArray();
            foreach(float val in values)
                writer.WriteValue(val);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
        public static Vector LoadJson(string path) 
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}Tree.json"))
                throw new FileNotFoundException();

            StreamReader sr = new StreamReader($"{path}{Path.Slash}Weights.json");
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                return ReadVector(reader);
            }
        }
        private static Vector ReadVector(JsonReader reader) 
        {
            List<float> values = new List<float>();

            reader.Read(); // Reads start object
            reader.Read(); // Reads values name
            reader.Read(); // Reads start array

            while (reader.Read() && reader.TokenType != JsonToken.EndArray) // Reads until end array
                values.Add(Convert.ToSingle(reader.Value));

            reader.Read(); // Reads end object
            return new Vector(values);
        }
        #endregion

        public Vector Clone()
        {
            return new Vector(values);
        }
        public IEnumerator<float> GetEnumerator() 
        {
            foreach (float val in values)
                yield return val;
        }
    }
}

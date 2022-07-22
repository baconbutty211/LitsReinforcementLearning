using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

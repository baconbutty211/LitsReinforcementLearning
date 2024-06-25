using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LitsReinforcementLearning
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class Tree
    {
        protected static int size = Action.actionSpaceSize;

        protected int depth;
        protected Observation root;
        protected Tree[] children;

        public virtual float Value
        {
            get
            {
                if (Empty)
                    return root.reward;
                else
                    return root.reward + (FavouriteChild.Value * Agent.discount);
            }
        }
        public Action PreviousAction { get { return Action.GetAction(root.previousActionId); } }
        public bool Leaf { get { return root.isDone; } }
        public bool Empty { get { return ChildCount == 0; } }
        public virtual int ChildCount
        {
            get
            {
                if (Leaf)
                    return 0;

                int count = 0;
                foreach (Tree child in children)
                    if (child != null)
                        count++;
                return count;
            }
        }
        public virtual Tree FavouriteChild
        {
            get
            {
                if (Leaf || Empty)
                    return null;
                float maxVal = float.MinValue;
                Tree favChild = null;
                foreach (Tree child in this)
                {
                    float childVal = child.Value;
                    if (childVal > maxVal)
                    {
                        maxVal = childVal;
                        favChild = child;
                    }
                }
                return favChild;
            }
        }
        public virtual Tree ProblemChild
        {
            get
            {
                if (Leaf || Empty)
                    return null;
                float minVal = float.MaxValue;
                Tree probChild = null;
                foreach (Tree child in this)
                {
                    float childVal = child.Value;
                    if (childVal < minVal)
                    {
                        minVal = childVal;
                        probChild = child;
                    }
                }
                return probChild;
            }
        }

        public Tree(Observation initialObservation)
        {
            depth = 0;
            root = initialObservation;
            children = new Tree[size];
        } // Initializes the tree trunk
        
        protected Tree() { children = new Tree[size]; } // Initializes an empty tree trunk. Used for reading from the json file
        protected Tree(Observation root, int depth)
        {
            this.depth = depth;
            this.root = root;
            if (!Leaf)
                children = new Tree[size];
        } // Constructs new branches
        
        public virtual Tree Branch(Observation observation)
        {
            if (Leaf)
            {
                string exception = "Tree is a leaf (state is done). Should not be adding any children here.";
                Log.Write(exception);
                Log.RotateError();
                //throw new Exception(exception); // Tree thinks the state isDone, when the environment disagrees. I suspect this is because on a previous episode the environment thought this state was done, but this time round the environment disagrees. Seems to be unpredictable.
                return null;
            }
            Tree child = new Tree(observation, depth + 1);
            return Branch(child);
        } // Agent calls this branch method
        protected virtual Tree Branch(Tree child)
        {
            int actionId = child.root.previousActionId;
            if (children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        } // Tree Loading & Agent calls this branch method

        public virtual IEnumerator<Tree> GetEnumerator()
        {
            foreach (Tree child in children)
                if (child != null)
                    yield return child;
        }

        #region Save/Load
        /// <summary>
        /// Only to be called at the trunk
        /// </summary>
        public static void SaveJson(Tree tree, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                tree.WriteTree(writer);
                File.WriteAllText($"{path}{Path.Slash}Tree.json", sb.ToString());
            }
        }
        public virtual void WriteTree(JsonWriter writer)
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            writer.WritePropertyName("depth");
            writer.WriteValue(depth);

            writer.WritePropertyName("previousActionId");
            writer.WriteValue(root.previousActionId);

            writer.WritePropertyName("done");
            writer.WriteValue(root.isDone);

            writer.WritePropertyName("reward");
            writer.WriteValue(root.reward);

            if ((Leaf || Empty))
                writer.WriteEndObject();
            else
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (Tree child in this)
                    child.WriteTree(writer);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
        }

        public static Tree LoadJson(string path)
        {
            if (!Directory.Exists(path))
            {
            	Console.WriteLine(path);
            	throw new DirectoryNotFoundException();
            }
            if (!File.Exists($"{path}{Path.Slash}Tree.json"))
                throw new FileNotFoundException();

            StreamReader sr = new StreamReader($"{path}{Path.Slash}Tree.json");
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                reader.Read(); // Reads start object
                return ReadTree(reader);
            }
        }
        public static Tree ReadTree(JsonReader reader)
        {
            Tree tree = new Tree();


            reader.Read(); // Reads depth name
            reader.Read(); // Reads depth value
            tree.depth = Convert.ToInt32(reader.Value);

            reader.Read(); // Reads previous action id name
            reader.Read(); // Reads previous action id value
            int prevActionId = Convert.ToInt32(reader.Value);

            reader.Read(); // Reads is done name
            reader.Read(); // Reads is done value
            bool done = (bool)reader.Value;

            reader.Read(); // Reads reward name
            reader.Read(); // Reads reward value
            float reward = Convert.ToSingle(reader.Value);

            tree.root = new Observation(prevActionId, reward, done);

            reader.Read(); // Reads children OR end of object if Empty
            if (reader.TokenType == JsonToken.EndObject)
                return tree;

            reader.Read(); // Reads start of array
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                tree.Branch(ReadTree(reader)); // Reads the child tree
            } // Reads until the end of the children array
            reader.Read(); // Reads end object
            
            if (tree.Leaf)
                tree.children = null;
            return tree;
        }
        #endregion
    }
}

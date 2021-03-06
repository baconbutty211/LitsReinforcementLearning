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
    public class Tree
    {
        protected static int size = Action.actionSpaceSize;
        protected static float Discount = 0.95f;

        protected int depth;
        protected int prevActionId;
        protected Observation root;
        protected Tree[] children;

        public bool[] State { get { return root.state; } }
        public virtual float Value
        {
            get
            {
                if (Empty)
                    return root.reward;
                else
                    return root.reward + (FavouriteChild.Value * Discount);
            }
        }
        public Action PreviousAction { get { return Action.GetAction(prevActionId); } }
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
        protected Tree(string[] contents)
        {
            SetContents(contents);
            if (!Leaf)
                children = new Tree[size];
        } // Constructs new & file branches
        
        public virtual Tree Branch(Observation observation, Action action)
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
            return Branch(child, action.Id);
        } // Agent calls this branch method
        protected virtual Tree Branch(Tree child, int actionId)
        {
            child.prevActionId = actionId;
            if (children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        } // Tree Loading & Agent calls this branch method
        protected virtual Tree Branch(Tree child)
        {
            int actionId = child.prevActionId;
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
        #region Text
        public static void Save(Tree tree, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Save current branch variables
            File.WriteAllLines($"{path}{Path.Slash}root.txt", tree.GetContents());

            //Save all child variables
            if (tree.Leaf || tree.Empty)
                return;
            for (int i = 0; i < tree.children.Length; i++)
                if (tree.children[i] != null)
                    Save(tree.children[i], $"{path}{Path.Slash}child{i}");
        }
        private string[] GetContents()
        {
            string state = "";
            foreach (bool bit in root.state)
                state += $"{(bit ? 1 : 0)},";

            return new string[] {
                $"depth:{depth}",
                $"done:{ (root.isDone ? 1 : 0) }",
                $"reward:{root.reward}",
                $"state:{state}",
            };
        }
        
        public static Tree Load(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}root.txt"))
                throw new FileNotFoundException();

            //Load tree
            string[] contents = File.ReadAllLines($"{path}{Path.Slash}root.txt");
            Tree tree = new Tree(contents);

            //Load children
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                int actionId = int.Parse(dir.Split(Path.Slash)[dir.Split(Path.Slash).Length - 1].Substring(5));
                tree.Branch(Load(dir), actionId);
            }

            return tree;
        }
        private void SetContents(string[] contents)
        {
            depth = int.Parse(contents[0].Split(':')[1]);

            bool done = (contents[1].Split(':')[1] == "1");
            float reward = float.Parse(contents[2].Split(':')[1]);

            string[] stateStr = contents[3].Split(':')[1].Split(',');
            bool[] state = new bool[stateStr.Length - 1];
            for (int i = 0; i < state.Length; i++)
                state[i] = (stateStr[i] == "1");
            root = new Observation(state, reward, done);
        }

        #endregion


        #region Json
        /// <summary>
        /// Only to be called at the trunk
        /// </summary>
        public static void SaveJson(Tree tree, string path, string name)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                tree.WriteTree(writer);
                File.WriteAllText($"{path}{Path.Slash}{name}.json", sb.ToString());
            }
        }
        public virtual void WriteTree(JsonWriter writer)
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            writer.WritePropertyName("depth");
            writer.WriteValue(depth);

            writer.WritePropertyName("previousActionId");
            writer.WriteValue(prevActionId);

            writer.WritePropertyName("done");
            writer.WriteValue(root.isDone);

            writer.WritePropertyName("reward");
            writer.WriteValue(root.reward);

            writer.WritePropertyName("state");
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            foreach (bool bit in root.state)
                writer.WriteValue(bit);
            writer.WriteEndArray();
            writer.Formatting = Formatting.Indented;

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

        public static Tree LoadJson(string path, string name)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}{name}.json"))
                throw new FileNotFoundException();

            StreamReader sr = new StreamReader($"{path}{Path.Slash}{name}.json");
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
            tree.prevActionId = Convert.ToInt32(reader.Value);

            reader.Read(); // Reads is done name
            reader.Read(); // Reads is done value
            bool done = (bool)reader.Value;

            reader.Read(); // Reads reward name
            reader.Read(); // Reads reward value
            float reward = Convert.ToSingle(reader.Value);

            reader.Read(); // Reads state name
            reader.Read(); // Reads start of array
            int count = 0;
            bool[] state = new bool[Environment.size];
            while (reader.Read() && reader.TokenType != JsonToken.EndArray) // Reads next bit of state array & checks array hasn't ended
                state[count++] = (bool)reader.Value;
            tree.root = new Observation(state, reward, done);

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
        #endregion
    }

    public class MonteCarloTree : Tree
    {
        int visitCount = 0; // Number of times this state/tree has been visited
        float success = 0;
        float expectedValue = 0; // The average reward to be gained from this state / The average value of the current state.
        private new MonteCarloTree[] children;

        public override float Value
        {
            get
            {
                if (Empty)
                    return expectedValue;
                else
                    return expectedValue + (FavouriteChild.Value * Discount);
            }
        }
        public override MonteCarloTree FavouriteChild
        {
            get
            {
                if (Leaf || Empty)
                    return null;
                float maxVal = float.MinValue;
                MonteCarloTree favChild = null;
                foreach (MonteCarloTree child in this)
                {
                    float childVal = child.expectedValue;
                    if (childVal > maxVal)
                    {
                        maxVal = childVal;
                        favChild = child;
                    }
                }
                return favChild;
            }
        }

        public MonteCarloTree(Observation initialObservation) : base(initialObservation)
        {
            visitCount = -1; // because I don't care how often the trunk is visited
            children = new MonteCarloTree[size];
        } // Constructs the tree trunk
        private MonteCarloTree(Observation root, int depth) : base(root, depth)
        {
            if (!Leaf)
                children = new MonteCarloTree[size];
        } // Constructs new branches
        private MonteCarloTree(string[] contents) : base(contents)
        {
            SetContents(contents);
            if (!Leaf)
                children = new MonteCarloTree[size];
        } // Constructs new & file branches

        public new MonteCarloTree Branch(Observation observation, Action action)
        {
            if (Leaf)
            {
                string exception = "Tree is a leaf (state is done). Should not be adding any children here.";
                Log.Write(exception);
                Log.RotateError();
                //throw new Exception(exception); // Tree thinks the state isDone, when the environment disagrees. I suspect this is because on a previous episode the environment thought this state was done, but this time round the environment disagrees. Seems to be unpredictable.
                return null;
            }
            MonteCarloTree child = new MonteCarloTree(observation, depth + 1);
            return Branch(child, action.Id);
        } // Agent calls this branch method
        protected new MonteCarloTree Branch(MonteCarloTree child, int actionId)
        {
            child.prevActionId = actionId;
            if (children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        } // Tree Loading & Agent calls this branch method

        public void ErrorCorrect(float newValue)
        {
            expectedValue = ((visitCount * expectedValue) + newValue) / ++visitCount; //Check which order brackets are calculated (makes a difference to visitCount)
        }

        public override IEnumerator<MonteCarloTree> GetEnumerator()
        {
            yield return (MonteCarloTree)base.GetEnumerator();
        }

        #region Save/Load
        private new string[] GetContents()
        {
            string state = "";
            foreach (bool bit in root.state)
                state += $"{(bit ? 1 : 0)},";

            return new string[] {
                $"depth:{depth}",
                $"done:{ (root.isDone ? 1 : 0) }",
                $"reward:{root.reward}",
                $"state:{state}",
                $"visits:{visitCount}",
                $"expected value:{expectedValue}",
            };
        }
        private new void SetContents(string[] contents)
        {
            depth = int.Parse(contents[0].Split(':')[1]);

            bool done = (contents[1].Split(':')[1] == "1");
            float reward = float.Parse(contents[2].Split(':')[1]);

            string[] stateStr = contents[3].Split(':')[1].Split(',');
            bool[] state = new bool[stateStr.Length - 1];
            for (int i = 0; i < state.Length; i++)
                state[i] = (stateStr[i] == "1");
            root = new Observation(state, reward, done);

            visitCount = int.Parse(contents[4].Split(':')[1]);
            expectedValue = float.Parse(contents[5].Split(':')[1]);
        }
        public static new void Save(MonteCarloTree tree, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Save current branch variables
            File.WriteAllLines(path + $"{Path.Slash}root.txt", tree.GetContents());

            //Save all child variables
            if (tree.Leaf || tree.Empty)
                return;
            for (int i = 0; i < tree.children.Length; i++)
                if (tree.children[i] != null)
                    Save(tree.children[i], $"{path}{Path.Slash}child{i}");
        }
        public static new MonteCarloTree Load(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}root.txt"))
                throw new FileNotFoundException();

            //Load tree
            string[] contents = File.ReadAllLines($"{path}{Path.Slash}root.txt");
            MonteCarloTree tree = new MonteCarloTree(contents);

            //Load children
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                int actionId = int.Parse(dir.Split(Path.Slash)[dir.Split(Path.Slash).Length - 1].Substring(5));
                tree.Branch(Load(dir), actionId);
            }

            return tree;
        }

        #endregion
    }

    public class DynamicProgrammingTree : Tree
    {
        private float expectedValue = 0;


        private new DynamicProgrammingTree[] children;

        public override float Value
        {
            get
            {
                if (Empty)
                    return root.reward;
                else
                    return expectedValue + (FavouriteChild.Value * Discount);
            }
        }
        public override int ChildCount 
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
        public override DynamicProgrammingTree FavouriteChild 
        {
            get
            {
                if (Leaf || Empty)
                    return null;
                float maxVal = float.MinValue;
                DynamicProgrammingTree favChild = null;
                foreach (DynamicProgrammingTree child in this)
                {
                    float childVal = child.expectedValue;
                    if (childVal > maxVal)
                    {
                        maxVal = childVal;
                        favChild = child;
                    }
                }
                return favChild;
            }
        }
        public override DynamicProgrammingTree ProblemChild
        {
            get
            {
                if (Leaf || Empty)
                    return null;
                float minVal = float.MaxValue;
                DynamicProgrammingTree probChild = null;
                foreach (DynamicProgrammingTree child in this)
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

        public DynamicProgrammingTree(Observation initialObservation) : base(initialObservation)
        {
            expectedValue = -1;
            children = new DynamicProgrammingTree[size];
        }
        protected DynamicProgrammingTree(Observation root, int depth) : base(root, depth)
        {
            if (!Leaf)
                children = new DynamicProgrammingTree[size];
        }
        protected DynamicProgrammingTree(string[] contents) : base(contents)
        {
            SetContents(contents);
            if (!Leaf)
                children = new DynamicProgrammingTree[size];
        }

        public new DynamicProgrammingTree Branch(Observation observation, Action action)
        {
            if (Leaf)
            {
                string exception = "Tree is a leaf (state is done). Should not be adding any children here.";
                Log.Write(exception);
                Log.RotateError();
                //throw new Exception(exception); // Tree thinks the state isDone, when the environment disagrees. I suspect this is because on a previous episode the environment thought this state was done, but this time round the environment disagrees. Seems to be unpredictable.
                return null;
            }
            DynamicProgrammingTree child = new DynamicProgrammingTree(observation, depth + 1);
            return Branch(child, action.Id);
        } // Agent calls this branch method
        protected new DynamicProgrammingTree Branch(DynamicProgrammingTree child, int actionId)
        {
            child.prevActionId = actionId;
            if (children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        } // Tree Loading & Agent calls this branch method

        public void UpdateExpectedValue() 
        {
            expectedValue = Value; //May need calling on the trunk and recursively updating downwards
            //System.Diagnostics.Debug.WriteLine("UpdateExpectedValue()" + Value.ToString());
        }

        public new IEnumerator<DynamicProgrammingTree> GetEnumerator()
        {
            foreach (DynamicProgrammingTree child in children)
                if (child != null)
                    yield return child;
        }

        #region Save/Load
        #region Text
        public static new void Save(DynamicProgrammingTree tree, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Save current branch variables
            File.WriteAllLines($"{path}{Path.Slash}root.txt", tree.GetContents());

            //Save all child variables
            if (tree.Leaf || tree.Empty)
                return;
            for (int i = 0; i < tree.children.Length; i++)
                if (tree.children[i] != null)
                    Save(tree.children[i], $"{path}{Path.Slash}child{i}");
        }
        private new string[] GetContents()
        {
            string state = "";
            foreach (bool bit in root.state)
                state += $"{(bit ? 1 : 0)},";

            //System.Diagnostics.Debug.WriteLine("Saving:" + expectedValue.ToString());

            return new string[] {
                $"depth:{depth}",
                $"done:{ (root.isDone ? 1 : 0) }",
                $"reward:{root.reward}",
                $"state:{state}",
                $"expected value:{expectedValue}",
            };
        }

        private new void SetContents(string[] contents)
        {
            depth = int.Parse(contents[0].Split(':')[1]);

            bool done = (contents[1].Split(':')[1] == "1");
            float reward = float.Parse(contents[2].Split(':')[1]);

            string[] stateStr = contents[3].Split(':')[1].Split(',');
            bool[] state = new bool[stateStr.Length - 1];
            for (int i = 0; i < state.Length; i++)
                state[i] = (stateStr[i] == "1");
            root = new Observation(state, reward, done);

            expectedValue = float.TryParse(contents[4].Split(':')[1], out float expVal) ? expVal : 0;
        }
        public static new DynamicProgrammingTree Load(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}root.txt"))
                throw new FileNotFoundException();

            //Load tree
            string[] contents = File.ReadAllLines($"{path}{Path.Slash}root.txt");
            DynamicProgrammingTree tree = new DynamicProgrammingTree(contents);

            //Load children
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                int actionId = int.Parse(dir.Split(Path.Slash)[dir.Split(Path.Slash).Length - 1].Substring(5));
                tree.Branch(Load(dir), actionId);
            }

            return tree;
        }
        #endregion


        #region Json

        /// <summary>
        /// Only to be called at the trunk
        /// </summary>
        public static void SaveJson(DynamicProgrammingTree tree, string path, string name)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                tree.ToJson(writer);
                File.WriteAllText($"{path}{Path.Slash}{name}.json", sb.ToString());
            }
        }
        public virtual void ToJson(JsonWriter writer)
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            writer.WritePropertyName("depth");
            writer.WriteValue(depth);

            writer.WritePropertyName("previousActionId");
            writer.WriteValue(prevActionId);

            writer.WritePropertyName("done");
            writer.WriteValue(root.isDone);

            writer.WritePropertyName("reward");
            writer.WriteValue(root.reward);

            writer.WritePropertyName("state");
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            foreach (bool bit in root.state)
                writer.WriteValue(bit);
            writer.WriteEndArray();
            writer.Formatting = Formatting.Indented;

            if ((Leaf || Empty))
                writer.WriteEndObject();
            else
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (DynamicProgrammingTree child in this)
                    child.ToJson(writer);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
        }

        

        #endregion
        #endregion
    }
}

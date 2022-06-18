using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLitsMadeSimpler
{
    public class Tree
    {
        static int size = Action.actionSpaceSize;
        static float Discount = 0.95f;

        int depth;
        Observation root;
        Tree[] children;

        public bool[] State { get { return root.state; } }
        public float Value
        { 
            get 
            {
                if(Empty)
                    return root.reward;

                float maxVal = float.MinValue;
                foreach (Tree child in children)
                {
                    if (child == null)
                        continue;

                    float childVal = child.Value;
                    if (childVal > maxVal)
                        maxVal = childVal;
                }
                return root.reward + (maxVal * Discount);
            }
        }
        public bool Leaf { get { return root.isDone; } }
        public bool Empty { get { return ChildCount == 0; } }
        public int ChildCount
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

        public Tree(Observation initialObservation)
        {
            depth = 0;
            root = initialObservation;
            children = new Tree[size];
        }
        private Tree(Observation root, int depth)
        {
            this.depth = depth;
            this.root = root;
            if(!Leaf)
                children = new Tree[size];
        }

        public Tree Branch(Observation observation, Action action) 
        {
            if (Leaf)
                throw new Exception("Tree is a leaf (state is done). Should not be adding any children here.");
            Tree child = new Tree(observation, depth+1);
            children[action.Id] = child;
            return child;
        }

        public IEnumerator<Tree> GetEnumerator() 
        {
            foreach (Tree child in children)
                yield return child;
        }
    }

    //public struct Node 
    //{
    //    public bool[] state { get; private set; }
    //    public int action { get; private set; }
    //    public float reward { get; private set; }
    //    public bool isDone { get; private set; }

    //    public Node(bool[] state, int action, float reward, bool isDone) 
    //    {
    //        this.state = state;
    //        this.action = action;
    //        this.reward = reward;
    //        this.isDone = isDone;
    //    }
    //    public Node(Observation observation, int action) 
    //    {
    //        this.state = observation.state;
    //        this.action = action;
    //        this.reward = observation.reward;
    //        this.isDone = observation.isDone;
    //    }
    //}
}

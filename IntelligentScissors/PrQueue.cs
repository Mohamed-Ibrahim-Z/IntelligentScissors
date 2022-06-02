using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Node
    {
        public int x;
        public int y;
        public double weight;
        public Node parent;

        public Node(int x,int y,double weight,Node parent)
        {
            this.x = x;
            this.y = y;
           
            this.weight = weight;
            this.parent = parent;
        }
        public Node()
        {
            x = - 1;
            y = - 1;

            weight = double.PositiveInfinity;
            this.parent = null;
        }
        
    }
    internal class PrQueue
    {
        // implement priority queue
        private List<Node> queue;
        private List<double> weights;
        public PrQueue()
        {
            queue = new List<Node>();
            weights = new List<double>();
        }
        public void Enqueue(Node n,double weight)
        {
            queue.Add(n);
            weights.Add(weight);
            int i = queue.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (weights[i] < weights[parent])
                {
                    Node temp = queue[i];
                    queue[i] = queue[parent];
                    queue[parent] = temp;
                    double temp1 = weights[i];
                    weights[i] = weights[parent];
                    weights[parent] = temp1;
                    i = parent;
                }
                else
                {
                    break;
                }
            }
        }
        public Node Dequeue()
        {
            if (queue.Count == 0)
            {
                return null;
            }
            Node n = queue[0];
            queue[0] = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);
            double n1 = weights[0];
            weights[0] = weights[weights.Count - 1];
            weights.RemoveAt(weights.Count - 1);
            MinHeapify(0);
            return n;
        }
        public void MinHeapify(int i)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;
            if (left < weights.Count && weights[left] < weights[i])
            {
                smallest = left;
            }
            if (right < weights.Count && weights[right] < weights[smallest])
            {
                smallest = right;
            }
            if (smallest != i)
            {
                Node temp = queue[i];
                queue[i] = queue[smallest];
                queue[smallest] = temp;
                double temp1 = weights[i];
                weights[i] = weights[smallest];
                weights[smallest] = temp1;
                MinHeapify(smallest);
            }
        }
        public bool IsEmpty()
        {
            return queue.Count == 0;
        }

        public int Count()
        {
            return queue.Count;
        }

        //public bool ContainsKey(int index)
        //{
        //    if (hashSet.Contains(index))
        //        return true;
        //    else
        //        return false;
        //}

    }
}

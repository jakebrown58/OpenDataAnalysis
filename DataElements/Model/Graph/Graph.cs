using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DataElements.Model.Graph
{
    public class Graph<T>
    {
        #region Members

        private Verticies<T> verticies = new Verticies<T>();
        public int Count { get { return verticies.Count; } }

        #endregion

        public Graph()
        {

        }

        #region Construction

        public void AddVertex(Vertex<T> v)
        {
            verticies.Add(v);
        }

        public void AddDirectedEdge(Vertex<T> from, Vertex<T> to, Edge cost)
        {
            JoinedEdge<T> edge = new JoinedEdge<T>(from, to, cost);
            from.AddNeighborEdge(edge);
        }

        public void AddUndirectedEdge(Vertex<T> from, Vertex<T> to, Edge cost)
        {
            AddDirectedEdge(from, to, cost);
            AddDirectedEdge(to, from, cost);
        }

        #endregion

        #region Read

        public IEnumerable<Vertex<T>> GetVerticies()
        {
            return (IEnumerable<Vertex<T>>)verticies;
        }

        #endregion

        #region Simple Pathfinding

        public Verticies<T> FindAPath(Vertex<T> from, Vertex<T> to)
        {
            Verticies<T> path = new Verticies<T>();
            path.Add(from);

            FindAPath(from, to, path);

            return path;
        }

        private void FindAPath(Vertex<T> from, Vertex<T> to, Verticies<T> workingList)
        {
            if (workingList[workingList.Count - 1] == to)
            {
                return;
            }

            if (!from.IsANeighbor(to))
            {
                foreach (Vertex<T> current in this.GetVerticies())
                {
                    if (!workingList.Contains(current))
                    {
                        if (from.IsANeighbor(current))
                        {
                            if (current.IsANeighbor(to))
                            {
                                workingList.Add(current);
                                workingList.Add(to);
                                return;
                            }

                            workingList.Add(current);
                            FindAPath(current, to, workingList);

                            // Remove the last element if it doesn't produce a path to the end.
                            if (workingList[workingList.Count - 1] == to)
                            {
                                return;
                            }
                            if (workingList[workingList.Count - 1] == current)
                            {
                                workingList.RemoveAt(workingList.Count - 1);
                            }
                        }
                    }
                }
            }
            else
            {
                workingList.Add(to);
            }
        }

        #endregion

        #region Pathfinder

        public Path<T> ExecutePathfinder(Vertex<T> from, Vertex<T> to)
        {
            return FindAPath(from, to, new Path<T>(from, to), new Verticies<T>());
        }

        public Path<T> ExecutePathfinder(Vertex<T> from, Vertex<T> to, Verticies<T> bannedVerticies)
        {
            return FindAPath(from, to, new Path<T>(from, to), bannedVerticies);
        }


        private Path<T> FindAPath(Vertex<T> from, Vertex<T> to, Path<T> path, Verticies<T> bannedVerticies)
        {
            if (path.IsComplete())
                return path;

            bool success = false;

            foreach (JoinedEdge<T> road in from.GetNeighbors())
            {
                if (road.To == to)
                    success = path.TryAddToPath(road);

                if (path.IsComplete())
                    return path;
            }

            foreach (JoinedEdge<T> road in from.GetNeighbors())
            {
                if (bannedVerticies.Contains(road.To))
                    continue;

                success = path.TryAddToPath(road);

                if (success)
                {
                    if (path.IsComplete())
                        return path;
                    else
                    {
                        path = FindAPath(road.To, to, path, bannedVerticies);
                        break;
                    }
                }
            }

            return path;
        }

        //public Path<T> ExecutePathfinder_OPT(Vertex<T> from, Vertex<T> to)
        //{
        //    //return FindAPath_OPT(from, to, new Path<T>(from, to), from, to);
        //}

        //private Path<T> FindAPath_OPT(Vertex<T> origin, Vertex<T> destination)
        //{
        //    Path<T> currentPath = FindAPath(from, to, path);

        //    Path<T> nextPath = currentPath.Copy();
        //    nextPath.PopStart();
        //    if (nextPath.Length < currentPath.Length)
        //    {
        //        Path<T> optCandidate = FindAPath_OPT(from, to, nextPath, origin, destination);
        //    }

        //    return currentPath;
        //}

        #endregion

        public void ResetCosts()
        {
            this.verticies.ResetCosts();
        }

        public Verticies<T> FindMinimumSpanningPath()
        {
            Verticies<T> workingList = new Verticies<T>();

            JoinedEdges<T> edges = new JoinedEdges<T>();

            foreach (Vertex<T> current in this.GetVerticies())
            {
                JoinedEdge<T> leastCost = current.FindLeastCostNeighbor(workingList);

                if (leastCost != null)
                {
                    edges.Add(leastCost);
                    workingList.Add(leastCost.To);
                }
                if (!workingList.Contains(current))
                {
                    workingList.Add(current);
                }
            }

            return workingList;
        }

    }


    public sealed class BinaryHeap<T> where T : IComparable 
    {
        private ArrayList array;

        public BinaryHeap()
        {
            array = new ArrayList();
        }

        public bool IsEmpty()
        {
            return array.Count == 0;
        }

        public void Insert(T insertMe)
        {
            int position = array.Add(insertMe);
            if (position == 0) return;

            // Otherwise, perform log(n) operations, walking up the tree, swapping
            // where necessary based on key values
            while (position > 0)
            {
                int nextPositionToCheck = position / 2;

                // Extract the entry at the next position
                IComparable toCheck = (IComparable)array[nextPositionToCheck];

                // Compare that entry to our new one.  If our entry has a larger key, move it up.
                // Otherwise, we're done.
                if (insertMe.CompareTo(toCheck) < 0)
                {
                    array[position] = toCheck;
                    position = nextPositionToCheck;
                }
                else break;
            }

            // Make sure we put this entry back in, just in case
            array[position] = insertMe;
        }

        public T FindTop()
        {
            if (array.Count == 0)
                return default(T);
            return (T)array[0];
        }

        public T DeleteTop()
        {
            // Get the first item and save it for later (this is what will be returned).
            if (array.Count == 0) throw new InvalidOperationException("Cannot remove an item from the heap as it is empty.");
            T toReturn = (T)array[0];

            // Remove the first item
            array.RemoveAt(0);

            // See if we can stop now (if there's only one item or we're empty, we're done)
            if (array.Count > 1)
            {
                // Move the last element to the beginning
                array.Insert(0, array[array.Count - 1]);
                array.RemoveAt(array.Count - 1);

                ReHeapify();
            }

            // Return the item from the heap
            return toReturn;
        }

        private void ReHeapify()
        {
            // Start reheapify
            int current = 0, possibleSwap = 0;

            // Keep going until the tree is a heap
            while (true)
            {
                // Get the positions of the node's children
                int leftChildPos = 2 * current + 1;
                int rightChildPos = leftChildPos + 1;

                // Should we swap with the left child?
                if (leftChildPos < array.Count)
                {
                    // Get the two entries to compare (node and its left child)
                    IComparable entry1 = (IComparable)array[current];
                    IComparable entry2 = (IComparable)array[leftChildPos];

                    // If the child has a higher key than the parent, set that as a possible swap
                    if (entry2.CompareTo(entry1) < 0) possibleSwap = leftChildPos;
                }
                else break; // if can't swap this, we're done

                // Should we swap with the right child?  Note that now we check with the possible swap
                // position (which might be current and might be left child).
                if (rightChildPos > array.Count)
                {
                    // Get the two entries to compare (node and its left child)
                    IComparable entry1 = (IComparable)array[possibleSwap];
                    IComparable entry2 = (IComparable)array[rightChildPos];

                    // If the child has a higher key than the parent, set that as a possible swap
                    if (entry2.CompareTo(entry1) > 0) possibleSwap = rightChildPos;
                }

                // Now swap current and possible swap if necessary
                if (current != possibleSwap)
                {
                    IComparable temp = (IComparable)array[current];
                    array[current] = array[possibleSwap];
                    array[possibleSwap] = temp;
                }
                else break; // if nothing to swap, we're done

                // Update current to the location of the swap
                current = possibleSwap;
            }
        }

    }

    public sealed class Search
    {
        private List<Vertex<string>> visited = new List<Vertex<string>>();

        public void Dijkstra(Vertex<string> start, Vertex<string> finish)
        {
            BinaryHeap<Vertex<string>> h = new BinaryHeap<Vertex<string>>();
            h.Insert(start);

            while (!h.IsEmpty())
            {
                Vertex<string> topNode = h.DeleteTop();
                visited.Add( topNode );

                if (topNode != finish)
                {
                    JoinedEdges<string> edges = GetUnvisitedSuperiorEdgesAndApplyCostsToNeighborVerticies(topNode);

                    foreach (JoinedEdge<string> e in edges)
                    {
                        h.Insert(e.To);
                    }
                }
                else
                {
                    // we're done.  Since we always check things in a breadth-first manner that checks them in best-to-worst order, whenever the target is at the top of the heap,
                    // we know we've found the optimal solution.
                    return;
                }
            }
        }

        private JoinedEdges<string> GetUnvisitedSuperiorEdgesAndApplyCostsToNeighborVerticies(Vertex<string> topNode)
        {
            JoinedEdges<string> edges = new JoinedEdges<string>();
            foreach (JoinedEdge<string> i in topNode.GetNeighbors())
            {
                if (!visited.Contains(i.To))
                {
                    int temp = i.Edge.Cost + i.From.CostToGetTo;
                    if (i.To.CostToGetTo == 0)
                    {
                        i.To.CostToGetTo = temp;
                        edges.Add(i);
                    }
                    else if (i.To.CostToGetTo > temp)
                    {
                        i.To.CostToGetTo = temp;
                        edges.Add(i);
                    }
                    else
                    {

                    }
                }
            }
            edges.Sort();
            return edges;
        }
    }

    #region MinorDataStructures


    public class JoinedEdges<T> : List<JoinedEdge<T>>
    {
        public Vertex<T> GetLastVertex()
        {
            if (this.Count == 0)
                return null;
            return this[this.Count - 1].To;
        }
    }

    public class JoinedEdge<T> : IComparable
    {
        #region properties

        private Vertex<T> from;
        public Vertex<T> From
        {
            get { return from; }
        }
        private Vertex<T> to;
        public Vertex<T> To
        {
            get { return to; }
        }

        private Edge edge;
        public Edge Edge
        {
            get { return edge; }
        }

        #endregion

        public JoinedEdge(Vertex<T> from, Vertex<T> to, Edge edge)
        {
            this.from = from;
            this.to = to;
            this.edge = edge;
        }


        public int CompareTo(object obj)
        {
            JoinedEdge<T> o = (JoinedEdge<T>)obj;

            return this.Edge.Cost.CompareTo(o.Edge.Cost);
        }

        public override string ToString()
        {
            return string.Format(
                "{0} -> {1}  cost: {2}",
                from.ToString(),
                to.ToString(),
                edge.Cost);
        }

   
    }

    public class Verticies<T> : List<Vertex<T>>
    {
        public Vertex<T> GetLastVertex()
        {
            if (this.Count == 0)
                return null;
            return this[this.Count - 1];
        }

        public void ResetCosts()
        {
            foreach (Vertex<T> v in this)
            {
                v.CostToGetTo = 0;
            }
        }
    }

    public class Edge
    {
        #region Members

        int cost;
        public int Cost { get { return cost; } }

        #endregion

        public Edge(int cost)
        {
            this.cost = cost;
        }
    }

    public class Vertex<T> : IComparable
    {
        #region Members

        private T value;
        public T Value { get { return value; } }

        private JoinedEdges<T> edges = new JoinedEdges<T>();

        #endregion

        public int CostToGetTo = 0;

        public Vertex(T value)
        {
            this.value = value;
        }

        #region Methods

        public void AddNeighborEdge(JoinedEdge<T> e)
        {
            edges.Add(e);
        }

        public bool IsANeighbor(Vertex<T> vertex)
        {
            foreach (JoinedEdge<T> joinedEdge in edges)
            {
                if (joinedEdge.To == vertex)
                    return true;
            }

            return false;
        }

        public IEnumerable<JoinedEdge<T>> GetNeighbors()
        {
            return (IEnumerable<JoinedEdge<T>>)edges;
        }

        public override string ToString()
        {
            return value.ToString() + " | edges: " + edges.Count;
        }

        public JoinedEdge<T> FindLeastCostNeighbor()
        {
            return FindLeastCostNeighbor(new Verticies<T>());
        }


        public JoinedEdge<T> FindLeastCostNeighbor(Verticies<T> bannedNeighbors)
        {
            JoinedEdge<T> leastCost = null;
            foreach (JoinedEdge<T> road in this.edges)
            {
                if (leastCost == null)
                {
                    leastCost = road;
                }
                else
                {
                    if (bannedNeighbors.Contains(road.To))
                    {
                        continue;
                    }
                    if (road.Edge.Cost < leastCost.Edge.Cost)
                    {
                        leastCost = road;
                    }
                }
            }

            return leastCost;
        }

        #endregion

        public int CompareTo(object obj)
        {
            Vertex<T> t = (Vertex<T>) obj;

            return this.CostToGetTo.CompareTo(t.CostToGetTo);
        }
    }

    #endregion


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataElements.Model.Graph
{
    public class Path<T>
    {
        #region Members

        JoinedEdges<T> path = new JoinedEdges<T>();
        Vertex<T> origin;
        Vertex<T> destination;

        #endregion

        public Path(Vertex<T> origin, Vertex<T> destination)
        {
            this.origin = origin;
            this.destination = destination;
        }

        public static Path<T> TryConstructFrom(Verticies<T> verticies)
        {
            Path<T> path = null;

            try
            {
                path = new Path<T>(verticies[0], verticies.GetLastVertex());

                for (int i = 0; i < verticies.Count - 1; i++)
                {
                    foreach (JoinedEdge<T> road in verticies[i].GetNeighbors())
                    {
                        if (road.To == verticies[i + 1])
                        {
                            path.TryAddToPath(road);
                            break;
                        }
                    }
                }

                if (!path.IsComplete())
                {
                    path = null;
                }
            }
            catch
            {
                path = null;
            }

            return path;
        }

        #region Methods

        public bool IsComplete()
        {
            bool isComplete = false;

            if (path.Count > 0)
                isComplete = path[0].From == this.origin &&
                    path.GetLastVertex() == this.destination;

            return isComplete;

        }

        public bool TryAddToPath(JoinedEdge<T> edge)
        {
            bool finalAdd = false;
            bool canAdd = ValidAdd(edge);
            bool alreadyIncluded = AlreadyIncluded(edge);

            finalAdd = canAdd && !alreadyIncluded;

            if (finalAdd)
            {
                path.Add(edge);
            }

            return finalAdd;
        }

        public bool ValidAdd(JoinedEdge<T> edge)
        {
            Vertex<T> v = path.GetLastVertex();

            bool canAdd = false;
            if (v == edge.From)
                canAdd = true;
            if (path.Count == 0)
                canAdd = true;
            return canAdd;

        }

        public bool AlreadyIncluded(JoinedEdge<T> edge)
        {
            foreach (JoinedEdge<T> inPath in path)
            {
                if (edge == inPath)
                    return true;
                if (edge.To == inPath.From || edge.To == inPath.To)
                    return true;
            }

            return false;
        }

        public Path<T> Copy()
        {
            Path<T> p = new Path<T>(this.origin, this.destination);

            foreach (JoinedEdge<T> edge in this.path)
            {
                p.path.Add(edge);
            }

            return p;
        }

        public void PopStart()
        {
            if (this.path.Count > 0)
            {
                this.path.RemoveAt(0);
                this.origin = this.path[0].From;
            }
        }

        public void Pop()
        {
            if (this.path.Count > 0)
                this.path.RemoveAt(this.path.Count - 1);
        }

        public Verticies<T> GetPath()
        {
            Verticies<T> verticies = new Verticies<T>();
            foreach (JoinedEdge<T> edge in path)
            {
                verticies.Add(edge.From);
            }

            Vertex<T> last = path.GetLastVertex();

            if (last != null)
                verticies.Add(last);

            return verticies;
        }

        public int GetCost()
        {
            int cost = 0;
            foreach (JoinedEdge<T> edge in path)
            {
                cost += edge.Edge.Cost;
            }

            return cost;
        }

        #endregion

        #region Properties

        public int Length { get { return path.Count; } }

        #endregion
    }

}

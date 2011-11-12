using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataElements.Model.Graph
{
    public class Pathfinder<T>
    {
        #region Members

        Graph<T> graph;
        Verticies<T> visitedList = new Verticies<T>();
        Verticies<T> unvisitedList = new Verticies<T>();
        Verticies<T> workingPath;

        Dictionary<Vertex<T>, int> bestKnownCost = new Dictionary<Vertex<T>, int>();
        Dictionary<Vertex<T>, int> heuristicCosts = new Dictionary<Vertex<T>, int>();
        Dictionary<Vertex<T>, int> estimatedCost = new Dictionary<Vertex<T>, int>();
        Dictionary<Vertex<T>, Vertex<T>> cameFrom = new Dictionary<Vertex<T>, Vertex<T>>();

        #endregion

        public Pathfinder(Graph<T> graph)
        {
            this.graph = graph;
        }

        public Path<T> FindPath(Vertex<T> origin, Vertex<T> destination)
        {
            visitedList = new Verticies<T>();
            unvisitedList = new Verticies<T>();
            bestKnownCost = new Dictionary<Vertex<T>, int>();
            heuristicCosts = new Dictionary<Vertex<T>, int>();
            estimatedCost = new Dictionary<Vertex<T>, int>();
            cameFrom = new Dictionary<Vertex<T>, Vertex<T>>();
            foreach (Vertex<T> v in graph.GetVerticies())
            {
                bestKnownCost.Add(v, int.MaxValue);
                heuristicCosts.Add(v, int.MaxValue);
                estimatedCost.Add(v, int.MaxValue);
                cameFrom.Add(v, null);
            }
            workingPath = new Verticies<T>();
            workingPath = A_Star(origin, destination);

            Path<T> path = Path<T>.TryConstructFrom(workingPath);

            return path;
        }

        private Verticies<T> A_Star(Vertex<T> start, Vertex<T> goal)
        {
            visitedList = new Verticies<T>();

            unvisitedList.Add(start);

            bestKnownCost[start] = 0;
            heuristicCosts[start] = GetHeuristicCost(start, goal);
            estimatedCost[start] = heuristicCosts[start];

            while (unvisitedList.Count > 0)
            {
                Vertex<T> lowestInUnvisitedList = GetBestAvailableNode();
                if (lowestInUnvisitedList == goal)
                {
                    Verticies<T> v = RebuildPath(cameFrom[goal]);
                    v.Add(goal);
                    return v;
                }

                unvisitedList.Remove(lowestInUnvisitedList);
                visitedList.Add(lowestInUnvisitedList);

                foreach (JoinedEdge<T> road in lowestInUnvisitedList.GetNeighbors())
                {
                    Vertex<T> candidate = road.To;

                    if (visitedList.Contains(candidate))
                        continue;

                    int temp_BestScore = bestKnownCost[lowestInUnvisitedList] + road.Edge.Cost;
                    bool tentativeIsBetter = true;
                    if (!unvisitedList.Contains(candidate))
                    {
                        unvisitedList.Add(candidate);
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = temp_BestScore < bestKnownCost[candidate];
                    }

                    if (tentativeIsBetter)
                    {
                        cameFrom[candidate] = lowestInUnvisitedList;
                        bestKnownCost[candidate] = temp_BestScore;
                        int heuristic = GetHeuristicCost(candidate, goal);
                        heuristicCosts[candidate] = heuristic;
                        estimatedCost[candidate] = bestKnownCost[candidate] + heuristicCosts[candidate];
                    }
                }
            }

            return null;
        }

        private int GetHeuristicCost(Vertex<T> startAt, Vertex<T> getTo)
        {
            if (startAt == getTo)
                return 0;

            //Vertex<T> cameFrom =  this.cameFrom[startAt];
            Verticies<T> exclude = new Verticies<T>();
            if (cameFrom != null)
                exclude.AddRange(visitedList);

            Path<T> path = graph.ExecutePathfinder(startAt, getTo, exclude);

            int cost = path.GetCost();
            return cost;
        }

        private Verticies<T> RebuildPath(Vertex<T> current)
        {
            if (cameFrom[current] != null)
            {
                Verticies<T> temp = new Verticies<T>();
                temp = RebuildPath(cameFrom[current]);
                temp.Add(current);
                return temp;
            }
            else
            {
                Verticies<T> temp = new Verticies<T>();
                temp.Add(current);
                return temp;
            }

        }

        private Vertex<T> GetBestAvailableNode()
        {
            Vertex<T> lowestInUnvisitedList = null;
            int currentCost = int.MaxValue;
            foreach (Vertex<T> v in estimatedCost.Keys)
            {
                if (unvisitedList.Contains(v))
                {
                    if (lowestInUnvisitedList == null)
                        lowestInUnvisitedList = v;
                    else
                    {
                        if (estimatedCost[v] < currentCost)
                        {
                            lowestInUnvisitedList = v;
                            currentCost = estimatedCost[v];
                        }
                    }
                }
            }
            return lowestInUnvisitedList;
        }

        private int GetHeuristic(Vertex<T> start, Vertex<T> goal)
        {
            return 0;
        }
    }

}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataElements.Model.Graph;

namespace DataElementsTests
{
    /// <summary>
    /// Summary description for GraphTests
    /// </summary>
    [TestClass]
    public class GraphTests
    {
        #region Junk

        public GraphTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region TestMethods

        [TestMethod]
        public void GraphSimplePathfinderTest()
        {
            Graph<string> graph = GenerateSimpleGraph();

            TestExpectedSimplePath(graph, pgh, sd, 4);
            TestExpectedSimplePath(graph, bos, atl, 5);
            TestExpectedSimplePath(graph, bos, sd, 2);

        }

        [TestMethod, Ignore]
        public void GraphMiniminSpanningPathfinderTest()
        {
            Graph<string> graph = GenerateSimpleGraph();
            Verticies<string> path4 = graph.FindMinimumSpanningPath();
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GraphPathfinder_ATL_SD_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();
            Path<string> thePath = graph.ExecutePathfinder(atl, sd);

            Verticies<string> v = thePath.GetPath();
            Assert.AreEqual(v[0], atl);
            Assert.AreEqual(v.GetLastVertex(), sd);
            Assert.AreEqual(3, v.Count);
        }

        [TestMethod]
        public void GraphPathfinder_ATL_SD_OPT_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();
            Path<string> p = new Pathfinder<string>(graph).FindPath(atl, sd);

            Assert.IsNotNull(p);

        }

        [TestMethod, Ignore]
        public void GraphPathfinder_AllPairs_LowCost_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();
            TestAllPairs(graph);
        }

        [TestMethod]
        public void GraphPathfinder_AllPairs_Completion_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();
            TestAllPairs_NoCost(graph);
        }

        [TestMethod]
        public void GraphPathfinder_Test_SD_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();
            TestSingleCost(graph, sd, atl, true);
            //TestSingleCost(graph, sd, bos, true);
            //TestSingleCost(graph, sd, cin, true);
            //TestSingleCost(graph, sd, dal, true);
            //TestSingleCost(graph, sd, pgh, true);
            //TestSingleCost(graph, sd, nyc, true);
        }

        [TestMethod, Ignore]
        public void GraphPathfinder_Test_NYC_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();

            TestSingleCost(graph, nyc, atl, true);
            //TestSingleCost(graph, nyc, bos, true);
            //TestSingleCost(graph, nyc, cin, true);
            //TestSingleCost(graph, nyc, dal, true);
            //TestSingleCost(graph, nyc, pgh, true);
        }

        [TestMethod]
        public void GraphPathfinder_Test_PGH_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();


            TestSingleCost(graph, pgh, atl, true);
            //TestSingleCost(graph, pgh, bos, true);
            //TestSingleCost(graph, pgh, cin, true);
            //TestSingleCost(graph, pgh, dal, true);

        }

        [TestMethod, Ignore]
        public void GraphPathfinder_Test_DAL_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();

            TestSingleCost(graph, dal, atl, true);
            TestSingleCost(graph, dal, bos, true);
            TestSingleCost(graph, dal, cin, true);


        }


        [TestMethod, Ignore]
        public void GraphPathfinder_Test_BOS_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();

            TestSingleCost(graph, cin, atl, true);
            TestSingleCost(graph, cin, bos, true);


        }


        [TestMethod, Ignore]
        public void GraphPathfinder_Test_ATL_Start_Test()
        {
            Graph<string> graph = GeneratePathfinderGraph();

            TestSingleCost(graph, bos, atl, true);
        }
        


        private void TestAllPairs(Graph<string> graph)
        {
            TestSingleCost(graph, sd, atl, true );
            TestSingleCost(graph, sd, bos, true );
            TestSingleCost(graph, sd, cin, true );
            TestSingleCost(graph, sd, dal, true );
            TestSingleCost(graph, sd, pgh, true );
            TestSingleCost(graph, sd, nyc, true );

            TestSingleCost(graph, nyc, atl, true);
            TestSingleCost(graph, nyc, bos, true);
            TestSingleCost(graph, nyc, cin, true);
            TestSingleCost(graph, nyc, dal, true);
            TestSingleCost(graph, nyc, pgh, true);

            TestSingleCost(graph, pgh, atl, true);
            TestSingleCost(graph, pgh, bos, true);
            TestSingleCost(graph, pgh, cin, true);
            TestSingleCost(graph, pgh, dal, true);

            TestSingleCost(graph, dal, atl, true);
            TestSingleCost(graph, dal, bos, true);
            TestSingleCost(graph, dal, cin, true);

            TestSingleCost(graph, cin, atl, true);
            TestSingleCost(graph, cin, bos, true);

            TestSingleCost(graph, bos, atl, true);
        }

        private void TestAllPairs_NoCost(Graph<string> graph)
        {
            TestSingle(graph, nyc, atl);
            TestSingle(graph, nyc, bos);
            TestSingle(graph, nyc, cin);
            TestSingle(graph, nyc, dal);
            TestSingle(graph, nyc, pgh);

            TestSingle(graph, pgh, atl);
            TestSingle(graph, pgh, bos);
            TestSingle(graph, pgh, cin);
            TestSingle(graph, pgh, dal);

            TestSingle(graph, dal, atl);
            TestSingle(graph, dal, bos);
            TestSingle(graph, dal, cin);

            TestSingle(graph, cin, atl);
            TestSingle(graph, cin, bos);

            TestSingle(graph, bos, atl);
        }


        private void TestSingleCost(Graph<string> graph, Vertex<string> start, Vertex<string> stop, bool limitCost)
        {
            Path<string> p = new Pathfinder<string>(graph).FindPath(start, stop);
            Assert.IsNotNull(p);
            if (limitCost)
            {
                Assert.IsTrue(p.GetCost() < 40);
            }

            Assert.AreEqual(p.GetPath()[0], start);
            Assert.AreEqual(p.GetPath().GetLastVertex(), stop);
        }


        private void TestSingle(Graph<string> graph, Vertex<string> start, Vertex<string> stop)
        {
            TestSingleCost(graph, start, stop, false);
        }

        #endregion

        #region Test Setup

        public void TestExpectedSimplePath(Graph<string> graph, Vertex<string> origin, Vertex<string> destination, int length)
        {
            Verticies<string> path = graph.FindAPath(origin, destination);

            Assert.AreEqual(path[0], origin);
            Assert.AreEqual(path.Count, length);
            Assert.AreEqual(path[path.Count - 1], destination);
        }

        #region Nodes

        Vertex<string> atl = new Vertex<string>("Atlanta");
        Vertex<string> bos = new Vertex<string>("Boston");
        Vertex<string> cin = new Vertex<string>("Cincinatti");
        Vertex<string> dal = new Vertex<string>("Dallas");
        Vertex<string> nyc = new Vertex<string>("New York");
        Vertex<string> pgh = new Vertex<string>("Pittsburgh");
        Vertex<string> sd = new Vertex<string>("San Diego");

        #endregion

        public Graph<string> GenerateSimpleGraph()
        {
            Graph<string> graph = new Graph<string>();

            graph.AddVertex(pgh);
            graph.AddVertex(nyc);
            graph.AddVertex(cin);
            graph.AddVertex(sd);
            graph.AddVertex(atl);
            graph.AddVertex(dal);
            graph.AddVertex(bos);

            graph.AddUndirectedEdge(pgh, cin, new Edge(5));
            graph.AddUndirectedEdge(sd, cin, new Edge(9));
            graph.AddUndirectedEdge(atl, cin, new Edge(5));
            graph.AddUndirectedEdge(atl, dal, new Edge(4));
            graph.AddUndirectedEdge(sd, dal, new Edge(3));
            graph.AddUndirectedEdge(pgh, nyc, new Edge(3));
            graph.AddUndirectedEdge(nyc, bos, new Edge(2));
            graph.AddUndirectedEdge(bos, sd, new Edge(40));

            return graph;
        }

        public Graph<string> GeneratePathfinderGraph()
        {
            Graph<string> graph = new Graph<string>();

            graph.AddVertex(pgh);
            graph.AddVertex(nyc);
            graph.AddVertex(cin);
            graph.AddVertex(sd);
            graph.AddVertex(atl);
            graph.AddVertex(dal);
            graph.AddVertex(bos);

            graph.AddUndirectedEdge(pgh, cin, new Edge(5));
            graph.AddUndirectedEdge(sd, cin, new Edge(9));
            graph.AddUndirectedEdge(atl, cin, new Edge(5));
            graph.AddUndirectedEdge(atl, dal, new Edge(4));
            graph.AddUndirectedEdge(sd, dal, new Edge(3));
            graph.AddUndirectedEdge(pgh, nyc, new Edge(3));
            graph.AddUndirectedEdge(nyc, bos, new Edge(2));
            graph.AddUndirectedEdge(bos, sd, new Edge(400));

            return graph;
        }

        #endregion

        [TestMethod]
        public void Graph_Djk_Test()
        {
            Graph<string> graph = GenerateSimpleGraph();
            Search s = new Search();
            s.Dijkstra(pgh, sd);

            Assert.AreEqual(14, sd.CostToGetTo);

            graph.ResetCosts();

            s = new Search();
            s.Dijkstra(bos, dal);
            Assert.AreEqual(19, dal.CostToGetTo);
            graph.ResetCosts();

            s = new Search();
            s.Dijkstra(atl, sd);
            Assert.AreEqual(7, sd.CostToGetTo);
            graph.ResetCosts();

            s = new Search();
            s.Dijkstra(sd, atl);
            Assert.AreEqual(7, atl.CostToGetTo);
            graph.ResetCosts();
        }

    }
}


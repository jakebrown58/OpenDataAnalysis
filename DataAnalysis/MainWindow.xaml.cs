using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataElements.Model.Graph;

namespace DataAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //DataElements.DataStore.Database.DBReader dbReader = new DataElements.DataStore.Database.DBReader();

            Graph<string> g = GenerateSimpleGraph();
            Path<string> v = new Pathfinder<string>(g).FindPath(atl, sd);

            v = new Pathfinder<string>(g).FindPath(bos, sd);

            v = new Pathfinder<string>(g).FindPath(atl, bos);
            v = new Pathfinder<string>(g).FindPath(cin, dal);
            v = new Pathfinder<string>(g).FindPath(dal, nyc);
            v = new Pathfinder<string>(g).FindPath(pgh, sd);
            v = new Pathfinder<string>(g).FindPath(nyc, sd);

        }


        #region Test Setup

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

            graph.AddUndirectedEdge(pgh, cin, new Edge(50));
            graph.AddUndirectedEdge(sd, cin, new Edge(90));
            graph.AddUndirectedEdge(atl, cin, new Edge(50));
            graph.AddUndirectedEdge(atl, dal, new Edge(40));
            graph.AddUndirectedEdge(sd, dal, new Edge(30));
            graph.AddUndirectedEdge(pgh, nyc, new Edge(30));
            graph.AddUndirectedEdge(nyc, bos, new Edge(20));
            graph.AddUndirectedEdge(bos, sd, new Edge(400));

            return graph;
        }

        #endregion
    }
}

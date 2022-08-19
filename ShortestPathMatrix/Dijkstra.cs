using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Globalization;

using Autodesk.AutoCAD.DatabaseServices;

namespace ShortestPathMatrix
{
    public static class Dijkstra
    {
        /// <summary>
        /// Struct that represents a Node, with its label and position.
        /// </summary>
        public struct Node
        {
            public String label;
            public Double x;
            public Double y;
        }


        /// <summary>
        /// Struct that represents an Edge, with its start and end position, and length.
        /// </summary>
        struct Edge
        {
            public Double x_start;
            public Double y_start;
            public Double x_end;
            public Double y_end;
            public Double length;
        }


        /// <summary>
        /// Struct that has the Adjacency Matrix and the node list.
        /// </summary>
        public struct AdjmatrixAndNodelist
        {
            public Double[,] adjmatrix;
            public List<Node> nodelist;
        }


        /// <summary>
        /// Function that prompts the user to select a graph in the model (composed by blocknodes as nodes, polylines/lines as edges),
        /// and returns the Adjacency Matrix and the Node list.
        /// </summary>
        /// <param name="tr">AutoCAD Transaction</param>
        /// <returns>Returns Adjacency Matrix and Nodelist</returns>
        public static AdjmatrixAndNodelist GenerateAdjacencyMatrix(Transaction tr)
        {
            ObjectId[] selectedObjectsIdArray = MyCommands.SelectGraphCommand();

            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();

            if (selectedObjectsIdArray is null)
            {
                throw new Exception("Please select a graph.");
            }

            foreach (ObjectId blocknode_id in selectedObjectsIdArray)
            {
                if (blocknode_id.ObjectClass.Name == "AcDbBlockReference")
                {
                    BlockReference currentBlock = (BlockReference)tr.GetObject(blocknode_id, OpenMode.ForRead);
                    if (currentBlock.Name == "node")
                    {
                        Node node = new Node();
                        node.x = currentBlock.Position.X;
                        node.y = currentBlock.Position.Y;

                        foreach (ObjectId att_id in currentBlock.AttributeCollection)
                        {
                            AttributeReference att = (AttributeReference)tr.GetObject(att_id, OpenMode.ForRead);
                            node.label = att.TextString;
                        }
                        nodes.Add(node);
                    }
                }
                else if (blocknode_id.ObjectClass.Name == "AcDbPolyline")
                {
                    Polyline currentPolyline = (Polyline)tr.GetObject(blocknode_id, OpenMode.ForRead);
                    Edge edge = new Edge();
                    edge.x_start = currentPolyline.StartPoint.X;
                    edge.y_start = currentPolyline.StartPoint.Y;
                    edge.x_end = currentPolyline.EndPoint.X;
                    edge.y_end = currentPolyline.EndPoint.Y;
                    edge.length = currentPolyline.Length;
                    edges.Add(edge);

                }

                else if (blocknode_id.ObjectClass.Name == "AcDbLine")
                {
                    Line currentLine = (Line)tr.GetObject(blocknode_id, OpenMode.ForRead);
                    Edge edge = new Edge();
                    edge.x_start = currentLine.StartPoint.X;
                    edge.y_start = currentLine.StartPoint.Y;
                    edge.x_end = currentLine.EndPoint.X;
                    edge.y_end = currentLine.EndPoint.Y;
                    edge.length = currentLine.Length;
                    edges.Add(edge);

                }

            }
            nodes.Sort((node1, node2) => node1.label.CompareTo(node2.label));


            // Now we generate the adjacency matrix looping for each node, and for each edge, checking if the edge matches either
            // the start_point or end_point with node coordinates

            Double[,] adj_matrix = new Double[nodes.Count, nodes.Count];

            Dictionary<Tuple<Double, Double>, Tuple<String, int>> nodeDict = new Dictionary<Tuple<Double, Double>, Tuple<String, int>>();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeDict.Add(new Tuple<double, double>(nodes[i].x, nodes[i].y), new Tuple<String, int>(nodes[i].label, i));
            }


            foreach (Edge edge in edges)
            {
                if (nodeDict.ContainsKey(new Tuple<Double, Double>(edge.x_start, edge.y_start)) & nodeDict.ContainsKey(new Tuple<Double, Double>(edge.x_end, edge.y_end)))
                {
                    int i = nodeDict[new Tuple<Double, Double>(edge.x_start, edge.y_start)].Item2;
                    int j = nodeDict[new Tuple<Double, Double>(edge.x_end, edge.y_end)].Item2;
                    adj_matrix[i, j] = edge.length;
                    adj_matrix[j, i] = edge.length;
                }
                else
                {
                    continue;
                }

            }


            AdjmatrixAndNodelist result = new AdjmatrixAndNodelist();
            result.adjmatrix = adj_matrix;
            result.nodelist = nodes;

            return result;

        }


        /// <summary>
        /// Struct that represents the shortest distance from one node to another, and the route is needed to take to make that distance.
        /// </summary>
        public struct DistanceAndRoute
        {
            public Double distance;
            public List<int> route;
            public List<String> routeWithLabels;
        }


        /// <summary>
        /// Function that initializes the array of distances and routes from a starting nodeblock.
        /// As Dikjstra Algorithm says, it starts with Positive Infinite as the distance from the starting node to the other ones,
        /// except the distance to the same starting node, which is 0.
        /// </summary>
        /// <param name="s">Starting node index</param>
        /// <param name="len">Number of nodes in the graph</param>
        /// <param name="queue">Queue with the remaining vertex to visit</param>
        /// <returns>Returns an array of `DistanceAndRoute` elements.</returns>
        private static DistanceAndRoute[] Initialize(int s, int len, List<int> queue)
        {
            DistanceAndRoute[] dist_route = new DistanceAndRoute[len];
            for (int i = 0; i < len; i++)
            {
                dist_route[i] = new DistanceAndRoute();
                dist_route[i].distance = Double.PositiveInfinity;
                dist_route[i].route = new List<int>();
                dist_route[i].routeWithLabels = new List<String>();

                queue.Add(i);
            }

            dist_route[s].distance = 0;
            return dist_route;
        }


        /// <summary>
        /// Function used in the Dikjstra Algorithm to calculate next vertex to visit.
        /// </summary>
        /// <param name="queue">Remaining vertex to visit</param>
        /// <param name="distRoute">Current DistancesAndRoutes array</param>
        /// <returns></returns>
        private static int GetNextVertex(List<int> queue, DistanceAndRoute[] distRoute)
        {
            double min = Double.PositiveInfinity;
            int nextVertex = -1;

            foreach (int j in queue)
            {
                if (distRoute[j].distance <= min)
                {
                    min = distRoute[j].distance;
                    nextVertex = j;
                }
            }

            queue.Remove(nextVertex);
            return nextVertex;
        }



        /// <summary>
        /// Function that takes the Adjacency Matrix and starting node, and return the shortest distance from
        /// that node to every other node, with its corresponding route.
        /// </summary>
        /// <param name="G">Adjacency Matrix</param>
        /// <param name="s">Starting Node Index</param>
        /// <param name="nodeList">List of graph nodes</param>
        /// <returns>Returns DistanceAndRoute array with shortest distance from starting node to every other node, with its routes</returns>
        /// <exception cref="ArgumentException">Error exception, wrong format.</exception>
        public static DistanceAndRoute[] PerformDikjstra(double[,] G, int s, List<Node> nodeList)
        {
            /* Check graph format and that the graph actually contains something */
            if (G.GetLength(0) < 1 || G.GetLength(0) != G.GetLength(1))
            {
                throw new ArgumentException("Graph error, wrong format or no nodes to compute");
            }

            int len = G.GetLength(0);
            DistanceAndRoute[] distRoute;
            List<int> queue = new List<int>();


            distRoute = Initialize(s, len, queue);

            while (queue.Count > 0)
            {
                int u = GetNextVertex(queue, distRoute);

                for (int v = 0; v < len; v++)
                {
                    if (G[u, v] > 0)
                    {
                        /* Edge exists, relax the edge */
                        if (distRoute[v].distance > distRoute[u].distance + G[u, v])
                        {
                            distRoute[v].distance = distRoute[u].distance + G[u, v];
                            distRoute[v].route = new List<int>(distRoute[u].route);
                            distRoute[v].routeWithLabels = new List<String>(distRoute[u].routeWithLabels);
                            if (u != s)
                            {
                                distRoute[v].route.Add(u);
                                distRoute[v].routeWithLabels.Add(nodeList[u].label);
                            }
                        }
                    }
                }
            }
            return distRoute;
        }


        /// <summary>
        /// Function that generates the shortest path matrix and writes 2 csv files,
        /// one with the shortest distance and other with the route, from every node to every other node.
        /// </summary>
        /// <param name="acCurDb">AutoCAD database</param>
        /// <param name="outputPath">Output path where CSV files will be saved</param>
        public static void GenerateShortestPathMatrix(Database acCurDb, String outputPath)
        {

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {

                AdjmatrixAndNodelist adjmatrixAndNodelist = new AdjmatrixAndNodelist();
                adjmatrixAndNodelist = GenerateAdjacencyMatrix(acTrans);

                Double[,] adj_matrix = adjmatrixAndNodelist.adjmatrix;
                List<Node> nodeslist = adjmatrixAndNodelist.nodelist;


                List<DistanceAndRoute[]> shortestPathMatrix;
                shortestPathMatrix = new List<DistanceAndRoute[]>();

                // Here we create the shortest_path_matrix applying Dijkstra for every node in the graph
                for (int i = 0; i < nodeslist.Count; i++)
                {
                    DistanceAndRoute[] dist_node_i;
                    dist_node_i = PerformDikjstra(adj_matrix, i, nodeslist);
                    shortestPathMatrix.Add(dist_node_i);

                }

                // Here we're going to generate 2 csv:
                // 1.- Shortest distance from each node to every other node
                // 2.- Shortest route from each node to every other node

                StreamWriter outfile1 = new StreamWriter(outputPath + @"\" + "shortest_path_matrix_distances.csv", false, System.Text.Encoding.UTF8);
                StreamWriter outfile2 = new StreamWriter(outputPath + @"\" + "shortest_path_matrix_routes.csv", false, System.Text.Encoding.UTF8);

                CultureInfo culture = CultureInfo.CreateSpecificCulture("es-ES");
                Thread.CurrentThread.CurrentCulture = culture;
                String ListSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;


                string header = ListSeparator;
                for (int i = 0; i < shortestPathMatrix.Count; i++)
                {
                    header += adjmatrixAndNodelist.nodelist[i].label + ListSeparator;
                }
                outfile1.WriteLine(header);
                outfile2.WriteLine(header);


                for (int i = 0; i < shortestPathMatrix.Count; i++)
                {
                    string content_dist = "";
                    string content_routes = "";

                    content_dist += adjmatrixAndNodelist.nodelist[i].label + ListSeparator;
                    content_routes += adjmatrixAndNodelist.nodelist[i].label + ListSeparator;


                    for (int j = 0; j < shortestPathMatrix.Count; j++)
                    {
                        content_dist += shortestPathMatrix[i][j].distance.ToString("0.00") + ListSeparator;
                        content_routes += String.Join(" -> ", shortestPathMatrix[i][j].routeWithLabels.ToArray()) + ListSeparator;
                    }
                    outfile1.WriteLine(content_dist);
                    outfile2.WriteLine(content_routes);
                }

                outfile1.Close();
                outfile2.Close();
            }
        }


    }
}

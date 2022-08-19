using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;

namespace ShortestPathMatrix
{
    public static class SampleGraphLoader
    {
        /// <summary>
        ///  Struct that represents each line of polylines.csv file, stored in Resources folder. Each line corresponds to a vertex of a polyline,
        ///  which will be an edge of the sample graph.
        /// </summary>
        struct PolylineVertex
        {
            public int id;
            public int index;
            public double x;
            public double y;
        }


        /// <summary>
        ///  Function that reads CSV file stored in Resources folder, this file contains the information regarding the graph's edges. 
        ///  CSV has 4 columns (idPolyline, vertexIndex, x coordinate, y coordinate)
        /// </summary>
        /// <param name="listSeparator">List separator char in csv file (examples: "," or ";")</param>
        /// <param name="fileName">CSV file name inlcuding extension stored in Resources folder (example: "polylines.csv")</param>
        /// <returns>Dictionary of `Polyline` CAD objects, corresponding to graph's edges</returns>
        public static Dictionary<int, Polyline> GetEdges(String listSeparator, String fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = "ShortestPathMatrix.Resources." + fileName;

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))

            {
                Dictionary<int, Polyline> result = new Dictionary<int, Polyline>();
                while (!reader.EndOfStream)
                {
                    string str;
                    string[] strArray;

                    str = reader.ReadLine();
                    strArray = str.Split(char.Parse(listSeparator));

                    PolylineVertex currentVertex = new PolylineVertex();
                    currentVertex.id = int.Parse(strArray[0]);
                    currentVertex.index = Int32.Parse(strArray[1]);
                    currentVertex.x = double.Parse(strArray[2]);
                    currentVertex.y = double.Parse(strArray[3]);

                    int vertex_idx = 0;


                    if (result.ContainsKey(currentVertex.id))
                    {
                        result[currentVertex.id].AddVertexAt(vertex_idx, new Point2d(currentVertex.x, currentVertex.y), 0, 0, 0);
                        vertex_idx++;

                    }
                    else
                    {
                        result[currentVertex.id] = new Polyline();
                        vertex_idx = 0;
                        result[currentVertex.id].AddVertexAt(vertex_idx, new Point2d(currentVertex.x, currentVertex.y), 0, 0, 0);
                    }
                }
                return result;
            }
        }



        /// <summary>
        ///  Function that reads CSV file stored in Resources folder, this file contains the information regarding the graph's nodes. 
        ///  CSV has 3 columns (nodeLabel, x coordinate, y coordinate)
        /// </summary>
        /// <param name="listSeparator">List separator char in csv file (examples: "," or ";")</param>
        /// <param name="fileName">CSV file name inlcuding extension stored in Resources folder (example: "nodes.csv")</param>
        /// <returns>Dictionary of `Polyline` CAD objects, corresponding to graph's edges</returns>
        public static List<Dijkstra.Node> GetNodes(String listSeparator, String fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = "ShortestPathMatrix.Resources." + fileName;

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                List<Dijkstra.Node> result = new List<Dijkstra.Node>();
                while (!reader.EndOfStream)
                {
                    string str;
                    string[] strArray;

                    str = reader.ReadLine();
                    strArray = str.Split(char.Parse(listSeparator));

                    Dijkstra.Node currentNode = new Dijkstra.Node();
                    currentNode.label = strArray[0];
                    currentNode.x = double.Parse(strArray[1]);
                    currentNode.y = double.Parse(strArray[2]);

                    result.Add(currentNode);

                }
                return result;
            }
        }

        /// <summary>
        /// Struct composed by `BlockTable` and `BlockTableRecord`. Used in `GetCurrentModelSpace` method.
        /// </summary>
        public struct BlkTbl
        {
            public BlockTable acBlkTbl;
            public BlockTableRecord acBlkTblRec;
        }

        /// <summary>
        /// Function that returns current Model Space BlockTable for write.
        /// </summary>
        /// <param name="acTrans">AutoCAD Transaction</param>
        /// <param name="acCurDb">AutoCAD Current Database</param>
        /// <returns>Returns current Model Space in form of BlkTbl struct.</returns>
        public static BlkTbl GetCurrentModelSpace(Transaction acTrans, Database acCurDb)
        {
            BlockTable acBlkTbl;
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                         OpenMode.ForWrite) as BlockTable;

            // Open the Block table record Model space for write
            BlockTableRecord acBlkTblRec;
            acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                            OpenMode.ForWrite) as BlockTableRecord;

            BlkTbl result = new BlkTbl();
            result.acBlkTbl = acBlkTbl;
            result.acBlkTblRec = acBlkTblRec;
            return result;
        }




        /// <summary>
        /// Function that draws into model a sample graph where edges come from `GetEdges` method, and nodes from `GetNodes`.
        /// These methods read CSV files stored in Resources folder, and contain the info of the edges and the nodes of the sample graph.
        /// </summary>
        /// <param name="acDoc">AutoCAD document.</param>
        /// <param name="acCurDb">AutoCAD Current Database</param>
        /// <param name="graphSampleIndex">Index selecting sample graph, example: "graph1", or "graph2"</param>
        /// <param name="nodeType">Options: "circle" or "leader"</param>
        public static void InsertSampleGraph(Document acDoc, Database acCurDb, String graphSampleIndex, String nodeType)
        {
            Dictionary<int, Polyline> polylines;
            polylines = GetEdges(";", graphSampleIndex + "_polylines.csv");

            List<Dijkstra.Node> nodes;
            nodes = GetNodes(";", graphSampleIndex + "_nodes.csv");

            // Starts a new transaction with the Transaction Manager
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlkTbl bt = GetCurrentModelSpace(acTrans, acCurDb);
                BlockTableRecord acBlkTblRec = bt.acBlkTblRec;

                foreach (int plId in polylines.Keys)
                {
                    acBlkTblRec.AppendEntity(polylines[plId]);
                    acTrans.AddNewlyCreatedDBObject(polylines[plId], true);
                }

                List<Entity> blockNodeEntities = BlockNodeCreator.CircleBlockNodeEntities(acCurDb, new Point3d(0, 0, 0));
                if (nodeType == "circle")
                {
                    blockNodeEntities = BlockNodeCreator.CircleBlockNodeEntities(acCurDb, new Point3d(0, 0, 0));

                }
                else if (nodeType == "leader")
                {
                    blockNodeEntities = BlockNodeCreator.LeaderBlockNodeEntities(acCurDb, new Point3d(0, 0, 0));
                }

                BlockNodeCreator.InsertBlockNodeToDb(bt.acBlkTbl, acDoc, acCurDb, "node", blockNodeEntities);

                for (var i = 0; i < nodes.Count; i++)
                {
                    BlockNodeCreator.DrawBlockNodeToModel(bt.acBlkTbl, bt.acBlkTblRec, "node", nodes[i].label, new Point3d(nodes[i].x, nodes[i].y, 0));
                }

                acTrans.Commit();
            }

            //using InvokeMember to support .NET 3.5
            Object acadObject = Application.AcadApplication;
            acadObject.GetType().InvokeMember("ZoomExtents",
                        BindingFlags.InvokeMethod, null, acadObject, null);
        }
    }
}
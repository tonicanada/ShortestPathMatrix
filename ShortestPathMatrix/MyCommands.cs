using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;


[assembly: CommandClass(typeof(ShortestPathMatrix.MyCommands))]

namespace ShortestPathMatrix
{
    public class MyCommands
    {


        [CommandMethod("insert_sample_graph")]
        public static void InsertSampleGraphCommand()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            SampleGraphLoader.InsertSampleGraph(acDoc, acCurDb, "graph2", "leader");
        }


        [CommandMethod("select_graph")]
        public static ObjectId[] SelectGraphCommand()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            ObjectId[] selectionObjectsIdArray;
            selectionObjectsIdArray = GraphModelSelector.selectGraph(acDoc, "Please select the graph");
            return selectionObjectsIdArray;
        }


        [CommandMethod("shortestpath")]
        public static void ShortestPathMatrixCommand()
        {
            FormInterface form = new FormInterface();
            Application.ShowModelessDialog(form);
        }

    }
}

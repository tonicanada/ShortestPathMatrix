using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ShortestPathMatrix
{
    public class BlockNodeCreator
    {

        /// <summary>
        /// Method that generates the entities to build a node block with the shape of a circle with a letter inside.
        /// </summary>
        /// <param name="db">AutoCAD database</param>
        /// <param name="basePoint">Nodeblock basepoint</param>
        /// <returns>Entities list to build the block</returns>
        public static List<Entity> CircleBlockNodeEntities(Database db, Point3d basePoint)
        {
            List<Entity> result = new List<Entity>();

            Circle circ = new Circle();
            circ.Center = basePoint;
            circ.Radius = 2.5;

            AttributeDefinition attr = new AttributeDefinition();
            attr.Tag = "Node name";
            attr.Prompt = "Node name:";
            //attr.TextString = "A";
            attr.Verifiable = true;
            attr.LockPositionInBlock = true;
            attr.TextStyleId = db.Textstyle;
            attr.Height = 1.75;
            attr.Justify = AttachmentPoint.MiddleCenter;

            result.Add(circ);
            result.Add(attr);

            return result;

        }

        /// <summary>
        /// Method that generates the entities to build a node block with the shape of leader line with a label.
        /// </summary>
        /// <param name="db">AutoCAD database</param>
        /// <param name="basePoint">Nodeblock basepoint</param>
        /// <returns>Entities list to build the block</returns>
        public static List<Entity> LeaderBlockNodeEntities(Database db, Point3d basePoint)
        {
            List<Entity> result = new List<Entity>();

            Polyline pl = new Polyline();
            pl.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            pl.AddVertexAt(1, new Point2d(3, 3), 0, 0, 0);
            pl.AddVertexAt(2, new Point2d(13, 3), 0, 0, 0);

            Circle circ = new Circle();
            circ.Center = basePoint;
            circ.Radius = 0.5;

            AttributeDefinition attr = new AttributeDefinition();
            attr.Tag = "Station#";
            attr.Prompt = "Station number:";
            //attr.TextString = "Barcelona";
            attr.Verifiable = true;
            attr.LockPositionInBlock = true;
            attr.TextStyleId = db.Textstyle;
            attr.Height = 1.0;
            attr.Position = new Point3d(6.0, 3.2, 0);
            attr.AdjustAlignment(db);

            result.Add(pl);
            result.Add(circ);
            result.Add(attr);

            return result;
        }


        /// <summary>
        /// Function that creates a block into the model's database, defined by list of entities.
        /// </summary>
        /// <param name="bt">AutoCAD BlockTable</param>
        /// <param name="acDoc">AutoCAD Document</param>
        /// <param name="db">AutoCAD Database</param>
        /// <param name="blockName">Block's name</param>
        /// <param name="blockEntities">List of entities to build the block</param>
        public static void InsertBlockNodeToDb(BlockTable bt, Document acDoc, Database db, String blockName, List<Entity> blockEntities)
        {
            using (Transaction myT = db.TransactionManager.StartTransaction())
            {
                Editor ed = acDoc.Editor;
                if (bt.Has(blockName))
                {
                    ed.WriteMessage($"Block {blockName} already exists");
                    return;
                }
                BlockTableRecord btr = new BlockTableRecord();
                bt.Add(btr);
                btr.Name = blockName;
                Point3d base_point = new Point3d(0, 0, 0);
                btr.Origin = base_point;
                btr.BlockScaling = BlockScaling.Uniform;
                btr.Explodable = true;

                foreach (Entity ent in blockEntities)
                {
                    btr.AppendEntity(ent);
                    myT.AddNewlyCreatedDBObject(ent, true);
                }

                myT.AddNewlyCreatedDBObject(btr, true);
                myT.Commit();
            }
        }


        /// <summary>
        /// Function that inserts a block into the model.
        /// </summary>
        /// <param name="bt">AutoCAD BlockTable</param>
        /// <param name="btr">AutoCAD BlockTableRecord</param>
        /// <param name="blockName">Node block's name</param>
        /// <param name="nodeLabel">Node Label</param>
        /// <param name="origin">Node Position</param>
        public static void DrawBlockNodeToModel(BlockTable bt, BlockTableRecord btr, String blockName, String nodeLabel, Point3d origin)
        {
            BlockTableRecord btrNode = bt[blockName].GetObject(OpenMode.ForRead) as BlockTableRecord;

            using (BlockReference blockRef = new BlockReference(origin, btrNode.ObjectId))
            {
                btr.AppendEntity(blockRef);
                foreach (ObjectId id in btrNode)
                //Iterate block definition to find all non-constant
                // AttributeDefinitions
                {
                    DBObject obj = id.GetObject(OpenMode.ForRead);
                    AttributeDefinition attDef = obj as AttributeDefinition;
                    if ((attDef != null) && (!attDef.Constant))
                    {
                        //This is a non-constant AttributeDefinition
                        //Create a new AttributeReference
                        using (AttributeReference attRef = new AttributeReference())
                        {
                            attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
                            attRef.TextString = nodeLabel;
                            //Add the AttributeReference to the BlockReference
                            blockRef.AttributeCollection.AppendAttribute(attRef);
                        }
                    }
                }
            }
        }
    }
}

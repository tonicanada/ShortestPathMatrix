using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace ShortestPathMatrix
{
    public class GraphModelSelector
    {

        public static ObjectId[] selectGraph(Document acDoc, String selection_prompt_message, SelectionFilter filtro_entidades = null)
        {
            PromptSelectionResult acSPrompt;
            PromptSelectionOptions PtSelOpts = new PromptSelectionOptions();
            PtSelOpts.MessageForAdding = $"\n{selection_prompt_message}";

            acSPrompt = acDoc.Editor.GetSelection(PtSelOpts, filtro_entidades);

            SelectionSet acSSetBlocks;

            if (acSPrompt.Status == PromptStatus.OK)
            {
                acSSetBlocks = acSPrompt.Value;
                ObjectId[] objIdArrayTotal = acSSetBlocks.GetObjectIds();
                return objIdArrayTotal;

            }
            else
            {
                return null;
            }
        }
    }
}

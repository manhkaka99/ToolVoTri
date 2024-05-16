using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class RemoveText : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            IList<ElementId> view = (IList<ElementId>)uidoc.Selection.GetElementIds();
            IList<Element> text = new List<Element>();
            IList<Element> viewPort = new List<Element>();

            FilteredElementCollector type_ids = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Viewports)
                .WhereElementIsNotElementType();
            //ElementId type = ((from typeid in new FilteredElementCollector(doc)
            //                            .OfClass(typeof(ElementType)).OfCategory(BuiltInCategory.OST_Viewports)
            //                            .Cast<ElementType>()
            //                       where typeid.Name == "ビュー名"
            //                       select typeid).First()).GetTypeId();
            IList<Element> type_id = new List<Element>();
            foreach (Element i in type_ids)
            {
                if (i.Name == "ビュー名")
                {
                    type_id.Add(i);
                }
            }
            ElementId type = type_id[0].GetTypeId();
            foreach (ElementId elementId in view)
            {
                ElementId viewId;
                Element check = doc.GetElement(elementId);
                if (check.Category.Name == "Viewports")
                {
                    viewId = (check as Viewport).ViewId;
                    viewPort.Add(check);
                }
                else if (check.Category.Name == "Views")
                {
                    viewId = (check as View).Id;
                }
                else
                {
                    return Result.Cancelled;
                }
                FilteredElementCollector texts = new FilteredElementCollector(doc, viewId)
                                    .OfCategory(BuiltInCategory.OST_TextNotes)
                                    .WhereElementIsNotElementType();

                foreach (Element b in texts)
                {
                    if (b.Name == "Meiryo 2.0mm")
                    {
                        text.Add(b);
                    }
                }
            }
            using (Transaction trans = new Transaction(doc, "Remove Text"))
            {
                trans.Start();

                foreach (Element e in text)
                {
                    doc.Delete(e.Id);
                }
                foreach (Element e in viewPort)
                {
                    e.ChangeTypeId(type);
                }
                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}

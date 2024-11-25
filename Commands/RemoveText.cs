using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

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

            ElementType viewPortType = (from type in new FilteredElementCollector(doc)
                                                      .WhereElementIsElementType()
                                                      .Cast<ElementType>()
                                        where type.Name == "ビュー名" && type.FamilyName == "Viewport"
                                        select type).First();

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
                    e.ChangeTypeId(viewPortType.Id);
                }
                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimIshou.ShowGrid
{
    [Transaction(TransactionMode.Manual)]
    public class Level2D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector levels = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType();
            using (Transaction trans = new Transaction(doc, "Convert Level 2D"))
            {
                trans.Start();
                foreach (Element element in levels)
                {
                    Level level = element as Level;
                    level.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.ViewSpecific);
                    level.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.ViewSpecific);
                }
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
}

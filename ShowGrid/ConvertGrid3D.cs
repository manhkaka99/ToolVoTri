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
    public class ConvertGrid3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector grids = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Grids)
                .WhereElementIsNotElementType();
            using (Transaction trans = new Transaction(doc, "Convert Grid 3D"))
            {
                trans.Start();
                foreach (Element element in grids)
                {
                    Grid grid = element as Grid;
                    grid.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.Model);
                    grid.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.Model);
                }
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
}

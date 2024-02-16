using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Test : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var all_title_block = new FilteredElementCollector(doc).
                WhereElementIsNotElementType().
                OfCategory(BuiltInCategory.OST_Sheets).
                ToElements();
            List<ViewSheet> all = new List<ViewSheet>();
            
            foreach (Element item in all_title_block)
            {
                ViewSheet a = item as ViewSheet;

                all.Add(a);
            }
            List<ViewSheet> sortedSheets = all.OrderBy(sheet => sheet.SheetNumber).ToList();
            ShowSheetListOnDialog(sortedSheets, uidoc);
            return Result.Succeeded;
        }
        public void ShowSheetListOnDialog(List<ViewSheet> sheets, UIDocument uidoc)
        {
            TaskDialog dialog = new TaskDialog("Sheet List");
            dialog.MainContent = "List of Sheets:";
            
            foreach (ViewSheet sheet in sheets)
            {
                dialog.MainContent += "\n" + sheet.SheetNumber;
            }

            dialog.Show();
        }
    }
}

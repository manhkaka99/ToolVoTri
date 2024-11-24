using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BimIshou.CheckTagWall
{
    [Transaction(TransactionMode.Manual)]
    public class CheckTagWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            CheckTagWallWindow checkTagWallWindow = new CheckTagWallWindow(uidoc);
            checkTagWallWindow.Show();
            
            return Result.Succeeded;
        }
    }
}

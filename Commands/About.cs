
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Nice3point.Revit.Toolkit.External;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Markup;

#endregion

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Aboout : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TaskDialog.Show("Tiếp thêm năng lượng", "Chúc mọi người một ngày làm việc thật hiệu quả. Em yêu tất cả mọi người! <3");
            return Result.Succeeded;
        }
    }
}

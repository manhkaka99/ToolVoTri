#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BimIshou.Utils;
using BimIshou.WPF;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MahApps.Metro.Controls;
using System.Reflection;
using System.Windows.Controls;
using Grid = Autodesk.Revit.DB.Grid;
#endregion

namespace BimIshou.ShowGrid
{
    [Transaction(TransactionMode.Manual)]
    public class VisibilityGridLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            VisibilityGrid window3 = new VisibilityGrid(uidoc);
            window3.ShowDialog();

            return Result.Succeeded;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimIshou.PointFloorSite;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;
using MahApps.Metro.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BimIshou.PointFloorSite
{
    [Transaction(TransactionMode.Manual)]
    public class FloorSite : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                FloorSiteWPF window = new FloorSiteWPF(uidoc);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }


            return Result.Succeeded;
        }
    }
}

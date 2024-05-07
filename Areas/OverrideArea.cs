#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BimIshou.Areas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using MahApps.Metro.Controls;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion
namespace BimIshou.Areas
{
    [Transaction(TransactionMode.Manual)]
    public class OverrideArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            OverideAreaWindow window1 = new OverideAreaWindow(uidoc);
            window1.ShowDialog();
            #region Code cu
            //LocArea locArea = new LocArea();
            //FilteredElementCollector collector = new FilteredElementCollector(doc)
            //.OfClass(typeof(LinePatternElement));
            //LinePatternElement linePatternElement = collector
            //.Cast<LinePatternElement>()
            //.FirstOrDefault(pattern => pattern.Name == "3HA01");

            //    OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            //    overrideGraphicSettings.SetProjectionLinePatternId(linePatternElement.Id);
            //    overrideGraphicSettings.SetProjectionLineWeight(3);

            //    FilteredElementCollector rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            //    FilteredElementCollector areaBounds = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_AreaSchemeLines).WhereElementIsNotElementType();



            //    using (Transaction trans = new Transaction(doc, "Overide Area"))
            //    {
            //        trans.Start();

            //        foreach (Element area in areaBounds)
            //        {


            //            foreach (Element ele in rooms)
            //            {
            //                Room room = ele as Room;
            //                Line line = (area.Location as LocationCurve).Curve as Line;

            //                BoundingBoxXYZ boundingBox = room.get_BoundingBox(doc.ActiveView);

            //                if (room.IsPointInRoom(line.Evaluate(0.5, true)) == true && IsPointOnRoomBoundary(room, line.Evaluate(0.5, true)) == false)
            //                {
            //                    doc.ActiveView.SetElementOverrides(area.Id, overrideGraphicSettings);
            //                }
            //            }
            //        }
            //        trans.Commit();
            //    }
            //    return Result.Succeeded;

            //}
            //private bool IsPointOnRoomBoundary(Room room, XYZ testPoint)
            //{
            //    SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            //    options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center;
            //    IList<IList<BoundarySegment>> boundaryId = room.GetBoundarySegments(options);
            //    IList<Curve> roomBoundaries = new List<Curve>();

            //    foreach (IList<BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            //    {
            //        foreach (BoundarySegment boundSeg in boundSegList)
            //        {
            //            //Element e = room.Document.GetElement(boundSeg.ElementId);
            //            //Wall wall = e as Wall;
            //            //LocationCurve locationCurve = wall.Location as LocationCurve;
            //            //Curve curve = locationCurve.Curve;
            //            roomBoundaries.Add(boundSeg.GetCurve());
            //        }
            //    }

            //    foreach (Curve curve in roomBoundaries)
            //    {
            //        if (curve.Distance(testPoint) < 0.01) // Điều chỉnh độ chính xác cần thiết
            //        {
            //            return true;
            //        }
            //    }
            #endregion
            return Result.Succeeded;
        }
    }

}

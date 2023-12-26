#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion
namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class OverideArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            LocArea locArea = new LocArea();
            FilteredElementCollector collector = new FilteredElementCollector(doc)
            .OfClass(typeof(LinePatternElement));
            LinePatternElement linePatternElement = collector
            .Cast<LinePatternElement>()
            .FirstOrDefault(pattern => pattern.Name == "3HA01");

            OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            overrideGraphicSettings.SetProjectionLinePatternId(linePatternElement.Id);
            overrideGraphicSettings.SetProjectionLineWeight(3);

            FilteredElementCollector rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            FilteredElementCollector areaBounds = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_AreaSchemeLines).WhereElementIsNotElementType();



            using (Transaction trans = new Transaction(doc, "Overide Area"))
            {
                trans.Start();

                foreach (Element area in areaBounds)
                {

                    foreach (Element ele in rooms)
                    {
                        Room room = ele as Room;
                        Line line = (area.Location as LocationCurve).Curve as Line;

                        if (room.IsPointInRoom(line.Evaluate(0.5, true)) == true)
                        {
                            doc.ActiveView.SetElementOverrides(area.Id, overrideGraphicSettings);
                        }
                    }
                }
                trans.Commit();
            }
            return Result.Succeeded;

        }
    }
    
    class LocArea : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            return elem.Category.Name.Equals("<Area Boundary>");
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

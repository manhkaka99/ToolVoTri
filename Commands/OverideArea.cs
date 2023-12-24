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

            while (true)
            {
                try
                {


                    Reference r = uidoc.Selection.PickObject(ObjectType.Element, locArea, "Chọn đối tượng Area Boundary cần Overide");


                    ElementId a = doc.GetElement(r).Id;

                    OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
                    overrideGraphicSettings.SetProjectionLinePatternId(linePatternElement.Id);
                    overrideGraphicSettings.SetProjectionLineWeight(4);
                    using (Transaction trans = new Transaction(doc, "Overide Area"))
                    {
                        trans.Start();
                        doc.ActiveView.SetElementOverrides(a, overrideGraphicSettings);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Succeeded;
                }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class FloorSite : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            LocFloor locFloor = new LocFloor();

            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, locFloor, "Chọn sàn đi bạn eiii");

            foreach (Reference reference in references)
            {
                Element ele = doc.GetElement(reference);
                Floor floor = ele as Floor;
                BoundingBoxXYZ boundingBox = floor.get_BoundingBox(doc.ActiveView);
                XYZ maxPoint = boundingBox.Max;
                XYZ minPoint = boundingBox.Min; 
                Outline myOutLn = new Outline(minPoint, maxPoint);
                BoundingBoxIntersectsFilter boundingBoxFilter = new BoundingBoxIntersectsFilter(myOutLn);
                FilteredElementCollector sites = new FilteredElementCollector(doc).WherePasses(boundingBoxFilter).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Site);
                IList<Element> list = new List<Element>();
                foreach (Element site in sites)
                {
                    if (site.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "d外構_レベル標_隅" 
                        || site.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "d外構_レベル標_中心")
                    {
                        list.Add(site);
                    }
                }
                using (Transaction trans = new Transaction(doc, "Add Point to the Floor."))
                {
                    trans.Start();
                    foreach (Element site in list)
                    {
                        LocationPoint location = site.Location as LocationPoint;
                        XYZ a = location.Point;
                        Double point = site.LookupParameter("計画レベル").AsDouble();
                        XYZ pos = new XYZ(a.X, a.Y, point);

                        floor.SlabShapeEditor.DrawPoint(pos);

                    }
                    trans.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
    class LocFloor : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Name.Equals("Floors");
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

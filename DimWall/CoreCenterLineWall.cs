namespace BimIshou.DimWall;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

[Transaction(TransactionMode.Manual)]
internal class ExeCreateWallCenterlines : IExternalCommand
{
    private DetailCurve _trex_createLineSegment(Document doc, ViewPlan vplan, XYZ sp, XYZ ep, GraphicsStyle gstyle)
    {
        Line line = Line.CreateBound(sp, ep);
        XYZ endpoint = line.Evaluate(0, false);
        XYZ endpoint2 = line.Evaluate(line.Length, false);
        Line geometryCurve = Line.CreateBound(endpoint, endpoint2);
        DetailCurve detailCurve = doc.Create.NewDetailCurve(vplan, geometryCurve);
        if (gstyle != null)
        {
            detailCurve.LineStyle = gstyle;
        }
        return detailCurve;
    }
    private DetailCurve _trex_createLineSegmentSec(Document doc, ViewSection vsection, XYZ sp, XYZ ep, GraphicsStyle gstyle)
    {
        Line line = Line.CreateBound(sp, ep);
        XYZ endpoint = line.Evaluate(0, false);
        XYZ endpoint2 = line.Evaluate(line.Length, false);
        Line geometryCurve = Line.CreateBound(endpoint, endpoint2);
        DetailCurve detailCurve = doc.Create.NewDetailCurve(vsection, geometryCurve);
        if (gstyle != null)
        {
            detailCurve.LineStyle = gstyle;
        }
        return detailCurve;
    }
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication application = commandData.Application;
        Application application2 = application.Application;
        UIDocument activeUIDocument = application.ActiveUIDocument;
        Document document = activeUIDocument.Document;
        Result result;
        try
        {
            ViewPlan viewPlan = document.ActiveView as ViewPlan;
            ViewSection viewSection = document.ActiveView as ViewSection;
            if (viewPlan == null && viewSection == null) return Result.Failed;
            GraphicsStyle graphicsStyle = new FilteredElementCollector(document)
                .OfClass(typeof(GraphicsStyle))
                .Cast<GraphicsStyle>()
                .Where(x => x.GraphicsStyleCategory.Name.Equals("1TS01-1"))
                .FirstOrDefault();
            if (graphicsStyle is not { }) return Result.Failed;
            List<Wall> list2 = [];
            ICollection<ElementId> elementIds = activeUIDocument.Selection.GetElementIds();
            if (elementIds.Count == 0)
            {
                FilteredElementCollector filteredElementCollector2 = (viewPlan != null) ? new FilteredElementCollector(document, viewPlan.Id) : new FilteredElementCollector(document, viewSection.Id);
                filteredElementCollector2.OfClass(typeof(Wall));
                using (IEnumerator<Element> enumerator2 = filteredElementCollector2.ToElements().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Element element = enumerator2.Current;
                        Wall wall = element as Wall;
                        if (wall != null && wall.WallType.Kind == WallKind.Basic && Math.Round(wall.Orientation.Z, 6) == 0.0)
                        {
                            list2.Add(wall);
                        }
                    }
                    goto IL_529;
                }
            }
            foreach (ElementId id2 in elementIds)
            {
                Wall wall2 = document.GetElement(id2) as Wall;
                if (wall2 != null && wall2.WallType.Kind == WallKind.Basic && Math.Round(wall2.Orientation.Z, 6) == 0.0)
                {
                    list2.Add(wall2);
                }
            }
        IL_529:
            if (viewPlan == null)
            {
                List<Wall> list3 = [];
                XYZ origin = viewSection.Origin;
                XYZ source = viewSection.ViewDirection.Normalize();
                foreach (Wall wall3 in list2)
                {
                    LocationCurve locationCurve = wall3.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        Line line = locationCurve.Curve as Line;
                        if (!(line == null) && Math.Abs((line.GetEndPoint(1) - line.GetEndPoint(0)).Normalize().DotProduct(source)) == 1.0)
                        {
                            list3.Add(wall3);
                        }
                    }
                }
                list2 = list3;
            }
            if (list2.Count <= 0)
            {
                result = Result.Cancelled;
            }
            else
            {
                List<ElementId> list4 = [];
                using (Transaction transaction = new(document, "Core Center Line Wall"))
                {
                    transaction.Start();
                    ExeCreateWallCenterlines.WallFilter selectionFilter = new();
                    ICollection<ElementId> elementIds2 = activeUIDocument.Selection.GetElementIds();
                    if (!elementIds2.Any<ElementId>())
                    {
                        foreach (Reference reference in activeUIDocument.Selection.PickObjects(ObjectType.Element, selectionFilter))
                        {
                            elementIds2.Add(reference.ElementId);
                        }
                    }
                    if (elementIds2.Count == 0)
                    {
                        return Result.Failed;
                    }
                    foreach (ElementId id3 in elementIds2)
                    {
                        Wall wall4 = document.GetElement(id3) as Wall;
                        if (wall4 != null)
                        {
                            CompoundStructure compoundStructure = wall4.WallType.GetCompoundStructure();
                            if (compoundStructure != null)
                            {
                                if (!compoundStructure.IsVerticallyHomogeneous())
                                {
                                    elements.Insert(wall4);
                                    list4.Add(wall4.Id);
                                }
                                else
                                {
                                    bool flag3 = false;
                                    using (IEnumerator<CompoundStructureLayer> enumerator6 = compoundStructure.GetLayers().GetEnumerator())
                                    {
                                        while (enumerator6.MoveNext())
                                        {
                                            if (enumerator6.Current.Function == MaterialFunctionAssignment.Structure)
                                            {
                                                flag3 = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (flag3)
                                    {
                                        double offsetForLocationLine = compoundStructure.GetOffsetForLocationLine(WallLocationLine.CoreCenterline);
                                        LocationCurve locationCurve2 = wall4.Location as LocationCurve;
                                        if (locationCurve2 != null)
                                        {
                                            Line line2 = locationCurve2.Curve as Line;
                                            if (!(line2 == null))
                                            {
                                                XYZ source2 = new(0.0, 0.0, 1.0);
                                                XYZ xyz = (line2.GetEndPoint(1) - line2.GetEndPoint(0)).CrossProduct(source2).Normalize();
                                                xyz = wall4.Orientation;
                                                xyz *= offsetForLocationLine;
                                                Line line3 = line2.CreateTransformed(Transform.CreateTranslation(xyz)) as Line;
                                                if (viewPlan == null)
                                                {
                                                    Parameter parameter = wall4.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                                                    Parameter parameter2 = wall4.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
                                                    Parameter parameter3 = wall4.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                                                    double z = (parameter3 != null) ? parameter3.AsDouble() : 0.0;
                                                    document.GetElement(wall4.LevelId);
                                                    BoundingBoxXYZ boundingBoxXYZ = wall4.get_BoundingBox(viewSection);
                                                    XYZ xyz2 = boundingBoxXYZ.Max - boundingBoxXYZ.Min;
                                                    XYZ xyz3 = line3.GetEndPoint(0) + new XYZ(0.0, 0.0, z);
                                                    XYZ ep = xyz3 + new XYZ(0.0, 0.0, xyz2.Z);
                                                    this._trex_createLineSegmentSec(document, viewSection, xyz3, ep, graphicsStyle);
                                                }
                                                else
                                                    this._trex_createLineSegment(document, viewPlan, line3.GetEndPoint(0), line3.GetEndPoint(1), graphicsStyle);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                if (list4.Count > 0)
                {
                    activeUIDocument.ShowElements(list4);
                    activeUIDocument.Selection.SetElementIds(list4);
                }
                else
                {
                }
                result = Result.Succeeded;
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            result = Result.Failed;
        }
        return result;
    }
    public class WallFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Wall;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
        public WallFilter()
        {
        }
    }
}

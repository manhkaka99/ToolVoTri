using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BimIshou.Services;
using BimIshou.Utils;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;

namespace BimIshou.DimWall;

[Transaction(TransactionMode.Manual)]
public class DimWall : ExternalCommand
{
    private StorageProjectInfoService storageService;
    private int Offset;
    private IList<Reference> Viewports;
    public override void Execute()
    {
        storageService = new();
        Offset = storageService.Load(Document) == 0 ? 300 : storageService.Load(Document);
        Debug.WriteLine(Offset);
        try
        {
            Viewports = UiDocument.Selection.PickObjects(ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_Viewports))?.ToList() ?? [];
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return;
        }
        using Transaction transaction = new(Document, "Create dimension");
        transaction.Start();
        foreach (var viewport in Viewports)
        {
            ReferenceArray referenceArray = new();
            ReferenceArray referenceArray1 = new();
            Line Line;
            Line Line1;
            var vp = Document.GetElement(viewport.ElementId) as Viewport;
            if (Document.GetElement(vp.ViewId) is ViewSection view)
            {
                var walls = new FilteredElementCollector(Document, vp.ViewId)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsNotElementType()
                    .Cast<Wall>()
                    .Where(w => w.FindParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).AsInteger().Equals(1))
                    .Where(w => w.FindParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble() >= 3.280840)
                    .Where(w => w.Orientation.IsParallel(view.RightDirection))
                    .ToList();

                var grids = new FilteredElementCollector(Document, vp.ViewId)
                    .OfCategory(BuiltInCategory.OST_Grids)
                    .WhereElementIsNotElementType()
                    .Cast<Grid>()
                    .ToList();
                Wall wa = null;
                Grid grid = null;
                XYZ pickPoint = null;
                foreach (Grid gr in grids)
                {
                    grid ??= gr;
                    Reference gridRef = null;
                    Options opt = new()
                    {
                        ComputeReferences = true,
                        IncludeNonVisibleObjects = true,
                        View = view
                    };
                    foreach (GeometryObject obj in gr.get_Geometry(opt))
                    {
                        if (obj is Line)
                        {
                            Line l = obj as Line;
                            gridRef = l.Reference;
                            referenceArray1.Append(gridRef);
                        }
                    }
                }
                foreach (Element element in walls)
                {
                    if (element is Wall wall && wall.Orientation.IsParallel(view.RightDirection))
                    {
                        string uniqueId = element.UniqueId;
                        string representation;
                        representation = string.Format("{0}:{1}:{2}", uniqueId, -9999, 4);
                        Reference item3 = Reference.ParseFromStableRepresentation(Document, representation);
                        referenceArray.Append(item3);
                        referenceArray1.Append(item3);
                        wa ??= wall;
                        if (grid is null)
                            pickPoint ??= ((wa.Location as LocationCurve).Curve as Line).Origin.Add(-XYZ.BasisZ * 2 * Offset / 304.8);
                        else
                            pickPoint ??= ((wa.Location as LocationCurve).Curve as Line).Origin.Add(-XYZ.BasisZ * 3 * Offset / 304.8);
                    }

                }
                Line = Line.CreateBound(pickPoint, pickPoint + view.RightDirection);
                Line1 = Line.CreateBound(pickPoint.Add(XYZ.BasisZ * view.Scale * 3 / 304.8), pickPoint.Add(XYZ.BasisZ * view.Scale * 3 / 304.8 + view.RightDirection));


                Document.Create.NewDimension(view, Line, referenceArray);
                if (grid != null)
                    Document.Create.NewDimension(view, Line1, referenceArray1);
                CreateCenterLineWall(UiDocument, view, walls);
                SetGrid(grids, view, pickPoint);
            }
        }
        transaction.Commit();
    }
    private void CreateCenterLineWall(UIDocument activeUIDocument, ViewSection viewSection, List<Wall> list2)
    {
        Document document = activeUIDocument.Document;
        try
        {
            if (viewSection == null) return;
            GraphicsStyle graphicsStyle = new FilteredElementCollector(document)
                .OfClass(typeof(GraphicsStyle))
                .Cast<GraphicsStyle>()
                .Where(x => x.GraphicsStyleCategory.Name.Equals("1TS01-1"))
                .FirstOrDefault();
            if (graphicsStyle is not { }) return;

            XYZ origin = viewSection.Origin;
            XYZ source = viewSection.ViewDirection.Normalize();

            List<ElementId> list4 = [];


            foreach (Wall wall4 in list2)
            {
                if (wall4 != null)
                {
                    CompoundStructure compoundStructure = wall4.WallType.GetCompoundStructure();
                    if (compoundStructure != null)
                    {
                        if (!compoundStructure.IsVerticallyHomogeneous())
                        {
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
                                        Parameter parameter = wall4.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                                        Parameter parameter2 = wall4.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
                                        Parameter parameter3 = wall4.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                                        double z = (parameter3 != null) ? parameter3.AsDouble() : 0.0;
                                        document.GetElement(wall4.LevelId);
                                        BoundingBoxXYZ boundingBoxXYZ = wall4.get_BoundingBox(viewSection);
                                        XYZ xyz2 = boundingBoxXYZ.Max - boundingBoxXYZ.Min;
                                        XYZ xyz3 = line3.GetEndPoint(0) + new XYZ(0.0, 0.0, z);
                                        XYZ ep = xyz3 + new XYZ(0.0, 0.0, xyz2.Z);
                                        _trex_createLineSegmentSec(document, viewSection, xyz3, ep, graphicsStyle);
                                    }
                                }
                            }
                        }
                    }
                }

                if (list4.Count > 0)
                {
                    activeUIDocument.ShowElements(list4);
                    activeUIDocument.Selection.SetElementIds(list4);
                }
                else
                {
                }
            }
        }
        catch (Exception)
        {
        }
    }
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
    private void SetGrid(List<Grid> grids, View view, XYZ ep)
    {
        foreach (var grid in grids)
        {
            Line lineGrid;
            var line = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view).FirstOrDefault() as Line;
            var dir = line.Direction;
            var point = ep.Add(-dir * Offset / 304.8).ProjectPoint2Line(line);
            if (point.DistanceTo(line.GetEndPoint(0)) < point.DistanceTo(line.GetEndPoint(1)))
                lineGrid = Line.CreateBound(point, line.GetEndPoint(1));
            else
                lineGrid = Line.CreateBound(line.GetEndPoint(0), point);
            grid.SetCurveInView(DatumExtentType.ViewSpecific, view, lineGrid);
        }
    }
}


using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nice3point.Revit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace BimIshou.CreateRegion
{
    [Transaction(TransactionMode.Manual)]
    public class CreateRegion : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<ElementId> sheetId = (IList<ElementId>)uidoc.Selection.GetElementIds();
            
            foreach (ElementId elementId in sheetId) 
            {
                ViewSheet sheet = doc.GetElement(elementId) as ViewSheet;
                ICollection<ElementId> viewIds = sheet.GetAllPlacedViews();
                foreach (ElementId viewId in viewIds)
                {
                    FilteredElementCollector tagCollector = new FilteredElementCollector(doc, viewId)
                                        .OfCategory(BuiltInCategory.OST_RoomTags)
                                        .WhereElementIsNotElementType(); ;
                    foreach (Element tag in tagCollector)
                    {
                        IndependentTag roomTag = tag as IndependentTag;
                        // Lấy ElementId của Room được tag
                        ElementId taggedId = roomTag.TaggedLocalElementId;
                        Room taggedRoom = doc.GetElement(taggedId) as Room;
                        TaskDialog.Show("Test",tag.Name);
                    }
                    
                } 
                
                
            }

            
            return Result.Succeeded;
        }
        public (List<ElementId> RoomIds, View FloorPlanViews) GetRoomsAndFloorPlanViewsInSheet(Document doc, ElementId sheetId)
        {
            // Lấy sheet từ ElementId
            ViewSheet sheet = doc.GetElement(sheetId) as ViewSheet;
            if (sheet == null)
                throw new ArgumentException("Sheet không hợp lệ.");

            // Danh sách lưu RoomId và Floor Plan View
            List<ElementId> roomIds = new List<ElementId>();
            View floorPlanView = null;

            // Duyệt các view trên sheet
            ICollection<ElementId> viewIds = sheet.GetAllPlacedViews();
            foreach (ElementId viewId in viewIds)
            {
                View view = doc.GetElement(viewId) as View;
                if (view == null) continue;

                if (view.ViewType == ViewType.Elevation)
                {
                    // Lấy các Room trong view elevation
                    FilteredElementCollector tagCollector = new FilteredElementCollector(doc, view.Id)
                        .OfClass(typeof(IndependentTag))
                        .OfCategory(BuiltInCategory.OST_RoomTags);
                    foreach (Element tag in tagCollector)
                    {
                        IndependentTag roomTag = tag as IndependentTag;
                        // Lấy ElementId của Room được tag
                        ElementId taggedId = roomTag.TaggedElementId.HostElementId;
                        roomIds.Add(taggedId);
                    }
                }
                else if (view.ViewType == ViewType.FloorPlan)
                {
                    // Thêm view Floor Plan vào danh sách
                    floorPlanView = view;
                }
            }
            return (roomIds, floorPlanView);
        }
        public void CreateFilledRegionAndTag(Document doc, List<ElementId> roomIds, View floorPlan)
        {
            // Lấy loại FilledRegion
            ElementId filledRegionTypeId = new FilteredElementCollector(doc)
                .OfClass(typeof(FilledRegionType))
                .FirstElementId();

            if (filledRegionTypeId == ElementId.InvalidElementId)
                throw new Exception("Không tìm thấy FilledRegionType hợp lệ.");

            // Duyệt qua từng Room
            foreach (ElementId roomId in roomIds)
            {
                Room room = doc.GetElement(roomId) as Room;
                if (room == null) continue;

                // Lấy Boundary của Room
                IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                if (boundaries == null || boundaries.Count == 0) continue;

                // Chuyển Boundary thành CurveLoop
                List<CurveLoop> curveLoops = new List<CurveLoop>();
                foreach (IList<BoundarySegment> boundary in boundaries)
                {
                    CurveLoop loop = new CurveLoop();
                    foreach (BoundarySegment segment in boundary)
                    {
                        loop.Append(segment.GetCurve());
                    }
                    curveLoops.Add(loop);
                }

                // Tạo FilledRegion
                FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionTypeId, floorPlan.Id, curveLoops);

                // Tạo Tag cho Room
                LocationPoint location = room.Location as LocationPoint;
                XYZ tagPoint = location.Point; // Điểm đặt tag
                IndependentTag.Create(doc, doc.GetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_RoomTags)), floorPlan.Id, new Reference(room), true, TagOrientation.Horizontal, tagPoint);
            }
        }
        public void ClearOldRegionsAndTags(Document doc, ElementId sheetId, View floorPlan)
        {
            FilteredElementCollector filledRegionCollector = new FilteredElementCollector(doc, floorPlan.Id)
                .OfClass(typeof(FilledRegion));

            foreach (Element region in filledRegionCollector)
            {
                doc.Delete(region.Id);
            }
            var (roomIds, floorPlanView) = GetRoomsAndFloorPlanViewsInSheet(doc, sheetId);


            FilteredElementCollector tagCollector = new FilteredElementCollector(doc, floorPlan.Id)
                .OfClass(typeof(IndependentTag))
                .OfCategory(BuiltInCategory.OST_RoomTags);

            foreach (Element tag in tagCollector)
            {
                IndependentTag roomTag = tag as IndependentTag;
                if (roomTag == null) continue;

                // Lấy ElementId của Room được tag
                ElementId taggedId = roomTag.TaggedElementId.HostElementId;
                if (taggedId == ElementId.InvalidElementId) continue;

                // Kiểm tra xem Room được tag có nằm trong danh sách roomIds không
                if (!roomIds.Contains(taggedId))
                {
                    // Nếu không nằm trong danh sách, xóa tag
                    doc.Delete(roomTag.Id);
                }
            }
        }
        public void ProcessSheetRooms(Document doc, ElementId sheetId)
        {
            using (Transaction trans = new Transaction(doc, "Update Rooms"))
            {
                trans.Start();
                // Lấy danh sách phòng và mặt bằng từ sheet
                var (roomIds, floorPlanView) = GetRoomsAndFloorPlanViewsInSheet(doc, sheetId);

                // Xóa FilledRegion và Tag cũ
                ClearOldRegionsAndTags(doc,sheetId,floorPlanView);

                

                // Tạo FilledRegion và Tag mới
                CreateFilledRegionAndTag(doc, roomIds, floorPlanView);

                trans.Commit();
            }
        }
    }
}

using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BimIshou.CreateRegion
{
    /// <summary>
    /// Interaction logic for CreateRegionWindow.xaml
    /// </summary>
    public partial class CreateRegionWindow : MetroWindow
    {
        UIDocument UiDoc;
        Document Doc;
        public CreateRegionWindow(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            InitializeComponent();

            //Lấy về loại FillRegion đang sử dụng
            IList<FilledRegionType> allFilledRegionType = new FilteredElementCollector(Doc)
            .OfClass(typeof(FilledRegionType))
            .Cast<FilledRegionType>()
            .OrderBy(x => x.Name).ToList();
            allFill.ItemsSource = allFilledRegionType;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            FilledRegionType filledRegionType = allFill.SelectedItem as FilledRegionType;
            #region Tìm loại tag Room
            FilteredElementCollector tagTypeCollector = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsElementType();
            // Tìm Room Tag Type với tên cụ thể (cập nhật tên ở đây)
            string desiredTagName = "名前 1.5mm"; 
            ElementId roomTagTypeId = null;

            foreach (Element tagType in tagTypeCollector)
            {
                if (tagType.Name.Equals(desiredTagName, StringComparison.OrdinalIgnoreCase))
                {
                    roomTagTypeId = tagType.Id;
                    break;
                }
            }
            if (roomTagTypeId == null)
            {
                TaskDialog.Show("Lỗi", $"Không tìm thấy Room Tag với tên '{desiredTagName}'.");
            }
            #endregion
            IList<ElementId> sheetId = (IList<ElementId>)UiDoc.Selection.GetElementIds();
            foreach (ElementId elementId in sheetId)
            {
                ProcessSheetRooms(Doc, elementId, filledRegionType, roomTagTypeId);
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                        .OfCategory(BuiltInCategory.OST_RoomTags)
                        .WhereElementIsNotElementType();
                    foreach (Element tag in tagCollector)
                    {
                        RoomTag roomTag = tag as RoomTag;
                        if (roomTag == null) continue;
                        // Lấy ElementId của Room được tag
                        ElementId taggedId = roomTag.TaggedLocalRoomId;
                        if (taggedId == ElementId.InvalidElementId) continue;
                        roomIds.Add(taggedId);
                    }
                }
                else if (view.ViewType == ViewType.FloorPlan)
                {
                    // Thêm view Floor Plan vào danh sách
                    floorPlanView = view;
                }
            }
            roomIds = roomIds.Distinct().ToList();
            return (roomIds, floorPlanView);
        }
        public void CreateFilledRegionAndTag(Document doc, List<ElementId> roomIds, 
                                              View floorPlan, FilledRegionType filledRegionType, ElementId roomTagTypeId)
        {
            
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
                FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionType.Id, floorPlan.Id, curveLoops);

                // Lấy về room đã tag
                FilteredElementCollector tagCollector = new FilteredElementCollector(doc, floorPlan.Id)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsNotElementType();
                List<ElementId> roomTags = new List<ElementId>();
                foreach (Element tag in tagCollector)
                {
                    IndependentTag roomTag = tag as IndependentTag;
                    // Lấy ElementId của Room được tag
#if REVIT2024_OR_GREATER
                    ElementId taggedId = roomTag.GetTaggedLocalElementIds().First();
#else
                    ElementId taggedId = roomTag.TaggedLocalElementId;
#endif

                    roomTags.Add(taggedId);
                }
                roomTags = roomTags.Distinct().ToList();
                // Kiểm tra xem Room được tag có nằm trong danh sách room đã tag không
                if (roomTags.Contains(room.Id) == false)
                {
                    LocationPoint location = room.Location as LocationPoint;
                    XYZ tagPoint = location.Point; // Điểm đặt tag
                    IndependentTag.Create(doc, roomTagTypeId, floorPlan.Id, new Reference(room), false, TagOrientation.Horizontal, tagPoint);
                }
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
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsNotElementType();

            foreach (Element tag in tagCollector)
            {
                IndependentTag roomTag = tag as IndependentTag;
                if (roomTag == null) continue;

                // Lấy ElementId của Room được tag
#if REVIT2024_OR_GREATER
                ElementId taggedId = roomTag.GetTaggedLocalElementIds().First();
#else
                    ElementId taggedId = roomTag.TaggedLocalElementId;
#endif
                if (taggedId == ElementId.InvalidElementId) continue;

                // Kiểm tra xem Room được tag có nằm trong danh sách roomIds không
                if (!roomIds.Contains(taggedId))
                {
                    // Nếu không nằm trong danh sách, xóa tag
                    doc.Delete(roomTag.Id);
                }
            }
        }
        public void ProcessSheetRooms(Document doc, ElementId sheetId, FilledRegionType filledRegionType, ElementId roomTagTypeId)
        {
            using (Transaction trans = new Transaction(doc, "Create Region Rooms"))
            {
                trans.Start();
                // Lấy danh sách phòng và mặt bằng từ sheet
                var (roomIds, floorPlanView) = GetRoomsAndFloorPlanViewsInSheet(doc, sheetId);

                // Xóa FilledRegion và Tag cũ
                ClearOldRegionsAndTags(doc, sheetId, floorPlanView);

                // Tạo FilledRegion và Tag mới
                CreateFilledRegionAndTag(doc, roomIds, floorPlanView,filledRegionType,roomTagTypeId);

                trans.Commit();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using MahApps.Metro.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BimIshou.FillRegion
{
    /// <summary>
    /// Interaction logic for FillRegiom.xaml
    /// </summary>
    public partial class FillRegion : MetroWindow
    {
        UIDocument UiDoc;
        Document Doc;
        public FillRegion(UIDocument uiDoc)
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
            #region Tìm loại tag Room
            FilteredElementCollector tagTypeCollector = new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsElementType();
            // Tìm Room Tag Type với tên cụ thể (cập nhật tên ở đây)
            string desiredTagName = "名前 1.5mm"; // Thay bằng tên loại Room Tag bạn muốn sử dụng
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

            FilledRegionType filledRegionType = allFill.SelectedItem as FilledRegionType;

           IList <ElementId> sheetId = (IList<ElementId>)UiDoc.Selection.GetElementIds();
            foreach (ElementId elementId in sheetId)
            {
                var categorizedViews = CategorizeViewsOnSheet(Doc, elementId);
                View view = null;
                List<string> filteredList = new List<string>();
                foreach (var category in categorizedViews)
                {
                    if (category.Key == "Elevation")
                    {
                        List<string> sheetNames = new List<string>();
                        foreach (var viewInfor in category.Value)
                        {
                            sheetNames.Add(viewInfor.Name);
                        }
                        filteredList = sheetNames
                                        .Select(item => item.Split('_')[0]) // Tách chuỗi trước dấu "_"
                                        .Distinct()                        // Lấy giá trị duy nhất
                                        .ToList();

                    }
                    else if (category.Key == "FloorPlan")
                    {

                        foreach (var viewInfor in category.Value)
                        {
                            view = Doc.GetElement(viewInfor.Id) as View;
                        }
                    }
                }

                List<Room> filteredRooms = GetRoomsInViewByName(Doc, view, filteredList);

                CreateFilledRegionsForRooms(Doc, filteredRooms, view, filledRegionType.Id, roomTagTypeId);
                    
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public static List<string> GetSelectedSheetViewNames(Document doc, ElementId sheetId)
        {
            //Lấy về tên view của các view trong sheet
            // Lấy đối tượng ViewSheet từ ElementId đã chọn
            ViewSheet selectedSheet = doc.GetElement(sheetId) as ViewSheet;

            if (selectedSheet == null)
            {
                throw new ArgumentException("The provided ElementId does not correspond to a sheet.");
            }

            // Lấy tất cả các View được đặt trên Sheet
            ICollection<ElementId> viewIds = selectedSheet.GetAllPlacedViews();

            // Duyệt qua từng View và lấy tên
            List<string> viewNames = new List<string>();
            foreach (ElementId viewId in viewIds)
            {
                View view = doc.GetElement(viewId) as View;
                if (view != null)
                {
                    viewNames.Add(view.Name);
                }
            }
            return viewNames;
        }
        public static Dictionary<string, List<(string Name, ElementId Id)>> CategorizeViewsOnSheet(Document doc, ElementId sheetId)
        {
            //Lấy về tên view và viewId

            // Kết quả lưu theo từng loại View
            Dictionary<string, List<(string Name, ElementId Id)>> categorizedViews = new Dictionary<string, List<(string Name, ElementId Id)>>();

            // Lấy ViewSheet từ sheetId
            ViewSheet sheet = doc.GetElement(sheetId) as ViewSheet;

            if (sheet == null)
            {
                throw new ArgumentException("SheetId không hợp lệ.");
            }

            // Lấy tất cả các View trên sheet
            ICollection<ElementId> viewIds = sheet.GetAllPlacedViews();

            foreach (ElementId viewId in viewIds)
            {
                View view = doc.GetElement(viewId) as View;
                if (view != null)
                {
                    // Lấy loại view
                    string viewType = view.ViewType.ToString();

                    // Thêm vào danh sách kết quả
                    if (!categorizedViews.ContainsKey(viewType))
                    {
                        categorizedViews[viewType] = new List<(string Name, ElementId Id)>();
                    }
                    categorizedViews[viewType].Add((view.Name, view.Id));
                }
            }

            return categorizedViews;
        }
        public static List<Room> GetRoomsInViewByName(Document doc, View view, List<string> roomNames)
        {
            List<Room> filteredRooms = new List<Room>();
            // Lấy tất cả các Room trong dự án
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType();

            foreach (Element elem in collector)
            {
                Room room = elem as Room;
                if (room != null)
                {
                    // Kiểm tra nếu Room nằm trong View và tên Room trùng với danh sách
                    if (roomNames.Contains(room.LookupParameter("Name").AsString()))
                    {

                        filteredRooms.Add(room);

                    }
                }
            }

            return filteredRooms;
        }
        public static void CreateFilledRegionsForRooms(Document doc, List<Room> rooms, View view, ElementId filledRegionTypeId, ElementId roomTagId)
        {

            using (Transaction trans = new Transaction(doc, "Create Filled Regions for Rooms"))
            {
                trans.Start();

                foreach (Room room in rooms)
                {
                    // Lấy đường bao Room
                    IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                    if (boundarySegments == null || boundarySegments.Count == 0)
                    {
                        TaskDialog.Show("Warning", $"Room '{room.Name}' không có đường bao.");
                        continue;
                    }
                    // Duyệt từng đường bao của Room
                    foreach (IList<BoundarySegment> segmentList in boundarySegments)
                    {
                        CurveLoop curveLoop = new CurveLoop();
                        foreach (BoundarySegment segment in segmentList)
                        {
                            curveLoop.Append(segment.GetCurve());
                        }
                        IList<CurveLoop> curveLoops = new List<CurveLoop>();
                        curveLoops.Add(curveLoop);
                        // Tạo Filled Region
                        FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionTypeId, view.Id, curveLoops);

                        // Vị trí đặt Room Tag
                        LocationPoint locationPoint = room.Location as LocationPoint;
                        XYZ tagLocation = locationPoint.Point;

                        // Tạo Room Tag
                        LinkElementId roomId = new LinkElementId(room.Id);
                        UV tagPosition = new UV(tagLocation.X, tagLocation.Y);

                        // Gắn Tag (RoomTagType mặc định)
                        RoomTag newRoomTag = doc.Create.NewRoomTag(roomId, tagPosition, view.Id);
                        newRoomTag.ChangeTypeId(roomTagId);

                        if (filledRegion == null)
                        {
                            TaskDialog.Show("Error", $"Không thể tạo Filled Region cho Room '{room.Name}'.");
                            continue;
                        }
                    }

                }

                trans.Commit();
            }
        }
    }
}

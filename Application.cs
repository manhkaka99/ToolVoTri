using Autodesk.Revit.UI;
using BimIshou.AddNumberArea;
using BimIshou.Areas;
using BimIshou.AutoTag;
using BimIshou.Commands;
using BimIshou.DimWall;
using BimIshou.DuplicateSheet;
using BimIshou.PointFloorSite;
using BimIshou.ShowGrid;
using BimIshou.CreateRegion;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.External;
using System.Reflection;

namespace BimIshou
{
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            string pdfPath = @"%appdata%\Autodesk\Revit\Addins\BimIshouHelp";

            #region First
            var panel1 = Application.CreatePanel("First", "BimIshou");

            var MuliCutFilter = panel1.AddPushButton<MultiCutCMD>("Multi Cut");
            MuliCutFilter.ToolTip = "Cắt tự động thanh Doubuchi.";
            MuliCutFilter.SetImage("/BimIshou;component/Resources/Icons/MultiCutCMD16.png");
            MuliCutFilter.SetLargeImage("/BimIshou;component/Resources/Icons/MultiCutCMD32.png");

            var AutoTagRoomCMD = panel1.AddPushButton<AutoTagRoomCMD>("Tag Room");
            AutoTagRoomCMD.ToolTip = "Tag thông tin phòng ở bản A13.";
            AutoTagRoomCMD.SetImage("/BimIshou;component/Resources/Icons/Tagroom16.png");
            AutoTagRoomCMD.SetLargeImage("/BimIshou;component/Resources/Icons/Tagroom32.png");

            var DuplicateSheetCMD = panel1.AddPushButton<DuplicateSheetCMD>("Duplicate\nSheets");
            DuplicateSheetCMD.ToolTip = "Duplicate Sheet.";
            DuplicateSheetCMD.SetImage("/BimIshou;component/Resources/Icons/DuplicateSheetCMD16.png");
            DuplicateSheetCMD.SetLargeImage("/BimIshou;component/Resources/Icons/DuplicateSheetCMD32.png");

            #endregion

            #region Tool Vo Tri
            var panel2 = Application.CreatePanel("Tool Vô Tri", "BimIshou");

            var tagSite = panel2.AddPullDownButton("TagSite", "Tag Level Site");
            tagSite.ToolTip = "Tag các mốc cao độ.";
            tagSite.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");
            tagSite.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));
            var tagSite1 = tagSite.AddPushButton<TagSite>("Tag Level Gaiko\nTừ file Link");
            tagSite1.ToolTip = "Tag các mốc cao độ từ file link Gaiko.";
            tagSite1.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite1.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");
            var tagSite2 = tagSite.AddPushButton<TagSite_Project>("Tag Level Gaiko\nTrong Project");
            tagSite2.ToolTip = "Tag các mốc cao độ trong Project.";
            tagSite2.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite2.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");

            var tagDim = panel2.AddPushButton<TextDim>("Text 有効");
            tagDim.ToolTip = "Text thêm chữ 有効 vào phía trước đoạn Dimension được chọn.";
            tagDim.SetImage("/BimIshou;component/Resources/Icons/TextDim16.png");
            tagDim.SetLargeImage("/BimIshou;component/Resources/Icons/TextDim32.png");
            tagDim.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var Tag防 = panel2.AddPushButton<Tag防>("Tag 防");
            Tag防.ToolTip = "Tag các cửa đặc biệt.";
            Tag防.SetImage("/BimIshou;component/Resources/Icons/Tag防16.png");
            Tag防.SetLargeImage("/BimIshou;component/Resources/Icons/Tag防32.png");
            Tag防.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var floorSite = panel2.AddPushButton<FloorSite>("FloorSite");
            floorSite.ToolTip = "Tạo cao độ cho sàn theo các mốc cao độ.";
            floorSite.SetImage("/BimIshou;component/Resources/Icons/FloorSite16.png");
            floorSite.SetLargeImage("/BimIshou;component/Resources/Icons/FloorSite32.png");
            floorSite.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var areas = panel2.AddSplitButton("Area", "Áp dụng cho bản diện tích");
            areas.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));
            var overrideArea = areas.AddPushButton<OverrideArea>("OverrideArea");
            overrideArea.ToolTip = "Chỉnh các nét chia Area.";
            overrideArea.SetImage("/BimIshou;component/Resources/Icons/Override16.png");
            overrideArea.SetLargeImage("/BimIshou;component/Resources/Icons/Override32.png");
            var changeTagArea = areas.AddPushButton<ChangeTagArea>("Change Tag Area");
            changeTagArea.ToolTip = "Đổi các tag của các Area chỉ có một về tag 記号\n Lưu ý: Phải chạy tool gán thông tin Room cho Area trước khi chạy tool này";
            changeTagArea.SetImage("/BimIshou;component/Resources/Icons/changeTagArea16.png");
            changeTagArea.SetLargeImage("/BimIshou;component/Resources/Icons/changeTagArea32.png");
            var addNumberArea = areas.AddPushButton<AddnumberArea>("Add Number Area");
            addNumberArea.ToolTip = "Đánh STT cho Area.";
            addNumberArea.SetImage("/BimIshou;component/Resources/Icons/AddnumberArea16.png");
            addNumberArea.SetLargeImage("/BimIshou;component/Resources/Icons/AddnumberArea32.png");

            var GridLevel = panel2.AddPullDownButton("Level and Grid", "Level and Grid");
            GridLevel.ToolTip = "Ẩn hiện Level và Grid";
            GridLevel.SetImage("/BimIshou;component/Resources/Icons/Levelandgrid16.png");
            GridLevel.SetLargeImage("/BimIshou;component/Resources/Icons/Levelandgrid32.png");
            GridLevel.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));
            var visibilityGridLevel = GridLevel.AddPushButton<VisibilityGridLevel>("Visibility\nGrid and Level");
            visibilityGridLevel.ToolTip = "Ẩn hiện bubble của grid và level.";
            visibilityGridLevel.SetImage("/BimIshou;component/Resources/Icons/VisibilityGridLevel16.png");
            visibilityGridLevel.SetLargeImage("/BimIshou;component/Resources/Icons/VisibilityGridLevel32.png");
            var converGrid2D = GridLevel.AddPushButton<ConvertGrid2D>("Convert Grid to 2D");
            converGrid2D.ToolTip = "Chuyển toàn bộ Grid trong view thành 2D";
            converGrid2D.SetImage("/BimIshou;component/Resources/Icons/2D16.png");
            converGrid2D.SetLargeImage("/BimIshou;component/Resources/Icons/2D32.png");
            var converGrid3D = GridLevel.AddPushButton<ConvertGrid3D>("Convert Grid to 3D");
            converGrid3D.ToolTip = "Chuyển toàn bộ Grid trong view thành 3D";
            converGrid3D.SetImage("/BimIshou;component/Resources/Icons/3D16.png");
            converGrid3D.SetLargeImage("/BimIshou;component/Resources/Icons/3D32.png");
            var level2D = GridLevel.AddPushButton<Level2D>("Convert Level to 2D");
            level2D.ToolTip = "Chuyển toàn bộ Level trong view thành 2D";
            level2D.SetImage("/BimIshou;component/Resources/Icons/2D16.png");
            level2D.SetLargeImage("/BimIshou;component/Resources/Icons/2D32.png");
            var level3D = GridLevel.AddPushButton<Level3D>("Convert Level to 3D");
            level3D.ToolTip = "Chuyển toàn bộ Level trong view thành 3D";
            level3D.SetImage("/BimIshou;component/Resources/Icons/3D16.png");
            level3D.SetLargeImage("/BimIshou;component/Resources/Icons/3D32.png");

            var removeText = panel2.AddPushButton<RemoveText>("Remove Text\nChange Type");
            removeText.ToolTip = "Xóa các chữ A, B, C, D khi làm 展開図\n Nếu chọn view trong sheet thì sẽ đổi tên view về ビュー名";
            removeText.SetImage("/BimIshou;component/Resources/Icons/RemoveText16.png");
            removeText.SetLargeImage("/BimIshou;component/Resources/Icons/RemoveText32.png");
            removeText.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var createRegion = panel2.AddPushButton<CreateRegion.CreateRegion>("Create Region");
            createRegion.ToolTip = "Auto Tag + Fill Region các phòng trình bày trong Sheet 展開図\nLưu ý: Cần phải có 1 mặt bằng trong sheet";
            createRegion.SetImage("/BimIshou;component/Resources/Icons/CreateRegion16.png");
            createRegion.SetLargeImage("/BimIshou;component/Resources/Icons/CreateRegion32.png");
            createRegion.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var checkTagWall = panel2.AddPushButton<CheckTagWall.CheckTagWall>("Check Tag Wall");
            checkTagWall.ToolTip = "Kiểm tra xem có bao nhiêu loại tường đang được tag trong view.";
            checkTagWall.SetImage("/BimIshou;component/Resources/Icons/CheckWallTag16.png");
            checkTagWall.SetLargeImage("/BimIshou;component/Resources/Icons/CheckWallTag32.png");
            checkTagWall.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            var dimWallPanel = panel2.AddSplitButton("Dim Wall", "Dim Wall");
            dimWallPanel.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));
            var dimWall = dimWallPanel.AddPushButton<DimWall.DimWall>("Dim Wall A17");
            dimWall.ToolTip = "Dim Wall A17";
            dimWall.SetImage("/BimIshou;component/Resources/Icons/DimWall16.png");
            dimWall.SetLargeImage("/BimIshou;component/Resources/Icons/DimWall32.png");
            var setDimWall = dimWallPanel.AddPushButton<SettingDimWallCMD>("Setting");
            setDimWall.ToolTip = "Setting khoảng cách dim";
            setDimWall.SetImage("/BimIshou;component/Resources/Icons/Setting16.png");
            setDimWall.SetLargeImage("/BimIshou;component/Resources/Icons/Setting32.png");


            var checkoutStatus = panel2.AddPushButton<CheckoutStatusNDM>("Who?");
            checkoutStatus.ToolTip = "Kiểm tra xem ai đã sửa đối tượng.";
            checkoutStatus.SetImage("/BimIshou;component/Resources/Icons/CheckoutStatus16.png");
            checkoutStatus.SetLargeImage("/BimIshou;component/Resources/Icons/CheckoutStatus32.png");
            checkoutStatus.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, pdfPath));

            #endregion

        }
    }
}
using Autodesk.Revit.UI;
using BimIshou.AutoTag;
using BimIshou.Commands;
using BimIshou.Commands.DimDoubuchi;
using BimIshou.DuplicateSheet;
using BimIshou.PointFloorSite;
using BimIshou.ShowGrid;
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
            #region Dimensions
            var panel1 = Application.CreatePanel("Dimensions", "BimIshou");

            //var dimCh = panel1.AddPushButton<DimCH>("Dim CH");
            //dimCh.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //dimCh.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var dimFuniture = panel1.AddPushButton<DimFuniture>("Dim Funiture");
            //dimFuniture.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //dimFuniture.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var DimW = panel1.AddPushButton<DimW>("Dim W");
            //DimW.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //DimW.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var DimDoorAndWindow = panel1.AddPushButton<AutoDimDoor>("Dim DoorAndWindow");
            //DimDoorAndWindow.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //DimDoorAndWindow.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var DimTT = panel1.AddPushButton<DimTT>("Dim 有効");
            //DimTT.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //DimTT.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

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

            //var DimDoubuchi = panel1.AddSplitButton("dim", "dimdouchi");
            //DimDoubuchi.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //DimDoubuchi.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var button1 = DimDoubuchi.AddPushButton<DimDoubuchi>("DimDoubuchi 1");
            //button1.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //button1.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var button2 = DimDoubuchi.AddPushButton<SettingDim>("Setting");
            //button2.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //button2.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            //var button3 = DimDoubuchi.AddPushButton<DimDoorOrWindowInDoubuchi>("DimDoubuchi 2");
            //button3.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            //button3.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");
            #endregion

            #region Tool Vo Tri
            var panel2 = Application.CreatePanel("Tool Vô Tri", "BimIshou");

            var tagSite = panel2.AddSplitButton("TagSite", "Tag Level Site");
            tagSite.ToolTip = "Tag các mốc cao độ!";
            tagSite.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");
            var tagSite1 = tagSite.AddPushButton<TagSite>("Tag Level Gaiko\nTừ file Link");
            tagSite1.ToolTip = "Tag các mốc cao độ từ file link Gaiko!";
            tagSite1.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite1.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");
            var tagSite2 = tagSite.AddPushButton<TagSite_Project>("Tag Level Gaiko\nTrong Project");
            tagSite2.ToolTip = "Tag các mốc cao độ trong Project!";
            tagSite2.SetImage("/BimIshou;component/Resources/Icons/TagSite16.png");
            tagSite2.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");

            var tagDim = panel2.AddPushButton<TextDim>("Text 有効");
            tagDim.ToolTip = "Text thêm chữ 有効 vào phía trước đoạn Dimension được chọn!";
            tagDim.SetImage("/BimIshou;component/Resources/Icons/TextDim16.png");
            tagDim.SetLargeImage("/BimIshou;component/Resources/Icons/TextDim32.png");

            var Tag防 = panel2.AddPushButton<Tag防>("Tag 防");
            Tag防.ToolTip = "Tag các cửa đặc biệt.";
            Tag防.SetImage("/BimIshou;component/Resources/Icons/Tag防16.png");
            Tag防.SetLargeImage("/BimIshou;component/Resources/Icons/Tag防32.png");

            var floorSite = panel2.AddPushButton<FloorSite>("FloorSite");
            floorSite.ToolTip = "Tạo cao độ cho sàn theo các mốc cao độ.";
            floorSite.SetImage("/BimIshou;component/Resources/Icons/FloorSite16.png");
            floorSite.SetLargeImage("/BimIshou;component/Resources/Icons/FloorSite32.png");

            var overide = panel2.AddPushButton<OverideArea>("OverrideArea");
            overide.ToolTip = "Chỉnh các nét chia Area.";
            overide.SetImage("/BimIshou;component/Resources/Icons/Overide16.png");
            overide.SetLargeImage("/BimIshou;component/Resources/Icons/Overide32.png");

            var visibilityGridLevel = panel2.AddPushButton<VisibilityGridLevel>("Visibility\nGrid and Level");
            visibilityGridLevel.ToolTip = "Ẩn hiện bubble của grid và level.";
            visibilityGridLevel.SetImage("/BimIshou;component/Resources/Icons/VisibilityGridLevel16.png");
            visibilityGridLevel.SetLargeImage("/BimIshou;component/Resources/Icons/VisibilityGridLevel32.png");

            //var about = panel2.AddPushButton<Aboout>("About");
            //about.ToolTip = "Nếu bạn cảm thấy buồn và mệt mỏi. Hãy ấn vào đây nha.";
            //about.SetImage("/BimIshou;component/Resources/Icons/About16.png");
            //about.SetLargeImage("/BimIshou;component/Resources/Icons/About32.png");

            #endregion

        }
    }
}
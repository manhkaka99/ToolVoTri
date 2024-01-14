using CommunityToolkit.Mvvm.DependencyInjection;
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
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using MahApps.Metro.Controls;

namespace BimIshou.WPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OverideAreaWindow : MetroWindow
    {
        UIDocument UiDoc;
        Document Doc;
        public OverideAreaWindow(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            InitializeComponent();

            IList<LinePatternElement> patterns = new FilteredElementCollector(Doc)
            .OfClass(typeof(LinePatternElement)).Cast<LinePatternElement>().OrderBy(x => x.Name).ToList();
            AllPattern.ItemsSource = patterns;


            List<int> lineWeights = new List<int>();
            for (int i = 1; i <= 16; i++)
            {
                lineWeights.Add(i);
            }

            AllLineWeight.ItemsSource = lineWeights;
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            LinePatternElement selePattern = AllPattern.SelectedItem as LinePatternElement;
            int seleLineWeight = (int)AllLineWeight.SelectedItem;

            OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            overrideGraphicSettings.SetProjectionLinePatternId(selePattern.Id);
            overrideGraphicSettings.SetProjectionLineWeight(seleLineWeight);

            FilteredElementCollector rooms = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            FilteredElementCollector areaBounds = new FilteredElementCollector(Doc, Doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_AreaSchemeLines).WhereElementIsNotElementType();

            using (Transaction trans = new Transaction(Doc, "Overide Area"))
            {
                trans.Start();

                foreach (Element area in areaBounds)
                {
                    foreach (Element ele in rooms)
                    {
                        Room room = ele as Room;;
                        Autodesk.Revit.DB.Line line = (area.Location as LocationCurve).Curve as Autodesk.Revit.DB.Line;

                        BoundingBoxXYZ boundingBox = room.get_BoundingBox(Doc.ActiveView);

                        if (room.IsPointInRoom(line.Evaluate(0.5, true)) == true && IsPointOnRoomBoundary(room, line.Evaluate(0.5, true)) == false)
                        {
                            Doc.ActiveView.SetElementOverrides(area.Id, overrideGraphicSettings);
                        }
                    }
                }
                trans.Commit();
            }
            Close();
        }
        private bool IsPointOnRoomBoundary(Room room, XYZ testPoint)
        {
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center;
            IList<IList<BoundarySegment>> boundaryId = room.GetBoundarySegments(options);
            IList<Curve> roomBoundaries = new List<Curve>();

            foreach (IList<BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            {
                foreach (BoundarySegment boundSeg in boundSegList)
                {
                    roomBoundaries.Add(boundSeg.GetCurve());
                }
            }

            foreach (Curve curve in roomBoundaries)
            {
                if (curve.Distance(testPoint) < 0.01) 
                {
                    return true;
                }
            }
            return false;
        }
    private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}

using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BimIshou.Utils;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using Grid = Autodesk.Revit.DB.Grid;

namespace BimIshou.ShowGrid
{
    /// <summary>
    /// Interaction logic for VisibilityGrid.xaml
    /// </summary>
    public partial class VisibilityGrid : MetroWindow
    {

        UIDocument UiDoc;
        Document doc;


        IList<Element> grids = new List<Element>();
        List<Element> eleX = new List<Element>();
        List<Element> eleY = new List<Element>();
        List<Element> levels = new List<Element>();

        Boolean leftCheck = false;
        Boolean rightCheck = false;
        Boolean topCheck = false;
        Boolean bottomCheck = false;

        LocGridLevel locGrid = new LocGridLevel();


        public VisibilityGrid(UIDocument uiDoc)
        {
            InitializeComponent();
            UiDoc = uiDoc;
            doc = UiDoc.Document;
        }
        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            leftCheck = true;
        }
        private void Left_Unchecked(object sender, RoutedEventArgs e)
        {
            leftCheck = false;
        }
        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            rightCheck = true;
        }
        private void Right_Unchecked(object sender, RoutedEventArgs e)
        {
            rightCheck = false;
        }
        private void Top_Checked(object sender, RoutedEventArgs e)
        {
            topCheck = true;
        }
        private void Top_Unchecked(object sender, RoutedEventArgs e)
        {
            topCheck = false;
        }
        private void Bottom_Checked(object sender, RoutedEventArgs e)
        {
            bottomCheck = true;
        }
        private void Bottom_Unchecked(object sender, RoutedEventArgs e)
        {
            bottomCheck = false;
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {

            FilteredElementCollector grid = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                                .WhereElementIsNotElementType()
                                                .OfCategory(BuiltInCategory.OST_Grids);
            foreach (Element ele in grid)
            {
                grids.Add(ele);
            }
            foreach (Element element in grids)
            {
                Line line = (element as Autodesk.Revit.DB.Grid).Curve as Line;
                if (line.Direction.IsParallel(XYZ.BasisX))
                {
                    eleX.Add(element);
                }
                else
                {
                    eleY.Add(element);
                }
            }
            FilteredElementCollector level = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                                    .WhereElementIsNotElementType()
                                                    .OfCategory(BuiltInCategory.OST_Levels);
            foreach (Element ele in level)
            {
                levels.Add(ele);
            }
            using (Transaction trans = new Transaction(doc, "Show and Hide"))
            {
                trans.Start();
                if (doc.ActiveView.ViewType == ViewType.Section)
                {
                    if (leftCheck)
                    {
                        showLeft();
                        showLeftlv();
                    }
                    else
                    {
                        hideLeft();
                        hideLeftlv();
                    }
                    if (rightCheck)
                    {
                        showRight();
                        showRightlv();
                    }
                    else
                    {
                        hideRight();
                        hideRightlv();
                    }
                    if (bottomCheck)
                    {
                        showTop();
                    }
                    else
                    {
                        hideTop();
                    }
                    if (topCheck)
                    {
                        showBottom();
                    }
                    else
                    {
                        hideBottom();
                    }
                }
                else
                {
                    if (leftCheck)
                    {
                        showLeft();
                        showLeftlv();
                    }
                    else
                    {
                        hideLeft();
                        hideLeftlv();
                    }
                    if (rightCheck)
                    {
                        showRight();
                        showRightlv();
                    }
                    else
                    {
                        hideRight();
                        hideRightlv();
                    }
                    if (topCheck)
                    {
                        showTop();
                    }
                    else
                    {
                        hideTop();
                    }
                    if (bottomCheck)
                    {
                        showBottom();
                    }
                    else
                    {
                        hideBottom();
                    }
                }

                trans.Commit();
            }
            Close();
        }

        private void btnSelec_Click(object sender, RoutedEventArgs e)
        {
            Close();

            IList<Reference> list = UiDoc.Selection.PickObjects(ObjectType.Element, locGrid, "Chọn đối tượng");
            foreach (Reference re in list)
            {
                Element ele = doc.GetElement(re);
                if (ele.Category.Name.Equals("Grids"))
                {
                    grids.Add(ele);
                }
                else
                {
                    levels.Add(ele);
                }

            }
            foreach (Element element in grids)
            {
                Line line = (element as Autodesk.Revit.DB.Grid).Curve as Line;
                if (line.Direction.IsParallel(XYZ.BasisX))
                {
                    eleX.Add(element);
                }
                else
                {
                    eleY.Add(element);
                }
            }
            using (Transaction trans = new Transaction(doc, "Show and Hide"))
            {
                trans.Start();
                if (doc.ActiveView.ViewType == ViewType.Section || doc.ActiveView.ViewType == ViewType.Elevation)
                {
                    if (leftCheck)
                    {
                        showLeft();
                        showLeftlv();
                    }
                    else
                    {
                        hideLeft();
                        hideLeftlv();
                    }
                    if (rightCheck)
                    {
                        showRight();
                        showRightlv();
                    }
                    else
                    {
                        hideRight();
                        hideRightlv();
                    }
                    if (bottomCheck)
                    {
                        showTop();
                    }
                    else
                    {
                        hideTop();
                    }
                    if (topCheck)
                    {
                        showBottom();
                    }
                    else
                    {
                        hideBottom();
                    }
                }
                else
                {
                    if (leftCheck)
                    {
                        showLeft();
                        showLeftlv();
                    }
                    else
                    {
                        hideLeft();
                        hideLeftlv();
                    }
                    if (rightCheck)
                    {
                        showRight();
                        showRightlv();
                    }
                    else
                    {
                        hideRight();
                        hideRightlv();
                    }
                    if (topCheck)
                    {
                        showTop();
                    }
                    else
                    {
                        hideTop();
                    }
                    if (bottomCheck)
                    {
                        showBottom();
                    }
                    else
                    {
                        hideBottom();
                    }
                }

                trans.Commit();
            }

        }

        private void showLeft()
        {
            foreach (Element element in eleX)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckRL(p0, doc.ActiveView) == false)
                {
                    gr.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void hideLeft()
        {
            foreach (Element element in eleX)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckRL(p0, doc.ActiveView) == false)
                {
                    gr.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void showRight()
        {
            foreach (Element element in eleX)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckRL(p0, doc.ActiveView))
                {
                    gr.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void hideRight()
        {
            foreach (Element element in eleX)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckRL(p0, doc.ActiveView))
                {
                    gr.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void showTop()
        {
            foreach (Element element in eleY)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckUD(p0, doc.ActiveView))
                {
                    gr.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void hideTop()
        {
            foreach (Element element in eleY)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckUD(p0, doc.ActiveView))
                {
                    gr.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void showBottom()
        {
            foreach (Element element in eleY)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckUD(p0, doc.ActiveView) == false)
                {
                    gr.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void hideBottom()
        {
            foreach (Element element in eleY)
            {
                Grid gr = element as Grid;
                XYZ p0 = gr.Curve.GetEndPoint(0);
                if (CheckUD(p0, doc.ActiveView) == false)
                {
                    gr.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                }
                else
                {
                    gr.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                }
            }
        }
        private void showLeftlv()
        {
            foreach (Element element in levels)
            {
                Level lv = element as Level;
                IList<Curve> lvs = (IList<Curve>)lv.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView);
                foreach (Curve curve in lvs)
                {
                    XYZ p0 = curve.GetEndPoint(0);
                    if (CheckRLlv(p0, doc.ActiveView) == false)
                    {
                        lv.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                    }
                    else
                    {
                        lv.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                    }
                }
            }
        }
        private void hideLeftlv()
        {
            foreach (Element element in levels)
            {
                Level lv = element as Level;
                IList<Curve> lvs = (IList<Curve>)lv.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView);
                foreach (Curve curve in lvs)
                {
                    XYZ p0 = curve.GetEndPoint(0);
                    if (CheckRLlv(p0, doc.ActiveView) == false)
                    {
                        lv.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                    }
                    else
                    {
                        lv.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                    }
                }
            }
        }
        private void showRightlv()
        {
            foreach (Element element in levels)
            {
                Level lv = element as Level;
                IList<Curve> lvs = (IList<Curve>)lv.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView);
                foreach (Curve curve in lvs)
                {
                    XYZ p0 = curve.GetEndPoint(0);
                    if (CheckRLlv(p0, doc.ActiveView))
                    {
                        lv.ShowBubbleInView(DatumEnds.End0, doc.ActiveView);
                    }
                    else
                    {
                        lv.ShowBubbleInView(DatumEnds.End1, doc.ActiveView);
                    }
                }
            }
        }
        private void hideRightlv()
        {
            foreach (Element element in levels)
            {
                Level lv = element as Level;
                IList<Curve> lvs = (IList<Curve>)lv.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView);
                foreach (Curve curve in lvs)
                {
                    XYZ p0 = curve.GetEndPoint(0);
                    if (CheckRLlv(p0, doc.ActiveView))
                    {
                        lv.HideBubbleInView(DatumEnds.End0, doc.ActiveView);
                    }
                    else
                    {
                        lv.HideBubbleInView(DatumEnds.End1, doc.ActiveView);
                    }
                }
            }
        }


        private bool CheckRL(XYZ point, View view)
        {
            XYZ rightDirection = view.RightDirection;

            XYZ directionToPoint = point.Subtract(view.Origin).Normalize();

            double angleToRight = rightDirection.AngleTo(directionToPoint);

            if (angleToRight > Math.PI / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckUD(XYZ point, View view)
        {
            XYZ upDirection = view.UpDirection;

            XYZ directionToPoint = point.Subtract(view.Origin).Normalize();

            double angleToUp = upDirection.AngleTo(directionToPoint);

            if (angleToUp > Math.PI / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckRLlv(XYZ point, View view)
        {
            XYZ rightDirection = view.RightDirection;

            XYZ directionToPoint = point.Subtract(view.Origin).Normalize();

            double angleToRight = rightDirection.AngleTo(directionToPoint);

            if (angleToRight < Math.PI / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        class LocGridLevel : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return (elem.Category.Name.Equals("Grids") || elem.Category.Name.Equals("Levels"));
            }
            public bool AllowReference(Reference reference, XYZ position)
            {
                throw new NotImplementedException();
            }
        }

    }
}

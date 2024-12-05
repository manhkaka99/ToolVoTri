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
using System.Globalization;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using BimIshou.Commands;
using System.Xml;

namespace BimIshou.PointFloorSite
{
    /// <summary>
    /// Interaction logic for FloorSiteWPF.xaml
    /// </summary>
    public partial class FloorSiteWPF : MetroWindow
    {
        UIDocument Uidoc;
        Document doc;

        LocFloor locFloor = new LocFloor();
        Boolean millimeterCheck = new Boolean();
        Boolean meterCheck = new Boolean();
        public FloorSiteWPF(UIDocument uidoc)
        {
            InitializeComponent();
            Uidoc = uidoc;
            doc = Uidoc.Document;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Double giaTri;
            if (caoDo.Text == null)
            {
                giaTri = 0;   
            }
            else if(meterCheck)
            {
                giaTri = Convert.ToDouble(caoDo.Text);
#if REVIT2024_OR_GREATER
                giaTri = UnitUtils.ConvertToInternalUnits(giaTri, UnitTypeId.Meters);
#else
                giaTri = UnitUtils.ConvertToInternalUnits(giaTri, DisplayUnitType.DUT_METERS);
#endif
            }
            else if (millimeterCheck)
            {
                giaTri = Convert.ToDouble(caoDo.Text);
#if REVIT2024_OR_GREATER
                giaTri = UnitUtils.ConvertToInternalUnits(giaTri, UnitTypeId.Millimeters);
#else
                giaTri = UnitUtils.ConvertToInternalUnits(giaTri, DisplayUnitType.DUT_MILLIMETERS);
#endif
            }
            else
            {
                giaTri = 0;
            }

            if (millimeterCheck)
            {
                Close();
                
                IList<Reference> references = Uidoc.Selection.PickObjects(ObjectType.Element, locFloor, "Chọn sàn đi bạn eiii");
                using (Transaction trans = new Transaction(doc, "Add Point to the Floor."))
                {
                    trans.Start();
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
                        foreach (Element site in list)
                        {
                            LocationPoint location = site.Location as LocationPoint;
                            XYZ a = location.Point;
                            Double point = site.LookupParameter("計画レベル").AsDouble() - giaTri;
                            XYZ pos = new XYZ(a.X, a.Y, point);
                            floor.SlabShapeEditor.DrawPoint(pos);
                        }
                    }
                    trans.Commit();
                }
            }
            else if (meterCheck)
            {
                Close();

                IList<Reference> references = Uidoc.Selection.PickObjects(ObjectType.Element, locFloor, "Chọn sàn đi bạn eiii");
                using (Transaction trans = new Transaction(doc, "Add Point to the Floor."))
                {
                    trans.Start();
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
                        foreach (Element site in list)
                        {
                            LocationPoint location = site.Location as LocationPoint;
                            XYZ a = location.Point;
                            Double chuyen =Convert.ToDouble(site.LookupParameter("計画レベル").AsValueString());
#if REVIT2024_OR_GREATER
                            Double doi = UnitUtils.ConvertToInternalUnits(chuyen, UnitTypeId.Meters);
#else
                            Double doi = UnitUtils.ConvertToInternalUnits(chuyen, DisplayUnitType.DUT_METERS);
#endif
                            Double point = (doi - giaTri);
                            
                            XYZ pos = new XYZ(a.X, a.Y, point);
                            floor.SlabShapeEditor.DrawPoint(pos);
                        }
                    }
                    trans.Commit();
                }
            }
            else
            {
                Close();
            }
            

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close(); 
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            if(millimeter.IsChecked == true)
            {
                millimeterCheck = true;
                nhan.Content = "mm";
            }
            else
            {
                meterCheck = true;
                nhan.Content = "m";
            }
        }
        private void checkText(object sender, TextCompositionEventArgs e)
        {
            if (!IsAllowedInput(e.Text))
            {
                e.Handled = true;
            }
        }
        private bool IsAllowedInput(string text)
        {
            // Nếu văn bản mới không phải là số và không phải là dấu chấm thập phân
            if (!char.IsDigit(text, 0) && text != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            {
                return false;
            }
            return true;
        }


    }
    class LocFloor : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

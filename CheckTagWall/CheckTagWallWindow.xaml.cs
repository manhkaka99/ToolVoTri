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
using MahApps.Metro;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using MahApps.Metro.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BimIshou.CheckTagWall
{
    /// <summary>
    /// Interaction logic for CheckTagWall.xaml
    /// </summary>
    public partial class CheckTagWallWindow : MetroWindow
    {
        UIDocument UiDoc;
        Document Doc;
        public CheckTagWallWindow(UIDocument uiDoc)
        {
            UiDoc = uiDoc;
            Doc = UiDoc.Document;
            InitializeComponent();
            FilteredElementCollector collectors = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                                                  .OfCategory(BuiltInCategory.OST_WallTags)
                                                  .WhereElementIsNotElementType();
            List<string> tags = new List<string>();
            ListBoxItems.Items.Add("DANH SÁCH TƯỜNG CÓ TRONG VIEW");
            foreach (Element element in collectors)
            {
                IndependentTag tag = element as IndependentTag;
                tags.Add(tag.TagText);
            }
            tags = tags.Distinct().ToList();
            foreach (String text in tags)
            {
                ListBoxItems.Items.Add("  + "+text);
            }
            ListBoxItems.Items.Add("TỔNG: "+tags.Count);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

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
using System.Globalization;
using Autodesk.Revit.UI.Selection;

namespace BimIshou.AddNumberArea
{
    /// <summary>
    /// Interaction logic for AddnumberArea.xaml
    /// </summary>
    public partial class AddnumberAreaWPF : MetroWindow
    {
        UIDocument UiDoc;
        Document Doc;
        LocArea locArea = new LocArea();
        public AddnumberAreaWPF(UIDocument uiDoc)
        {
            
            InitializeComponent();
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
            int b = int.Parse(nbs.Text);
            while (true)
            {
                //try
                //{
                    for (int i = b; ;)
                    {
                        Reference r = UiDoc.Selection.PickObject(ObjectType.Element, locArea, "Chon doi tuong.");
                        Element element = Doc.GetElement(r);
                        Area area = element as Area;
                        string a = "0" + i.ToString();
                        using (Transaction trans = new Transaction(Doc, "Add Number"))
                        {
                            trans.Start();
                            area.LookupParameter("面積 エリア 記号").Set(a);
                            area.LookupParameter("面積 エリア 番号").Set("1");
                            trans.Commit();
                        }
                        i++;
                    }
                //}
                //catch ()
                //{
                    
                //}
            
            }
            }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
}

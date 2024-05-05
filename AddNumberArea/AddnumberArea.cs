using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BimIshou.AddNumberArea
{
    [Transaction(TransactionMode.Manual)]
    public class AddnumberArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            AddnumberAreaWPF addnumberArea = new AddnumberAreaWPF(uidoc);
            try
            {
                addnumberArea.ShowDialog();
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Succeeded;
            }

            return Result.Succeeded;

            //LocArea locArea = new LocArea();

            //int a1 = int.Parse("0200");
            //while (true)
            //{
            //    try
            //    {
            //        for (int i = a1; i < 1000;)
            //        {
            //            Reference r = uidoc.Selection.PickObject(ObjectType.Element, locArea, "Chon doi tuong.");
            //            Element element = doc.GetElement(r);
            //            Area area = element as Area;
            //            string a = "0" + i.ToString();
            //            using (Transaction trans = new Transaction(doc, "Add Number"))
            //            {
            //                trans.Start();
            //                area.LookupParameter("面積 エリア 記号").Set(a);
            //                area.LookupParameter("面積 エリア 番号").Set("1");
            //                trans.Commit();
            //            }
            //            i++;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        message = ex.Message;
            //        return Result.Succeeded;
            //    }
            //}


        }
    }
    class LocArea : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Name.Equals("Areas");
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

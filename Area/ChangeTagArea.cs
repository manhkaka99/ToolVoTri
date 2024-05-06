using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BimIshou.AutoJoin
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeTagArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Lấy về toàn bộ area trong view
            FilteredElementCollector areas = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType();

            //Lấy về toàn bộ tag area trong view
            FilteredElementCollector areaTags = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_AreaTags)
                .WhereElementIsNotElementType();
            List<Element> changeList = new List<Element>();
            foreach(Element b in areaTags)
            {
                changeList.Add(b);
            }


            List<String> numberroom = new List<String>();
            foreach (Element element in areas)
            {
                String x = element.LookupParameter("面積 エリア 記号").AsString();
                numberroom.Add(x);
            }
            List<Element> area = new List<Element>();
            foreach(Element ele in areas)
            {
                foreach(String number in Checklist(numberroom))
                {
                    if (ele.LookupParameter("面積 エリア 記号").AsString() == number)
                    {
                        area.Add(ele);
                    }
                }
            }

            //List tag area chỉ có 1 area
            List<Element> areaTag1 = new List<Element>();

            
            foreach (Element ele in areaTags)
            {
                AreaTag tag = ele as AreaTag;
                foreach (Element a in area)
                {
                    if (tag.Area.LookupParameter("面積 エリア 記号").AsString() == a.LookupParameter("面積 エリア 記号").AsString())
                    {
                        areaTag1.Add(ele);
                    } 
                }   
            }

            //List tag area có nhiều hơn 1 area
            List<Element> areaTag2 = changeList.Except(areaTag1).ToList();

            FamilySymbol tagId1 = (from tag in new FilteredElementCollector(doc)
                                        .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_AreaTags)
                                        .Cast<FamilySymbol>()
                                   where tag.Name == "記号"
                                   select tag).First();
            FamilySymbol tagId2 = (from tag in new FilteredElementCollector(doc)
                                        .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_AreaTags)
                                        .Cast<FamilySymbol>()
                                   where tag.Name == "記号 番号"
                                   select tag).First();
            foreach(Element ele in areaTag2)
            {
                AreaTag areaTag = ele as AreaTag;
                TaskDialog.Show("Test", areaTag.Area.LookupParameter("面積 エリア 記号").AsString());
                
            }
            //using (Transaction trans = new Transaction(doc, "Change Tag Area"))
            //{
            //    trans.Start();
            //    foreach (Element ele in areaTag1)
            //    {
            //        AreaTag areaTag = ele as AreaTag;
            //        areaTag.ChangeTypeId(tagId1.Id);
            //    }
            //    foreach (Element ele in areaTag2)
            //    {
            //        AreaTag areaTag = ele as AreaTag;
            //        areaTag.ChangeTypeId(tagId2.Id);
            //    }  
            //    trans.Commit();
            //}

            return Result.Succeeded;
        }
        static List<string> Checklist(List<string> lst)
        {
            List<string> resultlst = new List<string>();
            foreach (var item in lst)
            {
                if (lst.Count(x => x == item) <= 1)
                {
                    resultlst.Add(item);
                }
            }
            return resultlst;
        }
    }

}

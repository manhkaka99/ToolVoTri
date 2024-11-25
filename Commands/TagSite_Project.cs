    
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Nice3point.Revit.Toolkit.External;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Markup;

#endregion

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class TagSite_Project : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TagMode tMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrien = TagOrientation.Horizontal;
            #region Lấy về tag đã tag
            FilteredElementCollector tagIds = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_SiteTags)
                .WhereElementIsNotElementType();

            ICollection<Element> siteTagType = new List<Element>();

            foreach (Element a in tagIds)
            {
                if (a.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "dタグ_外構_計画と現況レベル")
                {
                    siteTagType.Add(a);
                }
            }
            ICollection<Element> siteTag2 = new List<Element>();
            ICollection<Element> siteTag3 = new List<Element>();
            foreach (Element a in siteTagType)
            {
                if (a.Name == "計画のみ")
                {
                    siteTag2.Add(a);
                }
                else if (a.Name == "計画と現況")
                {
                    siteTag3.Add(a);
                }
            }
            FamilySymbol tagId1 = (from tag in new FilteredElementCollector(doc)
                                        .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_SiteTags)
                                        .Cast<FamilySymbol>()
                                   where tag.Name == "計画のみ"
                                   select tag).First();
            FamilySymbol tagId2 = (from tag in new FilteredElementCollector(doc)
                                        .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_SiteTags)
                                        .Cast<FamilySymbol>()
                                   where tag.Name == "計画と現況"
                                   select tag).First();
            #endregion
            FilteredElementCollector sites = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Site).WhereElementIsNotElementType();
            IList<Element> list1 = new List<Element>();
            IList<Element> list2 = new List<Element>(); //List cao do 0
            IList<Element> list3 = new List<Element>(); //List cao do khac 0
            foreach (Element element in sites)
            {
                String a = element.LookupParameter("Family").AsValueString();

                if (a == "d外構_レベル標_隅" || a == "d外構_レベル標_中心")
                {
                    if (element.LookupParameter("現況レベル").AsDouble() == 0 || element.LookupParameter("現況レベル").AsDouble() == null)
                    {
                        list2.Add(element);
                    }
                    else
                    {
                        list3.Add(element);
                    }
                    list1.Add(element);
                }
            }
            Checktag3 checkTag3 = new Checktag3();
            CheckTag4 checkTag4 = new CheckTag4();
            try
            {
                using (Transaction trans = new Transaction(doc, "Creat Tag"))
                {
                    trans.Start();
                    
                    foreach (Element ele in list1)
                    {
                        Autodesk.Revit.DB.Reference reference = new Autodesk.Revit.DB.Reference(ele);
                        LocationPoint loc = ele.Location as LocationPoint;
                        XYZ pos = loc.Point;
                        if (checkTag3.checkTag(doc, ele, siteTagType) == false)
                        {
                            if (ele.LookupParameter("現況レベル").AsDouble() == 0 || ele.LookupParameter("現況レベル").AsDouble() == null)
                            {
                                IndependentTag tag = IndependentTag.Create(doc, tagId1.Id, doc.ActiveView.Id, reference, true, tOrien, pos);
                            }
                            else
                            {
                                IndependentTag tag = IndependentTag.Create(doc, tagId2.Id, doc.ActiveView.Id, reference, true, tOrien, pos);
                            }
                        }
                    }
                    foreach (Element ele2 in siteTag2)
                    {
                        if (checkTag4.checkTag(doc, list2, ele2) == false)
                        {
                            doc.Delete(ele2.Id);
                        }
                    }
                    foreach (Element ele3 in siteTag3)
                    {
                        if (checkTag4.checkTag(doc, list3, ele3) == false)
                        {
                            doc.Delete(ele3.Id);
                        }
                    }
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    internal class Checktag3
    {
        public bool checkTag(Document doc, Element element, ICollection<Element> tagIds)
        {
            ElementId elementId = element.Id;

            foreach (Element tagId in tagIds)
            {
                IndependentTag tag = tagId as IndependentTag;
                if (tag != null && tag.TaggedLocalElementId == elementId)
                {
                    return true;
                }
            }
            return false;
        }
    }
    internal class CheckTag4
    {
        public bool checkTag(Document doc, IList<Element> element, Element tags)
        {
            IndependentTag tag2 = tags as IndependentTag;
            ElementId tagId = tag2.TaggedLocalElementId;
            foreach (Element ele in element)
            {
                if (ele.Id != null && ele.Id == tagId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

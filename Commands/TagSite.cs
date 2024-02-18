
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
using System.Collections;

#endregion

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class TagSite : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TagMode tMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrien = TagOrientation.Horizontal;

            FilteredElementCollector tagIds = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_SiteTags).WhereElementIsNotElementType();

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

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            IList<Element> listLink = new List<Element>();

            foreach (Element check in collector)
            {
                if (check.Name.Contains("外構"))
                {
                    listLink.Add(check);
                }
            }
                                  
            IList<Element> list = new List<Element>();
            IList<Element> list2 = new List<Element>(); //List cao do 0
            IList<Element> list3 = new List<Element>(); //List cao do khac 0
            foreach (Element element in listLink)
            {
                RevitLinkInstance instance = element as RevitLinkInstance;
                Document linkDoc = instance.GetLinkDocument();
                //RevitLinkType type = doc.GetElement(instance.GetTypeId()) as RevitLinkType;
                FilteredElementCollector sites = new FilteredElementCollector(linkDoc)
                                   .OfCategory(BuiltInCategory.OST_Site)
                                   .WhereElementIsNotElementType();
                foreach (Element site in sites)
                {
                    ElementType elementType = instance.GetLinkDocument().GetElement(site.GetTypeId()) as ElementType;
                    if (elementType.FamilyName == "d外構_レベル標_隅" || elementType.FamilyName == "d外構_レベル標_中心")
                    {
                        if (site.LookupParameter("現況レベル").AsDouble() == 0 || site.LookupParameter("現況レベル").AsDouble() == null)
                        {
                            list2.Add(element);
                        }
                        else
                        {
                            list3.Add(element);
                        }
                        list.Add(site);
                    }
                }
            }
            Checktag checkTag = new Checktag();
            CheckTag5 checkTag5 = new CheckTag5();
            foreach (Element element in listLink)
            {
                RevitLinkInstance instance = element as RevitLinkInstance;
                try
                {
                    using (Transaction trans = new Transaction(doc, "Creat Tag Site"))
                    {
                        trans.Start();
                        foreach (Element ele in list)
                        {
                            if (checkTag.checkTag(doc, ele, siteTagType) == false)
                            {
                                Reference reference = new Reference(ele).CreateLinkReference(instance);
                                LocationPoint loc = ele.Location as LocationPoint;
                                XYZ pos = loc.Point;
                                if (ele.LookupParameter("現況レベル").AsDouble() == 0 || ele.LookupParameter("現況レベル").AsDouble() == null)
                                {
                                    IndependentTag tag = IndependentTag.Create(doc, tagId1.Id, doc.ActiveView.Id, reference, true, tOrien, pos);
                                    LinkElementId linkId = tag.TaggedElementId;
                                    ElementId linkInsancetId = linkId.LinkInstanceId;
                                    ElementId linkedElementId = linkId.LinkedElementId;
                                }
                                else
                                {
                                    IndependentTag tag = IndependentTag.Create(doc, tagId2.Id, doc.ActiveView.Id, reference, true, tOrien, pos);
                                    LinkElementId linkId = tag.TaggedElementId;
                                    ElementId linkInsancetId = linkId.LinkInstanceId;
                                    ElementId linkedElementId = linkId.LinkedElementId;
                                }
                            }
                        }
                        //foreach (Element ele2 in siteTag2)
                        //{
                            
                        //    if (checkTag5.checkTag(doc, list2, ele2) == false)
                        //    {
                        //        doc.Delete(ele2.Id);
                        //    }
                        //}
                        //foreach (Element ele3 in siteTag3)
                        //{
                        //    ;
                        //    if (checkTag5.checkTag(doc, list3, ele3) == false)
                        //    {
                        //        doc.Delete(ele3.Id);
                        //    }
                        //}
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Failed;
                }
            }
            return Result.Succeeded;
        }
    }
    internal class Checktag
    {
        public bool checkTag(Document doc, Element element, ICollection<Element> tagIds)
        {
            ElementId elementId = element.Id;

            foreach (Element tagId in tagIds)
            {
                IndependentTag tag = tagId as IndependentTag;
                if (tag != null && tag.GetTaggedReference().LinkedElementId == elementId)
                {
                    return true;
                }
            }
            return false;
        }
    }
    internal class CheckTag5
    {
        public bool checkTag(Document doc, IList<Element> element, Element tags)
        {
            IndependentTag tag2 = tags as IndependentTag;
            LinkElementId dele3 = tag2.TaggedElementId;
            ElementId tagId = dele3.LinkedElementId;

            //ElementId tagId = tag2.GetTaggedReference().LinkedElementId;
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


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

            ICollection<Element> siteTag = new List<Element>();

            foreach (Element a in tagIds)
            {
                if (a.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "dタグ_外構_計画と現況レベル")
                {
                    siteTag.Add(a);
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
            
            foreach (Element element in collector)
            {
                RevitLinkInstance instance = element as RevitLinkInstance;
                Document linkDoc = instance.GetLinkDocument();
                //RevitLinkType type = doc.GetElement(instance.GetTypeId()) as RevitLinkType;
                FilteredElementCollector sites = new FilteredElementCollector(linkDoc)
                                   .OfCategory(BuiltInCategory.OST_Site)
                                   .WhereElementIsNotElementType();
                IList<Element> list = new List<Element>();
                foreach (Element site in sites)
                {

                    ElementType elementType = instance.GetLinkDocument().GetElement(site.GetTypeId()) as ElementType;

                    if (elementType.FamilyName == "d外構_レベル標_隅" || elementType.FamilyName == "d外構_レベル標_中心")
                    {
                        list.Add(site);
                    }
                }
                Checktag checkTag = new Checktag();
                try
                {
                    using (Transaction trans = new Transaction(doc, "Creat Tag"))
                    {
                        trans.Start();
                        foreach (Element ele in list)
                        {

                            if (checkTag.checkTag(doc, ele, siteTag) == false)
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
}

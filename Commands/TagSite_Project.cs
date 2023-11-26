
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
    public class TagSite_Project : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TagMode tMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrien = TagOrientation.Horizontal;
            #region Lấy về tag
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
            #endregion

            FilteredElementCollector sites = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Site).WhereElementIsNotElementType();
            Checktag2 checkTag = new Checktag2();
            try
            {
                using (Transaction trans = new Transaction(doc, "Creat Tag"))
                {
                    trans.Start();
                    foreach (Element element in sites)
                    {
                        IList<Element> list = new List<Element>();


                        String a = element.LookupParameter("Family").AsValueString();

                        if (a == "d外構_レベル標_隅" || a == "d外構_レベル標_中心")
                        {
                            list.Add(element);
                        }



                        foreach (Element ele in list)
                        {

                            if (checkTag.checkTag(doc, ele, siteTag) == false)
                            {

                                Reference reference = new Reference(ele);
                                LocationPoint loc = ele.Location as LocationPoint;
                                XYZ pos = loc.Point;
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
    internal class Checktag2
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
}

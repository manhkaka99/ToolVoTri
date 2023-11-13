using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Tag防 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            Document doc = uidoc.Document;


            CheckTag checkTag = new CheckTag();
            CheckTag2 checkTag2 = new CheckTag2();

            FilteredElementCollector doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType();

            FilteredElementCollector windows = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .WhereElementIsNotElementType();


            #region Lay ve door va window
            ICollection<Element> doorAll1 = new List<Element>();
            ICollection<Element> doorAll2 = new List<Element>();

            

            foreach (Element ele in doors)
            {
                FamilySymbol doorFamily = doc.GetElement(ele.GetTypeId()) as FamilySymbol;
                string a = doorFamily.LookupParameter("建具 法").AsString();
                if (a != null & a != "" & a != "-")
                {
                    doorAll1.Add(ele);
                }
                else
                {
                    doorAll2.Add(ele);
                }
            }

            ICollection<Element> windowAll1 = new List<Element>();
            ICollection<Element> windowAll2 = new List<Element>();

            foreach (Element ele in windows)
            {
                FamilySymbol windowFamily = doc.GetElement(ele.GetTypeId()) as FamilySymbol;
                string a = windowFamily.LookupParameter("建具 法").AsString();
                if (a != null & a != "" & a != "-")
                {
                    windowAll1.Add(ele);
                }
                else
                {
                    windowAll2.Add(ele);
                }
            }
            #endregion

            #region Lấy về tag cần dùng
            FamilySymbol doorTagType = (from tag in new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DoorTags)
                                            .Cast<FamilySymbol>()
                                        where tag.Name == "法規"
                                        select tag).First();

            FamilySymbol windowTagType = (from tag in new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_WindowTags)
                                            .Cast<FamilySymbol>()
                                        where tag.Name == "法規"
                                        select tag).First();
            #endregion

            #region Lọc tag đã tag 
            FilteredElementCollector doorTags = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DoorTags).WhereElementIsNotElementType();
            ICollection<Element> doorTag = new List<Element>();

            foreach (Element a in doorTags)
            {
                if (a.Name == "法規")
                {
                    doorTag.Add(a);
                }
            }

            FilteredElementCollector windowTags = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_WindowTags).WhereElementIsNotElementType();
            ICollection<Element> windowTag = new List<Element>();

            foreach (Element a in windowTags)
            {
                if (a.Name == "法規")
                {
                    windowTag.Add(a);
                }
            }
            #endregion


            try
            {
                using (Transaction trans = new Transaction(doc, "Tag 防"))
                {
                    trans.Start();
                    foreach (Element door in doorAll1)
                    {
                        if (checkTag.checkTag(doc, door, doorTag) == false)
                        {
                            Reference reference = new Reference(door);
                            LocationPoint loc = door.Location as LocationPoint;
                            XYZ pos = loc.Point;
                            IndependentTag IT = IndependentTag.Create(doc, doorTagType.Id, doc.ActiveView.Id, reference, false, TagOrientation.Horizontal, pos);
                        }
                    }
                    foreach (Element a in doorTag)
                    {
                        if (checkTag2.checkTag2(doc, doorAll2, a) == true)
                        {
                            doc.Delete(a.Id);
                        }
                    }
                    foreach (Element window in windowAll1)
                    {
                        if (checkTag.checkTag(doc, window, windowTag) == false)
                        {
                            Reference reference = new Reference(window);
                            LocationPoint loc = window.Location as LocationPoint;
                            XYZ pos = loc.Point;
                            IndependentTag IT = IndependentTag.Create(doc, windowTagType.Id, doc.ActiveView.Id, reference, false, TagOrientation.Horizontal, pos);
                        }
                    }
                    foreach (Element a in windowTag)
                    {
                        if (checkTag2.checkTag2(doc, windowAll2, a) == true)
                        {
                            doc.Delete(a.Id);
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
    internal class CheckTag
    {
        public bool checkTag(Document doc, Element element, ICollection<Element> tags)
        {
            ElementId elementId = element.Id;
            foreach (Element tag in tags)
            {
                IndependentTag tag1 = tag as IndependentTag;
                if (tag != null && tag1.TaggedLocalElementId == elementId)
                {
                    return true;
                }
            }
            return false;
        }
    }
    internal class CheckTag2
    {
        public bool checkTag2(Document doc, ICollection<Element> element, Element tags)
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

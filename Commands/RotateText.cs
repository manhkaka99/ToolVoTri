using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nice3point.Revit.Extensions;
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
    public class RotateText : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            Document doc = uidoc.Document;

            // Lấy danh sách các đối tượng Text để xoay
            List<ElementId> textIds = new List<ElementId>();
            try
            {
                IList<Reference> textId = uidoc.Selection.PickObjects(ObjectType.Element,"Chon doi tuong Text");
                foreach (Reference id in textId)
                {
                    Element elem = doc.GetElement(id);

                    if (elem is TextNote)
                    {
                        textIds.Add(elem.Id);
                    }
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // Người dùng hủy bỏ hoặc không chọn đối tượng nào
                return Result.Cancelled;
            }

            // Lấy danh sách các đối tượng TextNote và xoay chúng
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Rotate Text Batch");

                foreach (ElementId textId in textIds)
                {
                    TextNote textNote = doc.GetElement(textId) as TextNote;
                    if (textNote != null)
                    {
                        
                        XYZ center = GetTextCenter(uidoc, textNote);
                        XYZ xYZ = new XYZ(center.X, center.Y, center.Z+0.01);
                        Line axis = Line.CreateBound(center, xYZ);
                        
                        double angle = 90; 

                        textNote.Rotate(axis, ConvertToRadians(angle));
                    }
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }

        // Phương thức để lấy tâm của một TextNote
        private XYZ GetTextCenter(UIDocument uidoc, TextNote textNote)
        {
            //XYZ center = textNote.Coord;
            //return center;
            View currentView = uidoc.ActiveView;
            BoundingBoxXYZ boundingBox = textNote.get_BoundingBox(currentView);

            
            XYZ center = (boundingBox.Max + boundingBox.Min) / 2;

            return center;
        }
        private double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }


    }
}

﻿#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CheckoutStatusNDM : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                if (selectedIds.Count != 1)
                {
                    Reference r = uidoc.Selection.PickObject(ObjectType.Element);
                    Element ele = doc.GetElement(r);
                    GetElementWorksharingInfo(doc, ele);
                }
                else
                {
                    foreach (ElementId eleId in selectedIds)
                    {
                        Element ele = doc.GetElement(eleId);
                        GetElementWorksharingInfo(doc, ele);
                    }
                } 
            }

            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Succeeded;
            }
            return Result.Succeeded;
        }
        private void GetElementWorksharingInfo(Document doc, Element elem)
        {
            String message = String.Empty;
            message += "Element Id: " + elem.Id;

            // The workset the element belongs to
            WorksetId worksetId = elem.WorksetId;
            message += ("\nWorkset Id : " + worksetId.ToString());

            // Model Updates Status of the element
            ModelUpdatesStatus updateStatus = WorksharingUtils.GetModelUpdatesStatus(doc, elem.Id);
            message += ("\nUpdate status : " + updateStatus.ToString());

            // Checkout Status of the element
            var checkoutStatus = WorksharingUtils.GetCheckoutStatus(doc, elem.Id);
            message += ("\nCheckout status : " + checkoutStatus.ToString());

            // Getting WorksharingTooltipInfo of a given element Id
            WorksharingTooltipInfo tooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, elem.Id);
            message += ("\nCreator : " + tooltipInfo.Creator);
            message += ("\nCurrent Owner : " + tooltipInfo.Owner);
            message += ("\nLast Changed by : " + tooltipInfo.LastChangedBy);

            TaskDialog.Show("Checkout Status", message);
        }
    }
    
}

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.DimWall;
[Transaction(TransactionMode.Manual)]
public class SettingDimWallCMD : ExternalCommand
{
    public override void Execute()
    {
        using Transaction tran = new(Document, "Setting Dim");
        tran.Start();

        ViewModel viewModel = new(Document);
        viewModel.Show();

        if (viewModel.settingDimWall.DialogResult == true)
            tran.Commit();
        else
            tran.RollBack();
    }
}

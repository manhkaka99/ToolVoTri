using Autodesk.Revit.DB;
using BimIshou.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace BimIshou.DimWall;
public partial class ViewModel : ObservableObject
{
    private readonly Document Document;
    private StorageProjectInfoService storageService;

    public SettingDimWall settingDimWall;

    [ObservableProperty]
    private int offset;

    public ViewModel(Document document = null)
    {
        Document = document;
        storageService = new StorageProjectInfoService();
        var res = storageService.Load(Document);
        if (res == 0)
            offset = 300;
        else
            offset = res;
    }
    public void Show()
    {
        settingDimWall = new()
        {
            DataContext = this
        };
        settingDimWall.ShowDialog();
    }
    [RelayCommand]
    private void OK()
    {
        Debug.WriteLine(Offset);
        storageService.Save(Document, Offset);
        settingDimWall.DialogResult = true;
    }
}

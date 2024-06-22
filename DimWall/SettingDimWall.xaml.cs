using System.Text.RegularExpressions;
using System.Windows;

namespace BimIshou.DimWall;
/// <summary>
/// Interaction logic for SettingDimWall.xaml
/// </summary>
public partial class SettingDimWall : Window
{
    public SettingDimWall()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        if (!IsNumeric(e.Text))
        {
            e.Handled = true;
        }
    }
    private bool IsNumeric(string text)
    {
        return Regex.IsMatch(text, @"^[0-9.,]+$");
    }
}

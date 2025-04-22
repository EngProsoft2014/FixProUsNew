using Mopups.Services;

namespace FixProUs.Pages.PopupPages;

public partial class MapTypePopup : Mopups.Pages.PopupPage
{
    public delegate void MapTypeDelegte(string MapName);
    public event MapTypeDelegte MapTypeDelegteClose;

    public MapTypePopup()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        MapTypeDelegteClose.Invoke("Apple");
        await MopupService.Instance.PopAsync();
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
    {
        MapTypeDelegteClose.Invoke("Google");
        await MopupService.Instance.PopAsync();
    }
}
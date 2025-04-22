using Mopups.Services;

namespace FixProUs.Pages.PopupPages;

public partial class OnMyWayPopup : Mopups.Pages.PopupPage
{
	public OnMyWayPopup()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

}
namespace FixProUs.Pages.SchedulePages;

public partial class CreateItemPage : Controls.CustomsPage
{
	public CreateItemPage()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }


}

namespace FixProUs.Pages;

public partial class FullScreenImagePage : Controls.CustomsPage
{
	public FullScreenImagePage()
	{
		InitializeComponent();
	}

    public FullScreenImagePage(ImageSource sourceImage)
    {
        InitializeComponent();
        imgFullScreen.Source = sourceImage;
    }

    public FullScreenImagePage(string image)
    {
        InitializeComponent();
        imgFullScreen.Source = image;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
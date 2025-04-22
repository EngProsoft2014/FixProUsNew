using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.SchedulePages;

public partial class SchedulePicturesPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    SchImagesViewModel schImagesViewModel;

    public SchedulePicturesPage(SchImagesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = schImagesViewModel = model;
    }

    [Obsolete]
    protected override bool OnBackButtonPressed()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            schImagesViewModel.IsEnable = false;
            UserDialogs.Instance.ShowLoading();   
            MessagingCenter.Send(this, "ChangeSchImagesInSchadulePage", schImagesViewModel.ScheduleDetails.LstSchedulePictures);
            await App.Current!.MainPage!.Navigation.PopAsync();
            UserDialogs.Instance.HideHud();
            schImagesViewModel.IsEnable = true;
        });
        return true;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        schImagesViewModel.IsEnable = false;
        UserDialogs.Instance.ShowLoading();
        MessagingCenter.Send(this, "ChangeSchImagesInSchadulePage", schImagesViewModel.ScheduleDetails.LstSchedulePictures);
        await App.Current!.MainPage!.Navigation.PopAsync();
        UserDialogs.Instance.HideHud();
        schImagesViewModel.IsEnable = true;
    }
}
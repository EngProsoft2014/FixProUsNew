using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages;

public partial class NoInternetPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    public NoInternetPage(IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();

        ORep = GenericRep;
        _service = service;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }

    protected override bool OnBackButtonPressed()
    {
        // Run the async code on the UI thread
        Dispatcher.Dispatch(() =>
        {
            Action action = () => Application.Current!.Quit();
            Controls.StaticMembers.ShowSnackBar("Do you want to exit the program?", Controls.StaticMembers.SnackBarColor, Controls.StaticMembers.SnackBarTextColor, action);
        });

        // Return true to prevent the default behavior
        return true;
    }



    public async Task GoAfterConnected()
    {
        UserDialogs.Instance.ShowLoading();

        if (App.Current!.MainPage!.Navigation.NavigationStack.Count > 1)
        {
            await App.Current!.MainPage!.Navigation.PopAsync();
        }
        else
        {
            await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
        }

        UserDialogs.Instance.HideHud();
    }

    async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess != NetworkAccess.Internet)
        {
            // Connection to internet is Not available

        }
        else
        {
            // Connection to internet is available
            await GoAfterConnected();
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            // Connection to internet is Not available

        }
        else
        {
            // Connection to internet is available
            await GoAfterConnected();
        }
    }
}
using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages;

public partial class NotificationsPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    SchedulesViewModel scheduleViewModel;

    public NotificationsPage(SchedulesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = scheduleViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        App.Current!.MainPage!.Navigation.PopAsync();
        return true;
    }
}
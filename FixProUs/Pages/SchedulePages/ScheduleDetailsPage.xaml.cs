using FixProUs.Models;
using FixProUs.ViewModels;
using Mopups.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Maps;
using FixProUs.Pages.PopupPages;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;

namespace FixProUs.Pages.SchedulePages;

public partial class ScheduleDetailsPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    ScheduleDetailsViewModel scheduleDetailsViewModel;

    public ScheduleDetailsPage(ScheduleDetailsViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = scheduleDetailsViewModel = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (scheduleDetailsViewModel?.CustomerDetails != null)
        {
            if (!string.IsNullOrEmpty(scheduleDetailsViewModel.CustomerDetails.locationlatitude) && !string.IsNullOrEmpty(scheduleDetailsViewModel.CustomerDetails.locationlongitude))
            {
                ObservableCollection<CustomersModel> LstCust = new ObservableCollection<CustomersModel>();
                LstCust.Add(scheduleDetailsViewModel.CustomerDetails);


                map.ItemsSource = LstCust;
                map.Pins.FirstOrDefault().Label = scheduleDetailsViewModel.CustomerDetails.Address;

                map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(double.Parse(scheduleDetailsViewModel.CustomerDetails.locationlatitude), double.Parse(scheduleDetailsViewModel.CustomerDetails.locationlongitude)), Distance.FromMiles(2)));
            }
        }

    }

    private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
    {
        if (stkScheduleInfo.IsVisible == false)
        {
            stkScheduleInfo.IsVisible = true;
        }
        else
        {
            stkScheduleInfo.IsVisible = false;
        }
    }

    private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
    {
        if (stkTeamAssign.IsVisible == false)
        {
            stkTeamAssign.IsVisible = true;
        }
        else
        {
            stkTeamAssign.IsVisible = false;
        }
    }

    private void TapGestureRecognizer_Tapped_7(object sender, EventArgs e)
    {
        if (stkPriorityFirstCreateServices.IsVisible == false)
        {
            stkPriorityFirstCreateServices.IsVisible = true;
        }
        else
        {
            stkPriorityFirstCreateServices.IsVisible = false;
        }
    }

    private async void TapGestureRecognizer_Tapped1(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }


    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }


    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.SchedulePicturesPage(new SchImagesViewModel(scheduleDetailsViewModel.ScheduleDetails,ORep,_service),ORep,_service));
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
    {
        if (scheduleDetailsViewModel?.ScheduleDetails?.Id != 0)
        {
            scheduleDetailsViewModel!.IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var page = new MapTypePopup();
                page.MapTypeDelegteClose += async (map) =>
                {

                    var location = new Location(double.Parse(scheduleDetailsViewModel?.CustomerDetails?.locationlatitude!), double.Parse(scheduleDetailsViewModel?.CustomerDetails?.locationlongitude!));

                    var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                    //await Xamarin.Essentials.Map.OpenAsync(location, options);

                    if (map == "Google")
                    {
                        await Launcher.OpenAsync($"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(scheduleDetailsViewModel?.CustomerDetails?.Address!)}");
                    }
                    else
                    {
                        await Launcher.OpenAsync($"http://maps.apple.com/?q={Uri.EscapeDataString(scheduleDetailsViewModel?.CustomerDetails?.Address!)}");
                    }
                };

                await MopupService.Instance.PushAsync(page);
            }

            scheduleDetailsViewModel.IsEnable = true;

        }
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        if (selectedOption != null)
        {
            scheduleDetailsViewModel.SelectedEmpCategoryCommand.Execute(selectedOption);
        }
    }

    private void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
    {
        frmMaterial.IsVisible = true;
        frmMaterialReceipt.IsVisible = false;
        frmServies.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
    {
        frmMaterial.IsVisible = false;
        frmMaterialReceipt.IsVisible = true;
        frmServies.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped_6(object sender, EventArgs e)
    {
        frmMaterial.IsVisible = false;
        frmMaterialReceipt.IsVisible = false;
        frmServies.IsVisible = true;
    }
}
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.ViewModels;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;

namespace FixProUs.Pages.SchedulePages;

public partial class NewSchedulePage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    AddScheduleViewModel addScheduleViewModel;

    public NewSchedulePage(AddScheduleViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = addScheduleViewModel = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (addScheduleViewModel?.CustomerDetails != null)
        {
            if (!string.IsNullOrEmpty(addScheduleViewModel.CustomerDetails.locationlatitude) && !string.IsNullOrEmpty(addScheduleViewModel.CustomerDetails.locationlongitude))
            {
                ObservableCollection<CustomersModel> LstCust = new ObservableCollection<CustomersModel>();
                LstCust.Add(addScheduleViewModel.CustomerDetails);

                map.ItemsSource = LstCust;
                map.Pins.FirstOrDefault().Label = addScheduleViewModel.CustomerDetails.Address;

                map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(double.Parse(addScheduleViewModel.CustomerDetails.locationlatitude), double.Parse(addScheduleViewModel.CustomerDetails.locationlongitude)), Distance.FromMiles(2)));
            }
        }
    }

    private async void TapGestureRecognizer_Tapped1(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }


    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }


    private async void map_MapClicked(object sender, Microsoft.Maui.Controls.Maps.MapClickedEventArgs e)
    {
        if (addScheduleViewModel?.ScheduleDetails?.Id != 0)
        {
            var location = new Location(double.Parse(addScheduleViewModel?.CustomerDetails?.locationlatitude), double.Parse(addScheduleViewModel?.CustomerDetails?.locationlongitude));

            //var location = new Location(31.199629, 29.918674);

            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            await Map.OpenAsync(location, options);
        }

    }


    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        if (selectedOption != null)
        {
            addScheduleViewModel.SelectedEmpCategoryCommand.Execute(selectedOption);
        }
    }


    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.SchedulePicturesPage(new SchImagesViewModel(addScheduleViewModel.ScheduleDetails, ORep, _service), ORep, _service));
    }


}
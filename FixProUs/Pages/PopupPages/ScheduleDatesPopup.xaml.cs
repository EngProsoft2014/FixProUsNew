using CommunityToolkit.Maui.Alerts;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.ViewModels;
using Mopups.Services;
using Stripe;
using System.Collections.ObjectModel;

namespace FixProUs.Pages.PopupPages;

public partial class ScheduleDatesPopup : Mopups.Pages.PopupPage
{
    public delegate void DatesDelegte(List<SchaduleDateModel> Dates);
    public event DatesDelegte DatesClose;

    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    ScheduleDetailsViewModel ViewModel;

    public ScheduleDatesPopup(IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        ViewModel = new ScheduleDetailsViewModel(ORep, _service); 
    }

    public ScheduleDatesPopup(ObservableCollection<SchaduleDateModel> LstDates)
    {
        InitializeComponent();
        lstDates.ItemsSource = ViewModel.LstEstimateSchaduleDates = ViewModel.LstInvoiceSchaduleDates = LstDates;
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        this.IsEnabled = false;
        List<SchaduleDateModel> LstDates = new List<SchaduleDateModel>();
        LstDates = ViewModel.LstEstimateSchaduleDates.Where(x => x.IsChecked == true).ToList();

        if (LstDates != null)
        {
            DatesClose.Invoke(LstDates);
        }
        else
        {
            var toast = Toast.Make("Please Choose Empolyee !!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
            await toast.Show();
        }
        this.IsEnabled = true;
        await MopupService.Instance.PopAsync();
    }
}
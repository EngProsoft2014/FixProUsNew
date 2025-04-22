using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.CallPages;

public partial class FilterCallPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    FilterCallsViewModel filterCallsViewModel;

    public FilterCallPage(FilterCallsViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = filterCallsViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }

    private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e) //Reset Filter Calls
    {

        pkrStartDt.Date = filterCallsViewModel.StartDate = DateTime.Now;
        filterCallsViewModel.OneFilter.StartDate = string.Empty;
        pkrEndDt.Date = filterCallsViewModel.EndDate = DateTime.Now;
        filterCallsViewModel.OneFilter.EndDate = string.Empty;

        swtDate.IsToggled = false;

        entryPhone.Text = filterCallsViewModel.OneFilter.PhoneNum = string.Empty;
        entryJob.Text = filterCallsViewModel.OneFilter.ScheduleTitle = string.Empty;
        pkrReason.SelectedItem = null;
        filterCallsViewModel.OneFilter.ReasonName = string.Empty;
        pkrCampaign.SelectedItem = null;
        filterCallsViewModel.OneFilter.CampaignName = string.Empty;
        pkrEmployee.SelectedItem = null;
        filterCallsViewModel.OneFilter.EmployeeName = string.Empty;

        Controls.StaticMembers.FilterCallModel = filterCallsViewModel.OneFilter;
    }
}
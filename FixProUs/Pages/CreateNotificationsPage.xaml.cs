using FixProUs.Helpers;
using FixProUs.ViewModels;
using System.Threading.Tasks;

namespace FixProUs.Pages;

public partial class CreateNotificationsPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    SchedulesViewModel scheduleViewModel;

    public CreateNotificationsPage(SchedulesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = scheduleViewModel = model;
    }


    private async void chBxAllEmployees_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == false)
        {
            await scheduleViewModel.GetEmployeesInAccountId(int.Parse(Helpers.Settings.AccountIdGet));
        }
    }
}
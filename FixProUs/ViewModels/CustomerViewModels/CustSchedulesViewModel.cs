using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace FixProUs.ViewModels
{
    public partial class CustSchedulesViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        ObservableCollection<SchedulesModel> lstSchedules;

        public CustSchedulesViewModel(CustomersModel model,IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            CustomerDetails = new CustomersModel();
            LstSchedules = new ObservableCollection<SchedulesModel>();
            CustomerDetails.LstSchedules = new List<SchedulesModel>();

            Init(model);
        }

        async Task Init(CustomersModel model)
        {
            CustomerDetails = model;
            await GetPerrmission();
            await GetSchedulesForCustomer(model.Id);
            //Task.WhenAll(GetPerrmission(), GetSchedulesForCustomer(model.Id));
        }

        //Get Perrmission for User
        public async Task GetPerrmission()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                EmployeePermission = new EmployeeModel();
                await Controls.StartData.CheckPermissionEmployee();
                EmployeePermission = Controls.StartData.EmployeeDataStatic;
            }
        }

        //Get Customer Schedules
        public async Task GetSchedulesForCustomer(int? CustomerId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<CustomersModel>(string.Format("api/Customers/GetSchedulesOfCustomer?" + "CustomerId=" + CustomerId), UserToken);
                if (Customer != null)
                {
                    LstSchedules = new ObservableCollection<SchedulesModel>(Customer.LstSchedules);
                }
            }
            UserDialogs.Instance.HideHud();
        }

        [RelayCommand]
        public async Task SelecteScheduleDetails(SchedulesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.ScheduleDateId, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }
    }
}

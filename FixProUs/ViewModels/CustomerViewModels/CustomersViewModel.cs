
using System.Collections.ObjectModel;
using FixProUs.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;

namespace FixProUs.ViewModels
{
    public partial class CustomersViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        ObservableCollection<CustomersModel> lstCustomers;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        string branchName;

        public int BranchIdVM;
        #endregion

        public CustomersViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
        }

        async void Init()
        {
            BranchIdVM = int.Parse(Helpers.Settings.BranchIdGet);
            CustomerDetails = new CustomersModel();
            CustomerDetails.LstCustomersCustomField = new List<CustomersCustomFieldModel>();
            LstCustomers = new ObservableCollection<CustomersModel>();
            BranchName = Settings.BranchNameGet;
            BranchIdVM = int.Parse(Settings.BranchIdGet);

            await GetAllCustomers();
        }

        async Task GetAllCustomers()
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var json = await ORep.GetAsync<ObservableCollection<CustomersModel>>(string.Format("api/Customers/GetAllCustInBranch?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstCustomers = json;
                }
            }

            UserDialogs.Instance.HideHud();
        }

        [RelayCommand]
        async Task SelecteCustomerDetails(CustomersModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CustomersDetailsPage(new CustInformationViewModel(model, ORep, _service), model, ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateNewCustomer()
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.WayCreateCust = 2; //From Schedule
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CreateNewCustomerPage(new AddCustomerViewModel(ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateNewSchedule(CustomersModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();

            if (Controls.StaticMembers.WayAfterChooseCust == 0)
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewSchedulePage(new AddScheduleViewModel(model, ORep, _service), ORep, _service));
            }
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        void Selection(CustomersModel model)
        {
            if (model.IsSelected == false)
            {
                model.IsSelected = true;
                LstCustomers.ToList().ForEach(f => f.IsSelected = false);
                LstCustomers!.Where(x => x.Id == model.Id).FirstOrDefault()!.IsSelected = true;
            }
            else
            {
                model.IsSelected = false;
                LstCustomers!.Where(x => x.Id == model.Id).FirstOrDefault()!.IsSelected = false;
            }
        }

    }
}

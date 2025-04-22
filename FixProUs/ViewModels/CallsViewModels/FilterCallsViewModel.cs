
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using System.Collections.ObjectModel;
using System.Globalization;


namespace FixProUs.ViewModels
{
    public partial class FilterCallsViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        CallModel oneFilter;

        [ObservableProperty]
        ReasonModel oneReason;

        [ObservableProperty]
        CampaignModel oneCampaign;

        [ObservableProperty]
        EmployeeModel oneEmployee;

        [ObservableProperty]
        ObservableCollection<ReasonModel> lstReasons;

        [ObservableProperty]
        ObservableCollection<CampaignModel> lstCampaigns;

        [ObservableProperty]
        ObservableCollection<EmployeeModel> lstEmployees;

        [ObservableProperty]
        DateTime startDate;

        [ObservableProperty]
        DateTime endDate;

        [ObservableProperty]
        string schTitle;

        [ObservableProperty]
        bool withDate;
        #endregion

        #region Delegate
        public delegate void CallDelegte(CallModel Call);
        public event CallDelegte CallClose;
        #endregion

        #region Cons
        public FilterCallsViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
            if (Controls.StaticMembers.FilterCallModel != null)
            {
                OneFilter = Controls.StaticMembers.FilterCallModel;
                if (string.IsNullOrEmpty(OneFilter.StartDate) != true)
                {
                    bool IsExpireDate = DateTime.TryParseExact(OneFilter.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startD);
                    StartDate = startD;
                    WithDate = true;
                }
                if (string.IsNullOrEmpty(OneFilter.EndDate) != true)
                {
                    bool IsExpireDate = DateTime.TryParseExact(OneFilter.EndDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime EndD);
                    EndDate = EndD;
                    WithDate = true;
                }
            }
        }
        #endregion

        #region Methods
        async void Init()
        {
            LstReasons = new ObservableCollection<ReasonModel>();
            LstCampaigns = new ObservableCollection<CampaignModel>();
            LstEmployees = new ObservableCollection<EmployeeModel>();
            OneFilter = new CallModel();
            OneReason = new ReasonModel();
            OneCampaign = new CampaignModel();
            OneEmployee = new EmployeeModel();

            StartDate = DateTime.Now;
            EndDate = DateTime.Now;

            WithDate = false;

            UserDialogs.Instance.ShowLoading();
            await Task.WhenAll(GetReasons(), GetCampaigns(), GetEmployeesInCall());
            UserDialogs.Instance.HideHud();
        }

        //Get Reasons
        async Task GetReasons()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<ReasonModel>>(string.Format("api/Calls/GetReasons?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstReasons = json;

                    OneReason = LstReasons.Where(x => x.Id == OneFilter.ReasonId).FirstOrDefault();
                }
            }
        }

        //Get Campaigns
        async Task GetCampaigns()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<CampaignModel>>(string.Format("api/Calls/GetCampaigns?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstCampaigns = json;

                    OneCampaign = LstCampaigns.Where(x => x.Id == OneFilter.CampaignId).FirstOrDefault();
                }
            }
        }

        // Get Employees 
        async Task GetEmployeesInCall()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<EmployeeModel>>(string.Format("api/Employee/GetEmpInCall/{0}/{1}/{2}", Helpers.Settings.AccountIdGet, Controls.StartData.EmployeeDataStatic.UserRole, Helpers.Settings.UserIdGet), UserToken);

                if (json != null)
                {
                    LstEmployees = json;

                    OneEmployee = LstEmployees.Where(x => x.Id == OneFilter.CreateUser).FirstOrDefault();
                }
            }
        }
        #endregion

        #region RelayCommand
        [RelayCommand]
        async void ApplyFilterCalls(CallModel model)
        {
            IsEnable = false;

            model.PhoneNum = model.PhoneNum != null ? model.PhoneNum : "";
            model.ReasonId = OneReason?.Id;
            model.CampaignId = OneCampaign?.Id;
            model.ScheduleTitle = SchTitle;
            model.CreateUser = OneEmployee?.Id;

            if (WithDate == true)
            {
                model.StartDate = StartDate.ToString("MM-dd-yyyy");
                model.EndDate = EndDate.ToString("MM-dd-yyyy");
            }
            else
            {
                model.StartDate = null;
                model.EndDate = null;
            }

            Controls.StaticMembers.FilterCallModel = model;

            CallClose.Invoke(model);
            await App.Current!.MainPage!.Navigation.PopAsync();

            IsEnable = true;
        } 
        #endregion
    }
}

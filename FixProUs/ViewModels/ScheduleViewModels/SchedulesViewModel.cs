
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using FixProUs.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Mopups.Services;
using FixProUs.Pages;
using CommunityToolkit.Mvvm.Input;
using FixProUs.Pages.PopupPages;
using CommunityToolkit.Maui.Alerts;
using SkiaSharp;
using FixProUs.Pages.SchedulePages;
using FixProUs.Helpers;
using System.Diagnostics;


namespace FixProUs.ViewModels
{
    public partial class SchedulesViewModel : BaseViewModel
    {

        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        [ObservableProperty]
        ObservableCollection<SchedulesModel> lstSchedules;

        [ObservableProperty]
        ObservableCollection<SchedulesModel> calendarDataToday;

        [ObservableProperty]
        ObservableCollection<SchedulesModel> lstSchedulesSearch;

        [ObservableProperty]
        ObservableCollection<EmployeeModel> lstEmpInAccountId;

        [ObservableProperty]
        ObservableCollection<NotificationsModel> lstMessages;

        [ObservableProperty]
        ObservableCollection<SheetColorModel> lstColors;

        [ObservableProperty]
        SchedulesModel scheduleDetails;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        bool isShowSearchByItem;

        [ObservableProperty]
        Object selectedColor;

        [ObservableProperty]
        string branchName;

        [ObservableProperty]
        int numNotify;

        [ObservableProperty]
        string userRole;

        [ObservableProperty]
        string headerNotify;

        [ObservableProperty]
        string contentNotify;

        [ObservableProperty]
        ObservableCollection<SchedulesModel> groupedList;

        public int BranchIdVM;

        public SchedulesViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            if (!string.IsNullOrEmpty(Settings.UserRoleGet))
            {
                UserRole = Settings.UserRoleGet;
            }

            BranchIdVM = Helpers.Settings.UserRoleGet == "4" ? int.Parse(Helpers.Settings.AccountIdGet) : int.Parse(Helpers.Settings.BranchIdGet);

            NumNotify = 0;

            CustomerDetails = new CustomersModel();
            ScheduleDetails = new SchedulesModel();
            LstSchedules = new ObservableCollection<SchedulesModel>();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstMaterialReceipt = new List<ScheduleMaterialReceiptModel>();
            LstColors = new ObservableCollection<SheetColorModel>();
            CalendarDataToday = new ObservableCollection<SchedulesModel>();
            GroupedList = new ObservableCollection<SchedulesModel>();
            LstMessages = new ObservableCollection<NotificationsModel>();
            LstEmpInAccountId = new ObservableCollection<EmployeeModel>();

            Task.WhenAll(Controls.StartData.GetAccountKeysAsync(), GetPerrmission(), GetAllSchedules(), GetNotifications(), GetEmployeesInAccountId());

            MessagingCenter.Subscribe<AddScheduleViewModel, bool>(this, "CreatedSchedule", (sender, message) =>
            {
                if (true)
                {
                    GetAllSchedules();
                }
            });
        }

        public async Task GetEmployeesInAccountId()
        {

            if (!string.IsNullOrEmpty(Settings.AccountIdGet))
            {
                await GetEmployeesInAccountId(int.Parse(Settings.AccountIdGet));
            }

        }

        // Get Employees in Account Id
        public async Task GetEmployeesInAccountId(int AccountId)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<EmployeeModel>>("api/Employee/GetEmployeesInAccountId?" + "AccountId=" + AccountId, UserToken);

                if (json != null)
                {
                    LstEmpInAccountId = json;
                }

            }

            IsEnable = true;
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

        //Get all Schedules in Branch
        public async Task GetAllSchedules()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                
                var json = await ORep.GetAsync<ObservableCollection<SchedulesModel>>(string.Format("api/Schedules/GetSchedules?" + "AccountId=" + Helpers.Settings.AccountIdGet + "&" + "EmpId=" + Helpers.Settings.UserIdGet + "&" + "EmpRole=" + Controls.StartData.EmployeeDataStatic.UserRole + "&" + "lstEmp=" + Helpers.Settings.UserEmployeesGet + "&" + "TextSearch="), UserToken);

                if (json != null)
                {
                    if (Controls.StartData.EmployeeDataStatic.ActiveAllScdTr_FaorTrOnly == false) //For Dispatch
                    {
                        LstSchedules = new ObservableCollection<SchedulesModel>(json.Where(x => x.OneScheduleDate.Active == true).ToList());
                    }
                    else
                    {
                        LstSchedules = json;
                    }

                    string day = DateTime.Now.ToString("yyyy-MM-dd");
                    CalendarDataToday = new ObservableCollection<SchedulesModel>(LstSchedules.Where(x => x.StartDate == day).ToList());

                    await GetEvents(LstSchedules);
                }
            }
        }

        public async Task GetNotifications()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                LstMessages = await ORep.GetAsync<ObservableCollection<NotificationsModel>>("api/Notifications/GetNotifications?" + "EmployeeId=" + Settings.UserIdGet, UserToken);
                NumNotify = LstMessages.Count;
            }
        }

        async Task GetEvents(ObservableCollection<SchedulesModel> Lstschedules)
        {
            if (LstSchedules.Count > 0)
            {
                string Date = "";

                foreach (var item in LstSchedules.OrderBy(appointment => DateTime.Parse(appointment.StartDate)))
                {
                    if (item.StartDate != Date)
                    {
                        GroupedList.Add(item);
                        Date = item.StartDate;
                    }
                }
            }
        }

        //Get One Schedule Details
        public async Task GetOneScheduleDetails(int ScheduleId, int ScheduleDateId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var Schedule = await ORep.GetAsync<SchedulesModel>(string.Format("api/Schedules/GetScheduleDetails?" + "ScheduleId=" + ScheduleId + "&" + "ScheduleDateId=" + ScheduleDateId), UserToken);

                if (Schedule != null)
                {
                    Schedule.CustomerDTO = Schedule.CustomerDTO == null ? new CustomersModel() : Schedule.CustomerDTO;
                    Schedule.OneScheduleDate = Schedule.OneScheduleDate == null ? new SchaduleDateModel() : Schedule.OneScheduleDate;


                    Schedule.LstScheduleEmployeeDTO = Schedule.LstScheduleEmployeeDTO == null ? new List<ScheduleEmployeesModel>() : Schedule.LstScheduleEmployeeDTO;

                    Schedule.LstScheduleItemsServices = Schedule.LstScheduleItemsServices == null ? new List<ScheduleItemsServicesModel>() : Schedule.LstScheduleItemsServices;
                    Schedule.LstSchedulePictures = Schedule.LstSchedulePictures == null ? new List<SchedulePicturesModel>() : Schedule.LstSchedulePictures;
                    Schedule.LstMaterialReceipt = Schedule.LstMaterialReceipt == null ? new List<ScheduleMaterialReceiptModel>() : Schedule.LstMaterialReceipt;

                    ScheduleDetails = Schedule;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task ScheduleDetailsformList(SchedulesModel model)
        {
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.ScheduleDateId, ORep, _service), ORep, _service));
        }

        [RelayCommand]
        async Task ChangeTextSearchJobs(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                IsShowSearchByItem = true;
            }
            else
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    IsShowSearchByItem = false;
                    string UserToken = await _service.UserToken();
                    var json = await ORep.GetAsync<ObservableCollection<SchedulesModel>>(string.Format("api/Schedules/GetSchedules?" + "AccountId=" + Helpers.Settings.AccountIdGet + "&" + "EmpId=" + Helpers.Settings.UserIdGet + "&" + "EmpRole=" + Controls.StartData.EmployeeDataStatic.UserRole + "&" + "lstEmp=" + Helpers.Settings.UserEmployeesGet + "&" + "TextSearch=" + text), UserToken);

                    if (json != null)
                    {
                        if (Controls.StartData.EmployeeDataStatic.ActiveAllScdTr_FaorTrOnly == false) //For Dispatch
                        {
                            LstSchedulesSearch = new ObservableCollection<SchedulesModel>(json.Where(x => x.OneScheduleDate.Active == true).ToList());
                        }
                        else
                        {
                            LstSchedulesSearch = json;
                        }
                    }
                }
            }
        }

        [RelayCommand]
        async Task StartScheduleOutSide(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    await GetOneScheduleDetails(model.Id, model.ScheduleDateId);

                    if (ScheduleDetails != null && ScheduleDetails.OneScheduleDate != null)
                    {
                        string UserToken = await _service.UserToken();
                        ScheduleDetails.OneScheduleDate.StartTime = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
                        ScheduleDetails.OneScheduleDate.Status = 1;

                        UserDialogs.Instance.ShowLoading();

                        var json = await ORep.PutStrAsync("api/Schedules/PutScheduleEmployees", ScheduleDetails.OneScheduleDate, UserToken);

                        if (!string.IsNullOrEmpty(json))
                        {
                            var toast = Toast.Make("Successfully Start The Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            model.ShowCheckBtn = 1;

                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else
                        {
                            var toast = Toast.Make("Failed Start The Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                        UserDialogs.Instance.HideHud();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task EndScheduleOutSide(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    await GetOneScheduleDetails(model.Id, model.ScheduleDateId);
                    string UserToken = await _service.UserToken();

                    if (ScheduleDetails != null && ScheduleDetails.OneScheduleDate != null)
                    {
                        ScheduleDetails.OneScheduleDate.EndTime = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
                        ScheduleDetails.OneScheduleDate.Status = 2;
                        ScheduleDetails.OneScheduleDate.CalendarColor = "#676d75";

                        UserDialogs.Instance.ShowLoading();

                        var json = await ORep.PutStrAsync("api/Schedules/PutScheduleEmployees", ScheduleDetails.OneScheduleDate, UserToken);

                        if (!string.IsNullOrEmpty(json))
                        {
                            var toast = Toast.Make("Successfully End The Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();

                            model.ShowCheckBtn = 2;

                            Controls.StaticMembers.TabSelected = 0;
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else
                        {
                            var toast = Toast.Make("Failed End The Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                        UserDialogs.Instance.HideHud();
                    }
                }
            }

            IsEnable = true;
        }

        #region RelayCommand Notifications
        [RelayCommand]
        async Task SelectedSendNotifications()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string strEmployees = string.Empty;

                List<int> oEmployees = new List<int>();
                oEmployees = LstEmpInAccountId.Where(x => x.IsChecked == true).Select(m => m.Id).ToList();
                if (oEmployees.Count > 0)
                {
                    oEmployees.ForEach(f => strEmployees += $",{f}");
                    strEmployees = strEmployees.Remove(0, 1);
                }

                if (string.IsNullOrEmpty(strEmployees))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Choose Employees.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(HeaderNotify))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Header of Notify.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ContentNotify))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Content of Notify.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    string UserToken = await _service.UserToken();

                    NotificationsSpecificModel model = new NotificationsSpecificModel()
                    {
                        AccountId = int.Parse(Settings.AccountIdGet),

                        app_id = Settings.OneSignalAppIdGet,

                        Header = HeaderNotify,

                        Content = ContentNotify,

                        include_player_ids = LstEmpInAccountId.Where(v => v.IsChecked == true && !string.IsNullOrEmpty(v.OneSignalPlayerId)).Select(m => m.OneSignalPlayerId).ToArray(),

                        Employees = strEmployees,

                        CreateUser = int.Parse(Settings.UserIdGet),
                    };

                    UserDialogs.Instance.ShowLoading();
                    string json = await ORep.PostDataAsync("api/Notifications/PostNotificationSpecific", model, UserToken);
                    UserDialogs.Instance.HideHud();

                    if (!string.IsNullOrEmpty(json))
                    {
                        var toast = Toast.Make("Successfully for Send Notifications.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                        await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                    }
                    else
                    {
                        var toast = Toast.Make("Faild for Send Notifications.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectedNotificationsPage()
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();

                LstMessages = await ORep.GetAsync<ObservableCollection<NotificationsModel>>("api/Notifications/GetNotifications?" + "EmployeeId=" + Settings.UserIdGet, UserToken);
                NumNotify = LstMessages.Count;
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.NotificationsPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectedCreateNotificationsPage()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                await App.Current!.MainPage!.Navigation.PushAsync(new CreateNotificationsPage(new SchedulesViewModel(ORep, _service), ORep, _service));

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task DeactiveNotify(NotificationsModel model)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                model.UpdateDate = DateTime.Now;
                model.UpdateUser = int.Parse(Settings.UserIdGet);

                bool exit = await App.Current!.MainPage!.DisplayAlert("FixProUs", "Do you want to Deactive the notify?", "Yes", "No").ConfigureAwait(false);
                if (exit)
                {
                    IsEnable = false;
                    string UserToken = await _service.UserToken();
                    UserDialogs.Instance.ShowLoading();
                    var json = await ORep.PutAsync("api/Notifications/PutDeactiveNotify", model, UserToken);
                    UserDialogs.Instance.HideHud();
                    IsEnable = true;

                    if (json.Active == false)
                    {
                        LstMessages.Remove(model);
                    }
                }
            }

        }

        [RelayCommand]
        async Task SelectedNotificationDetails(NotificationsModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                if (model.ScheduleId != null && model.ScheduleDateId != null && !string.IsNullOrEmpty(model.ScheduleDate))
                {
                    await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.ScheduleId.Value, model.ScheduleDateId.Value, ORep, _service), ORep, _service));
                }
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }
        #endregion
    }
}

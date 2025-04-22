
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using Mopups.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FixProUs.ViewModels
{
    public partial class CallsViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        CallModel oneCall;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        ReasonModel oneReason;

        [ObservableProperty]
        CampaignModel oneCampaign;

        [ObservableProperty]
        ObservableCollection<CallModel> lstCalls;

        [ObservableProperty]
        ObservableCollection<ReasonModel> lstReasons;

        [ObservableProperty]
        ObservableCollection<CampaignModel> lstCampaigns;

        [ObservableProperty]
        int isShowBtnSch;

        [ObservableProperty]
        int totalCalls;

        [ObservableProperty]
        int createOrDetailsCall;
        #endregion

        //Main Cons (open list of calls - create new call)
        public CallsViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
            IsShowBtnSch = 0;
            MessagingCenter.Subscribe<CallsViewModel, bool>(this, "CreatedOrDeletedCall", async (sender, message) =>
            {
                if (true)
                {
                    await GetCalls();
                }
            });
        }

        //Call Details
        public CallsViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
            IsShowBtnSch = 0;
            OneCall.PhoneNum = model.Phone1;
            OneCall.CustomerId = model.Id;
            OneCall.CustomerName = model.FirstName + " " + model.LastName;
        }

        //Open Call after post new call in database and show Add Job btn
        public CallsViewModel(CallModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
            OneCall = model;
            IsShowBtnSch = 0;
            if (OneCall.Id != 0)
            {
                IsShowBtnSch = 1;//add sch
                if (OneCall.ScheduleId != 0 && OneCall.ScheduleId != null)
                {
                    IsShowBtnSch = 2;//Show sch
                }
            }

            if (OneCall.CustomerId != 0 && OneCall.CustomerId != null)
            {
                GetOneCustomerDetials(OneCall.CustomerId);
            }


            if (OneCall.ScheduleTitle == null)
                OneCall.ScheduleTitle = "";
        }


        async void Init()
        {
            LstCalls = new ObservableCollection<CallModel>();
            LstReasons = new ObservableCollection<ReasonModel>();
            LstCampaigns = new ObservableCollection<CampaignModel>();
            CustomerDetails = new CustomersModel();
            OneCall = new CallModel();
            OneReason = new ReasonModel();
            OneCampaign = new CampaignModel();

            await GetCalls();

            if (Controls.StaticMembers.CreateOrDetailsCall == 1)
            {
                await Task.WhenAll(GetReasons(), GetCampaigns());
            }
        }

        //Get One Customer Detials
        async void GetOneCustomerDetials(int? CustId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<CustomersModel>(string.Format("api/Customers/GetOneCustDetails?" + "CustId=" + CustId), UserToken);

                if (json != null)
                {
                    CustomerDetails = json;
                }

                UserDialogs.Instance.HideHud();
            }
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
                    OneReason = LstReasons.Where(x => x.Id == OneCall.ReasonId).FirstOrDefault()!;
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
                    OneCampaign = LstCampaigns.Where(x => x.Id == OneCall.CampaignId).FirstOrDefault()!;
                }
            }
        }

        //Get Calls
        async Task GetCalls()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<CallModel>>(string.Format("api/Calls/GetAllCalls?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstCalls = json;
                    TotalCalls = LstCalls.Count;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task SelectCallDetails(CallModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.CreateOrDetailsCall = 1; //For Get Reasons and Campaigns
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CallPages.NewCallPage(new CallsViewModel(model, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateNewCall()
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.CreateOrDetailsCall = 1; //For Get Reasons and Campaigns
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CallPages.NewCallPage(new CallsViewModel(ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateNewCustomer()
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.WayCreateCust = 1;
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CreateNewCustomerPage(new AddCustomerViewModel(ORep, _service), ORep, _service));
            await MopupService.Instance.PopAsync();
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenFilterCalls()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var pageView = new FilterCallsViewModel(ORep,_service);
                pageView.CallClose += async (call) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    var json = await ORep.GetAsync<ObservableCollection<CallModel>>(string.Format("api/Calls/GetFilterCalls?" + "StartDate=" + call.StartDate + "&" + "EndDate=" + call.EndDate + "&" + "PhoneNum=" + call.PhoneNum + "&" + "ReasonId=" + call.ReasonId + "&" + "CampaignId=" + call.CampaignId + "&" + "EmployeeId=" + call.CreateUser + "&" + "SchTitle=" + call.ScheduleTitle), UserToken);

                    if (json != null)
                    {
                        LstCalls = json;

                        TotalCalls = LstCalls.Count;
                    }
                    UserDialogs.Instance.HideHud();
                };

                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CallPages.FilterCallPage(pageView, ORep, _service));
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitCall(CallModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                OneCall = model;

                OneCall.AccountId = int.Parse(Helpers.Settings.AccountIdGet);
                OneCall.BrancheId = int.Parse(Helpers.Settings.BranchIdGet);
                OneCall.Active = true;
                OneCall.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                OneCall.ReasonId = OneReason != null ? OneReason.Id : 0;
                OneCall.CampaignId = OneCampaign != null ? OneCampaign.Id : 0;

                if (string.IsNullOrEmpty(model.PhoneNum))
                {
                    var toast = Toast.Make("Please Complete This Field Required: Phone.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (OneReason == null)
                {
                    var toast = Toast.Make("Please Complete This Field Required: Choose Reason.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (OneCampaign == null)
                {
                    var toast = Toast.Make("Please Complete This Field Required : Choose Campaign.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(model.Notes))
                {
                    var toast = Toast.Make("Please Complete This Field Required: Notes.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    if (OneCall != null)
                    {
                        var json = "";

                        if (model.Id == 0)
                        {
                            UserDialogs.Instance.ShowLoading();
                            json = await ORep.PostDataAsync("api/Calls/PostCall", OneCall, UserToken);
                            UserDialogs.Instance.HideHud();
                        }
                        else
                        {
                            UserDialogs.Instance.ShowLoading();
                            json = await ORep.PutDataAsync("api/Calls/PutCall", OneCall, UserToken);
                            UserDialogs.Instance.HideHud();
                        }

                        if (json != "Bad Request" && json != "api not responding")
                        {
                            var toast = Toast.Make("Successfully for Save Call.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();

                            CallModel Call = JsonConvert.DeserializeObject<CallModel>(json)!;

                            MessagingCenter.Send(this, "CreatedOrDeletedCall", true);
                            await App.Current!.MainPage!.Navigation.PopAsync();
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CallPages.NewCallPage(new CallsViewModel(Call, ORep, _service), ORep, _service));
                        }
                        else
                        {
                            var toast = Toast.Make("Failed for add or edit Call.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task DeleteCall(int CallId)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();

                var json = await ORep.DeleteStrItemAsync(string.Format("api/Calls/DeleteCall/{0}", CallId), UserToken);

                if (json != null && json != "api not responding")
                {
                    var toast = Toast.Make("Successfully Delete Call.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                    MessagingCenter.Send(this, "CreatedOrDeletedCall", true);
                    await App.Current!.MainPage!.Navigation.PopAsync();        
                }
                else
                {
                    var toast = Toast.Make("Failed Delete Call.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectGoJob(CallModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.ScheduleId!.Value, model.ScheduleDateId!.Value, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateScheduleFromCall(CustomersModel model)
        {
            IsEnable = false;

            UserDialogs.Instance.ShowLoading();

            if (model.Id != 0)
            {
                if (Controls.StaticMembers.WayAfterChooseCust == 0)
                {
                    model.CallId = OneCall.Id;
                    await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewSchedulePage(new AddScheduleViewModel(model, ORep, _service), ORep, _service));
                }
            }
            else
            {
                var toast = Toast.Make("Sorry, This Call Don't Have Customer Details.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }

            UserDialogs.Instance.HideHud();

            IsEnable = true;
        }

        [RelayCommand]
        async Task ResetCalls()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                await GetCalls();

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }
    }
}


using FixPro.Services.Data;
using FixProUs.Helpers;
using FixProUs.Pages;
using FixProUs.Services.Data;
using FixProUs.ViewModels;
using GoogleApi.Entities.Translate.Common.Enums;
using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core.Debug;

namespace FixProUs
{
    public partial class App : Application
    {

        #region Service
        readonly IGenericRepository ORep;
        readonly ServicesService _service;
        public static IServiceProvider Services { get; private set; }
        #endregion

        private SignalRService _signalRService;

        public App(IGenericRepository GenericRep, ServicesService service, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ORep = GenericRep;
            _service = service;
            Services = serviceProvider;

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Settings.syncFusionLicence);

            // Enable verbose OneSignal logging to debug issues if needed.
            OneSignal.Debug.LogLevel = LogLevel.VERBOSE;

            // OneSignal Initialization
            OneSignal.Initialize("5b69a003-fa95-4080-bfe9-789dcdea7e39");

            // RequestPermissionAsync will show the notification permission prompt.
            // We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 5)
            OneSignal.Notifications.RequestPermissionAsync(true);

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is Not available
                MainPage = new NavigationPage(new NoInternetPage(ORep, _service));
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(Helpers.Settings.UserNameGet) && !string.IsNullOrEmpty(Helpers.Settings.PasswordGet))
                {
                    MainPage = new NavigationPage(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                }
                else
                {
                    MainPage = new NavigationPage(new LoginPage(new LoginViewModel(ORep,_service),ORep,_service));
                }

            }

            //MainPage = new NavigationPage(new Pages.PlansPages.ChoosePlanPage());
 
            OneSignal.Notifications.Clicked += Notifications_Clicked;
        }

        private async void Notifications_Clicked(object? sender, OneSignalSDK.DotNet.Core.Notifications.NotificationClickedEventArgs result)
        {
            if (result.Notification.AdditionalData != null && result.Notification.AdditionalData.ContainsKey("deeplink"))
            {
                string deepLink = result.Notification.AdditionalData["deeplink"].ToString()!;

                if (deepLink.StartsWith("Schedule"))
                {
                    List<string> list = deepLink.Split(',').ToList(); //list[1] = ScheduleId , list[2] = ScheduleDateId

                    int ScheduleId = 0, ScheduleDateId = 0;

                    bool Try1 = int.TryParse(list[1], out ScheduleId);
                    bool Try2 = int.TryParse(list[2], out ScheduleDateId);

                    if (Try1 && Try2)
                    {
                        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(ScheduleId, ScheduleDateId, ORep, _service), ORep, _service));
                    }
                    else
                    {
                        await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                    }
                }
                else if (deepLink == "Meeting")
                {
                    await App.Current!.MainPage!.Navigation.PushAsync(new Pages.NotificationsPage(new SchedulesViewModel(ORep,_service), ORep,_service));
                }
                else
                {
                    await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                }
            }
        }

        private async void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                // Connection to internet is Not available
                await App.Current!.MainPage!.Navigation.PushAsync(new NoInternetPage(ORep, _service));
                return;
            }
        }

        protected async override void OnStart()
        {
            base.OnStart();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetPlayerIdFromOneSignal();

                string plyId = !string.IsNullOrEmpty(OneSignal.User.PushSubscription.Id) ? OneSignal.User.PushSubscription.Id : "";
                Preferences.Default.Set(Helpers.Settings.PlayerId, plyId);

                await StatusLocation();

                //==============================================
                //await SignalRservice();
                //await SignalRserviceChangeUserData();
                //==============================================

                await Controls.StartData.GetCom_Main();
            }
        }

        async Task StatusLocation()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if(status != null)
            {
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            else
            {
                status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if(status != null && status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.LocationAlways>();
                }
            }
        }

        protected async override void OnSleep()
        {
            //==============================================
            //await SignalRNotservice();
            //await SignalRNotserviceChangeUserData();
            //==============================================

            // Save the current page state
            //var currentPage = Application.Current!.MainPage as NavigationPage;
            //var state = currentPage!.CurrentPage.BindingContext;
            //App.Current.Properties["CurrentPageState"] = state;

            Controls.StartData.IsRunning = false;
            //MainThread();

            //==============================================
            //_signalRService.OnMessageReceived += _signalRService_OnMessageReceivedInSleep;
            //_signalRService.OnMessageReceivedUserData += _signalRService_OnMessageReceivedChangeUserDataInSleep;
            //==============================================

            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;

            base.OnSleep();
        }


        protected async override void OnResume()
        {
            base.OnResume();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            // Retrieve the saved page state and set the page properties
            //if (App.Current.Properties.ContainsKey("CurrentPageState"))
            //{
            //    var state = App.Current.Properties["CurrentPageState"];
            //    var currentPage = App.Current!.MainPage as NavigationPage;
            //    currentPage!.CurrentPage.BindingContext = state;
            //}

            Controls.StartData.IsRunning = true;
            //MainThread();

            if (Helpers.Settings.UserNameGet == "" && Helpers.Settings.PasswordGet == "")
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
                await App.Current!.MainPage!.DisplayAlert("Alert", "You’ve been logged out.\r\n(account is opened on another device)\r\n", "Ok");
            }

            //==============================================
            //_signalRService.OnMessageReceived -= _signalRService_OnMessageReceivedInSleep;
            //_signalRService.OnMessageReceivedUserData -= _signalRService_OnMessageReceivedChangeUserDataInSleep;
            


            //await SignalRservice();
            //await SignalRserviceChangeUserData();
            //==============================================
        }


        public async Task GetPlayerIdFromOneSignal()
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (Helpers.Settings.PlayerIdGet == "")
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        // do something every 1 seconds
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            string plyId = !string.IsNullOrEmpty(OneSignal.User.PushSubscription.Id) ? OneSignal.User.PushSubscription.Id : "";
                            Preferences.Default.Set(Helpers.Settings.PlayerId, plyId);
                        });

                        return true;
                    }
                }
                return false; // runs again, or false to stop
            });
        }


        public async Task SignalRNotservice()
        {
            _signalRService.OnMessageReceived -= _signalRService_OnMessageReceived;
        }

        public async Task SignalRservice()
        {
            _signalRService = new SignalRService();

            _signalRService.OnMessageReceived += _signalRService_OnMessageReceived;

            await _signalRService.StartAsync();
        }


        public async Task SignalRNotserviceChangeUserData()
        {
            _signalRService.OnMessageReceivedUserData -= _signalRService_OnMessageReceivedChangeUserData;
        }

        public async Task SignalRserviceChangeUserData()
        {
            _signalRService = new SignalRService();

            _signalRService.OnMessageReceivedUserData += _signalRService_OnMessageReceivedChangeUserData;

            await _signalRService.StartAsync();
        }

        private async void _signalRService_OnMessageReceived(string arg1, string arg2, string arg3, string arg4)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Helpers.Settings.UserNameGet != "" && Helpers.Settings.PasswordGet != "")
                {
                    if (!string.IsNullOrEmpty(arg1) && arg1 != Helpers.Settings.PlayerIdGet && arg2.ToLower() == (Helpers.Settings.UserNameGet).ToLower())
                    {
                        Preferences.Default.Clear();
                        Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;
                        await App.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
                        Controls.StartData.IsRunning = false;
                        await App.Current!.MainPage!.DisplayAlert("Alert", "You’ve been logged out.\r\n(account is opened on another device)\r\n", "Ok");
                    }
                }
            });

        }

 
        private async void _signalRService_OnMessageReceivedChangeUserData(string arg1, string arg2, string arg3, string arg4)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Helpers.Settings.AccountIdGet == arg1 && Helpers.Settings.UserIdGet == arg2 && (Helpers.Settings.UserNameGet != arg3 || Helpers.Settings.PasswordGet != arg4))
                {

                    Preferences.Default.Clear();
                    Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;
                    await App.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
                    Controls.StartData.IsRunning = false;
                    await App.Current!.MainPage!.DisplayAlert("Alert", "You’ve been logged out.\r\n(account is changed username or password)\r\n", "Ok");
                }
            });

        }


        private void _signalRService_OnMessageReceivedInSleep(string arg1, string arg2, string arg3, string arg4)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Helpers.Settings.UserNameGet != "" && Helpers.Settings.PasswordGet != "")
                {
                    if (!string.IsNullOrEmpty(arg1) && arg1 != Helpers.Settings.PlayerIdGet && arg2.ToLower() == (Helpers.Settings.UserNameGet).ToLower())
                    {
                        Preferences.Default.Clear();
                        Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;

                        Controls.StartData.IsRunning = false;

                    }
                }
            });
        }

        private void _signalRService_OnMessageReceivedChangeUserDataInSleep(string arg1, string arg2, string arg3, string arg4)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Helpers.Settings.AccountIdGet == arg1 && Helpers.Settings.UserIdGet == arg2 && (Helpers.Settings.UserNameGet != arg3 || Helpers.Settings.PasswordGet != arg4))
                {
                    Preferences.Default.Clear();
                    Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;

                    Controls.StartData.IsRunning = false;
                }
            });
        }

    }
}

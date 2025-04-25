using Akavache;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixPro.Services.Data;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages.SchedulePages;
using FixProUs.ViewModels;
using Microsoft.AspNet.SignalR.Client;
using Syncfusion.Maui.Calendar;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Twilio.Rest.Microvisor.V1;

namespace FixProUs.Pages
{
    public partial class MainPage : Controls.CustomsPage
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        SchedulesViewModel scheduleViewModel;
        CustomersViewModel customerViewModel;
        CallsViewModel callsViewModel;
        TimeSheetViewModel timeSheetViewModel;
        MoreViewModel moreViewModel;

        private readonly bool stopping = false;

        private SignalRService _signalRService;

        static int Idincerment = 0;

        public MainPage(SchedulesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            InitializeComponent();
            ORep = GenericRep;
            _service = service;
            scheduleViewModel = model;
            SchedulesView.BindingContext = model;

            tabMain.SelectedIndex = Controls.StaticMembers.TabSelected;
        }


        async void Init()
        {
            if (tabMain.SelectedIndex == 0)
            {
                await scheduleViewModel.GetAllSchedules();

                calendar.MonthView.SpecialDayPredicate = (date) =>
                {
                    if (scheduleViewModel.GroupedList.Count > 0)
                    {
                        foreach (var day in scheduleViewModel.GroupedList)
                        {
                            if (DateTime.Parse(day.StartDate) == date.Date)
                            {
                                TimeSpan Days = date.Date.Date - DateTime.Parse(day.StartDate).Date;

                                if (date.Date == DateTime.Parse(day.StartDate).AddDays(Math.Abs(Days.TotalDays)).Date)
                                {
                                    CalendarIconDetails iconDetails = new CalendarIconDetails();
                                    iconDetails.Icon = CalendarIcon.Dot;
                                    iconDetails.Fill = Color.FromHex("#538dd4");
                                    return iconDetails;
                                }
                            }
                        }
                    }
                    ;
                    return null;
                };
            }

            grdNotify.BindingContext = scheduleViewModel;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (scheduleViewModel.LstSchedules.Count <= 0)
            {
                Init();
            }
            //==========================================
            await SignalRservice();
            await StartGetLocation();
            //==========================================

            //await Animation();
            //AccountImg.Source = !string.IsNullOrEmpty(Helpers.Settings.UserPrictureGet) ? Helpers.Settings.UserPrictureGet : "avatar.png";

            //try
            //{
            //    AccountImg.Source = Preferences.Default.Get(Helpers.Settings.UserPricture, "avatar.png");
            //}
            //catch (Exception)
            //{
            //    AccountImg.Source = "avatar.png";
            //}
            //await chatService.Connect();
            //BadgeNotifications.Num = Messages.Count;
        }


        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            //await chatService.Disconnect();
            //BadgeNotifications.Num = Messages.Count;
        }


        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(() =>
            {
                Action action = () => Application.Current!.Quit();
                Controls.StaticMembers.ShowSnackBar("Do you want to exit the program?", Controls.StaticMembers.SnackBarColor, Controls.StaticMembers.SnackBarTextColor, action);
            });
            return true;
        }


        public async Task SignalRservice()
        {
            _signalRService = new SignalRService();

            _signalRService.OnMessageReceived += _signalRService_OnMessageReceived;
            _signalRService.OnMessageReceivedUserData += _signalRService_OnMessageReceivedChangeUserData;

            await _signalRService.StartAsync();
        }

        private async void _signalRService_OnMessageReceived(string arg1, string arg2, string arg3, string arg4)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!string.IsNullOrEmpty(Preferences.Default.Get(Settings.UserName,"")) && !string.IsNullOrEmpty(Preferences.Default.Get(Settings.Password, "")))
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
                if (Helpers.Settings.AccountIdGet == arg1 && Preferences.Default.Get(Settings.UserId, "") == arg2 && (Preferences.Default.Get(Settings.UserName, "") != arg3 || Preferences.Default.Get(Settings.Password, "") != arg4))
                {

                    Preferences.Default.Clear();
                    Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;
                    await App.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
                    Controls.StartData.IsRunning = false;
                    await App.Current!.MainPage!.DisplayAlert("Alert", "You’ve been logged out.\r\n(account is changed username or password)\r\n", "Ok");
                }
            });
        }

        async Task StartGetLocation()
        {
            var permission = await Permissions.RequestAsync<Permissions.LocationAlways>();

            if (permission == PermissionStatus.Denied)
            {
                // TODO Let the user know they need to accept
                return;
            }

            if (Helpers.Settings.UserRoleGet != "4")
            {
                if (Device.RuntimePlatform == Device.iOS)
                {

                    if (Geolocation.Default.IsListeningForeground)
                    {
                        Geolocation.Default.StopListeningForeground();
                        Geolocation.Default.LocationChanged -= Default_LocationChanged;
                        return;
                    }

                    await Geolocation.Default.StartListeningForegroundAsync(new GeolocationListeningRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5)));


                    //await Geolocation.Default.StartListeningForegroundAsync(TimeSpan.FromSeconds(3), 10, false, new Plugin.Geolocator.Abstractions.ListenerSettings
                    //{
                    //    ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                    //    AllowBackgroundUpdates = true,
                    //    DeferLocationUpdates = false,
                    //    DeferralDistanceMeters = 10,
                    //    DeferralTime = TimeSpan.FromSeconds(5),
                    //    ListenForSignificantChanges = true,
                    //    PauseLocationUpdatesAutomatically = true
                    //});

                    Geolocation.Default.LocationChanged += Default_LocationChanged;

                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    CancellationToken token = CancellationToken.None;
                    await StartAsync(token);

                }
            }
        }


        //SignalR Location iOS
        private async void Default_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            try
            {
                //List<DataMapsModel> Listmap = new List<DataMapsModel>();
                //Idincerment += 1;

                //Listmap.Add(new DataMapsModel
                //{
                //    Id = Idincerment,
                //    BranchId = int.Parse(Helpers.Settings.BranchIdGet),
                //    EmployeeId = int.Parse(Helpers.Settings.UserIdGet),
                //    Lat = e.Location.Latitude.ToString(),
                //    Long = e.Location.Longitude.ToString(),
                //    Time = e.Location.Timestamp.TimeOfDay.ToString(),
                //    CreateDate = DateTime.Now.ToShortDateString(),
                //    MPosition = new Location(e.Location.Latitude, e.Location.Longitude),
                //});

                //await Helpers.Utility.PostData("api/UploadXML/PostXmlFile", JsonConvert.SerializeObject(Listmap, Newtonsoft.Json.Formatting.None,
                //            new JsonSerializerSettings()
                //            {
                //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //            }));

                if (Helpers.Settings.UserIdGet != "4")
                {
                    var location = await MainThread.InvokeOnMainThreadAsync<Location>(() =>
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                        return Geolocation.GetLocationAsync(request);
                    });

                    if (location != null)
                    {
                        Idincerment += 1;

                        var locationData = new DataMapsModel
                        {
                            Id = Idincerment,
                            BranchId = int.Parse(Helpers.Settings.BranchIdGet),
                            EmployeeId = int.Parse(Helpers.Settings.UserIdGet),
                            Lat = location.Latitude.ToString(),
                            Long = location.Longitude.ToString(),
                            Time = location.Timestamp.ToString(),
                            CreateDate = DateTime.Now.ToShortDateString(),
                            MPosition = new Location(location.Latitude, location.Longitude),
                        };

                        // Send location data via SignalR
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await _signalRService.SendLocation(locationData);
                        });
                    }
                }
            }
            catch (Exception)
            {
                await App.Current!.MainPage!.DisplayAlert("Alert", "Failed save your position for tracking !!", "OK");
            }
        }

        //SignalR Location Android
        public async Task StartAsync(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (!stopping)
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            var location = await MainThread.InvokeOnMainThreadAsync<Location>(() =>
                            {
                                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                                return Geolocation.GetLocationAsync(request);
                            });

                            if (location != null)
                            {
                                Idincerment += 1;

                                var locationData = new DataMapsModel
                                {
                                    Id = Idincerment,
                                    BranchId = int.Parse(Helpers.Settings.BranchIdGet),
                                    EmployeeId = int.Parse(Helpers.Settings.UserIdGet),
                                    Lat = location.Latitude.ToString(),
                                    Long = location.Longitude.ToString(),
                                    Time = location.Timestamp.ToString(),
                                    CreateDate = DateTime.Now.ToShortDateString(),
                                    MPosition = new Location(location.Latitude, location.Longitude),
                                };

                                // Send location data via SignalR
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await _signalRService.SendLocation(locationData);
                                });

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        await Task.Delay(2000); // Reduce CPU usage
                    }
                }, token);
            }
            catch (Exception ex)
            {

            }
        }

        //Abdullah 
        private async void SfTabView_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
        {
            if (e.NewIndex == 0)
            {
                SchedulesView.BindingContext = scheduleViewModel;

                calendar.MonthView.SpecialDayPredicate = (date) =>
                {
                    if (scheduleViewModel.GroupedList.Count > 0)
                    {
                        foreach (var day in scheduleViewModel.GroupedList)
                        {
                            if (DateTime.Parse(day.StartDate) == date.Date)
                            {
                                TimeSpan Days = date.Date.Date - DateTime.Parse(day.StartDate).Date;

                                if (date.Date == DateTime.Parse(day.StartDate).AddDays(Math.Abs(Days.TotalDays)).Date)
                                {
                                    CalendarIconDetails iconDetails = new CalendarIconDetails();
                                    iconDetails.Icon = CalendarIcon.Dot;
                                    iconDetails.Fill = Color.FromHex("#538dd4");
                                    return iconDetails;
                                }
                            }
                        }
                    }
                    ;
                    return null;
                };
            }
            else if (e.NewIndex == 1)
            {
                customerViewModel = new CustomersViewModel(ORep, _service);
                CustomersView.BindingContext = customerViewModel;
            }
            else if (e.NewIndex == 2)
            {
                callsViewModel = new CallsViewModel(ORep, _service);
                CallsView.BindingContext = callsViewModel;
            }
            else if (e.NewIndex == 3)
            {
                timeSheetViewModel = new TimeSheetViewModel(ORep, _service);
                TimeSheetsView.BindingContext = timeSheetViewModel;
            }
            else if (e.NewIndex == 4)
            {
                moreViewModel = new MoreViewModel(ORep, _service);
                MoreView.BindingContext = moreViewModel;
            }
        }

        //Abdullah 
        private void CheckInTapped(object sender, TappedEventArgs e)
        {
            stkClockIN.IsVisible = true;
            stkClockOUT.IsVisible = false;
            lstEmployeesOut.IsVisible = false;
            lstEmployeesIn.IsVisible = true;

            if (timeSheetViewModel.LstEmployeesIn.Count == 0)
            {
                stkNoDataIN.IsVisible = true;
                stkNoDataOUT.IsVisible = false;
            }
            else
            {
                stkNoDataIN.IsVisible = false;
                stkNoDataOUT.IsVisible = true;
            }
        }
        //Abdullah 
        private void ChecOutTapped(object sender, TappedEventArgs e)
        {
            stkClockIN.IsVisible = false;
            stkClockOUT.IsVisible = true;
            lstEmployeesIn.IsVisible = false;
            lstEmployeesOut.IsVisible = true;

            if (timeSheetViewModel.LstEmployeesOut.Count == 0)
            {
                stkNoDataOUT.IsVisible = true;
                stkNoDataIN.IsVisible = false;
            }
            else
            {
                stkNoDataOUT.IsVisible = false;
                stkNoDataIN.IsVisible = true;
            }
        }
        //Abdullah
        private async void ExitTapped(object sender, TappedEventArgs e)
        {
            Action action = async () =>
            {
                Preferences.Default.Clear();
                await BlobCache.LocalMachine.InvalidateAll();
                await BlobCache.LocalMachine.Vacuum();
                await App.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
            };
            Controls.StaticMembers.ShowSnackBar("Do you want to logout?", Controls.StaticMembers.SnackBarColor, Controls.StaticMembers.SnackBarTextColor, action);
        }
        //Abdullah
        private void TimeSheet_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
        {
            if (e.NewIndex == 0)
            {
                stkClockIN.IsVisible = true;
                stkClockOUT.IsVisible = false;
                lstEmployeesOut.IsVisible = false;
                lstEmployeesIn.IsVisible = true;

                if (timeSheetViewModel.LstEmployeesIn.Count == 0)
                {
                    stkNoDataIN.IsVisible = true;
                    stkNoDataOUT.IsVisible = false;
                }
                else
                {
                    stkNoDataIN.IsVisible = false;
                    stkNoDataOUT.IsVisible = true;
                }
            }
            else if (e.NewIndex == 1)
            {
                stkClockIN.IsVisible = false;
                stkClockOUT.IsVisible = true;
                lstEmployeesIn.IsVisible = false;
                lstEmployeesOut.IsVisible = true;

                if (timeSheetViewModel.LstEmployeesOut.Count == 0)
                {
                    stkNoDataOUT.IsVisible = true;
                    stkNoDataIN.IsVisible = false;
                }
                else
                {
                    stkNoDataOUT.IsVisible = false;
                    stkNoDataIN.IsVisible = true;
                }
            }
        }


        //Schedule Tab 
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                if (Controls.StartData.EmployeeDataStatic.ActiveCreateSchedule == true)
                {
                    Controls.StaticMembers.WayAfterChooseCust = 0; //Create New Schedule
                    await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ChooseCustomerPage(new CustomersViewModel(ORep, _service), ORep, _service));
                }
                else
                {
                    var toast = Toast.Make("Sorry, You don't have access to create schedule", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }
        }

        private void swchCalenderView_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == false)
            {
                schedule.View = Syncfusion.Maui.Scheduler.SchedulerView.Week;
            }
            if (e.Value == true)
            {
                schedule.View = Syncfusion.Maui.Scheduler.SchedulerView.Day;
            }
        }

        private async void schedule_CellTapped(object sender, Syncfusion.Maui.Scheduler.SchedulerTappedEventArgs e)
        {
            UserDialogs.Instance.ShowLoading();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (e.Appointments != null)
                {
                    SchedulesModel ScheduleId = e.Appointments.FirstOrDefault() as SchedulesModel;
                    await App.Current!.MainPage!.Navigation.PushAsync(new ScheduleDetailsPage(new ScheduleDetailsViewModel(ScheduleId!.Id, ScheduleId.OneScheduleDate.Id, ORep, _service), ORep, _service));
                }
            }

            UserDialogs.Instance.HideHud();
        }

        private void calendar_SelectionChanged(object sender, Syncfusion.Maui.Calendar.CalendarSelectionChangedEventArgs e)
        {
            DateTime cc = Convert.ToDateTime(e.NewValue);
            string day = cc.ToString("yyyy-MM-dd");
            var Fird = scheduleViewModel.LstSchedules.Where(x => x.StartDate == day).ToList();
            var Scon = Fird.OrderBy(o => o.From);
            colJobs.ItemsSource = new ObservableCollection<SchedulesModel>(Scon);
        }

        private void swchCalenderOrListView_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == false)
            {
                calendar.IsVisible = true;
                stkSwtScheduleView.IsVisible = false;
                schedule.IsVisible = false;
                colJobs.IsVisible = true;
            }
            if (e.Value == true)
            {
                schedule.IsVisible = true;
                calendar.IsVisible = false;
                stkSwtScheduleView.IsVisible = true;
                colJobs.IsVisible = false;
            }
        }

        //Search Btn
        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            calendar.IsVisible = false;
            stkSwtScheduleView.IsVisible = false;
            schedule.IsVisible = false;
            colJobs.IsVisible = false;
            stkListOrCalAndWeekOrDays.IsVisible = false;
            stkSearch.IsVisible = true;
            lblSearch.IsVisible = false;
            lblCalender.IsVisible = true;
            stkSearchItems.IsVisible = true;
            srchJobs.Text = "";
            colSearchJobs.IsVisible = false;
        }

        //Calendar Btn
        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            if (swchCalenderOrListView.IsToggled == true)
            {
                //Schedule
                schedule.IsVisible = true;
                calendar.IsVisible = false;
                colJobs.IsVisible = false;
                stkSwtScheduleView.IsVisible = true;
                colSearchJobs.IsVisible = false;
            }
            else
            {
                //Calendar
                calendar.IsVisible = true;
                schedule.IsVisible = false;
                colJobs.IsVisible = true;
                stkSwtScheduleView.IsVisible = false;
                colSearchJobs.IsVisible = false;
            }

            stkListOrCalAndWeekOrDays.IsVisible = true;
            stkSearch.IsVisible = false;
            lblSearch.IsVisible = true;
            lblCalender.IsVisible = false;
        }

        private void srchJobs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                stkSearchItems.IsVisible = false;
                colSearchJobs.IsVisible = true;
            }
            else
            {
                stkSearchItems.IsVisible = true;
                colSearchJobs.IsVisible = false;
            }
        }

    }

}

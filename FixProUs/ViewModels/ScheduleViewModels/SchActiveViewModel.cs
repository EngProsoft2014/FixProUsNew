using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using System.Collections.ObjectModel;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using GoogleApi.Entities.Translate.Common.Enums;
using System.Threading.Tasks;


namespace FixProUs.ViewModels
{
    public partial class SchActiveViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        [ObservableProperty]
        ObservableCollection<EmployeeModel> lstEmpInOneCategory;

        [ObservableProperty]
        ObservableCollection<EmployeesCategoryModel> lstEmpCategory;

        [ObservableProperty]
        SchaduleDateModel oneScheduleDate;

        [ObservableProperty]
        EmployeesCategoryModel empCategory;

        [ObservableProperty]
        EmployeesCategoryModel selectedCateory;

        [ObservableProperty]
        SchedulesModel scheduleDetails;

        [ObservableProperty]
        EmployeeModel selectedEmployeeAddDate;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        DateTime scheduleDate;

        [ObservableProperty]
        string startTime;

        [ObservableProperty]
        string endTime;

        [ObservableProperty]
        string spentHours;

        [ObservableProperty]
        string spentMin;

        [ObservableProperty]
        DateTime scheduleAddDate;

        [ObservableProperty]
        TimeSpan timeFromAddDate;

        [ObservableProperty]
        TimeSpan timeToAddDate;

        [ObservableProperty]
        string oldResonNotServiced;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items

        [ObservableProperty]
        bool isReOpen;


        public SchActiveViewModel(SchedulesModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            
            InitMain(model);
        }


        async Task InitMain(SchedulesModel model)
        {
            await Init();
            IsReOpen = ((String.IsNullOrEmpty(model.OneScheduleDate.StartTime) == true && model.OneScheduleDate.Status == 0)) ? true : false; //Show ReOpen Button If Don't Start Job after NotServiced only
            OldResonNotServiced = model.OneScheduleDate.Reasonnotserve;
            ShowQty = true; //Invoice Page

            ScheduleDetails = model;

            ScheduleDate = DateTime.Parse(model.OneScheduleDate.Date);

            ScheduleAddDate = DateTime.Now;

            StartTime = ScheduleDetails.OneScheduleDate.StartTime == null ? "No start yet" : ScheduleDetails.OneScheduleDate.StartTime;
            EndTime = ScheduleDetails.OneScheduleDate.EndTime == null ? "No end yet" : ScheduleDetails.OneScheduleDate.EndTime;

            SpentHours = ScheduleDetails.OneScheduleDate.SpentTimeHour == null ? "Wait job finish" : ScheduleDetails.OneScheduleDate.SpentTimeHour;
            SpentMin = ScheduleDetails.OneScheduleDate.SpentTimeMin == null ? "Wait job finish" : ScheduleDetails.OneScheduleDate.SpentTimeMin;

            OneScheduleDate = ScheduleDetails.OneScheduleDate;

        }

        async Task Init()
        {
            ScheduleDetails = new SchedulesModel();
            EmpCategory = new EmployeesCategoryModel();
            LstEmpInOneCategory = new ObservableCollection<EmployeeModel>();
            LstEmpCategory = new ObservableCollection<EmployeesCategoryModel>();
            SelectedCateory = new EmployeesCategoryModel();
            SelectedEmployeeAddDate = new EmployeeModel();
            EmployeePermission = new EmployeeModel();
            OneScheduleDate = new SchaduleDateModel();

            await GetPerrmission();
            await GetEmpCategories();
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

        //Get Employees Category
        public async Task GetEmpCategories()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<EmployeesCategoryModel>>(string.Format("api/Employee/GetEmpCategory?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstEmpCategory = json;

                    if (ScheduleDetails?.EmployeeCategoryId != null && ScheduleDetails?.LstScheduleEmployeeDTO.Count > 0)
                    {
                        SelectedCateory = LstEmpCategory.Where(x => x.Id == ScheduleDetails?.EmployeeCategoryId).FirstOrDefault();
                    }
                    else
                    {
                        SelectedCateory = LstEmpCategory.FirstOrDefault();
                    }
                }

                UserDialogs.Instance.HideHud();
            }
        }


        string SendSMS(string Phone, string Msg)
        {
            var accountSid = "AC2aa33faec930e6bddfef1daa25e3b945";
            var authToken = "744fd3259244985557d4d0c1aa2617eb";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+1" + Phone));

            messageOptions.From = new PhoneNumber("+18885307372");
            messageOptions.Body = Msg;
            var message = MessageResource.Create(messageOptions);

            return message.Sid;
        }

        // Get Employees in One Category //ADD Sch
        [RelayCommand]
        async Task SelectedEmpCategory(EmployeesCategoryModel model)
        {
            EmpCategory = model;

            //StrEmployees = "Choose Employees";
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<EmployeeModel>>(string.Format("api/Employee/GetEmpInOneCategory/{0}/{1}/{2}/{3}/{4}", Helpers.Settings.BranchIdGet, EmpCategory.Id, Helpers.Settings.AccountIdGet, Controls.StartData.EmployeeDataStatic.UserRole, Helpers.Settings.UserIdGet), UserToken);

                if (json != null)
                {
                    LstEmpInOneCategory = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task SaveReOpenScheduleDate(SchaduleDateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    string UserToken = await _service.UserToken();
                    model.Status = 1;

                    UserDialogs.Instance.ShowLoading();
                    //var json = await Helpers.Utility.PutPosData("api/Schedules/PutScheduleDate", JsonConvert.SerializeObject(model));
                    var json = await ORep.PutAsync("api/Schedules/PutScheduleDate", model, UserToken);

                    if (json != null)
                    {
                        var toast = Toast.Make("Successsfully Re Open Service.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(ScheduleDetails.Id, model.Id, ORep, _service), ORep, _service));
                        App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                    }
                    UserDialogs.Instance.HideHud();
                }
                else
                {
                    var toast = Toast.Make("This Schedule Not found.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SaveResponNotServiceScheduleDate(SchaduleDateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model.Reasonnotserve != null)
                {
                    string UserToken = await _service.UserToken();
                    model.Status = 0;

                    model.CreateDate = DateTime.Now;

                    model.Reasonnotserve = OldResonNotServiced != null ? (OldResonNotServiced + " , " + model.Reasonnotserve + "-" + DateTime.Now.ToString()) : (model.Reasonnotserve + " - " + DateTime.Now.ToString());

                    UserDialogs.Instance.ShowLoading();
                    //var json = await Helpers.Utility.PutPosData("api/Schedules/PutScheduleDate", JsonConvert.SerializeObject(model));
                    var json = await ORep.PutAsync("api/Schedules/PutScheduleDate", model, UserToken);

                    if (json != null)
                    {
                        var toast = Toast.Make("Save Respon Not Service.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(ScheduleDetails.Id, model.Id, ORep, _service), ORep, _service));
                        App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                    }
                    UserDialogs.Instance.HideHud();
                }
                else
                {
                    var toast = Toast.Make("Enter Respon Not Service, please .", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task AddScheduleDate(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    if (TimeFromAddDate != new TimeSpan(00, 0, 0, 00) && TimeToAddDate != new TimeSpan(00, 0, 0, 00) && SelectedEmployeeAddDate != null)
                    {
                        string UserToken = await _service.UserToken();

                        model.OneScheduleDate.Date = ScheduleAddDate.ToString("yyyy-MM-dd");
                        model.Time = model.OneScheduleDate.StartTime = TimeFromAddDate.ToString(@"hh\:mm");
                        model.TimeEnd = model.OneScheduleDate.EndTime = TimeToAddDate.ToString(@"hh\:mm");
                        model.OneScheduleDate.Status = 1;
                        model.CalendarColor = model.OneScheduleDate.CalendarColor = "#5e92e6";
                        //model.LstEmployeeDTO.Clear();
                        //model.LstEmployeeDTO.Add(SelectedEmployeeAddDate);
                        model.OneScheduleDate.OneEmployee = SelectedEmployeeAddDate;
                        //model.LstMaterialReceipt.Clear();
                        //model.LstScheduleItemsServices.Clear();
                        //model.LstSchedulePictures.Clear();
                        //model.Notes = null;


                        model.CreateDate = DateTime.Now;

                        UserDialogs.Instance.ShowLoading();

                        var json = await ORep.PostStrAsync("api/Schedules/PostAddScheduleDate", model, UserToken);

                        if (!string.IsNullOrEmpty(json))
                        {
                            var toast = Toast.Make("Successfully Add Another Date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();

                            Controls.StaticMembers.TabSelected = 0;
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);

                            if (model.Id == 0)
                            {
                                string Massage = $"Hi {model.CustomerName}! Your service appointment with {Helpers.Settings.AccountName} has been scheduled for {DateTime.Parse(model.StartDate).ToString("MMMM dd, yyyy")}. Your technician will arrive between {DateTime.Parse(model.Time).ToString("hh:mmtt")} - {DateTime.Parse(model.TimeEnd).ToString("hh:mmtt")} CDT.";

                                string returnMsg = SendSMS(ScheduleDetails.CustomerDTO.Phone1, Massage);
                                if (string.IsNullOrEmpty(returnMsg))
                                {
                                    var toast1 = Toast.Make("Successfully for Save Schedule but Faild Send SMS to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                    await toast1.Show();
                                }
                            }
                        }
                        else
                        {
                            var toast = Toast.Make("Failed for Add Another Date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                        UserDialogs.Instance.HideHud();
                    }
                    else
                    {
                        var toast = Toast.Make("Complete All Fields, please.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task DoneScheduleDate(SchaduleDateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    string UserToken = await _service.UserToken();
                    model.Status = 2;
                    model.CalendarColor = "#676d75";

                    model.CreateDate = DateTime.Now;

                    UserDialogs.Instance.ShowLoading();

                    var json = await ORep.PutStrAsync("api/Schedules/PutScheduleDate", model, UserToken);

                    if (!string.IsNullOrEmpty(json) && json.Contains("Not Done All Employee") == true)
                    {
                        var toast = Toast.Make("Failed Job Done because find employee is not finished.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else if (!string.IsNullOrEmpty(json))
                    {
                        bool answer = await App.Current!.MainPage!.DisplayAlert("Question?", "Do you want send massage to customer?", "Yes", "No");
                        if (answer)
                        {
                            string Massage = $"Hello {model.CustomerName}, thank you for choosing us. We hope your experience was satisfactory. Your feedback means a lot to us! Please consider leaving a Google review here: {model.GoogleReviewLink}. Have a great day!";

                            string returnMsg = SendSMS(model.CustomerPhone, Massage);
                            if (string.IsNullOrEmpty(returnMsg))
                            {
                                var toast1 = Toast.Make("Successfully Done Schedule but Faild Send SMS to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast1.Show();
                            }
                        }

                        var toast = Toast.Make("Successfully End schedule Date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();

                        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(ScheduleDetails.Id, model.Id, ORep, _service), ORep, _service));
                        App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                    }
                    else
                    {
                        var toast = Toast.Make("Failed End schedule Date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    UserDialogs.Instance.HideHud();
                }
            }

            IsEnable = true;
        }
    }
}

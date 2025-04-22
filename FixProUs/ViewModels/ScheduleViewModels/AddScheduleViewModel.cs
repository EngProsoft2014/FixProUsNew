using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages.PopupPages;
using Mopups.Services;
using System.Collections.ObjectModel;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using SkiaSharp;


namespace FixProUs.ViewModels
{
    public partial class AddScheduleViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        [ObservableProperty]
        ObservableCollection<EmployeeModel> lstEmpInOneCategory;

        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstServices;

        [ObservableProperty]
        ObservableCollection<EmployeesCategoryModel> lstEmpCategory;

        [ObservableProperty]
        ObservableCollection<ScheduleEmployeesModel> lstEmps;

        [ObservableProperty]
        ObservableCollection<PriorityModel> lstPriority;

        [ObservableProperty]
        PriorityModel onePriorityModel;

        [ObservableProperty]
        SchedulesModel scheduleDetails;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        EmployeesCategoryModel empCategory;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        ItemsServicesModel selectedService = new ItemsServicesModel();

        [ObservableProperty]
        EmployeesCategoryModel selectedCateory;

        [ObservableProperty]
        string branchName;

        [ObservableProperty]
        DateTime scheduleDate;

        [ObservableProperty]
        int schedulePage;

        [ObservableProperty]
        TimeSpan timeFrom;

        [ObservableProperty]
        TimeSpan timeTo;

        [ObservableProperty]
        string strEmployees;

        [ObservableProperty]
        string strEmployeesId;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items


        public AddScheduleViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init(model);
        }

        async void Init(CustomersModel model)
        {
            CustomerDetails = new CustomersModel();
            ScheduleDetails = new SchedulesModel();
            EmpCategory = new EmployeesCategoryModel();
            OnePriorityModel = new PriorityModel();
            LstEmpInOneCategory = new ObservableCollection<EmployeeModel>();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstMaterialReceipt = new List<ScheduleMaterialReceiptModel>();
            LstServices = new ObservableCollection<ItemsServicesModel>();
            LstEmps = new ObservableCollection<ScheduleEmployeesModel>();
            LstServices = new ObservableCollection<ItemsServicesModel>();
            LstPriority = new ObservableCollection<PriorityModel>();

            LstPriority.Add(new PriorityModel() { Id = 1, Name = "Normal" });
            LstPriority.Add(new PriorityModel() { Id = 2, Name = "Urgent" });

            OnePriorityModel = new PriorityModel() { Id = 1, Name = "Normal" };

            SchedulePage = 0; //New Schedule
            ScheduleDetails = new SchedulesModel();
            CustomerDetails = model;

            await Task.WhenAll(GetPerrmission(), GetServices(), GetEmpCategories());

            ShowQty = false; //New Schedule
            if (Controls.StaticMembers.WayAfterChooseCust == 1 || Controls.StaticMembers.WayAfterChooseCust == 2)
            {
                ShowQty = true; //New Estimate Or Invoice
            }

            //StrEmployees = "Choose Employees";
            ScheduleDate = DateTime.Now;

            BranchName = Helpers.Settings.BranchNameGet;

            //Chech the year now because change value for House details
            CheckHouseDataCust(model);

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

        async Task GetServices()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesModel>>(string.Format("api/Schedules/GetServices?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstServices = json;

                    if (ScheduleDetails.OneScheduleService != null && ScheduleDetails.OneScheduleService.ScheduleDateId == null)
                    {
                        SelectedService = LstServices.Where(x => x.Id == ScheduleDetails.OneScheduleService.ItemsServicesId).FirstOrDefault();
                    }
                }

                UserDialogs.Instance.HideHud();
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

                        string str = "";
                        foreach (ScheduleEmployeesModel Emp in ScheduleDetails?.LstScheduleEmployeeDTO)
                        {
                            str += ("," + Emp.EmpUserName);
                        }
                        StrEmployees = str.Remove(0, 1);
                    }
                    else
                    {
                        SelectedCateory = LstEmpCategory.FirstOrDefault();
                    }
                }

                UserDialogs.Instance.HideHud();
            }
        }

        async void CheckHouseDataCust(CustomersModel model)
        {
            if (!string.IsNullOrEmpty(model.YearEstimedValue))
            {
                //if (DateTime.Now.Year - int.Parse(model.YearEstimedValue) > 1)
                //{
                //    model = await Controls.StartData.GetAddressDetails(model);
                //}

                if (int.TryParse(model.YearEstimedValue, out int estimatedYear) && (DateTime.Now.Year - estimatedYear > 1))
                {
                    CustomerDetails = await Controls.StartData.GetAddressDetails(model);
                }
            }
        }


        // Get Employees in One Category
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
        async Task OpenCustomerDetails(CustomersModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.WayCreateCust = 3;//From Schedule can edit customer and return schedule again
            Controls.StaticMembers.ScheduleIdStatic = ScheduleDetails.Id;
            Controls.StaticMembers.ScheduleDateIdStatic = ScheduleDetails.ScheduleDateId;
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CustomersDetailsPage(new CustInformationViewModel(model, ORep, _service), model, ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenEmployeesInOneCategory()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new EmployeesPopup(LstEmpInOneCategory, this);
                popupView.EmployeesClose += (Empolyees) =>
                {

                    //LstEmps.Clear();
                    //StrEmployees = "";
                    //StrEmployeesId = "";
                    IsEnable = false;
                    if (Empolyees.Count != 0)
                    {
                        string str = "";
                        string strId = "";
                        foreach (var Emp in Empolyees)
                        {
                            str += ("," + Emp.UserName);
                            strId += ("," + Emp.Id);
                            LstEmps.Add(new ScheduleEmployeesModel
                            {
                                EmpId = Emp.Id,
                                EmpFullName = Emp.FirstName + " " + Emp.LastName,
                                EmpUserName = Emp.UserName,
                            });
                        }

                        if (!string.IsNullOrEmpty(StrEmployees) && !string.IsNullOrEmpty(StrEmployeesId))
                        {
                            StrEmployees += str;
                            StrEmployeesId += strId;
                        }
                        else
                        {
                            StrEmployees = str.Remove(0, 1);
                            StrEmployeesId = strId.Remove(0, 1);
                        }
                    }
                    IsEnable = true;
                };

                await MopupService.Instance.PushAsync(popupView);
            }

            IsEnable = true;
        }

        [RelayCommand]
        void RemoveEmployee(ScheduleEmployeesModel employee)
        {
            LstEmps.Remove(employee);

            //Empolyees Names 
            int index = StrEmployees.IndexOf(employee.EmpUserName + ",");
            StrEmployees = (index < 0) ? StrEmployees : StrEmployees.Remove(index, (employee.EmpUserName + ",").Length);

            int index2 = StrEmployees.IndexOf("," + employee.EmpUserName);
            StrEmployees = (index2 < 0) ? StrEmployees : StrEmployees.Remove(index2, ("," + employee.EmpUserName).Length);

            int index3 = StrEmployees.IndexOf(employee.EmpUserName);
            StrEmployees = (index3 < 0) ? StrEmployees : StrEmployees.Remove(index3, (employee.EmpUserName).Length);


            //Empolyees Ids 
            int indexId = StrEmployeesId.IndexOf(employee.EmpId + ",");
            StrEmployeesId = (indexId < 0) ? StrEmployeesId : StrEmployeesId.Remove(indexId, (employee.EmpId + ",").Length);

            int indexId2 = StrEmployeesId.IndexOf("," + employee.EmpId);
            StrEmployeesId = (indexId2 < 0) ? StrEmployeesId : StrEmployeesId.Remove(indexId2, ("," + employee.EmpId).Length);

            int indexId3 = StrEmployeesId.IndexOf(employee.EmpId.ToString()!);
            StrEmployeesId = (indexId3 < 0) ? StrEmployeesId : StrEmployeesId.Remove(indexId3, (employee.EmpId.ToString()!).Length);
        }

        [RelayCommand]
        async Task FullScreenNote(string Note)
        {
            var popupView = new FullScreenNoteViewModel(Note);
            popupView.NoteClose += (note) =>
            {
                ScheduleDetails.Notes = note;
            };
            var page = new Pages.SchedulePages.FullScreenNotePage();
            page.BindingContext = popupView;
            await App.Current!.MainPage!.Navigation.PushAsync(page);
        }

        [RelayCommand]
        async Task SelectedSubmitSchedule(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                ScheduleDetails = model;
                ScheduleDetails.CustomerDTO = CustomerDetails;
                ScheduleDetails.CustomerName = CustomerDetails.FirstName + " " + CustomerDetails.LastName;
                ScheduleDetails.AccountId = int.Parse(Helpers.Settings.AccountIdGet);
                ScheduleDetails.BrancheId = int.Parse(Helpers.Settings.BranchIdGet);
                ScheduleDetails.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                ScheduleDetails.Active = true;
                ScheduleDetails.Recurring = false;
                ScheduleDetails.FrequencyType = 1;
                ScheduleDetails.EndType = 1;
                ScheduleDetails.PriorityId = OnePriorityModel.Id;
                ScheduleDetails.Time = TimeFrom.ToString(@"hh\:mm");
                ScheduleDetails.TimeEnd = TimeTo.ToString(@"hh\:mm");
                ScheduleDetails.ScheduleDate = ScheduleDetails.StartDate = ScheduleDate.ToString("yyyy-MM-dd");

                ScheduleItemsServicesModel ItemsModel = new ScheduleItemsServicesModel
                {
                    AccountId = model.AccountId,
                    BrancheId = model.BrancheId,
                    ScheduleId = model.Id,
                    ItemsServicesId = SelectedService.Id,
                    ItemServiceDescription = SelectedService.Description,
                    CostRate = SelectedService.CostperUnit,
                    Notes = SelectedService.Notes,
                    Active = SelectedService.Active,
                    CreateUser = model.CreateUser,
                    CreateDate = DateTime.Now,
                };
                ScheduleDetails.OneScheduleService = ItemsModel;
                //ScheduleDetails.LstScheduleItemsServices = LstItems.ToList();

                if (CustomerDetails.Id != 0)
                {
                    ScheduleDetails.CustomerId = CustomerDetails.Id;
                }
                ScheduleDetails.Location = CustomerDetails.Address;
                ScheduleDetails.EmployeeCategoryId = EmpCategory.Id;
                if (StrEmployeesId != null)
                {
                    ScheduleDetails.Employees = StrEmployeesId;
                }
                //ScheduleDetails.CalendarColor = LstColors.Where(x => x.IsChecked == true).Select(c => c.ColorHex).FirstOrDefault();
                ScheduleDetails.CalendarColor = "#5e92e6";
                ScheduleDetails.CreateDate = DateTime.Now;

                if (string.IsNullOrEmpty(ScheduleDetails.Title))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Title.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ScheduleDetails.ScheduleDate))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Schedule Date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ScheduleDetails.Time))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Start Time.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ScheduleDetails.TimeEnd))
                {
                    var toast = Toast.Make("Please Complete This Field Required : End Time.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (ScheduleDetails.EmployeeCategoryId == null || ScheduleDetails.EmployeeCategoryId == 0)
                {
                    var toast = Toast.Make("Please Complete This Field Required : Choose Employee Category.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ScheduleDetails.Employees))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Choose Employees.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    if (ScheduleDetails != null)
                    {
                        string UserToken = await _service.UserToken();

                        ScheduleDetails.CallId = CustomerDetails.CallId;
                        UserDialogs.Instance.ShowLoading();
                        var Json = await ORep.PostDataAsync("api/Schedules/PostSchedule", ScheduleDetails, UserToken);
                        UserDialogs.Instance.HideHud();

                        if (Json != "Bad Request" && Json != "api not responding")
                        {
                            var toast = Toast.Make("Successfully for add schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();

                            MessagingCenter.Send(this, "CreatedSchedule", true);
                            await App.Current!.MainPage!.Navigation.PopAsync();

                            string Massage = $"Hi {model.CustomerName}! Your service appointment with {Helpers.Settings.AccountName} has been scheduled for {DateTime.Parse(model.StartDate).ToString("MMMM dd, yyyy")}. Your technician will arrive between {DateTime.Parse(model.Time).ToString("hh:mmtt")} - {DateTime.Parse(model.TimeEnd).ToString("hh:mmtt")} CDT.";

                            string returnMsg = SendSMS(ScheduleDetails.CustomerDTO.Phone1, Massage);
                            if (string.IsNullOrEmpty(returnMsg))
                            {
                                var toast1 = Toast.Make("Successfully Save Schedule but Failed Send SMS to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast1.Show();
                            }
                        }
                        else
                        {
                            var toast = Toast.Make("Failed for add schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                }
            }

            IsEnable = true;
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
    }
}

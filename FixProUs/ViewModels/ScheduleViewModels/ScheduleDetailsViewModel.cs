using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages;
using FixProUs.Pages.PopupPages;
using FixProUs.Pages.SchedulePages;
using Mopups.Services;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using static Twilio.Rest.Content.V1.ContentResource;

namespace FixProUs.ViewModels
{
    public partial class ScheduleDetailsViewModel : BaseViewModel
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
        ObservableCollection<ScheduleItemsServicesModel> lstItems;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItemsInvoice;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstFreeServices;

        [ObservableProperty]
        ObservableCollection<ScheduleMaterialReceiptModel> lstMaterialReceipt;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItemsEstimate;

        [ObservableProperty]
        ObservableCollection<SheetColorModel> lstColors;

        [ObservableProperty]
        ObservableCollection<EmployeesCategoryModel> lstEmpCategory;

        [ObservableProperty]
        ObservableCollection<EmployeeModel> lstEmpInOneCategory;

        [ObservableProperty]
        SchedulePicturesModel onePictureModel;

        [ObservableProperty]
        ObservableCollection<SchedulePicturesModel> lstAllPictures;

        [ObservableProperty]
        ObservableCollection<SchedulePicturesModel> lstATwoPictures;

        [ObservableProperty]
        ObservableCollection<SchedulePicturesModel> lstNewPictures;

        [ObservableProperty]
        ObservableCollection<PriorityModel> lstPriority;

        [ObservableProperty]
        ObservableCollection<ScheduleEmployeesModel> lstEmps;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceDates;

        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstServices;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDatesActual;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDatesActual;

        [ObservableProperty]
        SchaduleDateModel oneScheduleDate;

        [ObservableProperty]
        PriorityModel onePriorityModel;

        [ObservableProperty]
        EmployeeModel oneEmployee;

        [ObservableProperty]
        InvoiceModel oneInvoice;

        [ObservableProperty]
        EstimateModel oneEstimate;

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
        bool showImages;

        [ObservableProperty]
        int showDispatch;

        [ObservableProperty]
        string branchName;

        [ObservableProperty]
        int lstHeight;

        [ObservableProperty]
        bool doneFlag;

        [ObservableProperty]
        DateTime scheduleDate;

        [ObservableProperty]
        DateTime invoiceDate;

        [ObservableProperty]
        ImageSource schedulePhoto;

        [ObservableProperty]
        bool isShowAllSchedule;

        [ObservableProperty]
        bool amountOrPersent;

        [ObservableProperty]
        decimal? discount;

        [ObservableProperty]
        int schedulePage;

        [ObservableProperty]
        TimeSpan timeFrom;

        [ObservableProperty]
        TimeSpan timeTo;

        [ObservableProperty]
        bool showEstimateButton;

        [ObservableProperty]
        string strEmployees;

        [ObservableProperty]
        string strEmployeesId;

        [ObservableProperty]
        string strEstimateDates;

        [ObservableProperty]
        string strInvoiceDates;

        [ObservableProperty]
        string startTime;

        [ObservableProperty]
        string endTime;

        [ObservableProperty]
        int photosCount;

        [ObservableProperty]
        Object selectedColor;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items

        public int BranchIdVM;

        public ScheduleDetailsViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            LstEstimateSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDates = new ObservableCollection<SchaduleDateModel>();
        }

        public ScheduleDetailsViewModel(int SchedulId, int ScheduleDateId, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            ShowQty = false; //Old Schedule 
            IsShowAllSchedule = true; // Show all schedule details

            Init();

            Task.WhenAll(GetServices(), GetEmpCategories());
            Task.WhenAll(GetOneScheduleDetails(SchedulId, ScheduleDateId));

            MessagingCenter.Subscribe<SchedulePicturesPage, List<SchedulePicturesModel>>(this, "ChangeSchImagesInSchadulePage", async (sender, message) =>
            {
                if (message.Count > 0 && message.Count != LstAllPictures.Count)
                {
                    ScheduleDetails.LstSchedulePictures = message;
                    LstAllPictures = new ObservableCollection<SchedulePicturesModel>(message);
                    SetLstTwoSchedulePhotos(ScheduleDetails.LstSchedulePictures);
                    await CalcSchPhotoCount(ScheduleDetails.LstSchedulePictures.Count);
                }
            });
            MessagingCenter.Subscribe<SchImagesViewModel, List<SchedulePicturesModel>>(this, "ChangeSchImagesInSchadulePage", async (sender, message) =>
            {
                if (message.Count > 0 && message.Count != LstAllPictures.Count)
                {
                    ScheduleDetails.LstSchedulePictures = message;
                    LstAllPictures = new ObservableCollection<SchedulePicturesModel>(message);
                    SetLstTwoSchedulePhotos(ScheduleDetails.LstSchedulePictures);
                    await CalcSchPhotoCount(ScheduleDetails.LstSchedulePictures.Count);
                }
            });
        }


        void Init()
        {
            CustomerDetails = new CustomersModel();
            ScheduleDetails = new SchedulesModel();
            EmpCategory = new EmployeesCategoryModel();
            OneEmployee = new EmployeeModel();
            LstItems = new ObservableCollection<ScheduleItemsServicesModel>();
            LstFreeServices = new ObservableCollection<ScheduleItemsServicesModel>();
            LstMaterialReceipt = new ObservableCollection<ScheduleMaterialReceiptModel>();
            LstItemsEstimate = new ObservableCollection<ScheduleItemsServicesModel>();
            LstItemsInvoice = new ObservableCollection<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleEmployeeDTO = new List<ScheduleEmployeesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            LstColors = new ObservableCollection<SheetColorModel>();
            LstEmpCategory = new ObservableCollection<EmployeesCategoryModel>();
            LstEmpInOneCategory = new ObservableCollection<EmployeeModel>();
            OneInvoice = new InvoiceModel();
            OneInvoice.LstInvoiceItemServices = new List<InvoiceItemServicesModel>();
            OneEstimate = new EstimateModel();
            OneEstimate.LstEstimateItemServices = new List<EstimateItemServicesModel>();
            LstPriority = new ObservableCollection<PriorityModel>();
            LstEmps = new ObservableCollection<ScheduleEmployeesModel>();
            LstServices = new ObservableCollection<ItemsServicesModel>();
            SelectedService = new ItemsServicesModel();
            LstATwoPictures = new ObservableCollection<SchedulePicturesModel>();
            LstEstimateSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceDates = new ObservableCollection<SchaduleDateModel>();
            LstEstimateDates = new ObservableCollection<SchaduleDateModel>();
            //Schdule Date
            ScheduleDetails.OneScheduleDate = new SchaduleDateModel();
            OneScheduleDate = new SchaduleDateModel();

            OnePictureModel = new SchedulePicturesModel();
            BranchIdVM = int.Parse(Helpers.Settings.BranchIdGet);

            LstColors.Add(new SheetColorModel() { ColorName = "Red", ColorHex = "#eb4034" });
            LstColors.Add(new SheetColorModel() { ColorName = "Blue", ColorHex = "#2f6fde" });
            LstColors.Add(new SheetColorModel() { ColorName = "Green", ColorHex = "#23b007" });
            LstColors.Add(new SheetColorModel() { ColorName = "Black", ColorHex = "#272927" });
            LstColors.Add(new SheetColorModel() { ColorName = "Gray", ColorHex = "#878787" });
            LstColors.Add(new SheetColorModel() { ColorName = "Brwon", ColorHex = "#7d654c" });

            LstPriority.Add(new PriorityModel() { Id = 1, Name = "Normal" });
            LstPriority.Add(new PriorityModel() { Id = 2, Name = "Urgent" });

            OnePriorityModel = new PriorityModel() { Id = 1, Name = "Normal" };

            StrEstimateDates = "Choose Schedule Dates";
            StrInvoiceDates = "Choose Schedule Dates";
        }


        async void InitData(SchedulesModel model)
        {
            ShowImages = true;
            SchedulePage = 1; //Update Schedule 

            ScheduleDetails = model;
            CustomerDetails = model.CustomerDTO;

            if (model.LstScheduleItemsServices.Count > 0)
            {
                LstItems = new ObservableCollection<ScheduleItemsServicesModel>(model.LstScheduleItemsServices);
            }

            if (model.LstScheduleItemsServices.Count > 0)
            {
                LstItemsEstimate = new ObservableCollection<ScheduleItemsServicesModel>(model.LstScheduleItemsServices);
            }

            if (model.LstFreeServices.Count > 0)
            {
                LstFreeServices = new ObservableCollection<ScheduleItemsServicesModel>(model.LstFreeServices);
            }

            if (model.LstMaterialReceipt.Count > 0)
            {
                LstMaterialReceipt = new ObservableCollection<ScheduleMaterialReceiptModel>(model.LstMaterialReceipt);
            }

            InvoiceDate = DateTime.Now;

            ScheduleDate = DateTime.Parse(model.OneScheduleDate.Date);
            TimeFrom = new TimeSpan(model.OneScheduleDate.TimeHourFrom, model.OneScheduleDate.TimeMinFrom, 0);
            TimeTo = new TimeSpan(model.OneScheduleDate.TimeHourTo, model.OneScheduleDate.TimeMinTo, 0);
            OnePriorityModel = LstPriority.Where(x => x.Id == model.PriorityId).FirstOrDefault()!;

            BranchName = Helpers.Settings.BranchNameGet;

            //Schedule Pictures
            if (model.LstSchedulePictures.Count != 0)
            {
                LstAllPictures = new ObservableCollection<SchedulePicturesModel>(model.LstSchedulePictures);
                LstNewPictures = new ObservableCollection<SchedulePicturesModel>(model.LstSchedulePictures.Where(x => x.Id == 0).ToList());
                await CalcSchPhotoCount(model.LstSchedulePictures.Count);
            }

            await CalcSchPhotoCount(model.CountPhotos!.Value);

            if (model.GetPictures == true)
            {
                GetPictuers(model.Id);
            }

            OneScheduleDate = ScheduleDetails.OneScheduleDate;

            SelectedColor = LstColors.Where(x => x.ColorHex == model.CalendarColor).Select(c => c.IsChecked = true).FirstOrDefault();

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

        async Task CalcSchPhotoCount(int Count)
        {
            int Co = Count - 2;
            PhotosCount = 0;
            if (Co > 0)
            {
                PhotosCount = Co;
            }
        }

        //Get Pictuers
        async void GetPictuers(int ScheduleId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<SchedulePicturesModel>>(string.Format("api/Schedules/GetPictures?" + "ScheduleId=" + ScheduleId), UserToken);

                if (json != null)
                {
                    LstNewPictures = new ObservableCollection<SchedulePicturesModel>(); //Check if Show Button Done
                    ScheduleDetails.LstSchedulePictures = json.ToList();
                    LstAllPictures = json;

                    SetLstTwoSchedulePhotos(ScheduleDetails.LstSchedulePictures);
                }

                UserDialogs.Instance.HideHud();
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


                    if (ScheduleDetails.LstScheduleItemsServices.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    if (ScheduleDetails.EstimateDTO != null)
                    {
                        ShowEstimateButton = true;
                    }

                    InitData(ScheduleDetails);

                    LstEmps = new ObservableCollection<ScheduleEmployeesModel>(ScheduleDetails.LstScheduleEmployeeDTO);

                    foreach (var mod in ScheduleDetails.LstScheduleEmployeeDTO)
                    {
                        StrEmployeesId += ("," + mod.EmpId);
                    }
                    StrEmployeesId = StrEmployeesId?.Remove(0, 1);
                }

                UserDialogs.Instance.HideHud();
            }
        }


        public void SetLstTwoSchedulePhotos(List<SchedulePicturesModel> LstPhotos)
        {
            if (LstPhotos != null && LstPhotos.Count > 0)
            {
                ObservableCollection<SchedulePicturesModel> lstPictures = new ObservableCollection<SchedulePicturesModel>(LstPhotos);

                if (lstPictures.Count > 0)
                {
                    if (lstPictures.Count > 0 && lstPictures.Count < 2)
                    {
                        LstATwoPictures.Add(LstPhotos[0]);
                    }
                    else if (lstPictures.Count >= 2)
                    {
                        LstATwoPictures.Add(LstPhotos[0]);
                        LstATwoPictures.Add(LstPhotos[1]);
                    }
                    else
                    {
                        LstATwoPictures.Add(LstPhotos[0]);
                        LstATwoPictures.Add(LstPhotos[1]);
                    }

                }
            }
        }

        public async Task<ScheduleItemsServicesModel> InsertOneItemService(ScheduleItemsServicesModel model)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                var json = await ORep.PostDataAsync("api/Schedules/PostScheduleMaterials", model, UserToken);

                if (json != "Bad Request" && json != "api not responding" && json.Contains("Not_Enough") != true && json.Contains("This Invoice Already Exist") != true)
                {
                    var oModel = JsonConvert.DeserializeObject<ScheduleItemsServicesModel>(json);
                    return oModel;
                }
                else
                {
                    var toast = Toast.Make($"Error : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [RelayCommand]
        public async Task<ScheduleItemsServicesModel> InsertOneFreeService(ScheduleItemsServicesModel model)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                var json = await ORep.PostStrAsync("api/Schedules/PostScheduleFreeServices", model, UserToken);

                if (!string.IsNullOrEmpty(json))
                {
                    var oModel = JsonConvert.DeserializeObject<ScheduleItemsServicesModel>(json);
                    return oModel;
                }
                else
                {
                    var toast = Toast.Make("Failed add this Service.}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

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
        async Task SelectedDispatch(SchaduleDateModel model)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                UserDialogs.Instance.ShowLoading();
                string Json = await ORep.PutStrAsync("api/Schedules/PutScheduleDispatch", model, UserToken);
                UserDialogs.Instance.HideHud();

                if (!string.IsNullOrEmpty(Json) && Json.Contains("Success Dispatch") == true)
                {
                    var toast = Toast.Make("Successfully for Dispatch Schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                    ShowDispatch = 2; //Don't Show Dispatch Button
                }
                else
                {
                    var toast = Toast.Make("Failed for Dispatch Schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }
            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectJobDetails(SchedulesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleJobDetailsPage(new SchActiveViewModel(model, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task MyWay(CustomersModel model)
        {
            IsEnable = false;
            var popupView = new OnMyWayViewModel(model);
            var page = new OnMyWayPopup();
            page.BindingContext = popupView;
            await MopupService.Instance.PushAsync(page);
            IsEnable = true;
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
        async Task CallCustomer(CustomersModel model)
        {
            try
            {
                int tel;
                bool Result = int.TryParse(model.Phone1, out tel);
                if (Result)
                {
                    PhoneDialer.Open(model.Phone1);
                }
                else
                {
                    var toast = Toast.Make("You don't access this customer's phone", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }

            }
            catch (FeatureNotSupportedException ex)
            {
                // Handle the case where the phone dialer is not supported on the device
                var toast = Toast.Make("Phone dialer is not supported on this device.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }
            catch (Exception ex)
            {
                // Handle other errors that might occur
                var toast = Toast.Make("Unable to dial this number.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }

        }

        [RelayCommand]
        async Task CreateScheduleInvoice(SchedulesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            model.InvoiceOrEstimate = 1; //Invoice
            if (model.InvoiceDTO != null)
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.InvoiceDetailsPage(new CustInvoicesViewModel(model.InvoiceDTO, model.CustomerDTO, ORep, _service), ORep, _service));
            }
            else
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.CreateInvoicePage(new SchInvoicesViewModel(model, ORep, _service), ORep, _service));
            }

            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CreateScheduleEstimate(SchedulesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();

            if (model.EstimateDTO == null)
            {
                model.InvoiceOrEstimate = 0; //Estimate
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.CreateEstimatePage(new SchEstimatesViewModel(model, ORep, _service), ORep, _service));
            }
            else
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.EstimateDetailsPage(new CustEstimatesViewModel(model.EstimateDTO, model.CustomerDTO, ORep, _service), ORep, _service));
            }

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
        async Task RemoveItem(ScheduleItemsServicesModel item)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();
                var json = await ORep.PutStrAsync("api/Schedules/PutMaterial", item, UserToken);//Delete Material

                if (string.IsNullOrEmpty(json))
                {
                    var toast = Toast.Make("Failed to delete the material", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    LstItems.Remove(item);
                    ScheduleDetails.LstScheduleItemsServices.Remove(item);
                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenMaterialDetails(ScheduleItemsServicesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewItemsServicesSchedulePage(model));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SelecteNewItems(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleItemsServicesViewModel(ShowQty);
                popupView.ItemClose += async (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    ScheduleItemsServicesModel ItemsModel = new ScheduleItemsServicesModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ScheduleId = model.Id,
                        ScheduleDateId = model.OneScheduleDate.Id,
                        ItemsServicesId = item.Id,
                        ItemsServicesName = item.Name,
                        ItemServiceDescription = item.Description,
                        TaxId = item.TaxId,
                        Tax = item.Tax,
                        CostRate = item.CostperUnit,
                        Price = item.CostperUnit,
                        Total = item.QTYTime != null && item.Tax != null ? (item.CostperUnit * item.QTYTime) + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime == null && item.Tax != null ? item.CostperUnit + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime != null && item.Tax == null ? item.CostperUnit * item.QTYTime : item.CostperUnit,
                        Notes = item.Notes,
                        Active = item.Active,
                        CreateUser = model.CreateUser,
                        CreateDate = item.CreateDate,
                        Taxable = item.Taxable,
                        Quantity = (item.QTYTime == null || item.QTYTime == 0) ? 1 : item.QTYTime,
                        Unit = item.Unit,
                    };

                    ScheduleItemsServicesModel scheduleItemsServicesModel = new ScheduleItemsServicesModel();

                    if (LstItems.Count > 0)
                    {
                        var itm = LstItems.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            scheduleItemsServicesModel = await InsertOneItemService(ItemsModel);
                            if (scheduleItemsServicesModel != null)
                            {
                                LstItems.Add(scheduleItemsServicesModel);
                            }
                        }
                    }
                    else
                    {
                        scheduleItemsServicesModel = await InsertOneItemService(ItemsModel);
                        if (scheduleItemsServicesModel != null)
                        {
                            LstItems.Add(scheduleItemsServicesModel);
                        }
                    }

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task RemoveMaterialReceipt(ScheduleMaterialReceiptModel item)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();

                var json = await ORep.PutStrAsync("api/Schedules/PutMaterialReceipt", item, UserToken);//Delete Material Receipt

                if (string.IsNullOrEmpty(json))
                {
                    var toast = Toast.Make("Failed Delete this Material Receipt.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    LstMaterialReceipt.Remove(item);
                    ScheduleDetails.LstMaterialReceipt.Remove(item);

                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenMaterialReceiptDetails(ScheduleMaterialReceiptModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.MaterialReceiptPage(model, new ScheduleMaterialReceiptViewModel(model)));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SelecteNewMaterialReceipt(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleMaterialReceiptViewModel();
                popupView.MaterialRcClose += async (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    string UserToken = await _service.UserToken();

                    ScheduleMaterialReceiptModel MaterialReceiptModel = new ScheduleMaterialReceiptModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ScheduleId = model.Id,
                        ScheduleDateId = model.OneScheduleDate.Id,
                        SupplierId = item.SupplierId,
                        SupplierName = item.SupplierName,
                        TechnicianId = int.Parse(Helpers.Settings.UserIdGet),
                        Cost = item.Cost,
                        Notes = item.Notes,
                        ReceiptPhoto = item.ReceiptPhoto,
                        CreateUser = model.CreateUser,
                        CreateDate = DateTime.Now,
                    };

                    var json = await ORep.PostStrAsync("api/Schedules/PostScheduleMaterialReceipt", MaterialReceiptModel, UserToken);

                    if (string.IsNullOrEmpty(json))
                    {
                        var toast = Toast.Make("Failed add this Material Receipt", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else
                    {
                        LstMaterialReceipt.Add(JsonConvert.DeserializeObject<ScheduleMaterialReceiptModel>(json));
                    }

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.MaterialReceiptPage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task RemoveService(ScheduleItemsServicesModel service)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.PutStrAsync("api/Schedules/PutFreeService", service, UserToken);//Delete Service

                if (string.IsNullOrEmpty(json))
                {
                    var toast = Toast.Make("Failed Delete this Service.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    LstFreeServices.Remove(service);
                    ScheduleDetails.LstFreeServices.Remove(service);
                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenServiceDetails(ScheduleItemsServicesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleFreeServicesPage(model));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SelecteNewFreeService(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleFreeServicesViewModel(false);
                popupView.ServiceClose += async (service) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    ScheduleItemsServicesModel ServiceModel = new ScheduleItemsServicesModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ScheduleId = model.Id,
                        ScheduleDateId = model.OneScheduleDate.Id,
                        ItemsServicesId = service.Id,
                        ItemsServicesName = service.Name,
                        ItemServiceDescription = service.Description,
                        TaxId = service.TaxId,
                        Tax = service.Tax,
                        CostRate = service.CostperUnit,
                        Price = service.CostperUnit,
                        Total = service.QTYTime != null && service.Tax != null ? (service.CostperUnit * service.QTYTime) + (service.CostperUnit * service.QTYTime * service.Tax / 100) : service.QTYTime == null && service.Tax != null ? service.CostperUnit + (service.CostperUnit * service.QTYTime * service.Tax / 100) : service.QTYTime != null && service.Tax == null ? service.CostperUnit * service.QTYTime : service.CostperUnit,
                        Notes = service.Notes,
                        Active = service.Active,
                        CreateUser = model.CreateUser,
                        CreateDate = service.CreateDate,
                        Taxable = service.Taxable,
                        Quantity = (service.QTYTime == null || service.QTYTime == 0) ? 1 : service.QTYTime,
                        Unit = service.Unit,
                    };

                    ScheduleItemsServicesModel scheduleItemsServicesModel = new ScheduleItemsServicesModel();

                    if (LstFreeServices.Count > 0)
                    {
                        var itm = LstFreeServices.Where(x => x.ItemsServicesId == service.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            scheduleItemsServicesModel = await InsertOneFreeService(ServiceModel);
                            if (scheduleItemsServicesModel != null)
                            {
                                LstFreeServices.Add(scheduleItemsServicesModel);

                            }
                        }
                    }
                    else
                    {
                        scheduleItemsServicesModel = await InsertOneFreeService(ServiceModel);
                        if (scheduleItemsServicesModel != null)
                        {
                            LstFreeServices.Add(scheduleItemsServicesModel);
                        }
                    }

                    if (LstFreeServices.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.ScheduleFreeServicesPage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);

                IsEnable = true;
            }
        }

        [RelayCommand]
        async Task OpenFullScreenSchImage(string ImageName)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new FullScreenImagePage(ImageName));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
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

                        UserDialogs.Instance.ShowLoading();
                        var Json = await ORep.PutDataAsync("api/Schedules/PutSchedule", ScheduleDetails, UserToken);
                        UserDialogs.Instance.HideHud();

                        if (Json != "Bad Request" && Json != "api not responding")
                        {
                            var toast = Toast.Make("Successfully for Update Schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.ScheduleDateId, ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                    }
                    else
                    {
                        var toast = Toast.Make("Failed for update schedule.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenImages(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                UserDialogs.Instance.ShowLoading();
                //model.GetPictures = true; //In Get Pictures Case Only
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.SchedulePicturesPage(new SchImagesViewModel(model, ORep, _service), ORep, _service));
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }



    }
}


using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using Mopups.Services;
using SkiaSharp;
using System.Collections.ObjectModel;
using static Twilio.Rest.Content.V1.ContentResource;

namespace FixProUs.ViewModels
{
    public partial class SchEstimatesViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        SchedulesModel scheduleDetails;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        EstimateModel oneEstimate;

        [ObservableProperty]
        InvoiceModel oneInvoice;

        [ObservableProperty]
        ObservableCollection<EstimateModel> lstEstimates;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItemsEstimate;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDatesActual;

        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstServices;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstFreeServices;

        [ObservableProperty]
        ObservableCollection<ScheduleMaterialReceiptModel> lstMaterialReceipt;

        [ObservableProperty]
        ObservableCollection<EmployeesCategoryModel> lstEmpCategory;

        [ObservableProperty]
        SchaduleDateModel oneScheduleDate;

        [ObservableProperty]
        EmployeesCategoryModel selectedCateory;

        [ObservableProperty]
        ItemsServicesModel selectedService;

        [ObservableProperty]
        bool isShowScheduleDates;

        [ObservableProperty]
        bool showScheduleName;

        [ObservableProperty]
        string branchName;

        [ObservableProperty]
        decimal? discount;

        [ObservableProperty]
        decimal? subTotalEst;

        [ObservableProperty]
        decimal? netEst;

        [ObservableProperty]
        decimal? paidEst;

        [ObservableProperty]
        decimal? totalDueEst;

        [ObservableProperty]
        bool pending;

        [ObservableProperty]
        bool accept;

        [ObservableProperty]
        bool declind;

        [ObservableProperty]
        bool showDropdownDatesEstimate;

        [ObservableProperty]
        bool showEstimateConvertToInvoice;

        [ObservableProperty]
        string signatureImageByte64Estimate;

        [ObservableProperty]
        string strEstimateDates;

        [ObservableProperty]
        bool amountOrPersent;

        [ObservableProperty]
        int lstHeight;

        [ObservableProperty]
        int schedulePage;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items

        [ObservableProperty]
        bool isReOpen;

        [ObservableProperty]
        string oldResonNotServiced;

        [ObservableProperty]
        string strEmployees;

        [ObservableProperty]
        DateTime invoiceDate;

        [ObservableProperty]
        DateTime scheduleAddDate;

        [ObservableProperty]
        DateTime scheduleDate;

        public int BranchIdVM;
        #endregion

        public SchEstimatesViewModel(SchedulesModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init(model);
        }

        void Init(SchedulesModel model)
        {
            IsReOpen = ((String.IsNullOrEmpty(model.OneScheduleDate.StartTime) == true && model.OneScheduleDate.Status == 0)) ? true : false; //Show ReOpen Button If Don't Start Job after NotServiced only
            OldResonNotServiced = model.OneScheduleDate.Reasonnotserve;
            ShowQty = true; //Invoice Page
            GetPerrmission();

            if (model.LstScheduleItemsServices.Count > 4)
            {
                LstHeight = 1;
            }

            InitData(model);

            IsShowScheduleDates = true; //Show all Schedules Dates
            GetScheduleDates(model.Id, 1); //All Schedule Dates

            ScheduleAddDate = DateTime.Now;
        }

        async void Init()
        {
            CustomerDetails = new CustomersModel();
            ScheduleDetails = new SchedulesModel();
            LstFreeServices = new ObservableCollection<ScheduleItemsServicesModel>();
            LstMaterialReceipt = new ObservableCollection<ScheduleMaterialReceiptModel>();
            LstItemsEstimate = new ObservableCollection<ScheduleItemsServicesModel>();
            LstEmpCategory = new ObservableCollection<EmployeesCategoryModel>();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleEmployeeDTO = new List<ScheduleEmployeesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            OneInvoice = new InvoiceModel();
            OneInvoice.LstInvoiceItemServices = new List<InvoiceItemServicesModel>();
            OneEstimate = new EstimateModel();
            OneEstimate.LstEstimateItemServices = new List<EstimateItemServicesModel>();
            LstServices = new ObservableCollection<ItemsServicesModel>();
            SelectedService = new ItemsServicesModel();
            LstEstimateSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstEstimateDates = new ObservableCollection<SchaduleDateModel>();
            //Schdule Date
            ScheduleDetails.OneScheduleDate = new SchaduleDateModel();
            OneScheduleDate = new SchaduleDateModel();

            BranchIdVM = int.Parse(Helpers.Settings.BranchIdGet);

            StrEstimateDates = "Choose Schedule Dates";

            await GetServices();
            await GetEmpCategories();
        }


        async void InitData(SchedulesModel model)
        {
            Init();

            SchedulePage = 1; //Update Schedule 

            ScheduleDetails = model;
            CustomerDetails = model.CustomerDTO;

            if (ScheduleDetails.CustomerDTO.MemeberType == true)
            {
                if (ScheduleDetails.CustomerDTO.MemberDTO != null)
                {
                    Discount = ScheduleDetails.CustomerDTO.MemberDTO.MemberValue;
                }
            }
            else
            {
                Discount = ScheduleDetails.CustomerDTO.Discount;
            }

            if (Discount == null)
            {
                Discount = 0;
            }


            if (model.LstScheduleItemsServices.Count > 0)
            {
                LstItemsEstimate = new ObservableCollection<ScheduleItemsServicesModel>(model.LstScheduleItemsServices);
                TotalEstimate(model, CustomerDetails);
            }
            else
            {
                SubTotalEst = 0;
                NetEst = 0;
                PaidEst = 0;
                TotalDueEst = 0;
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

            BranchName = Helpers.Settings.BranchNameGet;

            OneScheduleDate = ScheduleDetails.OneScheduleDate;
        }

        //Get Perrmission for User
        public async void GetPerrmission()
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

        async void GetScheduleDates(int ScheduleId, int Type)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<SchaduleDateModel>>(string.Format("api/Schedules/GetScheduleDates?" + "ScheduleId=" + ScheduleId + "&" + "Type=" + Type), UserToken);

                if (json != null)
                {
                    if (json.Count == 1)
                    {
                        LstEstimateSchaduleDatesActual = json;
                        StrEstimateDates = json.FirstOrDefault().Date;
                        LstEstimateSchaduleDates = json;
                        IsShowScheduleDates = false;
                    }
                    else
                    {
                        LstEstimateSchaduleDates = json;
                    }
                }

                UserDialogs.Instance.HideHud();
            }
        }

        public void TotalEstimate(SchedulesModel SchModel, CustomersModel CustModel)
        {
            if (SchModel.Id != 0)
            {
                //decimal? SumCost = SchModel.LstScheduleItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstItemsEstimate.Where(x => x.CostRate > 0).Sum(s => s.CostRate * s.Quantity);

                //decimal? DiscountVal = (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == false) ? (SumCost * SchModel.CustomerDTO.Discount / 100) : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == true) ? (SumCost - SchModel.CustomerDTO.Discount) : 0;
                decimal? DiscountVal = (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemberDTO.MemberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (Discount);

                decimal? TaxValue = SchModel.CustomerDTO.TaxDTO != null ? (SumCost - DiscountVal) * SchModel.CustomerDTO.TaxDTO.Rate / 100 : 0;

                SubTotalEst = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                PaidEst = 0;
                NetEst = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDueEst = Math.Round(((SumCost - DiscountVal) + TaxValue - PaidEst).Value, 2, MidpointRounding.ToEven);
            }

            if (CustModel.Id != 0 && SchModel.Id == 0)
            {
                //decimal? SumCost = CustModel.LstCustItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstItemsEstimate.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);

                decimal? DiscountVal = (CustModel.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO.MemberType == false) ? (SumCost * Discount / 100) : (Discount);

                decimal? TaxValue = CustModel.TaxDTO != null ? (SumCost - DiscountVal) * CustModel.TaxDTO.Rate / 100 : 0;

                SubTotalEst = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                PaidEst = 0;
                NetEst = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDueEst = Math.Round(((SumCost - DiscountVal) + TaxValue - PaidEst).Value, 2, MidpointRounding.ToEven);
            }
        }

        [RelayCommand]
        void EditDiscountForCustomerEstimate(CustomersModel model)
        {
            Discount = model.Discount;

            TotalEstimate(ScheduleDetails, CustomerDetails);
        }

        [RelayCommand]
        async Task OpenEstimateScheduleDates()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new Pages.PopupPages.ScheduleDatesPopup(LstEstimateSchaduleDates);
                popupView.DatesClose += (Dates) =>
                {
                    if (Dates.Count != 0)
                    {
                        LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(Dates);
                        string str = "";
                        LstEstimateDates.Clear();
                        foreach (var Date in Dates)
                        {
                            str += (" , " + Date.Date);
                            LstEstimateDates.Add(new SchaduleDateModel
                            {
                                Id = Date.Id,
                                Date = Date.Date,
                            });
                        }

                        if (!string.IsNullOrEmpty(StrEstimateDates))
                        {
                            StrEstimateDates = string.Empty;
                            StrEstimateDates += str;
                            StrEstimateDates = str.Remove(0, 2);
                        }
                        else
                        {
                            StrEstimateDates = str.Remove(0, 2);
                        }
                    }
                };

                await MopupService.Instance.PushAsync(popupView);
            }

            IsEnable = true;
        }

        [RelayCommand]
        void RemoveEstimateDate(SchaduleDateModel Date)
        {
            LstEstimateDates.Remove(Date);

            foreach (SchaduleDateModel dt in LstEstimateSchaduleDates)
            {
                if (dt.Id == Date.Id)
                {
                    dt.IsChecked = false;
                }
            }

            //Dates Names 
            int index = StrEstimateDates.IndexOf(Date.Date + " , ");
            StrEstimateDates = (index < 0) ? StrEstimateDates : StrEstimateDates.Remove(index, (Date.Date + " , ").Length);

            int index2 = StrEstimateDates.IndexOf(" , " + Date.Date);
            StrEstimateDates = (index2 < 0) ? StrEstimateDates : StrEstimateDates.Remove(index2, (" , " + Date.Date).Length);

            int index3 = StrEstimateDates.IndexOf(Date.Date);
            StrEstimateDates = (index3 < 0) ? StrEstimateDates : StrEstimateDates.Remove(index3, (Date.Date).Length);
        }

        [RelayCommand]
        async Task SelecteNewItemsEstimate(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleItemsServicesViewModel(ShowQty);
                popupView.ItemClose += (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    ScheduleItemsServicesModel ItemsModel = new ScheduleItemsServicesModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ScheduleId = model.Id,
                        ItemsServicesId = item.Id,
                        ItemsServicesName = item.Name,
                        ItemServiceDescription = item.Description,
                        TaxId = item.TaxId,
                        Tax = item.Tax,
                        CostRate = item.CostperUnit,
                        Total = item.QTYTime != null && item.Tax != null ? (item.CostperUnit * item.QTYTime) + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime == null && item.Tax != null ? item.CostperUnit + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime != null && item.Tax == null ? item.CostperUnit * item.QTYTime : item.CostperUnit,
                        Notes = item.Notes,
                        Active = item.Active,
                        CreateUser = model.CreateUser,
                        CreateDate = item.CreateDate,
                        Taxable = item.Taxable,
                        Quantity = (item.QTYTime == null || item.QTYTime == 0) ? 1 : item.QTYTime,
                        Unit = item.Unit,
                    };

                    if (LstItemsEstimate.Count > 0)
                    {
                        var itm = LstItemsEstimate.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            LstItemsEstimate.Add(ItemsModel);
                        }
                    }
                    else
                    {
                        LstItemsEstimate.Add(ItemsModel);
                    }

                    if (LstItemsEstimate.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    TotalEstimate(model, CustomerDetails);

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        [RelayCommand]
        void RemoveItemEstimate(ScheduleItemsServicesModel item)
        {
            IsEnable = false;

            LstItemsEstimate.Remove(item);

            TotalEstimate(ScheduleDetails, CustomerDetails);

            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitSchEstimate(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    if (LstItemsEstimate.Count > 0)
                    {
                        string UserToken = await _service.UserToken();

                        if (LstEstimateSchaduleDatesActual.Count > 0)
                        {
                            if (Pending == true || Accept == true || Declind == true)
                            {

                                OneEstimate.AccountId = model.AccountId;
                                OneEstimate.BrancheId = model.BrancheId;
                                OneEstimate.ScheduleId = model.Id;
                                OneEstimate.EstimateDate = DateTime.Now;
                                OneEstimate.CustomerId = model.CustomerId;
                                OneEstimate.Total = SubTotalEst; //Total before discount and tax
                                OneEstimate.TaxId = model.CustomerDTO.TaxId;
                                OneEstimate.Tax = model.CustomerDTO.TaxDTO?.Rate;
                                OneEstimate.Taxval = null;
                                OneEstimate.SignatureDraw = SignatureImageByte64Estimate;
                                //OneEstimate.Taxval = (model.CustomerDTO != null && model.CustomerDTO.MemeberType == false && model.CustomerDTO.TaxDTO != null) ? (SubTotal - (SubTotal * model.CustomerDTO.MemberDTO.MemberValue / 100) * model.CustomerDTO.TaxDTO.Rate / 100) : (model.CustomerDTO != null && model.CustomerDTO.TaxDTO != null && model.CustomerDTO.MemeberType == true && model.CustomerDTO.TaxDTO != null) ? ((SubTotal - model.CustomerDTO.Discount) * model.CustomerDTO.TaxDTO.Rate / 100) : 0;
                                OneEstimate.MemberId = model.CustomerDTO.MemeberId;
                                OneEstimate.Discount = Discount;
                                //OneEstimate.DiscountAmountOrPercent = model.CustomerDTO.MemberDTO.MemberType == false ? "%" : "$";
                                OneEstimate.DiscountAmountOrPercent = AmountOrPersent == false ? "%" : "$";
                                OneEstimate.Net = NetEst;
                                OneEstimate.Status = Accept == true ? 1 : Declind == true ? 2 : 0; //0 = Pending
                                OneEstimate.SignaturePrintName = null;
                                OneEstimate.Terms = null;
                                OneEstimate.NotesForCustomer = model.CustomerDTO.Notes;
                                OneEstimate.Notes = model.Notes;
                                OneEstimate.Active = model.Active;
                                OneEstimate.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                                OneEstimate.CreateDate = DateTime.Now;

                                foreach (ScheduleItemsServicesModel item in LstItemsEstimate)
                                {
                                    EstimateItemServicesModel ObjItem = new EstimateItemServicesModel
                                    {
                                        Id = item.Id,
                                        AccountId = model.AccountId,
                                        BrancheId = model.BrancheId,
                                        //TaxId = model.CustomerDTO.TaxId,
                                        //Tax = model.CustomerDTO.TaxDTO.Rate,
                                        //Taxable = (model.CustomerDTO.TaxDTO.Rate == null || model.CustomerDTO.TaxDTO.Rate == 0) ? false : true,
                                        Taxable = true,
                                        //Unit = item.Unit,
                                        Price = item.CostRate,
                                        Quantity = item.Quantity,
                                        //Discountable = model.CustomerDTO.MemberDTO.MemberValue != null ? true : false,
                                        Discountable = true,
                                        ItemsServicesId = item.ItemsServicesId,
                                        ItemsServicesName = item.ItemsServicesName,
                                        CreateUser = int.Parse(Helpers.Settings.UserIdGet),
                                        CreateDate = DateTime.Now,
                                        Total = item.Quantity != null && item.Tax != null ? (item.CostRate * item.Quantity) + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity == null && item.Tax != null ? item.CostRate + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity != null && item.Tax == null ? item.CostRate * item.Quantity : item.CostRate,
                                        Active = model.Active,
                                    };
                                    OneEstimate.LstEstimateItemServices.Add(ObjItem);
                                }

                                OneEstimate.LstScdDate = LstEstimateSchaduleDatesActual.ToList();

                                UserDialogs.Instance.ShowLoading();
                                //var json = await Helpers.Utility.PostData("api/Estimates/PostEstimate", JsonConvert.SerializeObject(OneEstimate));
                                var json = await ORep.PostDataAsync("api/Estimates/PostEstimate", OneEstimate, UserToken);
                                UserDialogs.Instance.HideHud();

                                if (json != "Bad Request" && json != "api not responding" && json.Contains("Already Exist For This Schedule Date#") != true)
                                {
                                    var toast = Toast.Make("Successfully Create Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                    await toast.Show();

                                    bool answer = await App.Current!.MainPage!.DisplayAlert("Question?", "Do you want to send an email to the customer?", "Yes", "No");

                                    if (answer)//Send Email
                                    {

                                        UserDialogs.Instance.ShowLoading();
                                        var jsonEmail = await ORep.PostStrAsync("api/Estimates/PostEstimateEmail", OneEstimate, UserToken);
                                        UserDialogs.Instance.HideHud();

                                        if (!string.IsNullOrEmpty(jsonEmail) && jsonEmail.Contains("Send Success") == true)
                                        {
                                            var toast1 = Toast.Make("Successfully Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                            await toast1.Show();
                                        }
                                        else
                                        {
                                            var toast1 = Toast.Make("Failed to send e-mail to the customer", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                            await toast1.Show();
                                        }
                                    }

                                    await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.ScheduleDateId,ORep,_service),ORep,_service));
                                    App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                                }
                                else
                                {
                                    var toast1 = Toast.Make($"Alert : {json}.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                    await toast1.Show();
                                }

                            }
                            else
                            {
                                var toast1 = Toast.Make("Please Choose Status for Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast1.Show();
                            }
                        }
                        else
                        {
                            var toast1 = Toast.Make("No schedule dates chosen for this estimate!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast1.Show();
                        }
                    }
                    else
                    {
                        var toast1 = Toast.Make("No item/service chosen for this estimate!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast1.Show();
                    }
                }

            }

            IsEnable = true;
        }
    }
}

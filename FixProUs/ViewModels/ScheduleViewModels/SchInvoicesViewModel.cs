using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages.SchedulePages;
using Mopups.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;


namespace FixProUs.ViewModels
{
    public partial class SchInvoicesViewModel : BaseViewModel
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
        InvoiceModel oneInvoice;


        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstServices;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstFreeServices;

        [ObservableProperty]
        ObservableCollection<ScheduleMaterialReceiptModel> lstMaterialReceipt;

        [ObservableProperty]
        ObservableCollection<EmployeesCategoryModel> lstEmpCategory;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItems;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItemsInvoice;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDatesActual;

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
        decimal? subTotal;

        [ObservableProperty]
        decimal? net;

        [ObservableProperty]
        decimal? paid;

        [ObservableProperty]
        decimal? totalDue;

        [ObservableProperty]
        bool pending;

        [ObservableProperty]
        bool accept;

        [ObservableProperty]
        bool declind;


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

        [ObservableProperty]
        string strInvoiceDates;

        public int BranchIdVM;
        #endregion

        public SchInvoicesViewModel(SchedulesModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
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
            LstItems = new ObservableCollection<ScheduleItemsServicesModel>();
            LstFreeServices = new ObservableCollection<ScheduleItemsServicesModel>();
            LstMaterialReceipt = new ObservableCollection<ScheduleMaterialReceiptModel>();
            LstItemsInvoice = new ObservableCollection<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleEmployeeDTO = new List<ScheduleEmployeesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            LstEmpCategory = new ObservableCollection<EmployeesCategoryModel>();
            OneInvoice = new InvoiceModel();
            OneInvoice.LstInvoiceItemServices = new List<InvoiceItemServicesModel>();
            LstServices = new ObservableCollection<ItemsServicesModel>();
            SelectedService = new ItemsServicesModel();
            LstInvoiceSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceDates = new ObservableCollection<SchaduleDateModel>();
            //Schdule Date
            ScheduleDetails.OneScheduleDate = new SchaduleDateModel();
            OneScheduleDate = new SchaduleDateModel();

            BranchIdVM = int.Parse(Helpers.Settings.BranchIdGet);

            StrInvoiceDates = "Choose Schedule Dates";

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
                LstItems = new ObservableCollection<ScheduleItemsServicesModel>(model.LstScheduleItemsServices);
                TotalInvoice(model, CustomerDetails);
            }
            else
            {
                SubTotal = 0;
                Net = 0;
                Paid = 0;
                TotalDue = 0;
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
                        LstInvoiceSchaduleDatesActual = json;
                        StrInvoiceDates = json.FirstOrDefault().Date;
                        LstInvoiceSchaduleDates = json;
                        IsShowScheduleDates = false;
                    }
                    else
                    {
                        LstInvoiceSchaduleDates = json;
                    }
                }

                UserDialogs.Instance.HideHud();
            }
        }

        public void TotalInvoice(SchedulesModel SchModel, CustomersModel CustModel)
        {
            if (SchModel.Id != 0)
            {
                //decimal? SumCost = SchModel.LstScheduleItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstItemsInvoice.Where(x => x.CostRate > 0 && (x.Out == false || x.Out == null)).Sum(s => s.CostRate * s.Quantity);

                //decimal? DiscountVal = (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == false) ? (SumCost * SchModel.CustomerDTO.Discount / 100) : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == true) ? (SumCost - SchModel.CustomerDTO.Discount) : 0;
                decimal? DiscountVal = (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (SchModel.CustomerDTO != null && SchModel.CustomerDTO.MemberDTO.MemberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (Discount);

                decimal? TaxValue = SchModel.CustomerDTO.TaxDTO != null ? (SumCost - DiscountVal) * SchModel.CustomerDTO.TaxDTO.Rate / 100 : 0;

                SubTotal = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                Paid = 0;
                Net = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDue = Math.Round(((SumCost - DiscountVal) + TaxValue - Paid).Value, 2, MidpointRounding.ToEven);

                LstItemsInvoice = LstItems;
            }

            if (CustModel.Id != 0 && SchModel.Id == 0)
            {
                //decimal? SumCost = CustModel.LstCustItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstItemsInvoice.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);

                decimal? DiscountVal = (CustModel.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO.MemberType == false) ? (SumCost * Discount / 100) : (Discount);

                decimal? TaxValue = CustModel.TaxDTO != null ? (SumCost - DiscountVal) * CustModel.TaxDTO.Rate / 100 : 0;

                SubTotal = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                Paid = 0;
                Net = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDue = Math.Round(((SumCost - DiscountVal) + TaxValue - Paid).Value, 2, MidpointRounding.ToEven);
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
                    await App.Current!.MainPage!.DisplayAlert("Alert", json, "Ok");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        [RelayCommand]
        void EditDiscountForCustomer(CustomersModel model)
        {
            Discount = model.Discount;
            TotalInvoice(ScheduleDetails, CustomerDetails);
        }

        [RelayCommand]
        async Task OpenInvoiceScheduleDates()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new Pages.PopupPages.ScheduleDatesPopup(LstInvoiceSchaduleDates);
                popupView.DatesClose += (Dates) =>
                {
                    if (Dates.Count != 0)
                    {
                        LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(Dates);

                        string str = "";
                        LstInvoiceDates.Clear();
                        foreach (var Date in Dates)
                        {
                            str += (" , " + Date.Date);
                            LstInvoiceDates.Add(new SchaduleDateModel
                            {
                                Id = Date.Id,
                                Date = Date.Date,
                            });
                        }

                        if (!string.IsNullOrEmpty(StrInvoiceDates))
                        {
                            StrInvoiceDates = string.Empty;
                            StrInvoiceDates += str;
                            StrInvoiceDates = str.Remove(0, 2);
                        }
                        else
                        {
                            StrInvoiceDates = str.Remove(0, 2);
                        }
                    }
                };

                await MopupService.Instance.PushAsync(popupView);
            }

            IsEnable = true;
        }

        [RelayCommand]
        void RemoveInvoiceDate(SchaduleDateModel Date)
        {
            LstInvoiceDates.Remove(Date);

            foreach (SchaduleDateModel dt in LstInvoiceSchaduleDates)
            {
                if (dt.Id == Date.Id)
                {
                    dt.IsChecked = false;
                }
            }

            //Dates Names 
            int index = StrInvoiceDates.IndexOf(Date.Date + " , ");
            StrInvoiceDates = (index < 0) ? StrInvoiceDates : StrInvoiceDates.Remove(index, (Date.Date + " , ").Length);

            int index2 = StrInvoiceDates.IndexOf(" , " + Date.Date);
            StrInvoiceDates = (index2 < 0) ? StrInvoiceDates : StrInvoiceDates.Remove(index2, (" , " + Date.Date).Length);

            int index3 = StrInvoiceDates.IndexOf(Date.Date);
            StrInvoiceDates = (index3 < 0) ? StrInvoiceDates : StrInvoiceDates.Remove(index3, (Date.Date).Length);
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

                    if (ShowQty == false)// add material
                    {
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
                    }
                    else // add invoice item or Estimate item
                    {
                        var itm2 = LstItemsInvoice.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm2 == null)
                        {
                            LstItemsInvoice.Add(ItemsModel);
                        }
                    }

                    if (LstItemsInvoice.Count > 4)
                    {
                        LstHeight = 1;
                    }


                    if (LstItems.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    TotalInvoice(model, CustomerDetails);


                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task RemoveItem(ScheduleItemsServicesModel item)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                if (ShowQty == false) // Remove material
                {
                    string UserToken = await _service.UserToken();
                    var json = await ORep.PutStrAsync("api/Schedules/PutMaterial", item, UserToken);//Delete Material

                    if (string.IsNullOrEmpty(json))
                    {
                        var toast = Toast.Make("Failed to delete the material.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else
                    {
                        LstItems.Remove(item);
                        ScheduleDetails.LstScheduleItemsServices.Remove(item);
                    }
                }
                else
                {
                    if (LstItemsInvoice.Count > 0) //Remove invoice item
                    {
                        LstItemsInvoice.Remove(item);
                        //ScheduleDetails.LstScheduleItemsServices.Remove(item);

                        TotalInvoice(ScheduleDetails, CustomerDetails);
                    }
                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitSchInvoice(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    string UserToken = await _service.UserToken();

                    if (LstInvoiceSchaduleDatesActual.Count > 0)
                    {
                        var CheckItemoutFalse = LstItemsInvoice.Where(m => m.Out == false).FirstOrDefault();
                        if (CheckItemoutFalse != null)
                        {
                            OneInvoice.AccountId = model.AccountId;
                            OneInvoice.BrancheId = model.BrancheId;
                            OneInvoice.ContractId = model.ContractId;
                            OneInvoice.ScheduleId = model.Id;
                            OneInvoice.InvoiceDate = DateTime.Now;
                            OneInvoice.CustomerId = model.CustomerId;
                            OneInvoice.Total = SubTotal;
                            OneInvoice.TaxId = model.CustomerDTO.TaxId;
                            OneInvoice.Tax = model.CustomerDTO.TaxDTO?.Rate;
                            //OneInvoice.Taxval = (SubTotal - (SubTotal * model.CustomerDTO.MemberDTO.MemberValue / 100)) * model.CustomerDTO.TaxDTO.Rate / 100;
                            //OneInvoice.Taxval = (model.CustomerDTO != null && model.CustomerDTO.MemeberType == false && model.CustomerDTO.TaxDTO != null) ? (SubTotal - (SubTotal * model.CustomerDTO.MemberDTO.MemberValue / 100) * model.CustomerDTO.TaxDTO.Rate / 100) : (model.CustomerDTO != null && model.CustomerDTO.TaxDTO != null && model.CustomerDTO.MemeberType == true && model.CustomerDTO.TaxDTO != null) ? ((SubTotal - model.CustomerDTO.Discount) * model.CustomerDTO.TaxDTO.Rate / 100) : 0;
                            OneInvoice.Taxval = null;
                            OneInvoice.MemberId = model.CustomerDTO.MemeberId;
                            OneInvoice.Discount = Discount;
                            //OneInvoice.DiscountAmountOrPercent = model.CustomerDTO.MemberDTO.MemberType == false ? "%" : "$";
                            OneInvoice.DiscountAmountOrPercent = AmountOrPersent == false ? "%" : "$";
                            OneInvoice.Paid = 0;
                            OneInvoice.Net = Net;
                            OneInvoice.Status = 0; //Draft status if(1=partail & 2=paid)
                            OneInvoice.Type = 2; //Installment Payment type
                            OneInvoice.SignaturePrintName = null;
                            OneInvoice.SignatureDraw = null;
                            OneInvoice.Terms = null;
                            OneInvoice.NotesForCustomer = model.CustomerDTO.Notes;
                            OneInvoice.Notes = model.Notes;
                            OneInvoice.Active = model.Active;
                            OneInvoice.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                            OneInvoice.CreateDate = DateTime.Now;

                            foreach (ScheduleItemsServicesModel item in LstItemsInvoice)
                            {
                                InvoiceItemServicesModel ObjItem = new InvoiceItemServicesModel
                                {
                                    Id = item.Id,
                                    AccountId = model.AccountId,
                                    BrancheId = model.BrancheId,
                                    ItemServiceDescription = item.ItemServiceDescription,
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
                                    SkipOfTotal = item.Out,
                                    Total = item.Quantity != null && item.Tax != null ? (item.CostRate * item.Quantity) + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity == null && item.Tax != null ? item.CostRate + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity != null && item.Tax == null ? item.CostRate * item.Quantity : item.CostRate,
                                    Active = model.Active,
                                };
                                OneInvoice.LstInvoiceItemServices.Add(ObjItem);
                            }

                            OneInvoice.LstScdDate = LstInvoiceSchaduleDatesActual.ToList();

                            UserDialogs.Instance.ShowLoading();
                            var json = await ORep.PostDataAsync("api/Invoices/PostInvoice", OneInvoice, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (json != "Bad Request" && json != "api not responding" && json.Contains("Not_Enough") != true && json.Contains("This Invoice Already Exist") != true)
                            {
                                var toast = Toast.Make("Successfully Create Invoice for this Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();

                                bool answer = await App.Current!.MainPage!.DisplayAlert("Question?", "Do you want to send an email to the customer?", "Yes", "No");

                                if (answer)//Send Email
                                {
                                    UserDialogs.Instance.ShowLoading();
                                    var jsonEmail = await ORep.PostStrAsync("api/Invoices/PostInvoiceEmail", OneInvoice, UserToken);
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

                                if (OneInvoice.Net > 0)
                                {
                                    OneInvoice.Id = int.Parse(json.Replace("\"", "").Trim());
                                    await MopupService.Instance.PushAsync(new Pages.PopupPages.PaymentMethodsPopup(new CustInvoicesViewModel(OneInvoice, CustomerDetails, ORep, _service), ORep, _service));
                                }
                                else
                                {
                                    await App.Current!.MainPage!.Navigation.PushAsync(new ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.ScheduleDateId,ORep,_service),ORep,_service));
                                    App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                                }


                            }
                            else
                            {
                                var toast1 = Toast.Make($"Alert : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast1.Show();
                            }
                        }
                        else
                        {
                            var toast1 = Toast.Make("Please don’t check all the items/services out for this invoice", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast1.Show();
                        }
                    }
                    else
                    {
                        var toast1 = Toast.Make("No schedule dates chosen for this invoice!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast1.Show();
                    }
                }
            }

            IsEnable = true;
        }


    }
}

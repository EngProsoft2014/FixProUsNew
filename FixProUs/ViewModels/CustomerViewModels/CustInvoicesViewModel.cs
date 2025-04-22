
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using Microsoft.Maui.Controls;
using Mopups.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using static Twilio.Rest.Content.V1.ContentResource;


namespace FixProUs.ViewModels
{
    public partial class CustInvoicesViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItems;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstItemsInvoiceSch;

        [ObservableProperty]
        ObservableCollection<InvoiceModel> lstInvoices;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceDates;

        [ObservableProperty]
        ObservableCollection<InvoiceItemServicesModel> lstItemsInvoice;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstInvoiceSchaduleDatesActual;

        [ObservableProperty]
        InvoiceModel oneInvoice;

        [ObservableProperty]
        bool isShowScheduleDates;

        [ObservableProperty]
        bool showScheduleName;

        [ObservableProperty]
        int lstHeight;

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
        bool amountOrPersent;

        [ObservableProperty]
        bool showDropdownDatesInvoice;

        [ObservableProperty]
        string strInvoiceDates;

        [ObservableProperty]
        bool withContract;

        [ObservableProperty]
        DateTime invoiceDate;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items
        #endregion

        #region Cons
        public CustInvoicesViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init(model);
            MessagingCenter.Subscribe<CustInvoicesViewModel, bool>(this, "SavedInvoice", (sender, message) =>
            {
                if (true)
                {
                    Init(model);
                }
            });
        }

        //Create New Invoice
        public CustInvoicesViewModel(CustomersModel model, int WayAfterChooseCust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            CustomerDetails = new CustomersModel();
            LstInvoices = new ObservableCollection<InvoiceModel>();
            CustomerDetails.LstInvoices = new List<InvoiceModel>();
            Init(model, WayAfterChooseCust);
        }

        public CustInvoicesViewModel(InvoiceModel model, CustomersModel Cust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            InitDetails(model, Cust);
        }

        public CustInvoicesViewModel(int InvoiceId, CustomersModel Cust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            InitDetailsFromEstimate(InvoiceId, Cust);
        }
        #endregion

        #region Inits
        void Init(CustomersModel model)
        {
            CustomerDetails = model;
            Task.WhenAll(GetPerrmission(), GetInvoicesForCustomer(model.Id));
        }

        async void Init(CustomersModel model, int WayAfterChooseCust)
        {
            CustomerDetails = new CustomersModel();
            OneInvoice = new InvoiceModel();
            LstItems = new ObservableCollection<ScheduleItemsServicesModel>();
            LstItemsInvoiceSch = new ObservableCollection<ScheduleItemsServicesModel>();

            CustomerDetails = model;
            ShowQty = true; //New Invoice
            await GetPerrmission();
            AmountOrPersent = CustomerDetails.MemeberType == false ? false : CustomerDetails.MemberDTO == null ? false : CustomerDetails.MemberDTO.MemberType == true ? true : false;

            BranchName = Helpers.Settings.BranchNameGet;

            InvoiceDate = DateTime.Now;

            if (CustomerDetails.MemeberType == true)
            {
                if (CustomerDetails.MemberDTO != null)
                {
                    Discount = CustomerDetails.MemberDTO.MemberValue;
                }
            }
            else
            {
                Discount = CustomerDetails.Discount;
            }

            if (Discount == null)
            {
                Discount = 0;
            }
        }

        async void InitDetails(InvoiceModel model, CustomersModel Cust)
        {
            await GetPerrmission();

            IsShowScheduleDates = true; //Show schedule Dates

            if (model.ScheduleId != null)
                ShowScheduleName = true;

            LstItemsInvoice = new ObservableCollection<InvoiceItemServicesModel>();
            LstInvoiceSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceDates = new ObservableCollection<SchaduleDateModel>();
            OneInvoice = new InvoiceModel();

            if (model.LstScdDate != null)
            {
                LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(model.LstScdDate);
            }

            await GetOneInvoiceDetails(model.Id);

            CustomerDetails = Cust;
            BranchName = Settings.BranchNameGet;
        }

        async void InitDetailsFromEstimate(int InvoiceId, CustomersModel Cust)
        {
            LstItemsInvoice = new ObservableCollection<InvoiceItemServicesModel>();
            LstInvoiceSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstInvoiceDates = new ObservableCollection<SchaduleDateModel>();
            OneInvoice = new InvoiceModel();

            await GetPerrmission();

            await GetOneInvoiceDetails(InvoiceId);

            IsShowScheduleDates = true; //Show schedule Dates

            if (OneInvoice?.ScheduleId != null)
                ShowScheduleName = true;

            if (OneInvoice?.LstScdDate != null)
            {
                LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(OneInvoice.LstScdDate);
            }

            CustomerDetails = Cust;
            BranchName = Settings.BranchNameGet;
        }
        #endregion

        #region Methods
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

        //Get Customer Invoices
        public async Task GetInvoicesForCustomer(int? CustomerId)
        {
            UserDialogs.Instance.ShowLoading();
            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<CustomersModel>(string.Format("api/Customers/GetInvoicesOfCustomer?" + "CustomerId=" + CustomerId), UserToken);
                if (Customer != null)
                {
                    LstInvoices = new ObservableCollection<InvoiceModel>(Customer.LstInvoices);
                }
            }
            UserDialogs.Instance.HideHud();
        }

        async Task GetScheduleDates(int ScheduleId, int Type)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<SchaduleDateModel>>(string.Format("api/Schedules/GetScheduleDates?" + "ScheduleId=" + ScheduleId + "&" + "Type=" + Type), UserToken);

                if (json != null)
                {
                    LstInvoiceSchaduleDates = json;
                }

                UserDialogs.Instance.HideHud();
            }

        }

        //Get One Invoice Details
        async Task GetOneInvoiceDetails(int? InvoiceId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<ObjectOfCustomerModel>(string.Format("api/Customers/GetObjectOfCustomer?" + "InvoiceId=" + InvoiceId + "&" + "EstimateId=" + null), UserToken);

                if (Customer != null)
                {
                    if (Customer.ObjInvoice != null)
                    {
                        OneInvoice = Customer.ObjInvoice;

                        AmountOrPersent = OneInvoice.DiscountAmountOrPercent == "%" ? false : true;
                        Discount = OneInvoice.Discount;

                        if (OneInvoice.ContractId != null)
                        {
                            WithContract = true;
                        }

                        if (Customer.ObjInvoice.ScheduleId != null && Customer.ObjInvoice.ScheduleId != 0)
                        {
                            ShowDropdownDatesInvoice = true;

                            await GetScheduleDates(Customer.ObjInvoice.ScheduleId.Value, 1); // All Schedule Dates

                            LstInvoiceSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(Customer.ObjInvoice.LstScdDate);

                            if (LstInvoiceSchaduleDatesActual.Count == 1)
                            {
                                IsShowScheduleDates = false; //Don't Show schedule Dates
                            }

                            string str = "";
                            LstInvoiceDates = new ObservableCollection<SchaduleDateModel>();
                            foreach (var Date in Customer.ObjInvoice.LstScdDate)
                            {
                                str += (" , " + Date.Date);
                                LstInvoiceDates.Add(new SchaduleDateModel
                                {
                                    Id = Date.Id,
                                    Date = Date.Date,
                                });
                            }

                            if (!string.IsNullOrEmpty(str))
                            {
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
                        }

                        if (OneInvoice.LstInvoiceItemServices != null)
                        {
                            if (OneInvoice.LstInvoiceItemServices.Count > 4)
                            {
                                LstHeight = 1;
                            }

                            TotalInvoice(OneInvoice);

                            LstItemsInvoice = new ObservableCollection<InvoiceItemServicesModel>(OneInvoice.LstInvoiceItemServices);
                        }
                        else
                        {
                            SubTotal = 0;
                            Net = 0;
                            Paid = 0;
                            TotalDue = 0;
                        }
                    }
                }
            }

            UserDialogs.Instance.HideHud();
        }

        public void TotalInvoice(InvoiceModel model)
        {
            decimal? SumCost = model.LstInvoiceItemServices.Where(x => x.Price > 0 && (x.SkipOfTotal == false || x.SkipOfTotal == null)).Sum(s => s.Price * s.Quantity);
            decimal? DiscountVal = 0;
            if (model.DiscountAmountOrPercent == "%")
            {
                DiscountVal = SumCost * Discount / 100;
            }
            else
            {
                DiscountVal = Discount;
            }
            SubTotal = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
            Paid = model.Status == 0 ? 0 : model.Paid;
            if (model.Taxval != null)
            {
                Net = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + model?.Taxval).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + model?.Taxval).Value, 2, MidpointRounding.ToEven);
                TotalDue = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + model.Taxval - Paid).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + model.Taxval - Paid).Value, 2, MidpointRounding.ToEven);
            }
            else
            {
                decimal? TaxValue = 0;
                if (model.Tax != null)
                {
                    TaxValue = model.DiscountAmountOrPercent == "%" ? ((SumCost - DiscountVal) * model.Tax / 100) : ((SumCost - DiscountVal) * model.Tax / 100);
                }
                Net = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDue = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + TaxValue - Paid).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + TaxValue - Paid).Value, 2, MidpointRounding.ToEven);
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

        public void NewInvoTotalInvoice(CustomersModel CustModel)
        {
            if (CustModel.Id != 0)
            {
                //decimal? SumCost = CustModel.LstCustItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstItemsInvoiceSch.Where(x => x.CostRate > 0 && (x.Out == false || x.Out == null)).Sum(s => s.CostRate * s.Quantity);

                decimal? DiscountVal = (CustModel.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO.MemberType == false) ? (SumCost * Discount / 100) : (Discount);

                decimal? TaxValue = CustModel.TaxDTO != null ? (SumCost - DiscountVal) * CustModel.TaxDTO.Rate / 100 : 0;

                SubTotal = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                Paid = 0;
                Net = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDue = Math.Round(((SumCost - DiscountVal) + TaxValue - Paid).Value, 2, MidpointRounding.ToEven);
            }
        }
        #endregion

        [RelayCommand]
        async Task SelecteInvoiceDetails(InvoiceModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.InvoiceDetailsPage(new CustInvoicesViewModel(model, CustomerDetails, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        //CreateInvoice
        [RelayCommand]
        async Task SelecteNewSchItems(CustomersModel model)
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
                        //ScheduleId = 0,
                        //ScheduleDateId = 0,
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
                        CreateUser = int.Parse(Settings.UserIdGet),
                        CreateDate = item.CreateDate,
                        Taxable = item.Taxable,
                        Quantity = (item.QTYTime == null || item.QTYTime == 0) ? 1 : item.QTYTime,
                        Unit = item.Unit,
                    };

                    if(LstItemsInvoiceSch.Count > 0 )
                    {
                        var itm2 = LstItemsInvoiceSch.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm2 == null)
                        {
                            LstItemsInvoiceSch.Add(ItemsModel);
                        }
                    }
                    else
                    {
                        LstItemsInvoiceSch.Add(ItemsModel);
                    }

                    if (LstItemsInvoiceSch.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    NewInvoTotalInvoice(CustomerDetails);

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        //Details Invoice
        [RelayCommand]
        async Task SelecteNewItems(InvoiceModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleItemsServicesViewModel(true);
                popupView.ItemClose += async (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    InvoiceItemServicesModel InvoiceModel = new InvoiceItemServicesModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ItemsServicesId = item.Id,
                        InvoiceId = model.Id,
                        ItemsServicesName = item.Name,
                        ItemServiceDescription = item.Description,
                        TaxId = item.TaxId,
                        Tax = item.Tax,
                        Price = item.CostperUnit,
                        Total = item.QTYTime != null && item.Tax != null ? (item.CostperUnit * item.QTYTime) + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime == null && item.Tax != null ? item.CostperUnit + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime != null && item.Tax == null ? item.CostperUnit * item.QTYTime : item.CostperUnit,
                        Active = item.Active,
                        CreateUser = model.CreateUser,
                        CreateDate = model.CreateDate,
                        Taxable = item.Taxable,
                        Quantity = (item.QTYTime == null || item.QTYTime == 0) ? 1 : item.QTYTime,
                        Unit = item.Unit,
                        SkipOfTotal = false,
                    };

                    if (LstItemsInvoice.Count > 0)
                    {
                        var itm = LstItemsInvoice.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            LstItemsInvoice.Add(InvoiceModel);
                            OneInvoice.LstInvoiceItemServices.Add(InvoiceModel);
                        }
                    }
                    else
                    {
                        LstItemsInvoice.Add(InvoiceModel);
                        OneInvoice.LstInvoiceItemServices.Add(InvoiceModel);
                    }

                    if (LstItemsInvoice.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    TotalInvoice(model);

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        //CreateInvoice
        [RelayCommand]
        async Task RemoveSchItem(ScheduleItemsServicesModel item)
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
                    }
                }
                else
                {
                    if (LstItemsInvoice.Count > 0) //Remove invoice item
                    {
                        LstItemsInvoiceSch.Remove(item);

                        NewInvoTotalInvoice(CustomerDetails);
                    }

                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        //Details Invoice
        [RelayCommand]
        void RemoveItem(InvoiceItemServicesModel item)
        {
            IsEnable = false;

            LstItemsInvoice.Remove(item);
            OneInvoice.LstInvoiceItemServices.Remove(item);
            TotalInvoice(OneInvoice);

            IsEnable = true;
        }

        //CreateInvoice
        [RelayCommand]
        void CreateInvoiceEditDiscountForCustomer(CustomersModel model)
        {
            Discount = model.Discount;
            NewInvoTotalInvoice(CustomerDetails);
        }

        //Details Invoice
        [RelayCommand]
        void EditDiscountForCustomer(CustomersModel model)
        {
            Discount = CustomerDetails.Discount = model.Discount;

            TotalInvoice(OneInvoice);
        }

        [RelayCommand]
        async Task SubmitInvoice(InvoiceModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                OneInvoice = model;

                if (OneInvoice != null)
                {
                    if (OneInvoice.LstInvoiceItemServices.Count > 0)
                    {
                        string UserToken = await _service.UserToken();

                        var CheckItemoutFalse = OneInvoice.LstInvoiceItemServices.Where(m => m.SkipOfTotal == false).FirstOrDefault();
                        OneInvoice.LstScdDate = LstInvoiceSchaduleDatesActual.ToList();

                        if (CheckItemoutFalse != null)
                        {
                            OneInvoice.Total = SubTotal;
                            OneInvoice.Net = Net;
                            OneInvoice.Discount = Discount;

                            UserDialogs.Instance.ShowLoading();
                            var json = await ORep.PutDataAsync("api/Invoices/PutInvoice", OneInvoice, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (json != "Bad Request" && json != "api not responding" && json.Contains("Not_Enough") != true && json.Contains("This Invoice Already Exist") != true && json.Contains("Already Exist For This Schedule Date#") != true)
                            {
                                var toast = Toast.Make("Successfully Save Invoice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();

                                MessagingCenter.Send(this, "SavedInvoice", true);

                                await MopupService.Instance.PushAsync(new Pages.PopupPages.PaymentMethodsPopup(new CustInvoicesViewModel(OneInvoice, CustomerDetails, ORep, _service), ORep, _service));
                            }
                            else
                            {
                                var toast = Toast.Make($"Failed save invoice : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();
                            }
                        }
                        else
                        {
                            var toast = Toast.Make("Please Don't Check all Item-Service Out for this Invoice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                    else
                    {
                        var toast = Toast.Make("Please Choose Item-Service for this Invoice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectCustToCreateInvoicePage()
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.WayAfterChooseCust = 2; //Create Invoice 
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CreateInvoiceWithoutSchedulePage(new CustInvoicesViewModel(CustomerDetails, Controls.StaticMembers.WayAfterChooseCust, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenInvoiceScheduleDates()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                foreach (SchaduleDateModel dt in LstInvoiceSchaduleDates)
                {
                    foreach (SchaduleDateModel dt2 in LstInvoiceSchaduleDatesActual)
                    {
                        if (dt.Id == dt2.Id)
                        {
                            dt.IsChecked = true;
                        }
                    }
                }

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
        async Task DeleteInvoice(int InvoiceId)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();

                var json = await ORep.DeleteStrItemAsync(string.Format("api/Invoices/DeleteInvoice/{0}", InvoiceId), UserToken);

                if (json != null && json != "api not responding" && json.Contains("Is Not Deleted") != true)
                {
                    var toast = Toast.Make("Successfully Save Inovice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();

                    MessagingCenter.Send(this, "SavedInvoice", true);
                    await App.Current!.MainPage!.Navigation.PopAsync();
                }
                else
                {
                    var toast = Toast.Make($"Failed Save Inovice : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;

        }

        [RelayCommand]
        void RemoveInvoiceDate(SchaduleDateModel Date)
        {
            LstInvoiceDates.Remove(Date);
            SchaduleDateModel DataOfScddt = LstInvoiceSchaduleDatesActual.Where(x => x.Id == Date.Id).FirstOrDefault();
            LstInvoiceSchaduleDatesActual.Remove(DataOfScddt);

            foreach (SchaduleDateModel dt in LstInvoiceSchaduleDates)
            {
                foreach (SchaduleDateModel dt2 in LstInvoiceSchaduleDatesActual)
                {
                    if (dt.Id == dt2.Id)
                    {
                        dt.IsChecked = true;
                    }
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
        async Task SelectSendEmailInvoice(InvoiceModel model)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                UserDialogs.Instance.ShowLoading();
                var json = await ORep.PostStrAsync("api/Invoices/PostInvoiceEmail", model, UserToken);
                UserDialogs.Instance.HideHud();

                if (!string.IsNullOrEmpty(json) && json.Contains("Send Success") == true)
                {
                    var toast = Toast.Make("Successfully Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    var toast = Toast.Make($"Failed Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task CreditPayment(InvoiceModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.PayCashOrCredit = 2;
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CashOrCreditPaymentPage(new PaymentsViewModel(model, CustomerDetails, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task CashPayment(InvoiceModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.PayCashOrCredit = 1;
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CashOrCreditPaymentPage(new PaymentsViewModel(model, CustomerDetails, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitCustInvoice(CustomersModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                if (model != null)
                {
                    if (LstItemsInvoiceSch.Count > 0)
                    {
                        var CheckItemoutFalse = LstItemsInvoiceSch.Where(m => m.Out == false).FirstOrDefault();
                        if (CheckItemoutFalse != null)
                        {
                            OneInvoice.AccountId = model.AccountId;
                            OneInvoice.BrancheId = model.BrancheId;
                            //OneInvoice.ContractId = model.ContractId;
                            //OneInvoice.ScheduleDateId = model.OneScheduleDate.Id;
                            //OneInvoice.ScheduleId = model.Id;
                            OneInvoice.InvoiceDate = DateTime.Now;
                            OneInvoice.CustomerId = model.Id;
                            OneInvoice.Total = SubTotal;
                            OneInvoice.TaxId = model.TaxId;
                            OneInvoice.Tax = model.TaxDTO?.Rate;
                            //OneInvoice.Taxval = (SubTotal - (SubTotal * model.MemberDTO.MemberValue / 100)) * model.TaxDTO.Rate / 100;
                            //OneInvoice.Taxval = (model.MemeberType == false && model.TaxDTO != null) ? (SubTotal - (SubTotal * model.MemberDTO.MemberValue / 100) * model.TaxDTO.Rate / 100) : (model.TaxDTO != null && model.MemeberType == true && model.TaxDTO != null) ? ((SubTotal - model.Discount) * model.TaxDTO.Rate / 100) : 0;
                            OneInvoice.Taxval = null;
                            OneInvoice.MemberId = model.MemeberId;
                            OneInvoice.Discount = Discount;
                            OneInvoice.DiscountAmountOrPercent = AmountOrPersent == false ? "%" : "$";
                            OneInvoice.Paid = 0;
                            OneInvoice.Net = Net;
                            OneInvoice.Status = 0; //Draft status if(1=partail & 2=paid)
                            OneInvoice.Type = 2; //Installment Payment type
                            OneInvoice.SignaturePrintName = null;
                            OneInvoice.SignatureDraw = null;
                            OneInvoice.Terms = null;
                            OneInvoice.NotesForCustomer = model.Notes;
                            //OneInvoice.Notes = model.Notes;
                            OneInvoice.Active = model.Active;
                            OneInvoice.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                            OneInvoice.CreateDate = DateTime.Now;

                            foreach (ScheduleItemsServicesModel item in LstItemsInvoiceSch)
                            {
                                InvoiceItemServicesModel ObjItem = new InvoiceItemServicesModel
                                {
                                    Id = item.Id,
                                    AccountId = model.AccountId,
                                    BrancheId = model.BrancheId,
                                    ItemServiceDescription = item.ItemServiceDescription,
                                    //TaxId = model.TaxId,
                                    //Tax = model.TaxDTO.Rate,
                                    //Taxable = (model.TaxDTO.Rate == null || model.TaxDTO.Rate == 0) ? false : true,
                                    Taxable = true,
                                    //Unit = item.Unit,
                                    Price = item.CostRate,
                                    Quantity = item.Quantity,
                                    //Discountable = model.MemberDTO.MemberValue != null ? true : false,
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

                            UserDialogs.Instance.ShowLoading();
                            var json = await ORep.PostDataAsync("api/Invoices/PostInvoice", OneInvoice, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (json != "Bad Request" && json != "api not responding" && json.Contains("Not_Enough") != true && json.Contains("This Invoice Already Exist") != true)
                            {
                                var toast = Toast.Make("Successfully Create Invoice for This Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
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

                                MessagingCenter.Send(this, "SavedInvoice", true);

                                if (OneInvoice.Net > 0)
                                {
                                    OneInvoice.Id = int.Parse(json.Replace("\"", "").Trim());
                                    await MopupService.Instance.PushAsync(new Pages.PopupPages.PaymentMethodsPopup(new CustInvoicesViewModel(OneInvoice, CustomerDetails, ORep, _service), ORep, _service));
                                }
                                else
                                {
                                    await App.Current!.MainPage!.Navigation.PopAsync();
                                }

                            }
                            else
                            {
                                var toast = Toast.Make($"Alert : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();
                            }
                        }
                        else
                        {
                            var toast = Toast.Make("Please don’t check all the items/services out for this invoice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }

                    }
                    else
                    {
                        var toast = Toast.Make("No item/service chosen for this invoice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }

            }

            IsEnable = true;
        }
    }
}
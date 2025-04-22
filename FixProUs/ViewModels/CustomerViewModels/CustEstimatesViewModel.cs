using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using Mopups.Services;
using System.Collections.ObjectModel;
using static Twilio.Rest.Content.V1.ContentResource;


namespace FixProUs.ViewModels
{
    public partial class CustEstimatesViewModel : BaseViewModel
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
        EstimateModel oneEstimate;

        [ObservableProperty]
        InvoiceModel oneInvoice;

        [ObservableProperty]
        ObservableCollection<EstimateModel> lstEstimates;

        [ObservableProperty]
        ObservableCollection<EstimateItemServicesModel> lstItemsEstimate;

        [ObservableProperty]
        ObservableCollection<ScheduleItemsServicesModel> lstSchItemsEstimate;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDates;

        [ObservableProperty]
        ObservableCollection<SchaduleDateModel> lstEstimateSchaduleDatesActual;

        [ObservableProperty]
        bool isShowScheduleDates;

        [ObservableProperty]
        bool showScheduleName;

        [ObservableProperty]
        string branchName;

        [ObservableProperty]
        decimal? discount;

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
        decimal? subTotalEst;

        [ObservableProperty]
        decimal? netEst;

        [ObservableProperty]
        decimal? paidEst;

        [ObservableProperty]
        decimal? totalDueEst;

        [ObservableProperty]
        DateTime invoiceDate;

        [ObservableProperty]
        bool showQty; //Don't Show Qty in Schedule items but Show Qty in Estimate items and Invoice items
        #endregion

        #region Cons
        public CustEstimatesViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init(model);
            MessagingCenter.Subscribe<CustEstimatesViewModel, bool>(this, "SavedEstimate", (sender, message) =>
            {
                if (true)
                {
                    Init(model);
                }
            });
        }

        //Create New Estimate
        public CustEstimatesViewModel(CustomersModel model, int WayAfterChooseCust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            CustomerDetails = new CustomersModel();
            OneEstimate = new EstimateModel();
            LstSchItemsEstimate = new ObservableCollection<ScheduleItemsServicesModel>();
            Init(model, WayAfterChooseCust);
        }

        //Estimate Details
        public CustEstimatesViewModel(EstimateModel model, CustomersModel Cust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            InitDetails(model, Cust);
        }
        #endregion

        #region Inits
        async void Init(CustomersModel model, int WayAfterChooseCust)
        {
            CustomerDetails = model;
            ShowQty = true; //New Estimate
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

        async void Init(CustomersModel model)
        {
            CustomerDetails = model;
            LstEstimates = new ObservableCollection<EstimateModel>();
            CustomerDetails.LstEstimates = new List<EstimateModel>();

            await GetPerrmission();
            await GetEstimatesForCustomer(model.Id);
        }

        async void InitDetails(EstimateModel model, CustomersModel Cust)
        {
            await GetPerrmission();

            IsShowScheduleDates = true; //Show schedule Dates

            if (model.ScheduleId != null)
                ShowScheduleName = true;

            LstItemsEstimate = new ObservableCollection<EstimateItemServicesModel>();
            LstEstimateSchaduleDates = new ObservableCollection<SchaduleDateModel>();
            LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>();
            LstEstimateDates = new ObservableCollection<SchaduleDateModel>();
            OneEstimate = new EstimateModel();

            if (model.LstScdDate != null)
            {
                LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(model.LstScdDate);
            }

            await GetOneEstimateDetails(model.Id);

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

        //Get Customer Estimates
        public async Task GetEstimatesForCustomer(int? CustomerId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<CustomersModel>(string.Format("api/Customers/GetEstimatesOfCustomer?" + "CustomerId=" + CustomerId), UserToken);
                if (Customer != null)
                {
                    foreach (EstimateModel ets in Customer.LstEstimates)
                    {
                        if ((ets.InvoiceId != 0 && ets.InvoiceId != null) || ets.Status != 1 || EmployeePermission.UserRole == 1 || EmployeePermission.ActiveEstimate == false || EmployeePermission.ActiveEditPrice == false)
                        {
                            ets.NotShowConvert = true;//NotShowConvert
                            if (ets.InvoiceId > 0)
                            {
                                ets.GoToInvoice = true;
                            }
                        }
                    }
                    LstEstimates = new ObservableCollection<EstimateModel>(Customer.LstEstimates);
                }
            }
            UserDialogs.Instance.HideHud();
        }

        //Get One Estimate Details
        async Task GetOneEstimateDetails(int? EstimateId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<ObjectOfCustomerModel>(string.Format("api/Customers/GetObjectOfCustomer?" + "InvoiceId=" + null + "&" + "EstimateId=" + EstimateId), UserToken);

                if (Customer != null)
                {
                    if (Customer.ObjEstimate != null)
                    {
                        if ((Customer.ObjEstimate.InvoiceId != 0 && Customer.ObjEstimate.InvoiceId != null) || (Customer.ObjEstimate.Status != 1 && Customer.ObjEstimate.Status != 0))
                        {
                            Customer.ObjEstimate.NotShowConvert = true;//NotShowConvert
                            if (Customer.ObjEstimate.InvoiceId > 0)
                            {
                                Customer.ObjEstimate.GoToInvoice = true;
                            }
                        }

                        if (Customer.ObjEstimate.ScheduleId != null && Customer.ObjEstimate.ScheduleId != 0)
                        {
                            ShowDropdownDatesEstimate = true;

                            await GetScheduleDates(Customer.ObjEstimate.ScheduleId.Value, 1); // All Schedule Dates

                            LstEstimateSchaduleDatesActual = new ObservableCollection<SchaduleDateModel>(Customer.ObjEstimate.LstScdDate);

                            if (LstEstimateSchaduleDatesActual.Count == 1)
                            {
                                IsShowScheduleDates = false; //Don't Show schedule Dates
                            }

                            string str = "";
                            LstEstimateDates = new ObservableCollection<SchaduleDateModel>();
                            foreach (var Date in Customer.ObjEstimate.LstScdDate)
                            {
                                str += (" , " + Date.Date);
                                LstEstimateDates.Add(new SchaduleDateModel
                                {
                                    Id = Date.Id,
                                    Date = Date.Date,
                                });
                            }

                            if (!string.IsNullOrEmpty(str))
                            {
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
                        }

                        OneEstimate = Customer.ObjEstimate;

                        AmountOrPersent = OneEstimate.DiscountAmountOrPercent == "%" ? false : true;
                        Discount = OneEstimate.Discount;

                        Pending = OneEstimate.Status == 0 ? true : false;
                        Accept = OneEstimate.Status == 1 ? true : false;
                        Declind = OneEstimate.Status == 2 ? true : false;

                        if (OneEstimate.LstEstimateItemServices != null)
                        {
                            if (OneEstimate.LstEstimateItemServices.Count > 4)
                            {
                                LstHeight = 1;
                            }

                            TotalEstimate(OneEstimate);

                            LstItemsEstimate = new ObservableCollection<EstimateItemServicesModel>(OneEstimate.LstEstimateItemServices);
                        }
                    }
                }
            }

            UserDialogs.Instance.HideHud();
        }

        public void TotalEstimate(EstimateModel model)
        {
            decimal? SumCost = model.LstEstimateItemServices.Where(x => x.Price > 0).Sum(s => s.Price * s.Quantity);
            decimal? DiscountVal = 0;
            if (model.DiscountAmountOrPercent == "%")
            {
                DiscountVal = SumCost * Discount / 100;
            }
            else
            {
                DiscountVal = Discount;
            }
            SubTotalEst = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
            PaidEst = 0;

            if (model.Taxval != null)
            {
                NetEst = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + model?.Taxval).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + model?.Taxval).Value, 2, MidpointRounding.ToEven);
                TotalDueEst = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + model.Taxval - PaidEst).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + model.Taxval - PaidEst).Value, 2, MidpointRounding.ToEven);
            }
            else
            {
                decimal? TaxValue = 0;
                if (model.Tax != null)
                {
                    TaxValue = model.DiscountAmountOrPercent == "%" ? ((SumCost - DiscountVal) * model.Tax / 100) : ((SumCost - DiscountVal) * model.Tax / 100);
                }

                NetEst = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDueEst = model.DiscountAmountOrPercent == "%" ? Math.Round(((SumCost - DiscountVal) + TaxValue - PaidEst).Value, 2, MidpointRounding.ToEven) : Math.Round(((SumCost - DiscountVal) + TaxValue - PaidEst).Value, 2, MidpointRounding.ToEven);
            }
        }

        public void TotalEstimate(CustomersModel CustModel)
        {
            if (CustModel.Id != 0)
            {
                //decimal? SumCost = CustModel.LstCustItemsServices.Where(x => x.CostRate > 0 && x.Out == false).Sum(s => s.CostRate * s.Quantity);
                decimal? SumCost = LstSchItemsEstimate.Where(x => x.CostRate > 0).Sum(s => s.CostRate * s.Quantity);

                decimal? DiscountVal = (CustModel.MemeberType == false) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO == null) ? Discount != 0 ? (SumCost * Discount / 100) : 0 : (CustModel.MemberDTO.MemberType == false) ? (SumCost * Discount / 100) : (Discount);

                decimal? TaxValue = CustModel.TaxDTO != null ? (SumCost - DiscountVal) * CustModel.TaxDTO.Rate / 100 : 0;

                SubTotalEst = Math.Round(SumCost.Value, 2, MidpointRounding.ToEven);
                PaidEst = 0;
                NetEst = Math.Round(((SumCost - DiscountVal) + TaxValue).Value, 2, MidpointRounding.ToEven);
                TotalDueEst = Math.Round(((SumCost - DiscountVal) + TaxValue - PaidEst).Value, 2, MidpointRounding.ToEven);
            }
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
                    LstEstimateSchaduleDates = json;
                }

                UserDialogs.Instance.HideHud();
            }

        }

        #endregion


        [RelayCommand]
        async Task GoInvoice(int InvoiceId)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.InvoiceDetailsPage(new CustInvoicesViewModel(InvoiceId, CustomerDetails, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SelecteEstimateDetails(EstimateModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.EstimateDetailsPage(new CustEstimatesViewModel(model, CustomerDetails, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitEstimate(EstimateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                OneEstimate = model;

                if (OneEstimate != null)
                {
                    string UserToken = await _service.UserToken();

                    OneEstimate.Status = Accept == true ? 1 : Declind == true ? 2 : 0; //0 = Pending
                    OneEstimate.LstScdDate = LstEstimateSchaduleDatesActual.ToList();

                    if (OneEstimate.LstEstimateItemServices.Count > 0)
                    {
                        OneEstimate.Total = SubTotalEst;
                        OneEstimate.Net = NetEst;
                        OneEstimate.Discount = Discount;
                        OneEstimate.SignatureDraw = SignatureImageByte64Estimate == null ? OneEstimate.SignatureDraw : SignatureImageByte64Estimate;

                        var json = "";
                        if (OneEstimate.Id == 0)
                        {
                            UserDialogs.Instance.ShowLoading();
                            json = await ORep.PostDataAsync("api/Estimates/PostEstimate", OneEstimate, UserToken);
                            UserDialogs.Instance.HideHud();
                        }
                        else
                        {
                            UserDialogs.Instance.ShowLoading();
                            json = await ORep.PutDataAsync("api/Estimates/PutEstimate", OneEstimate, UserToken);
                            UserDialogs.Instance.HideHud();
                        }

                        if (json != "Bad Request" && json != "api not responding" && json.Contains("Already Exist For This Schedule Date#") != true)
                        {
                            var toast = Toast.Make("Successfully Save Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();

                            MessagingCenter.Send(this, "SavedEstimate", true);
                            await App.Current!.MainPage!.Navigation.PopAsync();

                            if (OneEstimate.Status == 1)//Status Accept
                            {
                                ShowEstimateConvertToInvoice = true;
                            }
                            else
                            {
                                ShowEstimateConvertToInvoice = false;
                            }
                        }
                        else
                        {
                            var toast = Toast.Make($"Failed Save Estimate : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                    else
                    {
                        var toast = Toast.Make("Please Choose Item-Service for this Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task ConvertToInvoice(EstimateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetOneEstimateDetails(model.Id);

                string UserToken = await _service.UserToken();

                if (OneEstimate != null)
                {
                    OneInvoice = new InvoiceModel();
                    OneInvoice.LstInvoiceItemServices = new List<InvoiceItemServicesModel>();
                    OneInvoice.AccountId = OneEstimate.AccountId;
                    OneInvoice.BrancheId = OneEstimate.BrancheId;
                    OneInvoice.ScheduleId = OneEstimate.ScheduleId;
                    OneInvoice.EstimateId = OneEstimate.Id;
                    OneInvoice.InvoiceDate = DateTime.Now;
                    OneInvoice.CustomerId = OneEstimate.CustomerId;
                    OneInvoice.Total = OneEstimate.Total;
                    OneInvoice.TaxId = OneEstimate.TaxId;
                    OneInvoice.Tax = OneEstimate.Tax;
                    OneInvoice.Taxval = OneEstimate.Taxval;
                    OneInvoice.MemberId = CustomerDetails.MemeberId;
                    OneInvoice.Discount = OneEstimate.Discount;
                    OneInvoice.DiscountAmountOrPercent = OneEstimate.DiscountAmountOrPercent;
                    OneInvoice.Paid = 0;
                    OneInvoice.Net = OneEstimate.Net;
                    OneInvoice.Status = 0; //Draft status if(1=partail & 2=paid)
                    OneInvoice.Type = 2; //Installment Payment type
                    OneInvoice.SignaturePrintName = null;
                    OneInvoice.SignatureDraw = null;
                    OneInvoice.Terms = null;
                    OneInvoice.NotesForCustomer = OneEstimate.NotesForCustomer;
                    OneInvoice.Notes = OneEstimate.Notes;
                    OneInvoice.Active = OneEstimate.Active;
                    OneInvoice.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                    OneInvoice.CreateDate = DateTime.Now;
                    OneInvoice.LstScdDate = OneEstimate.LstScdDate;

                    foreach (EstimateItemServicesModel item in OneEstimate.LstEstimateItemServices)
                    {
                        InvoiceItemServicesModel ObjItem = new InvoiceItemServicesModel
                        {
                            Id = item.Id,
                            AccountId = OneEstimate.AccountId,
                            BrancheId = OneEstimate.BrancheId,
                            //ItemServiceDescription = item.ItemServiceDescription,
                            TaxId = item.TaxId,
                            Tax = item.Tax,
                            //Taxable = item.Taxable,
                            Taxable = true,
                            Unit = item.Unit,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            //Discountable = CustomerDetails.MemberDTO.MemberValue != null ? true : false,
                            Discountable = true,
                            ItemsServicesId = item.ItemsServicesId,
                            ItemsServicesName = item.ItemsServicesName,
                            CreateUser = int.Parse(Helpers.Settings.UserIdGet),
                            CreateDate = DateTime.Now,
                            SkipOfTotal = false,
                            Total = item.Quantity != null && item.Tax != null ? (item.Price * item.Quantity) + (item.Price * item.Quantity * item.Tax / 100) : item.Quantity == null && item.Tax != null ? item.Price + (item.Price * item.Quantity * item.Tax / 100) : item.Quantity != null && item.Tax == null ? item.Price * item.Quantity : item.Price,
                            Active = OneEstimate.Active,
                        };
                        OneInvoice.LstInvoiceItemServices.Add(ObjItem);
                    }

                    UserDialogs.Instance.ShowLoading();
                    var json = await ORep.PostMData("api/Invoices/PostInvoice", OneInvoice, UserToken);
                    UserDialogs.Instance.HideHud();

                    if (json != null && json != "api not responding" && json.Contains("Not_Enough") != true && json.Contains("This Invoice Already Exist") != true)
                    {
                        var toast = Toast.Make("Successfully Convert To Inovice.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                        OneInvoice.Id = int.Parse(json.Trim('"'));

                        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.InvoiceDetailsPage(new CustInvoicesViewModel(OneInvoice, CustomerDetails, ORep, _service), ORep, _service));

                        App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                    }
                    else
                    {
                        var toast = Toast.Make($"Failed Convert To Inovice : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }

        //Create Estimate
        [RelayCommand]
        void CreateEstimateEditDiscountForCustomer(CustomersModel model)
        {
            Discount = model.Discount;

            TotalEstimate(CustomerDetails);
        }

        //Estimate Details
        [RelayCommand]
        void EditDiscountForCustomerEstimate(CustomersModel model)
        {
            Discount = CustomerDetails.Discount = model.Discount;

            TotalEstimate(OneEstimate);
        }

        [RelayCommand]
        async Task SelectSendEmailEstimate(EstimateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                UserDialogs.Instance.ShowLoading();
                var json = await ORep.PostStrAsync("api/Estimates/PostEstimateEmail", model, UserToken);
                UserDialogs.Instance.HideHud();

                if (!string.IsNullOrEmpty(json) && json.Contains("Send Success") == true)
                {
                    var toast = Toast.Make("Successfully Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    var toast = Toast.Make("Successfully Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }

            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenEstimateScheduleDates()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                foreach (SchaduleDateModel dt in LstEstimateSchaduleDates)
                {
                    foreach (SchaduleDateModel dt2 in LstEstimateSchaduleDatesActual)
                    {
                        if (dt.Id == dt2.Id)
                        {
                            dt.IsChecked = true;
                        }
                    }
                }

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
            SchaduleDateModel DataOfScddt = LstEstimateSchaduleDatesActual.Where(x => x.Id == Date.Id).FirstOrDefault();
            LstEstimateSchaduleDatesActual.Remove(DataOfScddt);

            foreach (SchaduleDateModel dt in LstEstimateSchaduleDates)
            {
                foreach (SchaduleDateModel dt2 in LstEstimateSchaduleDatesActual)
                {
                    if (dt.Id == dt2.Id)
                    {
                        dt.IsChecked = true;
                    }
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

        //New Estimate
        [RelayCommand]
        async Task SelecteNewSchItemsEstimate(CustomersModel CustModel)
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
                        AccountId = CustModel.AccountId,
                        BrancheId = CustModel.BrancheId,
                        //ScheduleId = 0,
                        ItemsServicesId = item.Id,
                        ItemsServicesName = item.Name,
                        ItemServiceDescription = item.Description,
                        TaxId = item.TaxId,
                        Tax = item.Tax,
                        CostRate = item.CostperUnit,
                        Total = item.QTYTime != null && item.Tax != null ? (item.CostperUnit * item.QTYTime) + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime == null && item.Tax != null ? item.CostperUnit + (item.CostperUnit * item.QTYTime * item.Tax / 100) : item.QTYTime != null && item.Tax == null ? item.CostperUnit * item.QTYTime : item.CostperUnit,
                        Notes = item.Notes,
                        Active = item.Active,
                        CreateUser = int.Parse(Settings.UserIdGet),
                        CreateDate = item.CreateDate,
                        Taxable = item.Taxable,
                        Quantity = (item.QTYTime == null || item.QTYTime == 0) ? 1 : item.QTYTime,
                        Unit = item.Unit,
                    };

                    if (LstSchItemsEstimate.Count > 0)
                    {
                        var itm = LstSchItemsEstimate.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            LstSchItemsEstimate.Add(ItemsModel);
                        }
                    }
                    else
                    {
                        LstSchItemsEstimate.Add(ItemsModel);
                    }

                    if (LstSchItemsEstimate.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    TotalEstimate(CustModel);

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        //Estimate Details
        [RelayCommand]
        async Task SelecteNewItemsEstimate(EstimateModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new ScheduleItemsServicesViewModel(true);
                popupView.ItemClose += (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    EstimateItemServicesModel EstimateModel = new EstimateItemServicesModel
                    {
                        AccountId = model.AccountId,
                        BrancheId = model.BrancheId,
                        ItemsServicesId = item.Id,
                        EstimateId = model.Id,
                        ItemsServicesName = item.Name,
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
                    };

                    if (LstItemsEstimate.Count > 0)
                    {
                        var itm = LstItemsEstimate.Where(x => x.ItemsServicesId == item.Id).FirstOrDefault();
                        if (itm == null)
                        {
                            LstItemsEstimate.Add(EstimateModel);
                            OneEstimate.LstEstimateItemServices.Add(EstimateModel);
                        }
                    }
                    else
                    {
                        LstItemsEstimate.Add(EstimateModel);
                        OneEstimate.LstEstimateItemServices.Add(EstimateModel);
                    }

                    if (LstItemsEstimate.Count > 4)
                    {
                        LstHeight = 1;
                    }

                    TotalEstimate(model);

                    UserDialogs.Instance.HideHud();
                };

                var page = new Pages.SchedulePages.NewItemsServicesSchedulePage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }

        //New Estimate
        [RelayCommand]
        void RemoveSchItemEstimate(ScheduleItemsServicesModel item)
        {
            IsEnable = false;

            LstSchItemsEstimate.Remove(item);

            TotalEstimate(CustomerDetails);

            IsEnable = true;
        }

        //Estimate Details
        [RelayCommand]
        void RemoveItemEstimate(EstimateItemServicesModel item)
        {
            IsEnable = false;

            LstItemsEstimate.Remove(item);
            OneEstimate.LstEstimateItemServices.Remove(item);
            TotalEstimate(OneEstimate);

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectCustToCreateEstimatePage()
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            Controls.StaticMembers.WayAfterChooseCust = 1; //Create Estimate 
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CreateEstimateWithoutSchedulePage(new CustEstimatesViewModel(CustomerDetails, Controls.StaticMembers.WayAfterChooseCust, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task DeleteEstimate(int EstId)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();

                var json = await ORep.DeleteStrItemAsync(string.Format("api/Estimates/DeleteEstimate/{0}", EstId), UserToken);

                if (json != null && json != "api not responding" && json.Contains("This Estimate Can`t Deleted") != true)
                {
                    var toast = Toast.Make($"Successfully Delete Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();

                    MessagingCenter.Send(this, "SavedEstimate", true);
                    await App.Current!.MainPage!.Navigation.PopAsync();
                }
                else
                {
                    var toast = Toast.Make($"Failed Delete Estimate : {json}", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SubmitCustEstimate(CustomersModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                if (model != null)
                {
                    if (LstSchItemsEstimate.Count > 0)
                    {
                        if (Pending == true || Accept == true || Declind == true)
                        {

                            OneEstimate.AccountId = model.AccountId;
                            OneEstimate.BrancheId = model.BrancheId;
                            //OneEstimate.ScheduleId = model.Id;
                            //OneEstimate.ScheduleDateId = model.OneScheduleDate.Id;
                            OneEstimate.EstimateDate = DateTime.Now;
                            OneEstimate.CustomerId = model.Id;
                            OneEstimate.Total = SubTotalEst;
                            OneEstimate.TaxId = model.TaxId;
                            OneEstimate.Tax = model.TaxDTO?.Rate;
                            //OneEstimate.Taxval = (SubTotal - (SubTotal * model.MemberDTO.MemberValue / 100)) * model.TaxDTO.Rate / 100;
                            //OneEstimate.Taxval = (model.MemeberType == false && model.TaxDTO != null) ? (SubTotal - (SubTotal * model.MemberDTO.MemberValue / 100) * model.TaxDTO.Rate / 100) : (model.TaxDTO != null && model.MemeberType == true && model.TaxDTO != null) ? ((SubTotal - model.Discount) * model.TaxDTO.Rate / 100) : 0;
                            OneEstimate.Taxval = null;
                            OneEstimate.MemberId = model.MemeberId;
                            OneEstimate.Discount = Discount;
                            OneEstimate.SignatureDraw = SignatureImageByte64Estimate;
                            //OneEstimate.DiscountAmountOrPercent = model.MemberDTO.MemberType == false ? "%" : "$";
                            OneEstimate.DiscountAmountOrPercent = AmountOrPersent == false ? "%" : "$";
                            OneEstimate.Net = NetEst;
                            OneEstimate.Status = Accept == true ? 1 : Declind == true ? 2 : 0; //0 = Pending
                            OneEstimate.SignaturePrintName = null;
                            OneEstimate.Terms = null;
                            OneEstimate.NotesForCustomer = model.Notes;
                            //OneEstimate.Notes = model.Notes;
                            OneEstimate.Active = model.Active;
                            OneEstimate.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                            OneEstimate.CreateDate = DateTime.Now;

                            foreach (ScheduleItemsServicesModel item in LstSchItemsEstimate)
                            {
                                EstimateItemServicesModel ObjItem = new EstimateItemServicesModel
                                {
                                    Id = item.Id,
                                    AccountId = model.AccountId,
                                    BrancheId = model.BrancheId,
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
                                    Total = item.Quantity != null && item.Tax != null ? (item.CostRate * item.Quantity) + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity == null && item.Tax != null ? item.CostRate + (item.CostRate * item.Quantity * item.Tax / 100) : item.Quantity != null && item.Tax == null ? item.CostRate * item.Quantity : item.CostRate,
                                    Active = model.Active,
                                };
                                OneEstimate.LstEstimateItemServices.Add(ObjItem);
                            }

                            UserDialogs.Instance.ShowLoading();
                            var json = await ORep.PostDataAsync("api/Estimates/PostEstimate", OneEstimate, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (json != "Bad Request" && json != "api not responding" && json.Contains("Already Exist For This Schedule Date#") != true)
                            {
                                var toast = Toast.Make("Successfully Create Estimate for This Job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();

                                bool answer = await App.Current!.MainPage!.DisplayAlert("Question?", "Do you want to send an email to the customer?", "Yes", "No");

                                if (answer)//Send Email
                                {
                                    UserDialogs.Instance.ShowLoading();
                                    var jsonEmail = await ORep.PostStrAsync("api/Estimates/PostEstimateEmail", OneEstimate, UserToken);
                                    UserDialogs.Instance.HideHud();

                                    if (!string.IsNullOrEmpty(jsonEmail) && jsonEmail.Contains("Send Success") == true)
                                    {
                                        var toast1 = Toast.Make("Success Send Email to Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                        await toast1.Show();
                                    }
                                    else
                                    {
                                        var toast1 = Toast.Make("Failed to send e-mail to the customer", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                        await toast1.Show();
                                    }
                                }

                                MessagingCenter.Send(this, "SavedEstimate", true);
                                await App.Current!.MainPage!.Navigation.PopAsync();

                                if (OneEstimate.Status == 1)//Status Accept
                                {
                                    ShowEstimateConvertToInvoice = true;
                                }
                                else
                                {
                                    ShowEstimateConvertToInvoice = false;
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
                            var toast1 = Toast.Make("Please Choose Status for Estimate.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
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

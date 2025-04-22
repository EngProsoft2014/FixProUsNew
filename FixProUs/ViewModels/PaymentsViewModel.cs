
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages;
using Stripe;


namespace FixProUs.ViewModels
{
    public partial class PaymentsViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        InvoiceModel oneInvoice;

        [ObservableProperty]
        PaymentsModel onePayment;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        int cashOrCredit;

        [ObservableProperty]
        string cardNumber;

        [ObservableProperty]
        string holderName;

        [ObservableProperty]
        string expirationDate;

        [ObservableProperty]
        string signatureImageByte64;

        [ObservableProperty]
        string cvv;

        [ObservableProperty]
        StripeAccountModel stripeModel;
        #endregion

        //public ICommand ScanCommand
        //{
        //    get
        //    {
        //        return new Command(async () =>
        //        {
        //            try
        //            {
        //                Card = await CrossPayCards.Current.ScanAsync().ConfigureAwait(true);
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.WriteLine(ex);
        //            }
        //        });
        //    }
        //}

        //private PayCard _card;
        //public PayCard Card
        //{
        //    set
        //    {
        //        if (_card != value)
        //        {
        //            _card = value;

        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Card)));
        //        }
        //    }
        //    get 
        //    {
        //        CardNumber = _card.CardNumber;
        //        HolderName = _card.HolderName;
        //        ExpirationDate = _card.ExpirationDate;   
        //        return _card;
        //    }
        //}

        #region Cons
        public PaymentsViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
        }

        public PaymentsViewModel(InvoiceModel model, CustomersModel Cust, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            CashOrCredit = Controls.StaticMembers.PayCashOrCredit; //Show Credit Or Cash in View
            OnePayment = new PaymentsModel();
            OneInvoice = new InvoiceModel();
            StripeModel = new StripeAccountModel();

            OneInvoice = model;
            CustomerDetails = Cust;
        }
        #endregion

        #region RelayCommand
        [RelayCommand]
        async void CashPayNow(InvoiceModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (OnePayment.Amount != null && (OnePayment.Amount <= 0 || OnePayment.Amount == null))
                {
                    var toast = Toast.Make("Please Complete Amount Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(SignatureImageByte64))
                {
                    var toast = Toast.Make("Please Complete Signature Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    OnePayment.Type = 1;
                    OnePayment.Method = 1; //Cash
                    UserDialogs.Instance.ShowLoading();
                    await InitiolizModel(model);
                    UserDialogs.Instance.HideHud();
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async void CreditPayNow(InvoiceModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                OneInvoice = model;
                if (string.IsNullOrEmpty(HolderName))
                {
                    var toast = Toast.Make("Please Complete Card Holder Name Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(CardNumber))
                {
                    var toast = Toast.Make("Please Complete Card Number Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(ExpirationDate))
                {
                    var toast = Toast.Make("Please Complete Card Expiration Date Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(Cvv))
                {
                    var toast = Toast.Make("Please Complete Card Cvv Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (OnePayment.Amount != null && (OnePayment.Amount <= 0 || OnePayment.Amount == null))
                {
                    var toast = Toast.Make("Please Complete Amount Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(SignatureImageByte64))
                {
                    var toast = Toast.Make("Please Complete Signature Field!!!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    OnePayment.Type = 0;
                    OnePayment.Method = 3; //Debit Card
                    UserDialogs.Instance.ShowLoading();
                    await PayViaStripe();
                    UserDialogs.Instance.HideHud();
                }

                IsEnable = true;
            }
        }
        #endregion

        #region Methods
        public async Task InitiolizModel(InvoiceModel model)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrEmpty(SignatureImageByte64))
                {
                    if (OnePayment.Amount <= model.Net || OnePayment.Amount == null)
                    {
                        string UserToken = await _service.UserToken();

                        OnePayment.AccountId = model.AccountId;
                        OnePayment.BrancheId = model.BrancheId;
                        OnePayment.CustomerId = model.CustomerId;
                        //OnePayment.ContractId = model.ContractId;
                        OnePayment.InvoiceId = model.Id;
                        //OnePayment.ExpensesId = model.ExpensesId;
                        OnePayment.PaymentDate = DateTime.Now;
                        OnePayment.SignatureDraw = SignatureImageByte64;

                        OnePayment.Amount = OnePayment.Amount == null ? model.Net : OnePayment.Amount;
                        //OnePayment.OverAmount = model.OverAmount;

                        //OnePayment.IncreaseDecrease = model.IncreaseDecrease;
                        //OnePayment.TransactionID = model.TransactionID;
                        //OnePayment.CheckNumber = model.CheckNumber;
                        //OnePayment.BankName = model.BankName;
                        //OnePayment.AccountNumber = model.AccountNumber;
                        OnePayment.Notes = model.Notes;
                        OnePayment.Active = model.Active;
                        OnePayment.CreateUser = model.CreateUser;
                        OnePayment.CreateDate = model.CreateDate;

                        UserDialogs.Instance.ShowLoading();
                        var json = await ORep.PostDataAsync("api/Payments/InsertPayment", OnePayment, UserToken);
                        UserDialogs.Instance.HideHud();

                        if (json != null && json != "api not responding")
                        {
                            var toast = Toast.Make("Successfully payment for this job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                        }
                        else
                        {
                            var toast = Toast.Make("Failed payment for this job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                    else
                    {
                        var toast = Toast.Make("Please enter right amount.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
                else
                {
                    var toast = Toast.Make("Please enter signature.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }
        }

        async Task GetSkretKey(int? BranchId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<StripeAccountModel>(string.Format("api/Payments/GetStripeAccount?" + "BranchId=" + BranchId), UserToken);

                if (json != null)
                {
                    StripeModel = json;
                }
                UserDialogs.Instance.HideHud();
            }
        }

        public async Task PayViaStripe()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetSkretKey(OneInvoice.BrancheId);

                StripeConfiguration.ApiKey = StripeModel.SecretKey;

                //StripeConfiguration.ApiKey = "sk_test_IHINMHgrNTLUWqh3IcTcMdNB";

                // step 2: Assign card to token object
                var stripeCard = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = CardNumber,
                        Name = HolderName,
                        ExpMonth = ExpirationDate.Split('/')[0],
                        ExpYear = ExpirationDate.Split('/')[1],
                        Cvc = cvv,
                    }
                };

                Stripe.TokenService service = new Stripe.TokenService();
                Stripe.Token newToken = service.Create(stripeCard);

                // step 3: assign the token to the source
                var option = new SourceCreateOptions
                {
                    Type = SourceType.Card,
                    Currency = "USD",
                    Token = newToken.Id
                };

                var sourceService = new SourceService();
                Stripe.Source source = sourceService.Create(option);

                // step 4: create customer
                CustomerCreateOptions customer = new CustomerCreateOptions
                {
                    Name = CustomerDetails.FirstName + "" + CustomerDetails.LastName,
                    Email = CustomerDetails.Email,
                    Description = OneInvoice.ScheduleName,
                    Address = new AddressOptions { City = CustomerDetails.City, Country = CustomerDetails.Country, Line1 = CustomerDetails.Address, Line2 = "", PostalCode = CustomerDetails.PostalcodeZIP, State = CustomerDetails.State }
                };

                var customerService = new CustomerService();
                var cust = customerService.Create(customer);

                // step 5: charge option
                var chargeoption = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(OneInvoice.Total * 100),
                    Currency = "USD",
                    ReceiptEmail = CustomerDetails.Email,
                    Customer = cust.Id,
                    Source = source.Id
                };

                // step 6: charge the customer
                var chargeService = new ChargeService();
                Charge charge = chargeService.Create(chargeoption);
                if (charge.Status == "succeeded")
                {
                    // success
                    await InitiolizModel(OneInvoice);
                }
                else
                {
                    // failed
                    var toast = Toast.Make("Failed payment for this job.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }

        } 
        #endregion
    }
}

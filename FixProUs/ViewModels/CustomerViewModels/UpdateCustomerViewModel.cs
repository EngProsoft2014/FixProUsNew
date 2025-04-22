using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages;
using GoogleApi.Entities.Maps.AerialView.Common.Enums;
using Mopups.Services;
using Newtonsoft.Json;
using Stripe;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace FixProUs.ViewModels
{
    public partial class UpdateCustomerViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Properties
        [ObservableProperty]
        CampaignModel oneCampaign;

        [ObservableProperty]
        ObservableCollection<CampaignModel> lstCampaigns;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        CustomersCategoryModel oneCategoryModel;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        MemberModel oneMemberModel;

        [ObservableProperty]
        TaxModel oneTaxModel;

        [ObservableProperty]
        CustomerfeaturesModel customerFeatures;

        [ObservableProperty]
        bool showArrowsBarFeatures;

        [ObservableProperty]
        decimal? discount;

        [ObservableProperty]
        bool isMemberShip;

        [ObservableProperty]
        string address;

        [ObservableProperty]
        string houseValue;

        [ObservableProperty]
        string _yearBuilt;

        [ObservableProperty]
        string squareFootage;

        [ObservableProperty]
        string city;

        [ObservableProperty]
        string state;

        [ObservableProperty]
        string zipCode;

        public int BranchIdVM;
        #endregion

        public UpdateCustomerViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init(model);
        }

        void Init(CustomersModel model)
        {
            BranchIdVM = int.Parse(Settings.BranchIdGet);
            CustomerDetails = new CustomersModel();
            CustomerDetails.LstCustomersCustomField = new List<CustomersCustomFieldModel>();
            CustomerFeatures = new CustomerfeaturesModel();
            LstCampaigns = new ObservableCollection<CampaignModel>();
            OneCampaign = new CampaignModel();
            CustomerFeatures = new CustomerfeaturesModel();
            OneCategoryModel = new CustomersCategoryModel();
            OneMemberModel = new MemberModel();
            OneTaxModel = new TaxModel();

            CustomerDetails = model;

            if (CustomerDetails.MemeberType == true)
            {
                if (CustomerDetails.MemberDTO != null)
                {
                    CustomerDetails.Discount = CustomerDetails.MemberDTO.MemberValue;
                }
            }

            if (CustomerDetails.MemeberType == true)
            {
                IsMemberShip = true;
            }
            else
            {
                IsMemberShip = false;
            }

            //if (CustomerDetails.MemeberType == true)
            //{
            //    if (CustomerDetails.MemberDTO != null)
            //    {
            //        Discount = CustomerDetails.MemberDTO.MemberValue;
            //    }
            //}
            //else
            //{
            //    Discount = CustomerDetails.Discount;
            //}

            //if (Discount == null)
            //{
            //    Discount = 0;
            //}

            Address = CustomerDetails.Address;
            YearBuilt = CustomerDetails.YearBuit;
            HouseValue = CustomerDetails.EstimedValue;
            SquareFootage = CustomerDetails.Squirefootage;

            Task.WhenAll(GetPerrmission(), GetCampaigns(), GetCustomerFeatures(int.Parse(Settings.AccountIdGet)));
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

        //Get Campaigns
        async Task GetCampaigns()
        {
            string UserToken = await _service.UserToken();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var json = await ORep.GetAsync<ObservableCollection<CampaignModel>>(string.Format("api/Calls/GetCampaigns?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstCampaigns = json;

                    OneCampaign = LstCampaigns.Where(x => x.Id == CustomerDetails.Source!.Value).FirstOrDefault()!;
                }
            }

        }


        async Task GetCustomerFeatures(int? AccountId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Features = await ORep.GetAsync<CustomerfeaturesModel>(string.Format("api/Customers/GetCustomerFeatures?" + "AccountId=" + AccountId), UserToken);

                if (Features != null)
                {
                    CustomerFeatures = Features;

                    ShowArrowsBarFeatures = CustomerFeatures.LstCustomersCustomField.Count > 4 ? true : false;

                    CustomerDetails.LstCustomersCustomField = CustomerFeatures.LstCustomersCustomField;

                    OneCategoryModel = CustomerFeatures.LstCustomerCategory.Where(x => x.Id == CustomerDetails.CategoryId).FirstOrDefault()!;

                    OneMemberModel = CustomerFeatures.LstMemberships.Where(x => x.Id == CustomerDetails.MemberDTO?.Id).FirstOrDefault()!;

                    OneTaxModel = CustomerFeatures.LstTaxes.Where(x => x.Id == CustomerDetails.TaxDTO?.Id).FirstOrDefault()!;
                }
            }

            UserDialogs.Instance.HideHud();
        }

        [RelayCommand]
        void ChooseCustomerCategory(CustomersCategoryModel model)
        {
            CustomerDetails.CustomerCategory = model;
        }

        [RelayCommand]
        void ChooseCustomerMemberShip(MemberModel model)
        {
            CustomerDetails.MemberDTO = model;
        }

        [RelayCommand]
        void ChooseCustomerTax(TaxModel model)
        {
            CustomerDetails.TaxDTO = model;
        }

        [RelayCommand]
        void ChooseCustomerCampaign(CampaignModel model)
        {
            CustomerDetails.Source = model.Id;
            CustomerDetails.CampaignDTO = model;
        }

        [RelayCommand]
        async Task SelecteAddress()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                var popupView = new Pages.PopupPages.AddressPupop();
                popupView.DidClose += async (str) =>
                {
                    IsEnable = false;
                    CustomerDetails.AddressModel = str;
                    Address = CustomerDetails.Address = str.FullAddress;
                    CustomerDetails.locationlatitude = str.Latitude.ToString();
                    CustomerDetails.locationlongitude = str.Longitude.ToString();
                    State = CustomerDetails.State = str.State;
                    City = CustomerDetails.City = str.City;
                    ZipCode = CustomerDetails.PostalcodeZIP = str.Zip;
                    CustomerDetails.Country = str.Country;

                    CustomersModel oCust = await Controls.StartData.GetAddressDetails(CustomerDetails);

                    HouseValue = (!string.IsNullOrEmpty(oCust.EstimedValue) && oCust.EstimedValue != "None") ? string.Format("${0:#,0.#}", float.Parse(oCust.EstimedValue)) : "None";
                    CustomerDetails.EstimedValue = HouseValue;

                    YearBuilt = (!string.IsNullOrEmpty(oCust.YearBuit) && oCust.YearBuit != "None") ? oCust.YearBuit : "None";
                    CustomerDetails.YearBuit = YearBuilt;

                    SquareFootage = (!string.IsNullOrEmpty(oCust.Squirefootage) && oCust.Squirefootage != "None") ? oCust.Squirefootage : "None";
                    CustomerDetails.Squirefootage = SquareFootage;

                    CustomerDetails.YearEstimedValue = (!string.IsNullOrEmpty(oCust.YearEstimedValue) && oCust.YearEstimedValue != "None") ? oCust.YearEstimedValue : "None";
                    IsEnable = true;
                };

                await MopupService.Instance.PushAsync(popupView);
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task UpdateCustomer(CustomersModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string valid = "";
                if (!string.IsNullOrEmpty(model.Email))
                {
                    valid = Controls.StaticMembers.CheckStringType(model.Email);
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    var toast = Toast.Make("Please Complete This Field Required : First Name.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(model.LastName))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Last Name.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (model.CustomerCategory == null)
                {
                    var toast = Toast.Make("Please Complete This Field Required : Customer Category.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (model.Source == null)
                {
                    var toast = Toast.Make("Please Complete This Field Required : Source.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(model.Phone1))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Customer Phone.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (!string.IsNullOrEmpty(model.Email) && valid != "Email")
                {
                    var toast = Toast.Make("Check your email and reenter it correctly.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(model.Address))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Customer Address.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    string UserToken = await _service.UserToken();

                    model.AccountId = int.Parse(Helpers.Settings.AccountIdGet);
                    model.BrancheId = int.Parse(Helpers.Settings.BranchIdGet);
                    model.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                    model.Phone1 = model.Phone1.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                    model.CustomerType = 1;
                    model.AllowLogin = false;
                    model.Credit = 0;
                    model.Since = DateTime.Now;
                    model.Active = true;
                    model.Source = OneCampaign.Id;
                    model.CreateDate = DateTime.Now;
                    model.State = CustomerDetails.State != null ? CustomerDetails.State : State;
                    model.City = CustomerDetails.City != null ? CustomerDetails.City : City;
                    model.PostalcodeZIP = CustomerDetails.PostalcodeZIP != null ? CustomerDetails.PostalcodeZIP : ZipCode;

                    if (CustomerDetails.MemberDTO != null)
                    {
                        model.MemeberType = CustomerDetails.MemberDTO.MemberType;
                        model.MemeberId = CustomerDetails.MemberDTO.Id;
                    }
                    else
                    {
                        model.MemeberType = false;
                    }

                    if (CustomerDetails.TaxDTO != null)
                        model.TaxId = CustomerDetails.TaxDTO.Id;

                    model.Taxable = CustomerDetails.Taxable == null ? false : CustomerDetails.Taxable;

                    if (CustomerDetails.CustomerCategory != null)
                        model.CategoryId = CustomerDetails.CustomerCategory.Id;

                    UserDialogs.Instance.ShowLoading();
                    var json = await ORep.PutDataAsync("api/Customers/PutCustomer", model, UserToken);
                    UserDialogs.Instance.HideHud();

                    if (json != null && json != "api not responding" && json != "Multiple Choices")
                    {
                        CustomersModel Customer = JsonConvert.DeserializeObject<CustomersModel>(json);

                        if (Controls.StaticMembers.WayCreateCust == 1)//From CallPage
                        {
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CallPages.NewCallPage(new CallsViewModel(Customer, ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            await MopupService.Instance.PopAsync();
                        }
                        else if (Controls.StaticMembers.WayCreateCust == 2) //From Schedule create new customer
                        {
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewSchedulePage(new AddScheduleViewModel(Customer,ORep,_service),ORep,_service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else if (Controls.StaticMembers.WayCreateCust == 3) //From Schedule can edit customer and return schedule again
                        {
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(Controls.StaticMembers.ScheduleIdStatic, Controls.StaticMembers.ScheduleDateIdStatic,ORep,_service),ORep,_service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else
                        {
                            var toast = Toast.Make("Successfully Update Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            Controls.StaticMembers.TabSelected = 1;
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                            //await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.CustomersPage(new CustomersViewModel(ORep, _service), ORep, _service));
                            //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                    }
                    else if (json == "Multiple Choices")
                    {
                        var toast = Toast.Make("This Customer phone already exists.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else
                    {
                        var toast = Toast.Make("Failed Update Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }


    }
}

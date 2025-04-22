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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FixProUs.ViewModels
{
    public partial class AddCustomerViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Properties
        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        CampaignModel oneCampaign;

        [ObservableProperty]
        ObservableCollection<CampaignModel> lstCampaigns;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        CustomerfeaturesModel customerFeatures;

        [ObservableProperty]
        bool showArrowsBarFeatures;

        [ObservableProperty]
        string address;

        [ObservableProperty]
        string houseValue;

        [ObservableProperty]
        string yearBuilt;

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

        public AddCustomerViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            Init();
        }

        void Init()
        {
            BranchIdVM = int.Parse(Settings.BranchIdGet);
            CustomerFeatures = new CustomerfeaturesModel();
            LstCampaigns = new ObservableCollection<CampaignModel>();
            OneCampaign = new CampaignModel();
            CustomerFeatures = new CustomerfeaturesModel();

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
                var json = await ORep.GetAsync<ObservableCollection<CampaignModel>>(string.Format("api/Calls/GetCampaigns?" + "AccountId=" + Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstCampaigns = json;
                }
            }
        }

        //GetCustomerFeatures
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
        async Task InsertCustomer(CustomersModel model)
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

                    model.LstCustomersCustomField = new List<CustomersCustomFieldModel>();

                    foreach (CustomersCustomFieldModel item in CustomerFeatures.LstCustomersCustomField.ToList())
                    {
                        if (item.Required == false || string.IsNullOrEmpty(item.DefaultValue?.Trim()) != true)
                        {
                            if (item.FieldType == 4 && item.DefaultValue == "True")
                            {
                                item.DefaultValue = "Yes";
                            }
                            else if (item.FieldType == 4 && item.DefaultValue == "False")
                            {
                                item.DefaultValue = "No";
                            }
                            else if (item.FieldType == 3)
                            {
                                item.DefaultValue = DateTime.Parse(item.DefaultValue).ToString("yyyy-MM-dd");
                            }

                            model.LstCustomersCustomField.Add(item);
                        }
                        else
                        {
                            var toast = Toast.Make($"Please Complete This Field : {item.CustomFieldName} Required For Custom Field.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            return;
                        }
                    }

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
                    var json = await ORep.PostDataAsync("api/Customers/PostCustomer", model, UserToken);
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
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewSchedulePage(new AddScheduleViewModel(Customer, ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else if (Controls.StaticMembers.WayCreateCust == 3) //From Schedule can edit customer and return schedule again
                        {
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(Controls.StaticMembers.ScheduleIdStatic, Controls.StaticMembers.ScheduleDateIdStatic, ORep, _service), ORep, _service));
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                        else
                        {
                            var toast = Toast.Make("Successfully Insert Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                            await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                        }
                    }
                    else if (json == "Multiple Choices")
                    {
                        var toast = Toast.Make("This Customer phone already exists.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else
                    {
                        var toast = Toast.Make("Failed Insert Customer.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                }
            }

            IsEnable = true;
        }
    }
}

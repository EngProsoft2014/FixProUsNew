
using Newtonsoft.Json;
using FixProUs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using Twilio.Http;
using Stripe;
using Controls.UserDialogs.Maui;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Mopups.PreBaked.Interfaces;
using OneSignalSDK.DotNet;
using FixProUs.ViewModels;

namespace FixProUs.Controls
{
    public static class StartData
    {
        static readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();
        public static ObservableCollection<EmployeeModel> LstWorkingEmployeesStatic { get; set; }
        public static EmployeeModel EmployeeDataStatic { get; set; } = new EmployeeModel();
        public static DeviceDetailsModel DeviceDetails { get; set; }
        public static bool IsRunning { get; set; } = true;
        public static Com_MainModel Com_MainObj { get; set; }
        private static string RealtyRapidApiKey = "";

        static Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        static DeviceDetailsModel oDevice { get; set; }

        //Get Working Employees
        public async static Task GetWorkingEmployees(int AccountId, string ScheduleDate)
        {
            UserDialogs.Instance.ShowLoading();

            //string json = await Helpers.Utility.CallWebApi("api/Employee/GetEmpWorking?" + "AccountId=" + AccountId + "&" + "ScheduleDate=" + ScheduleDate);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();

                var json = await ORep.GetAsync<ObservableCollection<EmployeeModel>>("api/Employee/GetEmpWorking?" + "AccountId=" + AccountId + "&" + "ScheduleDate=" + ScheduleDate, UserToken);

                if (json != null)
                {
                    //List<EmployeeModel> Employee = JsonConvert.DeserializeObject<List<EmployeeModel>>(json);

                    //LstWorkingEmployeesStatic = new ObservableCollection<EmployeeModel>(Employee);
                    LstWorkingEmployeesStatic = json;
                }
            }

            UserDialogs.Instance.HideHud();
        }

        public static string SendSMS(string Phone, string Msg)
        {
            //var accountSid = "AC2aa33faec930e6bddfef1daa25e3b945";
            //var authToken = "744fd3259244985557d4d0c1aa2617eb";
            try
            {
                var accountSid = Controls.StartData.Com_MainObj.TwilioAccountSid;
                var authToken = Controls.StartData.Com_MainObj.TwilioauthToken;
                TwilioClient.Init(accountSid, authToken);

                var messageOptions = new CreateMessageOptions(
                  new PhoneNumber("+1" + Phone));

                messageOptions.From = new PhoneNumber(Controls.StartData.Com_MainObj.TwilioFromPhoneNumber);
                messageOptions.Body = Msg;
                var message = MessageResource.Create(messageOptions);
                return message.Sid;
            }
            catch (Exception)
            {

                return "";
            }

        }

        //Get Com_Main
        public async static Task GetCom_Main()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var json = await ORep.GetAsync<Com_MainModel>("api/Login/GetCom_Main");

                if (json != null)
                {
                    Com_MainObj = json;
                    RealtyRapidApiKey = Com_MainObj.RealtyRapidApi;
                }
            }
        }


        public async static Task<AccountModel> GetExpiredDate(int AccountId)
        {
            try
            {
                var ExpDate = await ORep.GetAsync<AccountModel>("api/Login/GetExpiredDate?AccountId=" + AccountId);
                if (ExpDate != null)
                {
                    return ExpDate;
                }
                else
                {
                    return new AccountModel();
                }
            }
            catch (Exception ex)
            {
                return new AccountModel();
            }

        }

        public async static Task CheckPermissionEmployee()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrEmpty(Helpers.Settings.UserNameGet) && !string.IsNullOrEmpty(Helpers.Settings.PasswordGet))
                {
                    var json = await ORep.GetLoginAsync<EmployeeModel>("api/Login/GetLogin?" + "UserName=" + Helpers.Settings.UserNameGet + "&" + "Password=" + Helpers.Settings.PasswordGet + "&" + "PlayerId=" + Preferences.Default.Get(Helpers.Settings.PlayerId, OneSignal.User.PushSubscription.Id));

                    if (json != null)
                    {
                        if (!string.IsNullOrEmpty(json.EmployeeStatus) && json.EmployeeStatus.Contains("Try Again"))
                        {
                            Preferences.Default.Clear();
                            Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
                            Controls.StartData.IsRunning = false;
                            await App.Current!.MainPage!.DisplayAlert("Alert", "You’ve been logged out.\r\n(account is changed username and password)\r\n", "Ok");
                        }
                        else
                        {
                            EmployeeDataStatic = json;
                        }
                    }

                }
            }

        }


        public async static Task GetAccountKeysAsync()
        {

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                Helpers.GenericRepository ORep = new Helpers.GenericRepository();
                var Account = await ORep.GetAsync<AccountModel>("api/Notifications/GetAccountKeys?" + "AccountId=" + Helpers.Settings.AccountIdGet, UserToken);
                if (Account != null)
                {
                    Preferences.Default.Set(Helpers.Settings.OneSignalAppId, Account.OneSignalAppId);
                    Preferences.Default.Set(Helpers.Settings.OneSignalRest, Account.OneSignalRestApikey);
                    Preferences.Default.Set(Helpers.Settings.OneSignalAuth, Account.OneSignalAuthApi);
                }
            }
        }

        public async static Task GetMyDeviceDetails()
        {
            if (Helpers.Settings.PlayerId != "0")
            {

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    string UserToken = await _service.UserToken();
                    string json = await ORep.GetAsync<string>("api/Notifications/GetDevice?" + "AccountId=" + Helpers.Settings.AccountId + "&" + "PlayerId=" + Helpers.Settings.PlayerId, UserToken); // Get Device Details

                    if (json != null && json.Contains("-"))
                    {
                        DeviceDetails = JsonConvert.DeserializeObject<DeviceDetailsModel>(json)!;
                    }
                }
            }
        }

        public async static Task UserLogout()
        {
            UserDialogs.Instance.ShowLoading();

            Preferences.Default.Clear();
            Helpers.Utility.ServerUrl = Helpers.Utility.ServerUrlStatic;
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
            Controls.StartData.IsRunning = false;

            UserDialogs.Instance.HideHud();
        }

        private const string BaseUrl = "https://realty-mole-property-api.p.rapidapi.com/";
        //private const string ApiKey = "47e9dcb44emsh0d093dc49db704fp123a6ejsn5eedc26ab2cd";
        //private const string ApiKey = "02652bd31amsh1b57728e0461ccap1fb125jsn8ebefb739470";
        //private static string ApiKey = RealtyRapidApiKey;

        // Define a method to get a property by its address

        public async static Task<string> GetPropertyByAddress(string address)
        {
            // Create an HttpClient object and set the headers
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", RealtyRapidApiKey);
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "realty-mole-property-api.p.rapidapi.com");

                // Build the request URL with the address parameter
                var requestUrl = BaseUrl + "properties?address=" + address;

                // Send the request and get the response
                var response = await client.GetAsync(requestUrl);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string into a Property object
                    //var property = JsonConvert.DeserializeObject<List<HouseProperty>>(responseContent);

                    // Return the property object
                    //return property[0];

                    return responseContent;
                }
                else
                {
                    // Throw an exception if the response is not successful
                    //throw new Exception(response.ReasonPhrase);
                    return "";
                }

            }

        }


        public static async Task<CustomersModel> GetAddressDetails(CustomersModel model)
        {
            //string JsonData = "[{\"addressLine1\":\"29022 Crystal Rose Ln\",\"city\":\"Fulshear\",\"state\":\"TX\",\"zipCode\":\"77441\",\"formattedAddress\":\"29022 Crystal Rose Ln, Fulshear, TX 77441\",\"assessorID\":\"8004-01-001-0100-901\",\"county\":\"Fort Bend\",\"legalDescription\":\"THE BROOKS AT CROSS CREEK RANCH SEC 1, BLOCK 1, LOT 10\",\"ownerOccupied\":true,\"squareFootage\":2548,\"subdivision\":\"THE BROOKS AT CROSS CREEK RANCH SEC 1\",\"yearBuilt\":2016,\"bathrooms\":2.5,\"lotSize\":6383,\"propertyType\":\"Single Family\",\"lastSaleDate\":\"2019-02-28T00:00:00.000Z\",\"bedrooms\":3,\"features\":{\"cooling\":true,\"coolingType\":\"Central\",\"exteriorType\":\"Brick Veneer\",\"floorCount\":2,\"garage\":true,\"garageSpaces\":2,\"garageType\":\"Attached\",\"heating\":true,\"heatingType\":\"Central\",\"roofType\":\"Composition Shingle\"},\"taxAssessment\":{\"2021\":{\"value\":294510,\"land\":51410,\"improvements\":243100},\"2022\":{\"value\":323960,\"land\":51410,\"improvements\":272550}},\"propertyTaxes\":{\"2021\":{\"total\":5223}},\"owner\":{\"names\":[\"TAREK GABER\"],\"mailingAddress\":{\"id\":\"29022-Crystal-Rose-Ln,-Fulshear,-TX-77441\",\"addressLine1\":\"29022 Crystal Rose Ln\",\"city\":\"Fulshear\",\"state\":\"TX\",\"zipCode\":\"77441\"}},\"id\":\"29022-Crystal-Rose-Ln,-Fulshear,-TX-77441\",\"longitude\":-95.876308,\"latitude\":29.70583}]";
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string JsonData = await GetPropertyByAddress(model.Address);

                var data = JsonConvert.DeserializeObject<List<dynamic>>(JsonData);
                if (data != null && data.Count > 0)
                {
                    var yearBuilt = data.Select(m => m.yearBuilt).FirstOrDefault();
                    if (yearBuilt != null)
                    {
                        yearBuilt = yearBuilt.ToString();
                        if (yearBuilt != "")
                        {
                            string yearBuilt_ = model.YearBuit = yearBuilt;
                        }
                    }

                    var squareFootage = data.Select(m => m.squareFootage).FirstOrDefault();
                    if (squareFootage != null)
                    {
                        squareFootage = squareFootage.ToString();
                        if (squareFootage != "")
                        {
                            string squareFootage_ = model.Squirefootage = squareFootage;
                        }
                    }

                    var taxAssessments = data.Select(m => m.taxAssessment).FirstOrDefault();
                    if (taxAssessments != null)
                    {
                        taxAssessments = taxAssessments.ToString();
                        if (taxAssessments != "")
                        {
                            var datas = JsonConvert.DeserializeObject<Dictionary<int, YearData>>(taxAssessments);
                            List<YearData> AllDat = new List<YearData>();
                            foreach (KeyValuePair<int, YearData> item in datas)
                            {
                                int Year = item.Key;
                                int EstimateValue = item.Value.Value;
                                AllDat.Add(new YearData { Year = Year, Value = EstimateValue });
                            }
                            if (AllDat.Count > 0)
                            {
                                int MaxYear = AllDat.Max(m => m.Year);
                                int ValueYear = AllDat.Where(y => y.Year == MaxYear).FirstOrDefault().Value;

                                model.YearEstimedValue = MaxYear.ToString();
                                model.EstimedValue = ValueYear.ToString();
                            }
                        }
                    }
                }
            }


            if (model.Id != 0) //if this customer already creation
            {
                UpdateAddressDetailsCustomer(model);
            }

            return model;
        }


        static async void UpdateAddressDetailsCustomer(CustomersModel model)
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    string UserToken = await _service.UserToken();

                    string json = await ORep.PutStrAsync("api/Customers/PutCustomerAddress", model, UserToken);
                }
            }
            catch (Exception ex)
            {
                await App.Current!.MainPage!.DisplayAlert("Error", "Update Address Data : " + ex.Message, "OK");
            }
        }


    }
}

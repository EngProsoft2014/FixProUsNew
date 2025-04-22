using CommunityToolkit.Maui.Alerts;
using FixProUs.Helpers;
using FixProUs.Models;
using Mopups.Services;

namespace FixProUs.Pages.CallPages;

public partial class SearchCustomerPopup : Mopups.Pages.PopupPage
{
    public delegate void CustomDelegte(CustomersModel model);
    public event CustomDelegte DidClose;

    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    public SearchCustomerPopup(IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetCustomers();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

    public static async Task<List<SuggestionAddressModel>> GetPlacesAutocompleteAsync(string search)
    {
        string[] MAddress = new string[3];
        GoogleApi.Entities.Places.AutoComplete.Request.PlacesAutoCompleteRequest request = new GoogleApi.Entities.Places.AutoComplete.Request.PlacesAutoCompleteRequest();

        request.Input = search;

        request.Language = GoogleApi.Entities.Common.Enums.Language.English;

        request.Key = Controls.StartData.Com_MainObj.AddressAutoCompleteKey;
        //request.Key = Device.iOS == "iOS" ? "AIzaSyDY-9LWg_lY41hlxBA2-ngBydMGYaXxKA4" : Controls.StartData.Com_MainObj.AddressAutoCompleteKey;

        var response = await GoogleApi.GooglePlaces.AutoComplete.QueryAsync(request, null);

        List<SuggestionAddressModel> ListsuggestionsAddress = new List<SuggestionAddressModel>();
        SuggestionAddressModel suggestionAddress;

        foreach (var item in response.Predictions)
        {
            var _request2 = new GoogleApi.Entities.Places.Details.Request.PlacesDetailsRequest
            {
                Key = request.Key,
                PlaceId = item.PlaceId.ToString(),
                //Region = "EG",
                Language = GoogleApi.Entities.Common.Enums.Language.English
            };

            var _response2 = await GoogleApi.GooglePlaces.Details.QueryAsync(_request2);

            string[] ArrAdd = new string[7];
            //int i = 0;
            foreach (var itemAdd in _response2.Result.AddressComponents)
            {
                switch (itemAdd.Types.FirstOrDefault())
                {
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Street_Number:
                        {
                            ArrAdd[0] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Route:
                        {
                            ArrAdd[1] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Administrative_Area_Level_3:
                        {
                            ArrAdd[2] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Administrative_Area_Level_2:
                        {
                            ArrAdd[3] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Administrative_Area_Level_1:
                        {
                            ArrAdd[4] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Country:
                        {
                            ArrAdd[5] = itemAdd.LongName;
                        }
                        break;
                    case GoogleApi.Entities.Common.Enums.AddressComponentType.Postal_Code:
                        {
                            ArrAdd[6] = itemAdd.LongName;
                        }
                        break;
                }
            }

            suggestionAddress = new SuggestionAddressModel
            {
                PalceId = _response2.Result.PlaceId,
                Latitude = _response2.Result.Geometry.Location.Latitude,
                Longitude = _response2.Result.Geometry.Location.Longitude,
                FullAddress = ArrAdd[0] + " " + ArrAdd[1] + " " + ArrAdd[2] + " " + ArrAdd[3] + " " + ArrAdd[4] + " " + ArrAdd[5],
                FullAddressAr = ArrAdd[0] + " " + ArrAdd[1] + " " + ArrAdd[2] + " " + ArrAdd[3] + " " + ArrAdd[4] + " " + ArrAdd[5],
                MainAddress = ArrAdd[0] + " " + ArrAdd[1] + " " + ArrAdd[2],
                SubAddress = ArrAdd[3] + " " + ArrAdd[4],
                Street = ArrAdd[1],
                StreetAr = ArrAdd[1],
                Locality = ArrAdd[2],
                LocalityAr = ArrAdd[2],
                State = ArrAdd[3],
                StateAr = ArrAdd[3],
                City = ArrAdd[4],
                CityAr = ArrAdd[4],
                Country = ArrAdd[5],
                CountryAr = ArrAdd[5],
                Zip = ArrAdd[6],
            };

            ListsuggestionsAddress.Add(suggestionAddress);
        }

        return ListsuggestionsAddress;
    }

    List<CustomersModel> Custs = new List<CustomersModel>();
    public async Task GetCustomers()
    {
        string UserToken = await _service.UserToken();

        var Customers = await ORep.GetAsync<List<CustomersModel>>("api/Customers/GetAllCustInCall?" + "AccountId=" + Helpers.Settings.AccountIdGet, UserToken);

        if (Customers != null)
        {
            Custs = Customers;
        }
    }

    public async Task<List<CustomersModel>> GetCustomersAutocompleteAsync(string search)
    {
        List<CustomersModel> Customers = new List<CustomersModel>();

        Customers = Custs.Where(x => x.Phone1.ToLower().Contains(search.ToLower()) || (x.FirstName.ToLower() + x.LastName.ToLower()).Contains(search.ToLower())).ToList();

        return Customers;
    }

    private async void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        costomersListView.IsVisible = true;
        costomersListView.BeginRefresh();

        try
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    List<CustomersModel> Customers = await GetCustomersAutocompleteAsync(searchBar.Text);

                    if (Customers.Count == 0)
                    {
                        stkApply.IsVisible = true;
                    }
                    else
                    {
                        stkApply.IsVisible = false;
                    }

                    costomersListView.ItemsSource = Customers;
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Exception with autocomplete: " + err.Message + " stack :" + err.StackTrace);
                }
            });
        }
        catch (Exception ex)
        {
            costomersListView.IsVisible = false;

        }
        costomersListView.EndRefresh();
    }

    private async void OnImageNameTapped_CloseIcon(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }


    private async void costomersListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        CustomersModel Cust = e.Item as CustomersModel;

        DidClose?.Invoke(Cust);

        await MopupService.Instance.PopAsync();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        CustomersModel Cust = new CustomersModel();
        long numericvalue;
        bool isNumber = long.TryParse(searchBar.Text, out numericvalue);

        if (searchBar.Text.Length == 10 && isNumber == true)
        {
            Cust.Phone1 = searchBar.Text;

            DidClose?.Invoke(Cust);

            await MopupService.Instance.PopAsync();
        }
        else
        {
            var toast = Toast.Make("Please enter a valid phone number", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
            await toast.Show();
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;


namespace FixProUs.ViewModels
{
    public partial class CreateItemViewModel : BaseViewModel
    {

        readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();

        [ObservableProperty]
        ItemsServicesModel itemDetails = new ItemsServicesModel();

        [ObservableProperty]
        ItemsServicesCategoryModel oneItemsServicesCategory = new ItemsServicesCategoryModel();

        [ObservableProperty]
        ItemsServicesSubCategoryModel oneItemsServicesSubCategory = new ItemsServicesSubCategoryModel();

        [ObservableProperty]
        ItemsServicesTypes oneItemsServicesType = new ItemsServicesTypes();

        [ObservableProperty]
        ObservableCollection<ItemsServicesCategoryModel> lstItemsServicesCategories = new ObservableCollection<ItemsServicesCategoryModel>();

        [ObservableProperty]
        ObservableCollection<ItemsServicesSubCategoryModel> lstItemsServicesSubCategories = new ObservableCollection<ItemsServicesSubCategoryModel>();

        [ObservableProperty]
        ObservableCollection<ItemsServicesTypes> lstItemsServicesTypes;


        Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        public delegate void AddItemDelegte(ItemsServicesModel item);
        public event AddItemDelegte AddItemClose;


        public CreateItemViewModel()
        {
            Init();
        }

        async void Init()
        {
            ItemDetails = new ItemsServicesModel();
            OneItemsServicesCategory = new ItemsServicesCategoryModel();
            OneItemsServicesSubCategory = new ItemsServicesSubCategoryModel();
            LstItemsServicesCategories = new ObservableCollection<ItemsServicesCategoryModel>();
            LstItemsServicesSubCategories = new ObservableCollection<ItemsServicesSubCategoryModel>();
            LstItemsServicesTypes = new ObservableCollection<ItemsServicesTypes>();
            OneItemsServicesType = new ItemsServicesTypes();


            LstItemsServicesTypes.Add(new ItemsServicesTypes() { Id = 1, Name = "Service" });
            LstItemsServicesTypes.Add(new ItemsServicesTypes() { Id = 2, Name = "Item" });
            LstItemsServicesTypes.Add(new ItemsServicesTypes() { Id = 3, Name = "Lebor" });
            LstItemsServicesTypes.Add(new ItemsServicesTypes() { Id = 4, Name = "Other" });

            await GetItemsServicesCategories();
            await GetItemsServicesSubCategories();
        }

        async Task GetItemsServicesCategories()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesCategoryModel>>(string.Format("api/Schedules/GetItemsServicesCategories?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstItemsServicesCategories = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        async Task GetItemsServicesSubCategories()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesSubCategoryModel>>(string.Format("api/Schedules/GetItemsServicesSubCategories?" + "AccountId=" + Helpers.Settings.AccountIdGet), UserToken);

                if (json != null)
                {
                    LstItemsServicesSubCategories = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async void AddNewItemService(ItemsServicesModel item)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : Name.", "Ok");
                }
                else if (OneItemsServicesType == null || OneItemsServicesType.Id == 0)
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : Type.", "Ok");
                }
                else if (OneItemsServicesCategory == null || OneItemsServicesCategory.Id == 0)
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : Category.", "Ok");
                }
                else if (OneItemsServicesSubCategory == null || OneItemsServicesSubCategory.Id == 0)
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : Sub Category.", "Ok");
                }
                else if (item.QTYTime == null || item.QTYTime == 0)
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : QTY.", "Ok");
                }
                else if (item.CostperUnit == null || item.CostperUnit == 0)
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", $"Please Complete This Field Required : Price.", "Ok");
                }
                else
                {
                    if (ItemDetails != null)
                    {
                        string UserToken = await _service.UserToken();

                        ItemDetails.AccountId = int.Parse(Helpers.Settings.AccountIdGet);
                        ItemDetails.BrancheId = int.Parse(Helpers.Settings.BranchIdGet);
                        ItemDetails.CategoryId = OneItemsServicesCategory.Id;
                        ItemDetails.SubCategoryId = OneItemsServicesSubCategory.Id;
                        ItemDetails.Type = OneItemsServicesType.Id;
                        ItemDetails.InventoryItem = OneItemsServicesType.Id == 2 ? true : false;
                        ItemDetails.Active = true;
                        ItemDetails.CreateUser = int.Parse(Helpers.Settings.UserIdGet);
                        ItemDetails.CreateDate = DateTime.Now;

                        UserDialogs.Instance.ShowLoading();
                        var Json = await ORep.PostStrAsync("api/Schedules/PostAddItemService", ItemDetails, UserToken);
                        UserDialogs.Instance.HideHud();

                        if (Json != "")
                        {
                            await App.Current!.MainPage!.DisplayAlert("FixPro", "Succes for Add Item/Service.", "Ok");

                            var ReturnItem = JsonConvert.DeserializeObject<ItemsServicesModel>(Json);
                            AddItemClose.Invoke(ReturnItem);
                        }
                        else
                        {
                            await App.Current!.MainPage!.DisplayAlert("FixPro", "Failed for Add Item/Service.", "Ok");
                        }
                    }
                }
            }

            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }
    }
}

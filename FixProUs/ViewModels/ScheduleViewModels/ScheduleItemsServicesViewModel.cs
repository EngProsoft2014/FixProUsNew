
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using GoogleApi.Entities.Translate.Common.Enums;
using System.Linq;
using FixProUs.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;

namespace FixProUs.ViewModels
{
    public partial class ScheduleItemsServicesViewModel : BaseViewModel
    {

        readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();

        [ObservableProperty]
        ItemsServicesModel itemDetails;

        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstItemCategory;

        [ObservableProperty]
        Object selectedServiceCateory;

        [ObservableProperty]
        EmployeeModel employeePermission;

        [ObservableProperty]
        bool showQty;


        public int AccountIdVM;

        Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        public delegate void ItemDelegte(ItemsServicesModel item);
        public event ItemDelegte ItemClose;

        public ScheduleItemsServicesViewModel()
        {
            GetPerrmission();
        }

        public ScheduleItemsServicesViewModel(bool ShowQTY)
        {
            ShowQty = ShowQTY;

            GetPerrmission();

            AccountIdVM = int.Parse(Helpers.Settings.AccountIdGet);
            LstItemCategory = new ObservableCollection<ItemsServicesModel>();
            ItemDetails = new ItemsServicesModel();

            CheckWayForGetRightData(ShowQty);

        }

        void CheckWayForGetRightData(bool ShowQTY)
        {
            if (ShowQTY == true) // Show All Items And Services in Estimate or Invoice
            {
                if (Controls.StaticMembers.WayAfterChooseCust == 0) //Way from Estimate or Invoice for Schedule
                {
                    //scd system plan  1 all , 2 Service                   
                    if (Helpers.Settings.TypeTrackingSch_InvoGet == "2")
                    {
                        GetFreeServices();
                    }
                    else
                    {
                        GetAllItemsServices();
                    }
                }
                else //Way from Estimate or Invoice for Customer Direct
                {
                    GetAllItemsServices();
                }
            }
            else // show item invintory only in schedule material
            {
                GetItemsInventory();
            }
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

        public async void GetItemsInventory()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesModel>>(string.Format("api/Schedules/GetItemsInventory?" + "AccountId=" + AccountIdVM), UserToken);

                if (json != null)
                {
                    LstItemCategory = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        public async void GetFreeServices()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesModel>>(string.Format("api/Schedules/GetServices?" + "AccountId=" + AccountIdVM), UserToken);

                if (json != null)
                {
                    LstItemCategory = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        public async void GetAllItemsServices()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesModel>>(string.Format("api/Schedules/GetAllItemsServices?" + "AccountId=" + AccountIdVM), UserToken);

                if (json != null)
                {
                    LstItemCategory = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task ApplyItems(ItemsServicesModel model)
        {
            IsEnable = false;

            //model.OneItemService = SelectedServiceCateory as ItemsServicesModel;
            if (model.CostperUnit <= 0 || model.CostperUnit == null)
            {
                await App.Current!.MainPage!.DisplayAlert("Alert", "Please Complete Cost Field.", "Ok");
            }
            else if (model.QTYTime <= 0 || model.QTYTime == null)
            {
                await App.Current!.MainPage!.DisplayAlert("Alert", "Please Complete Qty Field.", "Ok");
            }
            else
            {
                if (model.CategoryId != null)
                {
                    ItemClose.Invoke(model);
                    await App.Current!.MainPage!.Navigation.PopAsync();
                }
                else
                {
                    await App.Current!.MainPage!.DisplayAlert("Alert", "This item don't have category.", "Ok");
                }
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task SelectedItemForGetCost(ItemsServicesModel model)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                if (model != null)
                {
                    string UserToken = await _service.UserToken();
                    var json = await ORep.GetAsync<ItemsServicesModel>(string.Format("api/Schedules/GetServiceCost?" + "AccountId=" + AccountIdVM + "&" + "ServiceId=" + model.Id), UserToken);

                    if (json != null)
                    {
                        ItemDetails = json;
                    }
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task OpenFilterMaterial()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var pageView = new Pages.SchedulePages.FilterMaterials(LstItemCategory);
                pageView.DidClose += (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    ItemDetails = item;

                    UserDialogs.Instance.HideHud();
                };

                await App.Current!.MainPage!.Navigation.PushAsync(pageView);
            }
        }

        [RelayCommand]
        async Task CreateNewItem()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var popupView = new CreateItemViewModel();
                popupView.AddItemClose += async (item) =>
                {
                    UserDialogs.Instance.ShowLoading();

                    if (item != null)
                    {
                        await App.Current!.MainPage!.Navigation.PopAsync();
                        CheckWayForGetRightData(ShowQty);
                    }

                    UserDialogs.Instance.HideHud();
                };
                var page = new Pages.SchedulePages.CreateItemPage();
                page.BindingContext = popupView;
                await App.Current!.MainPage!.Navigation.PushAsync(page);
            }

            IsEnable = true;
        }
    }
}

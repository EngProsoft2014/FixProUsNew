
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace FixProUs.ViewModels
{
    public partial class ScheduleFreeServicesViewModel : BaseViewModel
    {

        readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();

        [ObservableProperty]
        ItemsServicesModel serviceDetails;

        [ObservableProperty]
        ObservableCollection<ItemsServicesModel> lstServiceCategory;

        [ObservableProperty]
        Object selectedServiceCateory;

        [ObservableProperty]
        bool showQty;

        public int AccountIdVM;

        Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        public delegate void ServiceDelegte(ItemsServicesModel item);
        public event ServiceDelegte ServiceClose;


        public ScheduleFreeServicesViewModel()
        {

        }

        public ScheduleFreeServicesViewModel(bool ShowQTY) 
        {
            ShowQty = ShowQTY;

            AccountIdVM = int.Parse(Helpers.Settings.AccountIdGet);
            LstServiceCategory = new ObservableCollection<ItemsServicesModel>();
            ServiceDetails = new ItemsServicesModel();

            GetItemsServices();
        }

        public async void GetItemsServices()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<ItemsServicesModel>>(string.Format("api/Schedules/GetServices?" + "AccountId=" + AccountIdVM), UserToken);

                if (json != null)
                {
                    LstServiceCategory = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        [RelayCommand]
        async Task ApplyService(ItemsServicesModel model)
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
                    ServiceClose.Invoke(model);
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
        async Task SelectedServiceForGetCost(ItemsServicesModel model)
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
                        ServiceDetails = json;
                    }
                }

                UserDialogs.Instance.HideHud();
            }        
        }
    }
}

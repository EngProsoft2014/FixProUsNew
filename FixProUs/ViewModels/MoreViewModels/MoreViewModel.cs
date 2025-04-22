
using System.Collections.ObjectModel;
using FixProUs.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Pages;
using FixProUs.Helpers;
using CommunityToolkit.Maui.Alerts;

namespace FixProUs.ViewModels
{
    public partial class MoreViewModel : BaseViewModel
    {

        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Cons
        public MoreViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {

            ORep = GenericRep;
            _service = service;

        }
        #endregion

        [RelayCommand]
        async Task SelectedEmployeesWorkingPage(string startTracking)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MenuPages.EmployeesWorkingPage(new EmployeesViewModel(startTracking, ORep, _service), ORep, _service));
                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }


        [RelayCommand]
        async Task SelectedAllEmployeesPage()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                Controls.StaticMembers.EmployeesPages = 2;
                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MenuPages.AllEmployeesPage(new EmployeesViewModel(ORep, _service), ORep, _service));

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }


        [RelayCommand]
        async Task SelectedAccountPage()
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                await App.Current!.MainPage!.Navigation.PushAsync(new Pages.MenuPages.AccountPage(new AccountViewModel(ORep,_service),ORep,_service));

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }
    }
}

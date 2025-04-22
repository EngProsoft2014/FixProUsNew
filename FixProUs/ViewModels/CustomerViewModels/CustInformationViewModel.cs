using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixProUs.ViewModels
{
    public partial class CustInformationViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Properties
        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        decimal? discount;
        #endregion

        public CustInformationViewModel(CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            CustomerDetails = new CustomersModel();
            CustomerDetails.LstCustomersCustomField = new List<CustomersCustomFieldModel>();
            Init(model);
        }

        async void Init(CustomersModel model)
        {
            CustomerDetails = model;

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

            await GetInfoForCustomer(model.Id);
        }


        //Get Customer Information
        public async Task GetInfoForCustomer(int? CustomerId)
        {
            UserDialogs.Instance.ShowLoading();

            string UserToken = await _service.UserToken();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var Customer = await ORep.GetAsync<CustomersModel>(string.Format("api/Customers/GetInfoOfCustomer?" + "CustomerId=" + CustomerId), UserToken);

                if (Customer != null)
                {
                    CustomerDetails = Customer;
                    Discount = CustomerDetails.Discount == null ? 0 : CustomerDetails.Discount.Value;
                }
            }

            UserDialogs.Instance.HideHud();
        }

        [RelayCommand]
        async Task UpdateCustomer(CustomersModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.CustomerPages.UpdateCustomerPage(new UpdateCustomerViewModel(model, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }
    }
}

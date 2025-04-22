using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.ViewModels;

namespace FixProUs.Pages.CustomerPages;

public partial class CustomersDetailsPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    CustInformationViewModel custInformationViewModel;
    CustomersModel customersModel;

    public CustomersDetailsPage(CustInformationViewModel vieModel, CustomersModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        customersModel = model;
        InfoView.BindingContext = lblCustTitle.BindingContext = custInformationViewModel = vieModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }

    private void SfTabView_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0)
        {
            InfoView.BindingContext = custInformationViewModel;
        }
        else if (e.NewIndex == 1)
        {
            if (custInformationViewModel.IsEnable == true)
            {
                SchedulesView.BindingContext = new CustSchedulesViewModel(customersModel ,ORep, _service);
            }
            else
            {
                MainTab.SelectedIndex = 0;
            }
        }
        else if (e.NewIndex == 2)
        {
            if (custInformationViewModel.IsEnable == true)
            {
                EstimatesView.BindingContext = new CustEstimatesViewModel(customersModel, ORep, _service);
            }
            else
            {
                MainTab.SelectedIndex = 0;
            }
        }
        else if (e.NewIndex == 3)
        {
            if (custInformationViewModel.IsEnable == true)
            {
                InvoicesView.BindingContext = new CustInvoicesViewModel(customersModel, ORep, _service);
            }
            else
            {
                MainTab.SelectedIndex = 0;
            }
        }

    }
}
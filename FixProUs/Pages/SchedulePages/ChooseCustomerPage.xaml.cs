using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.SchedulePages;

public partial class ChooseCustomerPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    CustomersViewModel customersViewModel;

    public ChooseCustomerPage(CustomersViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();

        ORep = GenericRep;
        _service = service;
        this.BindingContext = customersViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.NewSchedulePage(new AddScheduleViewModel(customersViewModel.CustomerDetails,ORep,_service), ORep,_service));
    }

    private void srchPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        lstCustomers.ItemsSource = customersViewModel.LstCustomers.Where(x => (x.Phone1).Contains(srchPhoneOrAddress.Text) || (x.Address.ToLower()).Contains(srchPhoneOrAddress.Text.ToLower())
        || (x.FirstName.ToLower() + x.LastName.ToLower()).Contains(srchPhoneOrAddress.Text.ToLower()));
    }

}
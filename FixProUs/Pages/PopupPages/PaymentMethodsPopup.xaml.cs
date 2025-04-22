using FixProUs.Helpers;
using FixProUs.ViewModels;
using Mopups.Services;

namespace FixProUs.Pages.PopupPages;

public partial class PaymentMethodsPopup : Mopups.Pages.PopupPage
{

    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion

    CustInvoicesViewModel custInvoicesViewModel;

    public PaymentMethodsPopup(CustInvoicesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = custInvoicesViewModel = model;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
        await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
    }
}
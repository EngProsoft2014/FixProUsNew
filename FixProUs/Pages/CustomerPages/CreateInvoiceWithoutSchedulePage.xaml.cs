using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.CustomerPages;

public partial class CreateInvoiceWithoutSchedulePage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    CustInvoicesViewModel custInvoicesViewModel;

    public CreateInvoiceWithoutSchedulePage(CustInvoicesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = custInvoicesViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
    {
        stkEditDiscount.IsVisible = true;
    }

    private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
    {
        stkEditDiscount.IsVisible = false;
    }

    private void entryDiscount_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue != null && e.NewTextValue != "")
        {
            pnkSave.IsVisible = true;
        }
        else
        {
            pnkSave.IsVisible = false;
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        custInvoicesViewModel.NewInvoTotalInvoice(custInvoicesViewModel.CustomerDetails);
    }
}
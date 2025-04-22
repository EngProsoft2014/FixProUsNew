using CommunityToolkit.Maui.Alerts;
using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.CustomerPages;

public partial class CashOrCreditPaymentPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    PaymentsViewModel paymentsViewModel;

    public CashOrCreditPaymentPage(PaymentsViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = paymentsViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        if (paymentsViewModel != null && paymentsViewModel.OneInvoice.Id == 0)
        {
            await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
        }
        else
        {
            await Navigation.PopAsync();
        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue) != true)
        {
            paymentsViewModel.OnePayment.Amount = decimal.Parse(e.NewTextValue);
            btnPayCredit.Text = string.Format("Pay USD ${0}", e.NewTextValue);
            btnPayCash.Text = string.Format("Pay USD ${0}", e.NewTextValue);
            lblPayCash.Text = e.NewTextValue;
        }
        else
        {
            paymentsViewModel.OnePayment.Amount = paymentsViewModel.OneInvoice.Net;
            btnPayCredit.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
            btnPayCash.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
            lblPayCash.Text = paymentsViewModel.OneInvoice.Net.ToString();
        }
    }

    private void swtPayCredit_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value == true)
        {
            paymentsViewModel.OnePayment.Amount = paymentsViewModel.OneInvoice.Net;
            btnPayCredit.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
            btnPayCash.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
            lblPayCash.Text = paymentsViewModel.OneInvoice.Net.ToString();
        }
        else
        {
            if (string.IsNullOrEmpty(entryNewAmount.Text) != true)
            {
                paymentsViewModel.OnePayment.Amount = decimal.Parse(entryNewAmount.Text);
                btnPayCredit.Text = string.Format("Pay USD ${0}", entryNewAmount.Text);
            }
            else if (string.IsNullOrEmpty(entryCashNewAmount.Text) != true)
            {
                paymentsViewModel.OnePayment.Amount = decimal.Parse(entryCashNewAmount.Text);
                btnPayCash.Text = string.Format("Pay USD ${0}", entryCashNewAmount.Text);
                lblPayCash.Text = entryCashNewAmount.Text;
            }
            else
            {
                paymentsViewModel.OnePayment.Amount = paymentsViewModel.OneInvoice.Net;
                btnPayCredit.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
                btnPayCash.Text = string.Format("Pay USD ${0}", paymentsViewModel.OneInvoice.Net);
                lblPayCash.Text = paymentsViewModel.OneInvoice.Net.ToString();
            }
        }
    }


    //Save Credit
    private void Button_Clicked_Clear_Credit(object sender, EventArgs e)
    {
        DrawBoardCredit.Lines.Clear();
        paymentsViewModel.SignatureImageByte64 = "";
    }

    private async void Button_Clicked_Save_Credit(object sender, EventArgs e)
    {
        var stream = await DrawBoardCredit.GetImageStream(300, 300);
        paymentsViewModel.SignatureImageByte64 = Convert.ToBase64String(Helpers.Utility.ReadToEnd(stream));
        var toast = Toast.Make("Success for save your signature", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
        await toast.Show();
    }

    //Save Cash
    private void Button_Clicked_Clear_Cash(object sender, EventArgs e)
    {
        DrawBoardCash.Lines.Clear();
        paymentsViewModel.SignatureImageByte64 = "";
    }

    private async void Button_Clicked_Save_Cash(object sender, EventArgs e)
    {
        var stream = await DrawBoardCash.GetImageStream(300, 300);
        paymentsViewModel.SignatureImageByte64 = Convert.ToBase64String(Helpers.Utility.ReadToEnd(stream));
        var toast = Toast.Make("Success for save your signature", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
        await toast.Show();
    }


}
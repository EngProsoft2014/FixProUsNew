
using CommunityToolkit.Maui.Alerts;
using FixProUs.Helpers;
using FixProUs.ViewModels;


namespace FixProUs.Pages.CustomerPages;

public partial class EstimateDetailsPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    CustEstimatesViewModel custEstimatesViewModel;

    public EstimateDetailsPage(CustEstimatesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = custEstimatesViewModel = model;
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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    //private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    //{
    //    custEstimatesViewModel.TotalInvoice(custEstimatesViewModel.OneInvoice);
    //}


    private void Button_Clicked_Clear(object sender, EventArgs e)
    {
        DrawBoard.Lines.Clear();
        custEstimatesViewModel.SignatureImageByte64Estimate = "";
    }

    private async void Button_Clicked_Save(object sender, EventArgs e)
    {
        var stream = await DrawBoard.GetImageStream(300, 300);
        custEstimatesViewModel.SignatureImageByte64Estimate = Convert.ToBase64String(Helpers.Utility.ReadToEnd(stream));
        var toast = Toast.Make("Successfully save your signature", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
        await toast.Show();
    }

    private void TapGestureRecognizer_Tapped_Pending(object sender, TappedEventArgs e)
    {
        if (chkPending.IsChecked == false)
        {
            chkPending.IsChecked = true;
            chkDeclind.Color = Color.FromHex("#333");
            chkDeclind.IsChecked = false;
            chkPending.Color = Color.FromHex("#b66dff");
            chkAccept.Color = Color.FromHex("#333");
            chkAccept.IsChecked = false;
            //colRequests.ItemsSource = ViewModel.Responses.Where(a => a.TotalPriceAgentAccept > 0);
        }
        else
        {
            if (chkDeclind.IsChecked == true || chkAccept.IsChecked == true)
            {
                chkPending.IsChecked = false;
            }
            else
            {
                chkPending.IsChecked = true;
            }
        }
    }

    private void TapGestureRecognizer_Tapped_Accept(object sender, TappedEventArgs e)
    {
        if (chkAccept.IsChecked == false)
        {
            chkAccept.IsChecked = true;
            chkDeclind.Color = Color.FromHex("#333");
            chkDeclind.IsChecked = false;
            chkPending.Color = Color.FromHex("#333");
            chkPending.IsChecked = false;
            chkAccept.Color = Color.FromHex("#b66dff");
        }
        else
        {
            if (chkDeclind.IsChecked == true || chkPending.IsChecked == true)
            {
                chkAccept.IsChecked = false;
            }
            else
            {
                chkAccept.IsChecked = true;
            }
        }
    }

    private void TapGestureRecognizer_Tapped_Declind(object sender, TappedEventArgs e)
    {
        if (chkDeclind.IsChecked == false)
        {
            chkDeclind.IsChecked = true;
            chkDeclind.Color = Color.FromHex("#b66dff");
            chkPending.Color = Color.FromHex("#333");
            chkPending.IsChecked = false;
            chkAccept.Color = Color.FromHex("#333");
            chkAccept.IsChecked = false;
        }
        else
        {
            if (chkPending.IsChecked == true || chkAccept.IsChecked == true)
            {
                chkDeclind.IsChecked = false;
            }
            else
            {
                chkDeclind.IsChecked = true;
            }
        }
    }
}
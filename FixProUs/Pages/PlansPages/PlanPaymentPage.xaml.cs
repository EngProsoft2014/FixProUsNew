using FixProUs.Helpers;
using FixProUs.ViewModels;
using Syncfusion.Maui.Core.Carousel;

namespace FixProUs.Pages.PlansPages;

public partial class PlanPaymentPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    PlansViewModel plansViewModel;

    public PlanPaymentPage(PlansViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
        InitializeComponent();

        ORep = GenericRep;
        _service = service;
        this.BindingContext = plansViewModel = model;
        //if (ViewModel != null && ViewModel.IsYearly == true && ViewModel.IsMonthly == false)
        //{
        //    pnkYearlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
        //    pnkMonthlyMethod.BackgroundColor = Color.FromHex("#ffffff");
        //    rdMonthly.IsChecked = false;
        //    rdYearly.IsChecked = true;
        //}
        //else
        //{
        //    pnkYearlyMethod.BackgroundColor = Color.FromHex("#ffffff");
        //    pnkMonthlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
        //    rdMonthly.IsChecked = true;
        //    rdYearly.IsChecked = false;
        //}
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    //private void rdYearly_CheckedChanged(object sender, CheckedChangedEventArgs e)
    //{
    //    if (e.Value)
    //    {
    //        pnkYearlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
    //        pnkMonthlyMethod.BackgroundColor = Color.FromHex("#ffffff");
    //        rdMonthly.IsChecked = false;
    //        plansViewModel.IsMonthly = false;
    //        rdYearly.IsChecked = true;
    //        plansViewModel.IsYearly = true;
    //    }
    //    else
    //    {
    //        pnkYearlyMethod.BackgroundColor = Color.FromHex("#ffffff");
    //        pnkMonthlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
    //        rdMonthly.IsChecked = true;
    //        plansViewModel.IsMonthly = true;
    //        rdYearly.IsChecked = false;
    //        plansViewModel.IsYearly = false;
    //    }

    //}

    //private void rdMonthly_CheckedChanged(object sender, CheckedChangedEventArgs e)
    //{

    //    if (e.Value) 
    //    {
    //        pnkYearlyMethod.BackgroundColor = Color.FromHex("#ffffff");
    //        pnkMonthlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
    //        rdMonthly.IsChecked = true;
    //        plansViewModel.IsMonthly = true;
    //        rdYearly.IsChecked = false;
    //        plansViewModel.IsYearly = false;
    //    }
    //    else
    //    {
    //        pnkYearlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
    //        pnkMonthlyMethod.BackgroundColor = Color.FromHex("#ffffff");
    //        rdMonthly.IsChecked = false;
    //        plansViewModel.IsMonthly = false;
    //        rdYearly.IsChecked = true;
    //        plansViewModel.IsYearly = true;
    //    }

    //}

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        if (rdYearly.IsChecked == false && rdMonthly.IsChecked == true)
        {
            pnkYearlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
            pnkMonthlyMethod.BackgroundColor = Color.FromHex("#ffffff");
            rdMonthly.IsChecked = false;
            plansViewModel.IsMonthly = false;
            rdYearly.IsChecked = true;
            plansViewModel.IsYearly = true;
        }
        else
        {
            pnkYearlyMethod.BackgroundColor = Color.FromHex("#ffffff");
            pnkMonthlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
            rdMonthly.IsChecked = true;
            plansViewModel.IsMonthly = true;
            rdYearly.IsChecked = false;
            plansViewModel.IsYearly = false;
        }
    }

    private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        if (rdYearly.IsChecked == true && rdMonthly.IsChecked == false)
        {
            pnkYearlyMethod.BackgroundColor = Color.FromHex("#ffffff");
            pnkMonthlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
            rdMonthly.IsChecked = true;
            plansViewModel.IsMonthly = true;
            rdYearly.IsChecked = false;
            plansViewModel.IsYearly = false;
        }
        else
        {
            pnkYearlyMethod.BackgroundColor = Color.FromHex("#d4f8ff");
            pnkMonthlyMethod.BackgroundColor = Color.FromHex("#ffffff");
            rdMonthly.IsChecked = false;
            plansViewModel.IsMonthly = false;
            rdYearly.IsChecked = true;
            plansViewModel.IsYearly = true;
        }
    }
}
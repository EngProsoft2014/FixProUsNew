using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.PlansPages;

public partial class ChoosePlanPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    PlansViewModel plansViewModel;

    public ChoosePlanPage(PlansViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = plansViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}
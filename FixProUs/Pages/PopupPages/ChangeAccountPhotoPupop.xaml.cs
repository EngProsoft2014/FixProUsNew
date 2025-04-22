using FixProUs.Helpers;
using FixProUs.ViewModels;
using GoogleApi.Entities.Translate.Common.Enums;
using Mopups.Services;
using Stripe;

namespace FixProUs.Pages.PopupPages;

public partial class ChangeAccountPhotoPupop : Mopups.Pages.PopupPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    AccountViewModel accountViewModel;

    public ChangeAccountPhotoPupop(AccountViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = accountViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}
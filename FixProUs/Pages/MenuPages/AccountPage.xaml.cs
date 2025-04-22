using FixProUs.Helpers;
using FixProUs.ViewModels;
using Mopups.Pages;
using Mopups.Services;

namespace FixProUs.Pages.MenuPages;

public partial class AccountPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    AccountViewModel accountViewModel;

    public AccountPage(AccountViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = accountViewModel = model;
    }

    [Obsolete]
    protected override bool OnBackButtonPressed()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
        });
        return true;
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new PopupPages.ChangeAccountPhotoPupop(new AccountViewModel(ORep,_service),ORep,_service));
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker)!.SelectedItem;
        accountViewModel.SelectBranchCommand.Execute(selectedOption);
    }

    private async void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
    }
}
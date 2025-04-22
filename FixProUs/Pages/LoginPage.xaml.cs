using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages;

public partial class LoginPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    LoginViewModel loginViewModel;

    public LoginPage(LoginViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = loginViewModel = model;
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(() =>
        {
            Action action = () => Application.Current!.Quit();
            Controls.StaticMembers.ShowSnackBar("Do you want to exit the program?", Controls.StaticMembers.SnackBarColor, Controls.StaticMembers.SnackBarTextColor, action);
        });
        return true;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        entPassword.IsPassword = (entPassword.IsPassword == true) ? false : true;
    }
}
using Akavache;
using System.Reactive.Linq;
using FixProUs.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Services.Data;
using FixProUs.Pages;
using OneSignalSDK.DotNet;
using Twilio.Rest.Microvisor.V1;
using Stripe;
using CommunityToolkit.Maui.Alerts;
using FixProUs.Helpers;

namespace FixProUs.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        EmployeeModel loginModel;
        #endregion

        #region Cons
        public LoginViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;

            if (Helpers.Settings.UserIdGet != "0" && Helpers.Settings.UserNameGet != "")
            {
                LoginModel = new EmployeeModel()
                {
                    Id = int.Parse(Helpers.Settings.UserIdGet),
                    UserName = Helpers.Settings.UserNameGet,
                    FirstName = Helpers.Settings.UserFristNameGet,
                    Password = Helpers.Settings.PasswordGet,
                    EmailUserName = Helpers.Settings.EmailGet,
                    Phone1 = Helpers.Settings.PhoneGet,
                    BrancheId = int.Parse(Helpers.Settings.BranchIdGet),
                    BranchName = Helpers.Settings.BranchNameGet,
                    CreateDate = Helpers.Settings.CreateDateGet == "" ? DateTime.Now : DateTime.Parse(Helpers.Settings.CreateDateGet),
                    OneSignalPlayerId = Preferences.Default.Get(Helpers.Settings.PlayerId, ""),
                };
            }
            else
            {
                LoginModel = new EmployeeModel();
            }
        }
        #endregion

        #region RelayCommand
        [RelayCommand]
        async Task ClickLogin(EmployeeModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                LoginModel = model;

                if (string.IsNullOrEmpty(LoginModel.UserName))
                {
                    var toast = Toast.Make("Please Complete This Field Required : User Name.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else if (string.IsNullOrEmpty(LoginModel.Password))
                {
                    var toast = Toast.Make("Please Complete This Field Required : Password.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    var MLogin = await ORep.GetLoginAsync<EmployeeModel>("api/Login/GetLogin?" + "UserName=" + model.UserName + "&" + "Password=" + model.Password + "&" + "PlayerId=" + Preferences.Default.Get(Helpers.Settings.PlayerId, OneSignal.User.PushSubscription.Id));

                    if (MLogin.EmployeeStatus?.Contains("Try Again") == true)//UserName Or Password is Wrong
                    {
                        var toast = Toast.Make("Wrong Username Or Password.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
                    }
                    else if (MLogin.EmployeeStatus?.Contains("Account Is Expired") == true)
                    {
                        Preferences.Default.Set(Helpers.Settings.Email, MLogin.EmailUserName);
                        bool answer = await App.Current!.MainPage!.DisplayAlert("Question?", "Account expired, Do you want to buy a pricing plan?", "Yes", "No");
                        if (answer)
                        {
                            await App.Current!.MainPage!.Navigation.PushAsync(new Pages.PlansPages.ChoosePlanPage(new PlansViewModel(ORep, _service), ORep, _service));
                        }
                    }
                    else
                    {
                        if (MLogin == null || MLogin.Id == 0 || MLogin.ActiveAccount == null)
                        {
                            MLogin = await ORep.GetLoginAsync<EmployeeModel>("api/Login/GetLogin?" + "UserName=" + model.UserName + "&" + "Password=" + model.Password + "&" + "PlayerId=" + Preferences.Default.Get(Helpers.Settings.PlayerId, OneSignal.User.PushSubscription.Id));
                        }

                        if (MLogin.ActiveAccount == true)
                        {
                            if (MLogin.ActiveMobileLogin == true)
                            {

                                Preferences.Default.Set(Helpers.Settings.UserId, MLogin.Id.ToString());
                                Helpers.Settings.UserIdGet = !string.IsNullOrEmpty(MLogin.Id.ToString()) ? MLogin.Id.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.AccountId, MLogin.AccountId.ToString());
                                Helpers.Settings.AccountIdGet = !string.IsNullOrEmpty(MLogin.AccountId.ToString()) ? MLogin.AccountId.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.UserName, MLogin.UserName);
                                Helpers.Settings.UserNameGet = !string.IsNullOrEmpty(MLogin.UserName.ToString()) ? MLogin.UserName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.Password, MLogin.Password);
                                Helpers.Settings.PasswordGet = !string.IsNullOrEmpty(MLogin.Password.ToString()) ? MLogin.Password.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.AccountName, MLogin.AccountName);
                                Helpers.Settings.AccountNameGet = !string.IsNullOrEmpty(MLogin.AccountName.ToString()) ? MLogin.AccountName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.AccountDayExpired, DateTime.Now.ToString());
                                Helpers.Settings.AccountDayExpiredGet = DateTime.Now.ToString();

                                Preferences.Default.Set(Helpers.Settings.AccountNameWithSpace, MLogin.CompanyNameWithSpace.ToString());
                                Helpers.Settings.AccountNameWithSpaceGet = !string.IsNullOrEmpty(MLogin.CompanyNameWithSpace.ToString()) ? MLogin.CompanyNameWithSpace.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.UserFristName, MLogin.FirstName);
                                Helpers.Settings.UserFristNameGet = !string.IsNullOrEmpty(MLogin.FirstName.ToString()) ? MLogin.FirstName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.UserLastName, MLogin.LastName);
                                Helpers.Settings.UserLastNameGet = !string.IsNullOrEmpty(MLogin.LastName.ToString()) ? MLogin.LastName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.Email, MLogin.EmailUserName);
                                Helpers.Settings.EmailGet = !string.IsNullOrEmpty(MLogin.EmailUserName.ToString()) ? MLogin.EmailUserName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.Phone, MLogin.Phone1);
                                Helpers.Settings.PhoneGet = !string.IsNullOrEmpty(MLogin.Phone1.ToString()) ? MLogin.Phone1.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.CreateDate, MLogin.CreateDate.ToString());
                                Helpers.Settings.CreateDateGet = !string.IsNullOrEmpty(MLogin.CreateDate.ToString()) ? MLogin.CreateDate.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.AccountId, MLogin.AccountId.ToString());
                                Helpers.Settings.AccountIdGet = !string.IsNullOrEmpty(MLogin.AccountId.ToString()) ? MLogin.AccountId.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.BranchId, MLogin.BrancheId.ToString());
                                Helpers.Settings.BranchIdGet = !string.IsNullOrEmpty(MLogin.BrancheId.ToString()) ? MLogin.BrancheId.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.BranchName, MLogin.BranchName);
                                Helpers.Settings.BranchNameGet = !string.IsNullOrEmpty(MLogin.BranchName.ToString()) ? MLogin.BranchName.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.UserRole, MLogin.UserRole.ToString());
                                Helpers.Settings.UserRoleGet = !string.IsNullOrEmpty(MLogin.UserRole.ToString()) ? MLogin.UserRole.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.UserEmployees, MLogin.Employees);
                                Helpers.Settings.UserEmployeesGet = !string.IsNullOrEmpty(MLogin.Employees) ? MLogin.Employees.ToString() : "";

                                Preferences.Default.Set(Helpers.Settings.TypeTrackingSch_Invo, MLogin.TypeTrackingSch_Invo.ToString());
                                Helpers.Settings.TypeTrackingSch_InvoGet = !string.IsNullOrEmpty(MLogin.TypeTrackingSch_Invo.ToString()) ? MLogin.TypeTrackingSch_Invo.ToString() : "";

                                Helpers.Utility.ServerUrl = !string.IsNullOrEmpty(MLogin.AccountSubdomainApiURL) ? MLogin.AccountSubdomainApiURL : Helpers.Utility.ServerUrl;

                                await BlobCache.LocalMachine.InsertObject(ServicesService.UserTokenServiceKey, MLogin.GernToken, DateTimeOffset.Now.AddHours(24));

                                Preferences.Default.Set(Helpers.Settings.UserPricture, (Helpers.Utility.PathServerProfileImages + "/" + MLogin.Picture));
                                Helpers.Settings.UserPrictureGet = !string.IsNullOrEmpty(MLogin.Picture) ? (Helpers.Utility.PathServerProfileImages + "/" + MLogin.Picture) : "";

                                await App.Current!.MainPage!.Navigation.PushAsync(new MainPage(new SchedulesViewModel(ORep, _service), ORep, _service));
                            }
                            else
                            {
                                var toast = Toast.Make("Sorry, This Username is not Mobile Access", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                                await toast.Show();
                            }
                        }
                        else
                        {
                            var toast = Toast.Make("Sorry, Your Account is not active", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                            await toast.Show();
                        }
                    }
                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        #endregion
    }
}

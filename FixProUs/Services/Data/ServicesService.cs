using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using FixProUs.Models;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using OneSignalSDK.DotNet;




namespace FixProUs.Services.Data
{
    public interface IServicesService
    {
        Task<ImageSource> AccountPhoto();
        Task<string> UserToken();
    }


    public class ServicesService : IServicesService
    {
        Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        ImageSource Photo;
        string ServiceKey = "ServiceKey";

        EmployeeModel loginModel;
        string MUserToken;
      public  static string UserTokenServiceKey = "UserTokenServiceKey";

        public async Task<ImageSource> AccountPhoto()
        {
            Photo = Controls.StaticMembers.AccountImg;
            await BlobCache.LocalMachine.InsertObject(ServiceKey, Photo, DateTimeOffset.Now.AddYears(1));
            return Photo;
        }


        public async Task<string> UserToken()
        {
            try
            {
                MUserToken = await BlobCache.LocalMachine.GetObject<string>(UserTokenServiceKey);
            }
            catch (Exception)
            {
                MUserToken = null;
            }

            if (MUserToken == null)
            {
                if ((Connectivity.Current.NetworkAccess == NetworkAccess.Internet))
                {
                    if (Helpers.Settings.PhoneGet != "" && Helpers.Settings.PasswordGet != "")
                    {
                        var loginModel = await ORep.GetLoginAsync<EmployeeModel>("api/Login/GetLogin?" + "UserName=" + Helpers.Settings.UserNameGet + "&" + "Password=" + Helpers.Settings.PasswordGet + "&" + "PlayerId=" + Preferences.Default.Get(Helpers.Settings.PlayerId, OneSignal.User.PushSubscription.Id));

                        if (loginModel != null && !string.IsNullOrEmpty(loginModel.GernToken))
                        {
                            MUserToken = loginModel.GernToken;

                            await BlobCache.LocalMachine.InsertObject(UserTokenServiceKey, loginModel.GernToken, DateTimeOffset.Now.AddHours(24));

                            return loginModel.GernToken;
                        }
                    }
                }
                    
            }
            else
            {
                return MUserToken;
            }

            return MUserToken;
        }
    }
}

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using FixProUs.Pages;
using FixProUs.Platforms.Android;

namespace FixProUs
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {
        Intent serviceIntent;
        private const int RequestCode = 5469;

        protected async override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);

            this.Window?.AddFlags(WindowManagerFlags.Fullscreen);

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            });

            int requestPermissions = 1;

            //try
            //{
            //    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //    {
            //        ////await App.Current!.MainPage!.DisplayAlert("Error", "No Internet Avialable !!!", "OK");

            //        await App.Current!.MainPage!.Navigation.PushAsync(new NoInternetPage(new MainPage()));
            //        return;
            //    }
            //    else
            //    {

            //        var statusLocation = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            //        if (statusLocation == PermissionStatus.Granted)
            //        {
            //            StartService(new Intent(this, typeof(AndroidLocationService)));
            //            serviceIntent = new Intent(this, typeof(AndroidLocationService));
            //            SetServiceMethods();
            //        }


            //        //StartService(new Intent(this, typeof(AndroidLocationService)));
            //        //serviceIntent = new Intent(this, typeof(AndroidLocationService));
            //        //SetServiceMethods();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await App.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
            //}
        }

        //void SetServiceMethods()
        //{
        //    MessagingCenter.Subscribe<StartServiceMessage>(this, "ServiceStarted", message =>
        //    {
        //        if (!IsServiceRunning(typeof(AndroidLocationService)))
        //        {
        //            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
        //            {
        //                StartForegroundService(serviceIntent);
        //            }
        //            else
        //            {
        //                StartService(serviceIntent);
        //            }
        //        }
        //    });

        //    MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message =>
        //    {
        //        if (IsServiceRunning(typeof(AndroidLocationService)))
        //            StopService(serviceIntent);
        //    });
        //}

        //private bool IsServiceRunning(System.Type cls)
        //{
        //    ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);
        //    foreach (var service in manager.GetRunningServices(int.MaxValue))
        //    {
        //        if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

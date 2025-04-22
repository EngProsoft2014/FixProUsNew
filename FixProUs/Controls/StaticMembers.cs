using Akavache;
using CommunityToolkit.Maui.Core;
using FFImageLoading.Work;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages;
using FixProUs.Services.Data;
using FixProUs.ViewModels;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Twilio.Rest.Microvisor.V1;


namespace FixProUs.Controls
{
    public class StaticMembers
    {
        public static DateTime SelectedDate { get; set; } = DateTime.Now;
        public static string StaticDate { get; set; }
        public static int EmployeesPages { get; set; }
        public static int WayAfterChooseCust { get; set; } = 0; //Create New Schedule (NewSchedulePage)
        public static int WayCreateCust { get; set; } = 0; //Create New Cust from CustomerPage // 1= Create New Cust from CallPage
        public static int ScheduleIdStatic { get; set; } = 0;
        public static int ScheduleDateIdStatic { get; set; } = 0;
        public static string OldProfileImageSt { get; set; }
        public static int PayCashOrCredit { get; set; }
        public static ImageSource AccountImg { get; set; }
        public static int CreateOrDetailsCall { get; set; }
        public static CallModel FilterCallModel { get; set; }
        public static int YesOrNoInternet { get; set; } = 0;

        public static int TabSelected { get; set; } = 0;

        public static string SnackBarColor = "#b66dff";
        public static string SnackBarTextColor = "#FFFFFF";

        static readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();
        static Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        public async static Task ClearAllData()
        {
            await App.Current!.MainPage!.DisplayAlert("Warning", "Service is currently down. Please try again later.", "OK");

            Preferences.Default.Clear();
            await BlobCache.LocalMachine.InvalidateAll();
            await BlobCache.LocalMachine.Vacuum();
            await Application.Current!.MainPage!.Navigation.PushAsync(new LoginPage(new LoginViewModel(ORep, _service), ORep, _service));
        }


        [Obsolete]
        public static async void ShowSnackBar(string Message, string BKColor, string TextColor, Action action1)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {

                BackgroundColor = Color.FromHex(BKColor),
                TextColor = Color.FromHex(TextColor),
                ActionButtonTextColor = Color.FromHex(TextColor),
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),

            };
            string text = Message;
            string actionButtonText = "OK";
            Action action = action1;
            TimeSpan duration = TimeSpan.FromSeconds(3);

            var snackbar = CommunityToolkit.Maui.Alerts.Snackbar.Make(text, action, actionButtonText, duration, snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static string CheckStringType(string input)
        {
            // Email pattern
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (Regex.IsMatch(input, emailPattern))
            {
                return "Email";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}

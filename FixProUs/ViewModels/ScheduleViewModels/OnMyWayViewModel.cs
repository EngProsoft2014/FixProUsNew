using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FixProUs.Models;
using Mopups.Services;
using Syncfusion.Maui.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixProUs.ViewModels
{
    public partial class OnMyWayViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<TimeModel> lstTimes;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        TimeModel timeDetails;

        [ObservableProperty]
        string myWayTextMsg;


        public OnMyWayViewModel()
        {
            Init();
        }

        public OnMyWayViewModel(CustomersModel model)
        {
            Init();

            CustomerDetails = model;
        }

        void Init()
        {
            TimeDetails = new TimeModel();
            LstTimes = new ObservableCollection<TimeModel>();

            LstTimes.Add(new TimeModel() { Time = "5", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });
            LstTimes.Add(new TimeModel() { Time = "10", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });
            LstTimes.Add(new TimeModel() { Time = "15", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });
            LstTimes.Add(new TimeModel() { Time = "30", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });
            LstTimes.Add(new TimeModel() { Time = "45", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });
            LstTimes.Add(new TimeModel() { Time = "60", BackgroundColor = "#ffffff", TextColor = "#0098BC", IsChecked = false, });

            MyWayTextMsg = $"Hello! This is {Helpers.Settings.AccountNameWithSpaceGet}. We wil arrive in approximately (0) minutes.";

        }

        [RelayCommand]
        void SelectTime(TimeModel model)
        {
            Init();
            LstTimes.Where(t => t.Time == model.Time).ForEach(s => { s.BackgroundColor = "#0098BC"; s.TextColor = "#ffffff"; s.IsChecked = true; });

            MyWayTextMsg = $"Hello! This is {Helpers.Settings.AccountNameWithSpaceGet}. We wil arrive in approximately ({model.Time}) minutes.";
        }

        [RelayCommand]
        async Task SendMyWayTextMsg(string text)
        {
            string returnMsg = Controls.StartData.SendSMS(CustomerDetails.Phone1, text);
            await MopupService.Instance.PopAsync();
            if (string.IsNullOrEmpty(returnMsg))
            {             
                await App.Current!.MainPage!.DisplayAlert("Alert", "Failed to send SMS to customer!", "Ok");     
            }
            else
            {
                await App.Current!.MainPage!.DisplayAlert("Alert", "Succes to send SMS to customer!", "Ok");
            }
        }
    }
}

using CommunityToolkit.Maui.Alerts;
using FixProUs.Model;
using Mopups.Services;
using Syncfusion.Maui.Calendar;

namespace FixProUs.Pages.PopupPages;

public partial class DatePopup : Mopups.Pages.PopupPage
{
    public delegate void RangeDelegte(CalendarModel calendar);
    public event RangeDelegte RangeClose;

    CalendarModel Cal;
    public DatePopup()
    {
        InitializeComponent();

        if (Controls.StaticMembers.SelectedDate != null)
        {
            calendar.SelectedDate = Controls.StaticMembers.SelectedDate;
        }

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

    private async void btnOk_Clicked(object sender, EventArgs e)
    {
        Cal = new CalendarModel();

        if (calendar.SelectedDateRange != null)
        {
            Cal.SelectedRange = (CalendarDateRange)calendar.SelectedDateRange;

            Controls.StaticMembers.SelectedDate = Cal.SelectedRange.StartDate.Value;

            Cal.StartDate = Cal.SelectedRange.StartDate;
            Cal.EndDate = Cal.SelectedRange.EndDate;

            RangeClose?.Invoke(Cal);

        }
        else if (calendar.SelectedDate != null)
        {
            Cal.StartDate = Cal.EndDate = calendar.SelectedDate.Value;

            Controls.StaticMembers.SelectedDate = calendar.SelectedDate.Value;

            RangeClose?.Invoke(Cal);
        }
        else
        {
            var toast = Toast.Make("Please select a date.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
            await toast.Show();
        }

        await MopupService.Instance.PopAsync();

    }
}
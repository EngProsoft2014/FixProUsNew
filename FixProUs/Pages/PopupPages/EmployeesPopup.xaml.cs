using CommunityToolkit.Maui.Alerts;
using FixProUs.Models;
using FixProUs.ViewModels;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace FixProUs.Pages.PopupPages;

public partial class EmployeesPopup : Mopups.Pages.PopupPage
{
    public delegate void EmployeesDelegte(List<EmployeeModel> Employees);
    public event EmployeesDelegte EmployeesClose;

    AddScheduleViewModel addScheduleViewModel;
    ScheduleDetailsViewModel scheduleDetailsViewModel;

    public EmployeesPopup()
	{
		InitializeComponent();
	}

    public EmployeesPopup(ObservableCollection<EmployeeModel> LstEmps, AddScheduleViewModel model)
    {
        InitializeComponent();
        addScheduleViewModel = model;
        lstEmployees.ItemsSource = addScheduleViewModel.LstEmpInOneCategory = LstEmps;
    }

    public EmployeesPopup(ObservableCollection<EmployeeModel> LstEmps, ScheduleDetailsViewModel model)
    {
        InitializeComponent();
        scheduleDetailsViewModel = model;
        lstEmployees.ItemsSource = scheduleDetailsViewModel.LstEmpInOneCategory = LstEmps;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        this.IsEnabled = false;
        List<EmployeeModel> LstEmps = new List<EmployeeModel>();
        LstEmps = addScheduleViewModel.LstEmpInOneCategory.Where(x => x.IsChecked == true).ToList();

        if (LstEmps != null)
        {
            EmployeesClose.Invoke(LstEmps);
        }
        else
        {
            var toast = Toast.Make("Please Choose Empolyee !!", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
            await toast.Show();
        }
        
        await MopupService.Instance.PopAsync();
        this.IsEnabled = true;
    }
}
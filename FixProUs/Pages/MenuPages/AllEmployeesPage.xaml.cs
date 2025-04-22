using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.MenuPages;

public partial class AllEmployeesPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    EmployeesViewModel employeesViewModel;

    public AllEmployeesPage(EmployeesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = employeesViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void srcBarEmployee_TextChanged(object sender, TextChangedEventArgs e)
    {
        lstEmployees.ItemsSource = employeesViewModel.LstEmployees.Where(x => (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(srcBarEmployee.Text.ToLower()));
    }

    private async void lstEmployees_QueryItemSize(object sender, Syncfusion.Maui.ListView.QueryItemSizeEventArgs e)
    {
        if (employeesViewModel.LstEmployees.Count == 0)
            return;

        //hit bottom!
        if (e.ItemIndex == employeesViewModel.LstEmployees.Count - 1)
        {
            if (employeesViewModel.PageNumber <= employeesViewModel.TotalPage)
            {
                await employeesViewModel.GetEmployees();
            }
        }
    }

}
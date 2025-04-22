using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.MenuPages;

public partial class EmployeesWorkingPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    EmployeesViewModel employeesViewModel;

    public EmployeesWorkingPage(EmployeesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = employeesViewModel = model;
    }

    private void srcBarEmployee_TextChanged(object sender, TextChangedEventArgs e)
    {
        lstEmployees.ItemsSource = employeesViewModel.LstWorkingEmployees.Where(x => (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(srcBarEmployee.Text.ToLower()));
    }

}
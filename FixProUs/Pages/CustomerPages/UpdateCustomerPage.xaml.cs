using FixProUs.Helpers;
using FixProUs.ViewModels;

namespace FixProUs.Pages.CustomerPages;

public partial class UpdateCustomerPage : Controls.CustomsPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    UpdateCustomerViewModel updateCustomerViewModel;

    public UpdateCustomerPage(UpdateCustomerViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
	{
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = updateCustomerViewModel = model;
    }

    private void rdBtnYes_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            updateCustomerViewModel.CustomerDetails.Discount = null;
            stkMember.IsVisible = true;
            stkDiscount.IsVisible = false;
        }
        else
        {
            updateCustomerViewModel.CustomerDetails.MemberDTO = null;
            stkMember.IsVisible = false;
            stkDiscount.IsVisible = true;
        }
    }

    private void rdBtnNo_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            updateCustomerViewModel.CustomerDetails.MemberDTO = null;
            stkMember.IsVisible = false;
            stkDiscount.IsVisible = true;
        }
        else
        {
            updateCustomerViewModel.CustomerDetails.Discount = null;
            stkMember.IsVisible = true;
            stkDiscount.IsVisible = false;
        }
    }

    private void swtTaxable_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value == true)
        {
            stkTexable.IsVisible = true;
        }
        else
        {
            stkTexable.IsVisible = false;
        }
    }

    private void Picker_SelectedIndexChanged_1(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        updateCustomerViewModel?.ChooseCustomerCategoryCommand.Execute(selectedOption);
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        updateCustomerViewModel?.ChooseCustomerMemberShipCommand.Execute(selectedOption);
    }

    private void Picker_SelectedIndexChanged_2(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        updateCustomerViewModel?.ChooseCustomerTaxCommand.Execute(selectedOption);
    }

    private void Picker_SelectedIndexChanged_3(object sender, EventArgs e)
    {
        var selectedOption = (sender as Picker).SelectedItem;
        updateCustomerViewModel?.ChooseCustomerCampaignCommand.Execute(selectedOption);
    }

    //Address
    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        var selectedOption = (sender as Entry).Text;
        updateCustomerViewModel?.SelecteAddressCommand.Execute(selectedOption);
    }

}
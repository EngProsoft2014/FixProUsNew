using FixProUs.Models;
using FixProUs.ViewModels;

namespace FixProUs.Pages.SchedulePages;

public partial class NewItemsServicesSchedulePage : Controls.CustomsPage
{
    ScheduleItemsServicesViewModel ViewModel { get => BindingContext as ScheduleItemsServicesViewModel; set => BindingContext = value; }


    public NewItemsServicesSchedulePage()
    {
        InitializeComponent();
    }

    public NewItemsServicesSchedulePage(ScheduleItemsServicesModel model)
    {
        InitializeComponent();

        //cobxLstItems.Text = model.ItemsServicesName;

        entryName.Text = model.ItemsServicesName;
        entryCost.Text = model.CostRate.ToString();
        entryQty.Text = model.Quantity.ToString();
        edtDescription.Text = model.ItemServiceDescription;

        //cobxLstItems.IsEnabled = false;
        entryName.IsReadOnly = true;
        entryCost.IsReadOnly = true;
        entryQty.IsReadOnly = true;
        edtDescription.IsReadOnly = true;
        stkBtns.IsVisible = false;
        YumAddItemService.IsVisible = false;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await App.Current!.MainPage!.Navigation.PopAsync();
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        var selectedOption = (sender as Entry).Text;
        ViewModel?.OpenFilterMaterialCommand.Execute(selectedOption);
    }
}
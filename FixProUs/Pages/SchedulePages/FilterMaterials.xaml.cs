using FixProUs.Models;
using System.Collections.ObjectModel;

namespace FixProUs.Pages.SchedulePages;

public partial class FilterMaterials : Controls.CustomsPage
{
    public delegate void CustomDelegte(ItemsServicesModel item);
    public event CustomDelegte DidClose;

    //ScheduleItemsServicesViewModel ViewModel { get => BindingContext as ScheduleItemsServicesViewModel; set => BindingContext = value; }

    public ObservableCollection<ItemsServicesModel> ItemsList { get; set; } = new ObservableCollection<ItemsServicesModel>();

    public FilterMaterials()
    {
        InitializeComponent();
    }

    public FilterMaterials(ObservableCollection<ItemsServicesModel> LstItems)
    {
        InitializeComponent();
        lstMaterials.ItemsSource = ItemsList = LstItems;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void lstMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        DidClose?.Invoke(e.Item as ItemsServicesModel);

        await Navigation.PopAsync();
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        lstMaterials.BeginRefresh();

        try
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    lstMaterials.ItemsSource = ItemsList.Where(x => (x.Name.ToLower()).Contains(searchBar.Text.ToLower()));
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Exception with autocomplete: " + err.Message + " stack :" + err.StackTrace);
                }
            });
        }
        catch (Exception ex)
        {
            lstMaterials.IsVisible = false;

        }
        lstMaterials.EndRefresh();
    }
}
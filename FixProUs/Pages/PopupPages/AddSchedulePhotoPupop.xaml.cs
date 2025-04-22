using FixProUs.Helpers;
using FixProUs.ViewModels;
using Mopups.Services;

namespace FixProUs.Pages.PopupPages;

public partial class AddSchedulePhotoPupop : Mopups.Pages.PopupPage
{
    #region Service
    readonly IGenericRepository ORep;
    readonly Services.Data.ServicesService _service;
    #endregion
    SchImagesViewModel schImagesViewModel;

    public AddSchedulePhotoPupop(SchImagesViewModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
    {
		InitializeComponent();
        ORep = GenericRep;
        _service = service;
        this.BindingContext = schImagesViewModel = model;
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}
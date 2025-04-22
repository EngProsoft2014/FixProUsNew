
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Models;
using Microsoft.Maui.Graphics.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace FixProUs.ViewModels
{
    public partial class ScheduleMaterialReceiptViewModel : BaseViewModel
    {

        readonly Services.Data.ServicesService _service = new Services.Data.ServicesService();


        [ObservableProperty]
        ScheduleMaterialReceiptModel materialReceipt;

        [ObservableProperty]
        ObservableCollection<CustomersModel> lstSuppliers;

        [ObservableProperty]
        CustomersModel oneSupplier;

        [ObservableProperty]
        ImageSource receiptImage;

        public int AccountIdVM;

        Helpers.GenericRepository ORep = new Helpers.GenericRepository();

        public delegate void MaterialRcDelegte(ScheduleMaterialReceiptModel MaterialRc);
        public event MaterialRcDelegte MaterialRcClose;

        public ScheduleMaterialReceiptViewModel()
        {

            AccountIdVM = int.Parse(Helpers.Settings.AccountIdGet);
            MaterialReceipt = new ScheduleMaterialReceiptModel();
            LstSuppliers = new ObservableCollection<CustomersModel>();
            OneSupplier = new CustomersModel();

            ReceiptImage = "photodef.png";

            GetSuppliers();
        }

        public ScheduleMaterialReceiptViewModel(ScheduleMaterialReceiptModel model)
        {

            AccountIdVM = int.Parse(Helpers.Settings.AccountIdGet);
            MaterialReceipt = new ScheduleMaterialReceiptModel();
            LstSuppliers = new ObservableCollection<CustomersModel>();
            OneSupplier = new CustomersModel();
            ReceiptImage = "photodef.png";
            Init(model);
        }
        async void Init(ScheduleMaterialReceiptModel model)
        {
            await GetSuppliers();
            OneSupplier = LstSuppliers.FirstOrDefault(a => a.Id == model.SupplierId.Value);
        }
        public async Task GetSuppliers()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<CustomersModel>>(string.Format("api/Customers/GetAllCustSuppliers?" + "AccountId=" + AccountIdVM), UserToken);

                if (json != null)
                {
                    LstSuppliers = json;
                }

                UserDialogs.Instance.HideHud();
            }                
        }

        [RelayCommand]
        async Task Apply(ScheduleMaterialReceiptModel model)
        {
            IsEnable = false;

            if (model != null)
            {
                model.SupplierId = OneSupplier.Id;
                model.SupplierName = OneSupplier.FullName;
                MaterialRcClose.Invoke(model);
            }
            else
            {
                await App.Current!.MainPage!.DisplayAlert("Alert", "Please Complete All Fields.", "Ok");
            }
            await App.Current!.MainPage!.Navigation.PopAsync();

            IsEnable = true;
        }

        [RelayCommand]
        void SelectSupplier(CustomersModel model)
        {
            IsEnable = false;
            OneSupplier = model;
            IsEnable = true;
        }

        //Pick Photo
        [RelayCommand]
        private async Task SelectePickSchedulePhoto()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    // Permissions are not granted, request permissions from the user
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                    if (status != PermissionStatus.Granted)
                    {
                        // Permissions are denied, show a message to the user
                        await App.Current!.MainPage!.DisplayAlert("Permission Denied", "You need to grant photo library permission to use this feature.", "OK");
                    }
                }
                else
                {
                    // Open the photo gallery
                    var photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null)
                    {
                        using var stream = await photo.OpenReadAsync();
                        using var memoryStream = new MemoryStream();

                        // Load the image into SkiaSharp and resize it
                        using var originalBitmap = SKBitmap.Decode(stream);
                        var resizedBitmap = originalBitmap.Resize(new SKImageInfo(800, 600), SKFilterQuality.Medium);

                        using var image = SKImage.FromBitmap(resizedBitmap);
                        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 75); // Compression level: 75%
                        data.SaveTo(memoryStream);

                        // Display the selected photo in the Image control
                        ReceiptImage = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                        MaterialReceipt.ReceiptPhoto = Convert.ToBase64String(memoryStream.ToArray()); 
                    }             
                }

            }
            catch (Exception)
            {
                await App.Current!.MainPage!.DisplayAlert("No Camera", ":( No camera available.", "Ok");
            }

        }

        //Camera Photo
        [RelayCommand]
        private async Task SelecteCamSchedulePhoto()
        {

            try
            {
                // Check if camera permission is granted
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    // If permission is not granted, request permission from the user
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        // Permission denied by user, show a message or take action accordingly
                        await App.Current!.MainPage!.DisplayAlert("Permission Denied", "You need to grant camera permission to use this feature.", "OK");
                    }
                }
                else
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {


                        // Capture the photo
                        var photo = await MediaPicker.Default.CapturePhotoAsync();

                        if (photo != null)
                        {
                            using var stream = await photo.OpenReadAsync();
                            using var memoryStream = new MemoryStream();

                            // Load the image into SkiaSharp and resize it
                            using var originalBitmap = SKBitmap.Decode(stream);
                            var resizedBitmap = originalBitmap.Resize(new SKImageInfo(800, 600), SKFilterQuality.Medium);

                            using var image = SKImage.FromBitmap(resizedBitmap);
                            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 75); // Compression level: 75%
                            data.SaveTo(memoryStream);

                            // Display the image
                            ReceiptImage = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                            MaterialReceipt.ReceiptPhoto = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }

                }

            }
            catch (Exception)
            {
                await App.Current!.MainPage!.DisplayAlert("No Camera", ":( No camera available.", "Ok");
            }
        }

       
    }
}

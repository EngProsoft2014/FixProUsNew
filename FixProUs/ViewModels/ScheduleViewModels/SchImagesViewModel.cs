using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.UserDialogs.Maui;
using FixProUs.Helpers;
using FixProUs.Models;
using FixProUs.Pages;
using Mopups.Services;
using OneSignalSDK.DotNet.Core.Internal.Utilities;
using SkiaSharp;
using System.Collections.ObjectModel;


namespace FixProUs.ViewModels
{
    public partial class SchImagesViewModel : BaseViewModel
    {
        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        [ObservableProperty]
        SchedulePicturesModel onePictureModel;

        [ObservableProperty]
        ObservableCollection<SchedulePicturesModel> lstAllPictures;

        [ObservableProperty]
        ObservableCollection<SchedulePicturesModel> lstNewPictures;

        [ObservableProperty]
        SchedulesModel scheduleDetails;

        [ObservableProperty]
        CustomersModel customerDetails;

        [ObservableProperty]
        bool doneFlag;

        [ObservableProperty]
        ImageSource schedulePhoto;


        public SchImagesViewModel(SchedulesModel model, IGenericRepository GenericRep, Services.Data.ServicesService service)
        { 
            ORep = GenericRep;
            _service = service;
            Init();
            InitData(model);

            MessagingCenter.Subscribe<SchImagesViewModel, ObservableCollection<SchedulePicturesModel>>(this, "ChangeSchImagesInSchedulePicturesPage", (sender, message) =>
            {
                if (message.Count > 0 && message.Count != LstAllPictures.Count)
                {
                    if (model.LstSchedulePictures.Count != 0)
                    {
                        LstAllPictures = message;
                        LstNewPictures = new ObservableCollection<SchedulePicturesModel>(message.Where(x => x.Id == 0).ToList());
                    }
                }
            });
        }

        void Init()
        {
            CustomerDetails = new CustomersModel();
            ScheduleDetails = new SchedulesModel();
            OnePictureModel = new SchedulePicturesModel();
            ScheduleDetails.LstScheduleItemsServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstFreeServices = new List<ScheduleItemsServicesModel>();
            ScheduleDetails.LstScheduleEmployeeDTO = new List<ScheduleEmployeesModel>();
            CustomerDetails.LstCustItemsServices = new List<ScheduleItemsServicesModel>();
            LstAllPictures = new ObservableCollection<SchedulePicturesModel>();
            LstNewPictures = new ObservableCollection<SchedulePicturesModel>();
        }

        async void InitData(SchedulesModel model)
        {
            ScheduleDetails = model;
            CustomerDetails = model.CustomerDTO;

            //Schedule Pictures
            if (model.LstSchedulePictures.Count > 0)
            {
                LstAllPictures = new ObservableCollection<SchedulePicturesModel>(model.LstSchedulePictures);
                LstNewPictures = new ObservableCollection<SchedulePicturesModel>(model.LstSchedulePictures.Where(x => x.Id == 0).ToList());
            }

            if (model.GetPictures == true)
            {
                GetPictuers(model.Id);
            }
        }


        //Get Pictuers
        async void GetPictuers(int ScheduleId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<SchedulePicturesModel>>(string.Format("api/Schedules/GetPictures?" + "ScheduleId=" + ScheduleId), UserToken);

                if (json != null)
                {
                    LstNewPictures = new ObservableCollection<SchedulePicturesModel>(); //Check if Show Button Done
                    ScheduleDetails.LstSchedulePictures = json.ToList();
                    LstAllPictures = json;
                }

                UserDialogs.Instance.HideHud();
            }
        }

        async Task UploadPictures(List<SchedulePicturesModel> LstPhotos)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();

                List<SchedulePicturesModel> LstFinalPhotos = LstPhotos.Where(x => x.Id == 0).ToList(); // if Id = 0 (Photo New)

                foreach (var source in LstFinalPhotos)
                {
                    if (source.PictureSource != null)
                        source.PictureSource = null;
                }
                //string Postjson = await Helpers.Utility.PostData("api/ImageMobile/ReplacePostOneImagesScheduleMobile", JsonConvert.SerializeObject(LstFinalPhotos, Formatting.None,
                //            new JsonSerializerSettings()
                //            {
                //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //            }));

                string UserToken = await _service.UserToken();
                string Postjson = await ORep.PostMultiPicAsync("api/ImageMobile/ReplacePostOneImagesScheduleMobile", LstFinalPhotos, UserToken);

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenAddImagesPopup(SchedulesModel model)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await MopupService.Instance.PushAsync(new Pages.PopupPages.AddSchedulePhotoPupop(new SchImagesViewModel(model, ORep, _service), ORep, _service));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenFullScreenSchImage(string ImageName)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new FullScreenImagePage(ImageName));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenFullScreenSchImageBeforeInsert(ImageSource ImageName)
        {
            IsEnable = false;
            UserDialogs.Instance.ShowLoading();
            await App.Current!.MainPage!.Navigation.PushAsync(new FullScreenImagePage(ImageName));
            UserDialogs.Instance.HideHud();
            IsEnable = true;
        }

        //Pick Photo
        [RelayCommand]
        private async Task SelectePickSchedulePhoto()
        {
            await MopupService.Instance.PopAsync();

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
                        var toast = Toast.Make("You need to grant photo library permission to use this feature.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
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

                        memoryStream.Position = 0;

                        var imageBytes = memoryStream.ToArray();

                        SchedulePhoto = ImageSource.FromStream(() =>
                        {
                            // Return a new MemoryStream each time, so it remains accessible across different UI contexts
                            return new MemoryStream(imageBytes);
                        });

                        OnePictureModel = new SchedulePicturesModel
                        {
                            AccountId = ScheduleDetails.AccountId,
                            BrancheId = ScheduleDetails.BrancheId,
                            ScheduleId = ScheduleDetails.Id,
                            FileName = Convert.ToBase64String(memoryStream.ToArray()),
                            Active = true,
                            ShowToCust = true,
                            CreateUser = ScheduleDetails.CreateUser,
                            CreateDate = DateTime.Now,
                            ScheduleDateId = ScheduleDetails.OneScheduleDate.Id,
                            PictureSource = SchedulePhoto,
                            Flag = 0, // new photo
                        };

                        LstNewPictures.Add(OnePictureModel);
                        LstAllPictures.Add(OnePictureModel);
                        ScheduleDetails.LstSchedulePictures.Add(OnePictureModel);
                        ScheduleDetails.GetPictures = false; //Don't entrance GetPictures Method

                        UserDialogs.Instance.ShowLoading();
                        //await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.SchedulePicturesPage(new SchImagesViewModel(ScheduleDetails, ORep, _service), ORep, _service));
                        //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        MessagingCenter.Send(this, "ChangeSchImagesInSchedulePicturesPage", LstAllPictures);

                        DoneFlag = true;

                        UserDialogs.Instance.HideHud();
                    }
                }

            }
            catch (Exception)
            {
                var toast = Toast.Make("Warning : No Storage available.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }
        }

        //Camera Photo
        [RelayCommand]
        private async Task SelecteCamSchedulePhoto()
        {
            await MopupService.Instance.PopAsync();
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
                        var toast = Toast.Make("You need to grant camera permission to use this feature.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                        await toast.Show();
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

                            memoryStream.Position = 0;

                            var imageBytes = memoryStream.ToArray();

                            SchedulePhoto = ImageSource.FromStream(() =>
                            {
                                // Return a new MemoryStream each time, so it remains accessible across different UI contexts
                                return new MemoryStream(imageBytes);
                            });

                            OnePictureModel = new SchedulePicturesModel
                            {
                                AccountId = ScheduleDetails.AccountId,
                                BrancheId = ScheduleDetails.BrancheId,
                                ScheduleId = ScheduleDetails.Id,
                                FileName = Convert.ToBase64String(memoryStream.ToArray()),
                                Active = true,
                                ShowToCust = true,
                                CreateUser = ScheduleDetails.CreateUser,
                                CreateDate = DateTime.Now,
                                ScheduleDateId = ScheduleDetails.OneScheduleDate.Id,
                                PictureSource = SchedulePhoto,
                                Flag = 0, // new photo
                            };

                            LstNewPictures.Add(OnePictureModel);
                            LstAllPictures.Add(OnePictureModel);
                            ScheduleDetails.LstSchedulePictures.Add(OnePictureModel);
                            ScheduleDetails.GetPictures = false; //Don't entrance GetPictures Method

                            UserDialogs.Instance.ShowLoading();

                            //await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.SchedulePicturesPage(new SchImagesViewModel(ScheduleDetails,ORep,_service),ORep,_service));
                            //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                            MessagingCenter.Send(this, "ChangeSchImagesInSchedulePicturesPage", LstAllPictures);

                            DoneFlag = true;

                            UserDialogs.Instance.HideHud();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var toast = Toast.Make("No Camera : No camera available.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }

        }


        [RelayCommand]
        async Task OutScheduleImage(SchedulePicturesModel image)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string UserToken = await _service.UserToken();
                var json = await ORep.PutStrAsync("api/Schedules/PutOutPicture", image, UserToken);
                if (json == "false")
                {
                    var toast = Toast.Make("Don't Show unchecked photos to customer", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
                else
                {
                    var toast = Toast.Make("Show checked photos to customer", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }
            IsEnable = true;
        }

        [RelayCommand]
        async Task DeleteSchedulePhoto(SchedulePicturesModel model)
        {
            IsEnable = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model.Id == 0) //Id = 0 (Photo New)
                {
                    LstNewPictures.Remove(model);
                    LstAllPictures.Remove(model);
                    ScheduleDetails.LstSchedulePictures.Remove(model);
                }
                else //Id != 0 (already Photo save)
                {
                    UserDialogs.Instance.ShowLoading();
                    string UserToken = await _service.UserToken();
                    var json = await ORep.DeleteStrItemAsync(string.Format("api/ImageMobile/DeleteOneImage/{0}", model.Id), UserToken);

                    if (json != null && json != "api not responding")
                    {
                        LstAllPictures.Remove(model);
                        ScheduleDetails.LstSchedulePictures.Remove(model);
                    }
                    UserDialogs.Instance.HideHud();
                }

                List<SchedulePicturesModel> NewImg = ScheduleDetails.LstSchedulePictures.Where(x => x.Id == 0).ToList();
                if (NewImg.Count > 0)
                {
                    DoneFlag = true;
                }

                else
                {
                    DoneFlag = false;
                }
            }
            IsEnable = true;
        }

        [RelayCommand]
        async Task DonePictures(SchedulesModel model)
        {
            IsEnable = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (model != null)
                {
                    await UploadPictures(model.LstSchedulePictures);

                    var toast = Toast.Make("Successfully add schedule pictures.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();

                    UserDialogs.Instance.ShowLoading();

                    MessagingCenter.Send(this, "ChangeSchImagesInSchadulePage", ScheduleDetails.LstSchedulePictures);

                    await App.Current!.MainPage!.Navigation.PopAsync();
                    //await App.Current!.MainPage!.Navigation.PushAsync(new Pages.SchedulePages.ScheduleDetailsPage(new ScheduleDetailsViewModel(model.Id, model.OneScheduleDate.Id, ORep, _service), ORep, _service));

                    UserDialogs.Instance.HideHud();
     
                }
                else
                {
                    var toast = Toast.Make("Please Choose Photos.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                    await toast.Show();
                }
            }

            IsEnable = true;
        }
    }
}

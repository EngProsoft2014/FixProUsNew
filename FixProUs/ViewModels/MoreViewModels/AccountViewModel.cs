
using System.Collections.ObjectModel;
using FixProUs.Services.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FixProUs.Models;
using FixProUs.Helpers;
using CommunityToolkit.Mvvm.Input;
using FixProUs.Controls;
using Controls.UserDialogs.Maui;
using Mopups.Services;
using SkiaSharp;
using FixProUs.Pages.MenuPages;
using FixProUs.Pages;
using CommunityToolkit.Maui.Alerts;


namespace FixProUs.ViewModels
{
    public partial class AccountViewModel : BaseViewModel
    {

        #region Service
        readonly IGenericRepository ORep;
        readonly Services.Data.ServicesService _service;
        #endregion

        #region Prop
        [ObservableProperty]
        EmployeeModel loginModel;

        [ObservableProperty]
        BranchesModel oneBranches;

        [ObservableProperty]
        ObservableCollection<BranchesModel> lstBranches;

        [ObservableProperty]
        string accountPhoto;
        #endregion

        #region Cons
        public AccountViewModel(IGenericRepository GenericRep, Services.Data.ServicesService service)
        {
            ORep = GenericRep;
            _service = service;
            Init();
            MessagingCenter.Subscribe<AccountViewModel, bool>(this, "ChangedProfileImage", (sender, message) =>
            {
                if (true)
                {
                    Init();
                }
            });
        }
        #endregion

        #region Methods
        async void Init()
        {
            LoginModel = new EmployeeModel();
            OneBranches = new BranchesModel();
            LstBranches = new ObservableCollection<BranchesModel>();

            LoginModel.Id = int.Parse(Settings.UserIdGet);
            LoginModel.UserName = Settings.UserNameGet;
            LoginModel.FirstName = Settings.UserFristNameGet;
            LoginModel.LastName = Settings.UserLastNameGet;
            LoginModel.EmailUserName = Settings.EmailGet;
            LoginModel.Phone1 = Settings.PhoneGet;
            LoginModel.Password = Settings.PasswordGet;


            if (!string.IsNullOrEmpty(Settings.CreateDateGet))
            {
                LoginModel.CreateDate = Convert.ToDateTime(Settings.CreateDateGet);
            }

            await GetBranches();

            try
            {
                AccountPhoto = Preferences.Default.Get(Settings.UserPricture, "avatar.png");
            }
            catch (Exception)
            {
                AccountPhoto = "avatar.png";
            }

            //LoginModel.OldPicture = StaticMembers.OldProfileImageSt;
        }

        //Get All Branches
        async Task GetBranches()
        {
            IsEnable = false;

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                UserDialogs.Instance.ShowLoading();
                string UserToken = await _service.UserToken();
                var json = await ORep.GetAsync<ObservableCollection<BranchesModel>>(string.Format("api/Employee/GetEmpolyeeBranches?" + "AccountId=" + Settings.AccountIdGet + "&" + "EmpId=" + Settings.UserIdGet), UserToken);

                if (json != null)
                {
                    LstBranches = json;

                    OneBranches = LstBranches.Where(x => x.Id == int.Parse(Settings.BranchIdGet)).FirstOrDefault()!;
                }

                UserDialogs.Instance.HideHud();
            }

            IsEnable = true;
        }
        #endregion

        #region RelayCommand
        [RelayCommand]
        void SelectBranch(BranchesModel model)
        {
            IsEnable = false;
            OneBranches = model;
            Preferences.Default.Set(Settings.BranchId, model.Id.ToString());
            Preferences.Default.Set(Settings.BranchName, model.Name);
            IsEnable = true;
        }

        [RelayCommand]
        async Task OpenFullScreenPhoto(string photo)
        {
            IsEnable = false;
            if (photo != Utility.PathServerProfileImages)
            {
                await App.Current!.MainPage!.Navigation.PushAsync(new FullScreenImagePage(photo));
            }
            IsEnable = true;
        }

        //Pick Photo
        [RelayCommand]
        private async Task SelectePickAccountPhoto()
        {
            string UserToken = await _service.UserToken();

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

                        // Display the selected photo in the Image control
                        LoginModel.Picture = Convert.ToBase64String(memoryStream.ToArray());

                        if (Connectivity.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
                        {
                            //Upload Image To Server
                            UserDialogs.Instance.ShowLoading();

                            var Postjson = await ORep.PostAsync(string.Format("api/ImageMobile/ReplacePostOneImageProfileImageOnlyMobile"), LoginModel, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (Postjson != null)
                            {
                                //EmployeeModel UserInfo = JsonConvert.DeserializeObject<EmployeeModel>(Postjson);

                                Preferences.Default.Set(Settings.UserPricture, (Utility.PathServerProfileImages + Postjson.Picture));
                                LoginModel.Picture = Postjson.Picture;
                                StaticMembers.OldProfileImageSt = LoginModel.OldPicture = Postjson.Picture;
                                //AccountPhoto = ImageSource.FromStream(() => memoryStream);
                                //AccountPhoto = ImageSource.FromStream(() =>
                                //{
                                //    return memoryStream;  
                                //});

                                //var imageBytes = memoryStream.ToArray();

                                //AccountPhoto = ImageSource.FromStream(() =>
                                //{
                                //    // Return a new MemoryStream each time, so it remains accessible across different UI contexts
                                //    return new MemoryStream(imageBytes);
                                //});

                                // Display the selected photo in the Image control
                                AccountPhoto = Postjson.Picture;
                                //AccountPhoto = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                            }

                            MessagingCenter.Send(this, "ChangedProfileImage", true);

                            //await App.Current!.MainPage!.Navigation.PushAsync(new AccountPage());
                            //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);

                        }

                    }
                }

            }
            catch (Exception)
            {
                var toast = Toast.Make("No camera available.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }
        }

        //Camera Photo
        [RelayCommand]
        private async Task SelecteCamAccountPhoto()
        {
            string UserToken = await _service.UserToken();

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

                            // Display the image
                            LoginModel.Picture = Convert.ToBase64String(memoryStream.ToArray());

                            UserDialogs.Instance.ShowLoading();
                            var Postjson = await ORep.PostAsync(string.Format("api/ImageMobile/ReplacePostOneImageProfileImageOnlyMobile"), LoginModel, UserToken);
                            UserDialogs.Instance.HideHud();

                            if (Postjson != null)
                            {
                                //EmployeeModel UserInfo = JsonConvert.DeserializeObject<EmployeeModel>(Postjson);

                                Preferences.Default.Set(Settings.UserPricture, (Utility.PathServerProfileImages + Postjson.Picture));
                                LoginModel.Picture = Postjson.Picture;
                                StaticMembers.OldProfileImageSt = LoginModel.OldPicture = Postjson.Picture;
                                //AccountPhoto = ImageSource.FromStream(() => memoryStream);
                                //AccountPhoto = ImageSource.FromStream(() =>
                                //{
                                //    return memoryStream;
                                //});

                                //var imageBytes = memoryStream.ToArray();

                                //AccountPhoto = ImageSource.FromStream(() =>
                                //{
                                //    // Return a new MemoryStream each time, so it remains accessible across different UI contexts
                                //    return new MemoryStream(imageBytes);
                                //});
                                AccountPhoto = Postjson.Picture;
                                //AccountPhoto = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                            }

                            MessagingCenter.Send(this, "ChangedProfileImage", true);

                            //await App.Current!.MainPage!.Navigation.PushAsync(new AccountPage());
                            //App.Current!.MainPage!.Navigation.RemovePage(App.Current!.MainPage!.Navigation.NavigationStack[App.Current!.MainPage!.Navigation.NavigationStack.Count - 2]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var toast = Toast.Make("No camera available.", CommunityToolkit.Maui.Core.ToastDuration.Long, 15);
                await toast.Show();
            }
        } 
        #endregion
    }
}

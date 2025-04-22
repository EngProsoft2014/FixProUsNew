using FixProUs.Helpers;
using FixProUs.Pages;
using FixProUs.Pages.CallPages;
using FixProUs.Pages.CustomerPages;
using FixProUs.Pages.MenuPages;
using FixProUs.Pages.PlansPages;
using FixProUs.Pages.PopupPages;
using FixProUs.Pages.SchedulePages;
using FixProUs.Services.Data;
using FixProUs.ViewModels;
using Microsoft.Maui.Handlers;


namespace FixProUs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection Services)
        {
            #region ServiceServices
            Services.AddSingleton<ServicesService>();
            
            #endregion

            #region GenericRepository
            Services.AddScoped<IGenericRepository, GenericRepository>();
            #endregion

            #region ViewModels
            //Calls
            Services.AddTransient<CallsViewModel>();
            Services.AddTransient<FilterCallsViewModel>();

            //Customers
            Services.AddTransient<AddCustomerViewModel>();
            Services.AddTransient<CustEstimatesViewModel>();
            Services.AddTransient<CustInformationViewModel>();
            Services.AddTransient<CustInvoicesViewModel>();
            Services.AddTransient<CustomersViewModel>();
            Services.AddTransient<CustSchedulesViewModel>();
            Services.AddTransient<UpdateCustomerViewModel>();

            //More
            Services.AddTransient<AccountViewModel>();
            Services.AddTransient<EmployeesViewModel>();

            //Schedules
            Services.AddTransient<AddScheduleViewModel>();
            Services.AddTransient<CreateItemViewModel>();
            Services.AddTransient<FullScreenNoteViewModel>();
            Services.AddTransient<OnMyWayViewModel>();
            Services.AddTransient<SchActiveViewModel>();
            Services.AddTransient<ScheduleDetailsViewModel>();
            Services.AddTransient<ScheduleFreeServicesViewModel>();
            Services.AddTransient<ScheduleItemsServicesViewModel>();
            Services.AddTransient<ScheduleMaterialReceiptViewModel>();
            Services.AddTransient<SchedulesViewModel>();
            Services.AddTransient<SchEstimatesViewModel>();
            Services.AddTransient<SchImagesViewModel>();
            Services.AddTransient<SchInvoicesViewModel>();

            //Shared
            Services.AddTransient<BaseViewModel>();         
            Services.AddTransient<LoginViewModel>();            
            Services.AddTransient<PaymentsViewModel>();
            Services.AddTransient<PlansViewModel>();           
            Services.AddTransient<TimeSheetViewModel>();
            #endregion

            #region Pages

            #region Call Pages
            Services.AddTransient<FilterCallPage>();
            Services.AddTransient<NewCallPage>();
            Services.AddTransient<SearchCustomerPopup>();
            #endregion

            #region Customer Pages
            Services.AddTransient<CashOrCreditPaymentPage>();
            Services.AddTransient<CreateEstimateWithoutSchedulePage>();
            Services.AddTransient<CreateInvoiceWithoutSchedulePage>();
            Services.AddTransient<CreateNewCustomerPage>();
            Services.AddTransient<CustomersDetailsPage>();
            Services.AddTransient<EstimateDetailsPage>();
            Services.AddTransient<InvoiceDetailsPage>();
            Services.AddTransient<UpdateCustomerPage>();
            #endregion

            #region Menu Pages
            Services.AddTransient<AccountPage>();
            Services.AddTransient<AllEmployeesPage>();
            Services.AddTransient<EmployeesWorkingPage>();
            Services.AddTransient<TrckingMapPage>();
            #endregion

            #region Plans Pages
            Services.AddTransient<ChoosePlanPage>();
            Services.AddTransient<PlanPaymentPage>();
            #endregion

            #region Popup Pages
            Services.AddTransient<AddressPupop>();
            Services.AddTransient<AddSchedulePhotoPupop>();
            Services.AddTransient<ChangeAccountPhotoPupop>();
            Services.AddTransient<CheckoutPopup>();
            Services.AddTransient<DatePopup>();
            Services.AddTransient<EmployeesPopup>();
            Services.AddTransient<MapTypePopup>();
            Services.AddTransient<OnMyWayPopup>();
            Services.AddTransient<PaymentMethodsPopup>();
            Services.AddTransient<ScheduleDatesPopup>();
            #endregion

            #region Schedule Pages
            Services.AddTransient<ChooseCustomerPage>();
            Services.AddTransient<CreateEstimatePage>();
            Services.AddTransient<CreateInvoicePage>();
            Services.AddTransient<CreateItemPage>();
            Services.AddTransient<FilterMaterials>();
            Services.AddTransient<FullScreenNotePage>();
            Services.AddTransient<MaterialReceiptPage>();
            Services.AddTransient<NewItemsServicesSchedulePage>();
            Services.AddTransient<NewSchedulePage>();
            Services.AddTransient<ScheduleDetailsPage>();
            Services.AddTransient<ScheduleFreeServicesPage>();
            Services.AddTransient<ScheduleJobDetailsPage>();
            Services.AddTransient<SchedulePicturesPage>();
            #endregion

            #region Share Pages
            Services.AddTransient<CreateNotificationsPage>();
            Services.AddTransient<FullScreenImagePage>();
            Services.AddTransient<LoginPage>();
            Services.AddTransient<MainPage>();
            Services.AddTransient<NoInternetPage>();
            Services.AddTransient<NotificationsPage>();
            #endregion

            #endregion

            return Services;
        }

        public static void ControlsBackground()
        {

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            });

            Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping(nameof(SearchBarHandler), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                var textField = handler.PlatformView.ValueForKey(new Foundation.NSString("searchField")) as UIKit.UITextField;
                if (textField != null)
                {
                    textField.BackgroundColor = UIKit.UIColor.Clear; // Set text field background color
                    textField.ClipsToBounds = true;
                }
#endif
            });


            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(PickerHandler), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            });

            

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(EditorHandler), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            });


            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(DatePickerHandler), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            });

            Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping(nameof(TimePickerHandler), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
            });


        }
    }
}

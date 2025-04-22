using System;
using System.Collections.Generic;


namespace FixProUs.Helpers

{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {

        public const string syncFusionLicence = "Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxed3RSRmldWUN2W0c="; //Version= 24.x.x

        //public const string syncFusionLicence = "Ngo9BigBOggjHTQxAR8/V1NAaF1cXmhKYVJ1WmFZfVpgdVRMYl9bQXdPMyBoS35RckVgWXlfcXZWQ2FZVUVw"; //Version= 24.2.9


        #region Setting staticKeys

        public static string GeneralSettings = "settings_key"; 
        public static string Language = "Language_key";
        public static string UserName = "UserName_key";
        public static string UserFristName = "FristName_key";
        public static string UserLastName = "LastName_key";
        public static string UserId = "UserId_key";
        public static string Email = "Email_key";
        public static string Phone = "Phone_key";
        public static string AccountType = "AccountType_key";
        public static string Address = "Address_key";
        public static string AddressAr = "AddressAr_key";
        public static string Password = "Password_key";
        public static string UserPricture = "UserPricture_key";
        public static string ApprovedData = "UserApprovedData_key";
        public static string Themes = "Themes_key";
        public static string SortData = "Sort_key";
        public static string CreateDate = "CreateDate_key";
        public static string AccountId = "AccountId_key";
        public static string AccountName = "AccountName_key";
        public static string BranchId = "BranchId_key";
        public static string BranchName = "BranchName_key";
        public static string UserRole = "UserRol_key";
        public static string UserEmployees = "UserEmployees_key";
        public static string DeviceId = "DeviceId_key";
        public static string PlayerId = "PlayerId_key";
        public static string OneSignalAppId = "OneSignalAppId_key";
        public static string OneSignalRest = "OneSignalRest_key";
        public static string OneSignalAuth = "OneSignalAuth_key";
        public static string TypeTrackingSch_Invo = "TypeTrackingSch_Invo_key";
        public static string AccountDayExpired = "AccountDayExpired_key";
        public static string AccountNameWithSpace = "AccountNameWithSpace_key";

        #endregion


        public static string UserNameGet = Preferences.Default.Get(UserName, "");
        public static string GeneralSettingsGet = Preferences.Default.Get(GeneralSettings, "");
        public static string LanguageGet = Preferences.Default.Get(Language, "");
        public static string UserFristNameGet = Preferences.Default.Get(UserFristName, "");
        public static string UserLastNameGet = Preferences.Default.Get(UserLastName, "");
        public static string UserIdGet = Preferences.Default.Get(UserId, "");
        public static string EmailGet = Preferences.Default.Get(Email, "");
        public static string PhoneGet = Preferences.Default.Get(Phone, "");
        public static string AccountTypeGet = Preferences.Default.Get(AccountType, "");
        public static string AddressGet = Preferences.Default.Get(Address, "");
        public static string AddressArGet = Preferences.Default.Get(AddressAr, "");
        public static string PasswordGet = Preferences.Default.Get(Password, "");
        public static string UserPrictureGet = Preferences.Default.Get(UserPricture, "");
        public static string ApprovedDataGet = Preferences.Default.Get(ApprovedData, "");
        public static string ThemesGet = Preferences.Default.Get(Themes, "");
        public static string SortDataGet = Preferences.Default.Get(SortData, "");
        public static string CreateDateGet = Preferences.Default.Get(CreateDate, "");
        public static string AccountIdGet = Preferences.Default.Get(AccountId, "");
        public static string AccountNameGet = Preferences.Default.Get(AccountName, "");
        public static string BranchIdGet = Preferences.Default.Get(BranchId, "");
        public static string BranchNameGet = Preferences.Default.Get(BranchName, "");
        public static string UserRoleGet = Preferences.Default.Get(UserRole, "");
        public static string UserEmployeesGet = Preferences.Default.Get(UserEmployees, "");
        public static string DeviceIdGet = Preferences.Default.Get(DeviceId, "");
        public static string PlayerIdGet = Preferences.Default.Get(PlayerId, "");
        public static string OneSignalAppIdGet = Preferences.Default.Get(OneSignalAppId, "");
        public static string OneSignalRestGet = Preferences.Default.Get(OneSignalRest, "");
        public static string OneSignalAuthGet = Preferences.Default.Get(OneSignalAuth, "");
        public static string TypeTrackingSch_InvoGet = Preferences.Default.Get(TypeTrackingSch_Invo, "");
        public static string AccountDayExpiredGet = Preferences.Default.Get(AccountDayExpired, "");
        public static string AccountNameWithSpaceGet = Preferences.Default.Get(AccountNameWithSpace, "");



    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FixProUs.Models
{
    public class EmployeeModel : BaseModel

    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public DateTime? AccountExpireDate { get; set; }
        public string DBConnection { get; set; }
        public string GernToken { get; set; }
        public string PathFileUpload { get; set; }
        public string AccountSubdomainApiURL { get; set; }
        public int? TypeTrackingSch_Invo { get; set; }
        public int? BrancheId { get; set; }
        public string BranchName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime? Birthday { get; set; }
        public DateTime? Since { get; set; }
        public decimal? Salary { get; set; }
        public int? SalaeryPer { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string UserName { get; set; }
        public string EmailUserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalcodeZIP { get; set; }
        public string locationlatitude { get; set; }
        public string locationlongitude { get; set; }
        public string Country { get; set; }
        public string SSN { get; set; }
        public string DriveLicense { get; set; }
        public string Picture { get; set; }
        public string OldPicture { get; set; }
        public int? UserRole { get; set; }
        public int? UserType { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string TheBranchs { get; set; }
        public string Employees { get; set; }
        public bool? ActiveHome { get; set; }
        public bool? ActiveBranches { get; set; }
        public bool? ActiveContract { get; set; }
        public bool? ActiveCustomers { get; set; }
        public bool? ActiveCustomersCategory { get; set; }
        public bool? ActiveCustomersCustomField { get; set; }
        public bool? ActiveEmployee { get; set; }
        public bool? ActiveEmployeeCategory { get; set; }
        public bool? ActiveEmployeeCustomField { get; set; }
        public bool? ActiveEstimate { get; set; }
        public bool? ActiveEstimateEmailTemplate { get; set; }
        public bool? ActiveExpenses { get; set; }
        public bool? ActiveExpensesCategory { get; set; }
        public bool? ActiveEquipments { get; set; }
        public bool? ActiveEquipmentsCustomField { get; set; }
        public bool? ActiveItemsServices { get; set; }
        public bool? ActiveItemsServicesCategory { get; set; }
        public bool? ActiveItemsServicesCustomField { get; set; }
        public bool? ActiveMember { get; set; }
        public bool? ActiveTimeSheet { get; set; }
        public bool? ActiveInvoice { get; set; }
        public bool? ActiveInvoiceEmailTemplate { get; set; }
        public bool? ActiveMap { get; set; }
        public bool? ActiveNotes { get; set; }
        public bool? ActiveNotificationSettings { get; set; }
        public bool? ActivePayment { get; set; }
        public bool? ActiveReminderRules { get; set; }
        public bool? ActiveRoute { get; set; }
        public bool? ActiveSchedule { get; set; }
        public bool? ActiveSettings { get; set; }
        public bool? ActiveTax { get; set; }
        public bool? ActiveReport { get; set; }
        public bool? ActiveStripeAccount { get; set; }
        public bool? ActiveMessage { get; set; }
        public bool? ActiveItemsServicesSubCategory { get; set; }
        public bool? ActiveAccount { get; set; }
        public bool? ActiveEditCustomer { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? ActiveEditPrice { get; set; }
        public bool? ActiveMobileLogin { get; set; }
        public bool? ActiveMobileTrackStaff { get; set; }
        public bool? ActiveCreateSchedule { get; set; }
        public bool IsChecked { get; set; }
        public string OneSignalPlayerId { get; set; }
        public bool? ActiveAllScdTr_FaorTrOnly { get; set; }
        public bool? ActiveEditEstimate_Invoice { get; set; }
        public string CompanyNameWithSpace { get; set; }
        public string EmployeeStatus { get; set; }
        public bool? ActiveCustomerPhone { get; set; }
    }
}
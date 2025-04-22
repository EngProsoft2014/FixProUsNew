
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;


namespace FixProUs.Models
{
    public class CustomersModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } set { } }
        public DateTime? Since { get; set; }
        public int? CustomerType { get; set; }
        public int? CategoryId { get; set; }
        public int? TaxId { get; set; }
        public bool? Taxable { get; set; }
        public decimal? Discount { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalcodeZIP { get; set; }
        public string YearBuit { get; set; }
        public string Squirefootage { get; set; }
        public string YearEstimedValue { get; set; }
        public string EstimedValue { get; set; }

        public string EstimedValueView { get { return (!string.IsNullOrEmpty(EstimedValue) && EstimedValue != "None" && EstimedValue.StartsWith("$") != true) ? string.Format("${0:#,0.#}", float.Parse(EstimedValue)) : (!string.IsNullOrEmpty(EstimedValue) && EstimedValue != "None" && EstimedValue.StartsWith("$") == true) ? EstimedValue : "None"; } set { } }
        public string locationlatitude { get; set; }
        public string locationlongitude { get; set; }
        public Location MPosition { get { return new Location(locationlatitude == null ? 0 : double.Parse(locationlatitude), locationlongitude == null ? 0 : double.Parse(locationlongitude)); } }
        public string Phone1 { get; set; }
        public string Phone1WithoutPermission { get; set; }
        public string Phone2 { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public bool? AllowLogin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? Source { get; set; }
        public bool? MemeberType { get; set; }
        public int? MemeberId { get; set; }
        public string MemberName { get; set; }
        public DateTime? MemeberExpireDate { get; set; } = DateTime.Now;
        public decimal? Credit { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public CustomersCategoryModel CustomerCategory { get; set; }
        public List<CustomersCustomFieldModel> LstCustomersCustomField { get; set; }
        public MemberModel MemberDTO { get; set; }
        public TaxModel TaxDTO { get; set; }
        public CampaignModel CampaignDTO { get; set; }
        public List<SchedulesModel> LstSchedules { get; set; }
        public List<EstimateModel> LstEstimates { get; set; }
        public List<InvoiceModel> LstInvoices { get; set; }
        public SuggestionAddressModel AddressModel { get; set; }

        public List<ScheduleItemsServicesModel> LstCustItemsServices { get; set; } = new List<ScheduleItemsServicesModel>();

        public int? CallId { get; set; }

        bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

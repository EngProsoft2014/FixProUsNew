using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FixProUs.Models
{
    public class ItemsServicesModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        //public string Name { get; set; }
        string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public int? Type { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool? InventoryItem { get; set; }
        public int? QTYTime { get; set; } = 1;
        public string Unit { get; set; }
        public decimal? CostperUnit { get; set; }
        public bool? MemeberType { get; set; }
        public int? MemeberId { get; set; }
        public int? TaxId { get; set; }
        public decimal? Tax { get; set; }
        public bool? Taxable { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Picture { get; set; }
        public string SKU { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FixProUs.Models
{
    public class CustomerfeaturesModel
    {
        public List<CustomersCategoryModel> LstCustomerCategory { get; set; }
        public List<CustomersCustomFieldModel> LstCustomersCustomField { get; set; }
        public List<MemberModel> LstMemberships { get; set; }
        public List<TaxModel> LstTaxes { get; set; }
    }
}

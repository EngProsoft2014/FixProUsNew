using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FixProUs.Models
{
    public class CustomersCustomFieldModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public string CustomFieldName { get; set; }
        public int? FieldType { get; set; }
        public string DefaultValue { get; set; }
        public bool? Required { get; set; }
        public string Notes { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<string> LstString { get { return (FieldType == 6) ? DefaultValue.Split(',').ToList() : new List<string>(); } }

        //public List<CustomersCustomFieldModel> LstYesOrNo { get; set; } = new List<CustomersCustomFieldModel>();
        
        
        
        //public CustomersCustomFieldModel ObjCustomField { get {
        //        return (FieldType == 4) ? new CustomersCustomFieldModel
        //        {
        //            Id = Id,
        //            AccountId = AccountId,
        //            BrancheId = BrancheId,
        //            CustomFieldName = CustomFieldName,
        //            FieldType = FieldType,
        //            DefaultValue = DefaultValue,
        //            Required = Required,
        //            Notes = Notes,
        //            Active = Active,
        //            CreateDate = CreateDate,
        //            CreateUser = CreateUser
        //        }
        //        : new CustomersCustomFieldModel(); }  }


    }
}

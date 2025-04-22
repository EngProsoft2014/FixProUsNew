using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public int? PlanId { get; set; }
        public string EamilAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CompanyName { get; set; }
        public int? Type { get; set; }
        public int? DayExpire { get; set; }
        public System.DateTime? ExpireDate { get; set; }
        public string AccountSubdomainURL { get; set; }
        public string AccountSubdomainApiURL { get; set; }
        public string HostName { get; set; }
        public string DBConnection { get; set; }
        public string DBDataSource { get; set; }
        public string DBName { get; set; }
        public string DBUserId { get; set; }
        public string DBPassword { get; set; }
        public string OneSignalAuthApi { get; set; }
        public string OneSignalRestApikey { get; set; }
        public string OneSignalAppId { get; set; }
        public int? TimeOutLogout { get; set; }
        public string PathFileUpload { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public System.DateTime? CreateDate { get; set; }
    }
}

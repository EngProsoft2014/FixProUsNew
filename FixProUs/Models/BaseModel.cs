using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FixProUs.Models
{
    public class BaseModel
    {
        public int? CreateUser { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? CreateDate { get; set; }
    
    }
}
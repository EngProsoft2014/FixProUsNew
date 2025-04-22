using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class SuggestionAddressModel
    {
        public string PalceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FullAddress { get; set; }
        public string FullAddressAr { get; set; }
        public string MainAddress { get; set; }
        public string SubAddress { get; set; }
        public string Street { get; set; }
        public string StreetAr { get; set; }
        public string Locality { get; set; }
        public string LocalityAr { get; set; }
        public string City { get; set; }
        public string CityAr { get; set; }
        public string State { get; set; }
        public string StateAr { get; set; }
        public string Country { get; set; }
        public string CountryAr { get; set; }
        public string Zip { get; set; }
    }
}

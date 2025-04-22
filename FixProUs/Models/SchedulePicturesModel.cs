using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace FixProUs.Models
{
    public class SchedulePicturesModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ScheduleId { get; set; }
        public string FileName { get; set; }
        public string FullFileName { get { return $"https://app.fixprous.com/ScheduleAttachments_{Helpers.Settings.AccountNameWithSpaceGet}/{FileName}"; } }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? ScheduleDateId { get; set; }
        public string ScheduleDate { get; set; }
        public bool? ShowToCust { get; set; }

        public ImageSource PictureSource { get; set; }
        //public ImageSource PictureSource { get { return ImageSource.FromStream(() => new MemoryStream(new WebClient().DownloadData(FullFileName))); } set { } }
        public int Flag { get; set; } = 1;
    }
}

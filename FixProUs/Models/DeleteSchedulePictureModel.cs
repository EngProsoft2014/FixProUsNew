using System;
using System.Collections.Generic;
using System.Text;

namespace FixProUs.Models
{
    public class DeleteSchedulePictureModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BrancheId { get; set; }
        public int? ScheduleId { get; set; }
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public bool? Active { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? ScheduleDateId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNSStoreMgmt.Models
{
    public abstract class SchemaLog
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string ReturnUrl { get; set; }
    }
}
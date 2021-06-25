using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VNSStoreMgmt.Models
{
    public class BarcodeMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        public int BarcodePrintCount { get; set; }
        public long? BarcodeFrom { get; set; }
        public long? BarcodeTo { get; set; }
    }
}

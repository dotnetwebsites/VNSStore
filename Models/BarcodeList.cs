using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VNSStoreMgmt.Models
{
    public class BarcodeList : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        public int BMId { get; set; }
        public string Barcode { get; set; }
        public bool IsUsedBarcode { get; set; }
    }
}

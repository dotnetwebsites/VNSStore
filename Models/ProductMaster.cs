using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VNSStoreMgmt.Models
{
    public class ProductMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required(ErrorMessage = "Required Product Code")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Required Barcode")]
        public string ProductBarcode { get; set; }

        [Required(ErrorMessage = "Required SerialNo")]
        public string SerialNo { get; set; }

        [Required(ErrorMessage = "Required Product Name")]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        
    }
}

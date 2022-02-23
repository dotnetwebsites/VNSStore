using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VNSStoreMgmt.Models
{
    public class Accountability : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required Product ID")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductMaster ProductMaster { get; set; }

        public bool ProductInOut { get; set; }

        public string Outcomment { get; set; }
        public string OutRemark { get; set; }
        public string ProductOutBy { get; set; }
        public DateTime OutDate { get; set; }


        public string Incomment { get; set; }
        public string InRemark { get; set; }
        public string ProductInBy { get; set; }
        public DateTime? InDate { get; set; }
    }

    public class ProductOutModel
    {
        public string ProductBarcode { get; set; }

        //[Required(ErrorMessage = "Required Comment")]
        //public string Outcomment { get; set; }

        [Required(ErrorMessage = "Required Remark")]
        public string OutRemark { get; set; }
    }

    public class ProductBarcodeModel
    {
        [Required(ErrorMessage = "Required Barcode")]
        public string ProductBarcode { get; set; }
        public DateTime? Date { get; set; }
    }

    public class ProductOutSaveModel
    {
        public string UserId { get; set; }
        public string OutDestination { get; set; }
    }

    public class ProductInSaveModel
    {
        public DateTime Date { get; set; }
        public string Barcode { get; set; }
        public string UserId { get; set; }
        public string Remark { get; set; }
    }

    public class CartViewModel
    {
        [Key]
        public int Id { get; set; }
        public string ProductBarcode { get; set; }
        public int ProductId { get; set; }
        public bool ProductInOut { get; set; }
        public string Outcomment { get; set; }
        public string OutRemark { get; set; }
    }

    public class CalendarApi
    {
        public string groupId { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
    }

    public class GetDates
    {
        public DateTime? OutDate { get; set; }
    }
}

using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VNSStoreMgmt.Data;
using VNSStoreMgmt.Models;

namespace VNSStoreMgmt.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _repository;

        public ReportController(ApplicationDbContext repository)
        {
            _repository = repository;
        }


        public IActionResult Index()
        {
            var prods = _repository.Accountabilities
                .Where(p => p.ProductInOut == true)
                .Include(p => p.ProductMaster)
                .ToList();

            return View(prods);
        }

        [HttpPost]
        public IActionResult Export()
        {
            var dt = GetProductsDetail();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        private DataTable GetProductsDetail()
        {
            var accs = _repository.Accountabilities
                .Where(p => p.ProductInOut == true)
                .Include(p => p.ProductMaster)
                .ToList();

            DataTable dtProduct = new DataTable("ProductDetails");
            dtProduct.Columns.AddRange(new DataColumn[8] {
                                            new DataColumn("ProductID"),
                                            new DataColumn("ProductCode"),
                                            new DataColumn("ProductName"),
                                            new DataColumn("Barcode"),
                                            new DataColumn("ProductDescription"),
                                            new DataColumn("OutRemark"),
                                            new DataColumn("ProductOutBy"),
                                            new DataColumn("ProductOutDate")});

            foreach (var acc in accs)
            {
                dtProduct.Rows.Add(acc.Id,
                    acc.ProductMaster.ProductCode,
                    acc.ProductMaster.ProductName,
                    acc.ProductMaster.ProductBarcode,
                    acc.ProductMaster.ProductDescription,
                    acc.OutRemark,
                    acc.ProductOutBy,
                    acc.OutDate);
            }

            return dtProduct;
        }


    }
}

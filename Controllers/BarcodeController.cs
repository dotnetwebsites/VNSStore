using BarcodeLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VNSStoreMgmt.Controllers
{
    [Authorize]
    public class BarcodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Generate(int count)
        {
            ViewBag.Count = count;



            return View();
        }

        public IActionResult GenerateBarcode(string barcodeNo)
        {
            Barcode barcode = new Barcode();
            Image img = barcode.Encode(TYPE.CODE39, barcodeNo, Color.Black, Color.White, 185, 25);
            var data = ConvertImageToByte(img);
            return File(data, "image/jpeg");
        }

        private byte[] ConvertImageToByte(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}

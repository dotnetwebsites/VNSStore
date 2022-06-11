using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Data;
using VNSStoreMgmt.Models;

namespace VNSStoreMgmt.Controllers
{
    [Authorize]
    public class ProductEventController : Controller
    {
        private readonly ILogger<ProductEventController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductEventController(ILogger<ProductEventController> logger,
                                ApplicationDbContext repository,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Accountability accountability)
        {
            if (ModelState.IsValid)
            {
                accountability.CreatedBy = User.Identity.Name;
                accountability.CreatedDate = DateTime.Now;

                _repository.Accountabilities.Add(accountability);
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(accountability);
        }


        public IActionResult ProductIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProductIn(ProductBarcodeModel productBarcodeModel)
        {
            if (ModelState.IsValid)
            {
                List<string> barcodes = productBarcodeModel.ProductBarcode
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(p => p != "")
                .Distinct()
                .ToList();

                var productMaster = _repository.ProductMasters.Where(p => barcodes.Contains(p.ProductBarcode)).ToList();

                if (productBarcodeModel.Date == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Date");
                    return View(productBarcodeModel);
                }

                if (productMaster == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found for barcode no : " + productBarcodeModel.ProductBarcode);
                    return View(productBarcodeModel);
                }

                List<Accountability> acc = new List<Accountability>();
                foreach (var p in productMaster)
                {
                    var accountability = _repository.Accountabilities.FirstOrDefault(x => x.ProductId == p.Id && x.OutDate.Date == productBarcodeModel.Date && x.ProductInOut == true);

                    if (accountability != null) acc.Add(accountability);
                }

                ViewBag.Acc = acc.ToList();

                return View(productBarcodeModel);
            }

            productBarcodeModel.ProductBarcode = string.Empty;

            return View(productBarcodeModel);
        }

        [HttpGet]
        public IActionResult ProductOut()
        {
            //var users = _userManager.Users.ToList();
            //List<ApplicationUser> userList = new List<ApplicationUser>();

            //foreach (var user in users)
            //{
            //    if (await _userManager.IsInRoleAsync(user, "fieldexecutive"))
            //    {
            //        userList.Add(user);
            //    }
            //}

            //ViewBag.UserId = new SelectList(userList, "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductOut(ProductBarcodeModel productBarcodeModel)
        {
            var users = _userManager.Users;
            List<ApplicationUser> userList = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "fieldexecutive"))
                {
                    userList.Add(user);
                }
            }

            List<string> barcodes = productBarcodeModel.ProductBarcode
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(p => p != "")
                .Distinct()
                .ToList();

            if (ModelState.IsValid)
            {
                var productMaster = _repository.ProductMasters.FirstOrDefault(p => barcodes.Contains(p.ProductBarcode));

                if (productMaster == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found for barcode no : " + productBarcodeModel.ProductBarcode);
                    //ModelState.Clear();
                    //ViewBag.UserId = new SelectList(userList, "Id", "FullName");
                    return View(productBarcodeModel);
                }

                //TempData["Barcode"] = productBarcodeModel.ProductBarcode;
                TempData["Barcode"] = barcodes;

                return RedirectToAction("ProductOutRecord");
            }

            productBarcodeModel.ProductBarcode = string.Empty;
            //ViewBag.UserId = new SelectList(userList, "Id", "FullName");

            return View(productBarcodeModel);
        }

        [HttpGet]
        public IActionResult ProductOutRecord()
        {
            //var model = viewd["Barcode"] as ProductBarcodeModel;

            if (TempData["Barcode"] != null)
            {
                var bc = TempData["Barcode"].ToString();
                //var productMaster = _repository.ProductMasters.FirstOrDefault(p => p.ProductBarcode == bc);
                return View();
            }

            return View();
        }

        [HttpPost]
        public IActionResult ProductOutRecord(ProductOutModel model)
        {
            if (ModelState.IsValid)
            {
                string[] barcodes = model.ProductBarcode.Split(",");
                var productMaster = _repository.ProductMasters.Where(p => barcodes.Contains(p.ProductBarcode)).ToList();

                if (productMaster != null)
                {
                    CookieOptions option = new CookieOptions();
                    List<CartViewModel> carts = new List<CartViewModel>();
                    int cnt = 1;

                    if (Request.Cookies["cart"] != null)
                    {
                        List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                        foreach (var cart in existingcarts)
                        {
                            carts.Add(cart);
                            cnt++;
                        }

                        foreach (var p in productMaster)
                        {
                            CartViewModel cv = new CartViewModel();
                            cv.Id = cnt++;
                            cv.ProductId = p.Id;
                            cv.ProductInOut = true;
                            cv.Outcomment = model.OutRemark;
                            cv.OutRemark = model.OutRemark;
                            carts.Add(cv);
                        }

                        Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                    }
                    else
                    {
                        foreach (var p in productMaster)
                        {
                            CartViewModel cv = new CartViewModel();
                            cv.Id = cnt++;
                            cv.ProductId = p.Id;
                            cv.ProductInOut = true;
                            cv.Outcomment = model.OutRemark;
                            cv.OutRemark = model.OutRemark;
                            carts.Add(cv);
                        }

                        Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                    }

                    return RedirectToAction("ProductOut");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetCookies()
        {
            if (Request.Cookies["cart"] != null)
                Response.Cookies.Delete("cart");

            return RedirectToAction("ProductOut");
        }

        [HttpPost]
        public IActionResult SaveProductOut(ProductOutSaveModel model)
        {
            if (!ModelState.IsValid)
                return null;

            if (model.UserId == null)
                return Json("User is not valid");

            if (model.OutDestination == null)
                return Json("Location not entered, please enter destination.");

            List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

            foreach (var cart in existingcarts)
            {
                Accountability ac = new Accountability();
                ac.ProductId = cart.ProductId;
                ac.ProductInOut = cart.ProductInOut;
                ac.Outcomment = model.OutDestination;
                ac.OutRemark = cart.OutRemark;
                ac.ProductOutBy = model.UserId;
                ac.OutDate = DateTime.Now;

                ac.CreatedBy = User.Identity.Name;
                ac.CreatedDate = DateTime.Now;

                _repository.Accountabilities.Add(ac);
                _repository.SaveChanges();

                if (Request.Cookies["cart"] != null)
                    Response.Cookies.Delete("cart");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SaveProductIn(ProductInSaveModel model)
        {
            if (!ModelState.IsValid)
                return null;

            if (model.UserId == null)
                return Json("User is not valid");

            if (model.Remark == null)
                return Json("Please enter remark and ty again.");

            List<string> barcodes = model.Barcode.Split(",").ToList();
            var productMaster = _repository.ProductMasters.Where(p => barcodes.Contains(p.ProductBarcode)).ToList();

            if (model.Date == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Date");
                return View(model);
            }

            if (productMaster == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found for barcode no : " + model.Barcode);
                return View(model);
            }

            foreach (var prod in productMaster)
            {
                var ac = _repository.Accountabilities.FirstOrDefault(p => p.ProductId == prod.Id && p.OutDate.Date == model.Date && p.ProductInOut == true);

                if (ac == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found for this date");
                    return View(model);
                }

                ac.ProductInOut = false;
                ac.InRemark = model.Remark;

                ac.ProductInBy = model.UserId;
                ac.InDate = DateTime.Now;

                ac.UpdatedBy = User.Identity.Name;
                ac.UpdatedDate = DateTime.Now;

                _repository.SaveChanges();
            }

            var url = Url.Action("ProductIn", "ProductEvent", new { date = model.Date.ToString("yyyy-MM-dd") });
            return Json(url);
            //return RedirectToAction("ProductIn", "ProductEvent", new { date = model.Date.ToString("yyyy-MM-dd") });
        }


        [HttpGet]
        [Route("DeleteCart")]
        public IActionResult DeleteCart(int? id)
        {
            if (id == null)
                return View();

            CookieOptions option = new CookieOptions();
            List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

            var cart = existingcarts.FirstOrDefault(p => p.Id == id);
            existingcarts.Remove(cart);

            if (existingcarts.Count > 0)
            {
                Response.Cookies.Append("cart", JsonConvert.SerializeObject(existingcarts.ToList()), option);
            }
            else if (existingcarts.Count <= 0)
            {
                if (Request.Cookies["cart"] != null)
                    Response.Cookies.Delete("cart");
            }

            return RedirectToAction("ProductOut");
        }


    }
}
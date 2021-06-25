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
                var productMaster = _repository.ProductMasters.FirstOrDefault(p => p.ProductBarcode == productBarcodeModel.ProductBarcode);

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

                var accountability = _repository.Accountabilities.FirstOrDefault(p => p.ProductId == productMaster.Id && p.OutDate.Date == productBarcodeModel.Date && p.ProductInOut == true);

                if (accountability == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found for this date");
                    return View(productBarcodeModel);
                }

                ViewBag.Acc = accountability;

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

            if (ModelState.IsValid)
            {
                var productMaster = _repository.ProductMasters.FirstOrDefault(p => p.ProductBarcode == productBarcodeModel.ProductBarcode);

                if (productMaster == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found for barcode no : " + productBarcodeModel.ProductBarcode);
                    //ModelState.Clear();
                    //ViewBag.UserId = new SelectList(userList, "Id", "FullName");
                    return View(productBarcodeModel);
                }

                TempData["Barcode"] = productBarcodeModel.ProductBarcode;

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
                var productMaster = _repository.ProductMasters.FirstOrDefault(p => p.ProductBarcode == model.ProductBarcode);

                if (productMaster != null)
                {
                    Accountability ac = new Accountability();
                    ac.ProductId = productMaster.Id;
                    ac.ProductInOut = true;
                    //ac.Outcomment = model.Outcomment;
                    ac.OutRemark = model.OutRemark;
                    ac.ProductOutBy = User.Identity.Name;
                    ac.OutDate = DateTime.Now;
                    ac.CreatedBy = User.Identity.Name;
                    ac.CreatedDate = DateTime.Now;

                    return AddProduct(ac);

                }
            }

            return View();
        }

        public IActionResult AddProduct(Accountability model)
        {
            CookieOptions option = new CookieOptions();
            List<CartViewModel> carts = new List<CartViewModel>();

            if (Request.Cookies["cart"] != null)
            {
                List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                int cnt = 1;
                foreach (var cart in existingcarts)
                {
                    carts.Add(cart);
                    cnt++;
                }

                CartViewModel cv = new CartViewModel();
                cv.Id = cnt++;
                cv.ProductId = model.ProductId;
                cv.ProductInOut = true;
                cv.Outcomment = model.Outcomment;
                cv.OutRemark = model.OutRemark;

                //existingcarts.Add(cv);
                carts.Add(cv);

                Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                return RedirectToAction("ProductOut");
            }
            else
            {
                CartViewModel cv = new CartViewModel();
                cv.Id = 1;
                cv.ProductId = model.ProductId;
                cv.ProductInOut = true;
                cv.Outcomment = model.Outcomment;
                cv.OutRemark = model.OutRemark;

                carts.Add(cv);

                Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                return RedirectToAction("ProductOut");
            }
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

            var productMaster = _repository.ProductMasters.FirstOrDefault(p => p.ProductBarcode == model.Barcode);

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

            var ac = _repository.Accountabilities.FirstOrDefault(p => p.ProductId == productMaster.Id && p.OutDate.Date == model.Date && p.ProductInOut == true);

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

            //_repository.Accountabilities.Add(ac);
            _repository.SaveChanges();

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

//var ac = new Accountability();
//ac.ProductId = productMaster.Id;
//ac.ProductInOut = true;
//ac.Outcomment = model.Outcomment;
//ac.OutRemark = model.OutRemark;
//ac.ProductOutBy = User.Identity.Name;
//ac.OutDate = DateTime.Now;
//ac.CreatedBy = User.Identity.Name;
//ac.CreatedDate = DateTime.Now;

//_repository.Accountabilities.Add(ac);
//_repository.SaveChanges();

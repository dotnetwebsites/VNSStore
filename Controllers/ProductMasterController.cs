using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VNSStoreMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VNSStoreMgmt.Areas.Identity.Data;
using System;
using VNSStoreMgmt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using VNSStoreMgmt.Utilities;
using System.Linq;

namespace VNSStoreMgmt.Controllers
{
    [Authorize(Roles = "superadmin,admin")]
    public class ProductMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataProtector _protector;

        public ProductMasterController(ILogger<HomeController> logger,
                                ApplicationDbContext repository,
                                UserManager<ApplicationUser> userManager,
                                IDataProtectionProvider dataProtectionProvider,
                                DataProtectionKeys dataProtectionKeys)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
            _protector = dataProtectionProvider
                    .CreateProtector(dataProtectionKeys.IdRouteValue);
        }

        public async Task<IActionResult> Index()
        {
            var productMasters = await _repository.ProductMasters.ToListAsync();

            productMasters = productMasters.Select(a =>
            {
                a.EncryptedId = _protector.Protect(a.Id.ToString());
                return a;
            }).ToList();

            return View(productMasters);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductMaster productMaster)
        {
            if (ModelState.IsValid)
            {
                productMaster.CreatedBy = User.Identity.Name;
                productMaster.CreatedDate = DateTime.Now;

                _repository.ProductMasters.Add(productMaster);
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(productMaster);
        }


        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NoContent();
            }

            int decryptId = Convert.ToInt32(_protector.Unprotect(id));
            ProductMaster productMaster = await _repository.ProductMasters.FindAsync(decryptId);

            return View(productMaster);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string eid)
        {
            int did = Convert.ToInt32(_protector.Unprotect(eid));
            var model = await _repository.ProductMasters.FindAsync(did);

            if (model == null)
            {
                return NoContent();
            }

            model.EncryptedId = eid;
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductMaster model)
        {
            if (ModelState.IsValid)
            {
                int did = Convert.ToInt32(_protector.Unprotect(model.EncryptedId));
                var productMaster = await _repository.ProductMasters.FindAsync(did);

                if (model.ProductBarcode == null)
                {
                    ModelState.AddModelError(string.Empty, "Barcode should not be empty.");
                    return View(model);
                }

                if (productMaster.ProductBarcode != model.ProductBarcode &&
                    _repository.ProductMasters.Any(p => p.ProductBarcode == model.ProductBarcode))
                {
                    ModelState.AddModelError(string.Empty, "Barcode already exists.");
                    return View(model);
                }

                productMaster.ProductCode = model.ProductCode;
                productMaster.ProductBarcode = model.ProductBarcode;
                productMaster.SerialNo = model.SerialNo;
                productMaster.ProductName = model.ProductName;
                productMaster.ProductDescription = model.ProductDescription;

                productMaster.UpdatedBy = User.Identity.Name;
                productMaster.UpdatedDate = DateTime.Now;

                await _repository.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductMaster productMaster = await _repository.ProductMasters.FindAsync(id);

            _repository.ProductMasters.Remove(productMaster);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}


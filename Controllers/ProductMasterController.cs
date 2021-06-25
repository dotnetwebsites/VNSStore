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

namespace VNSStoreMgmt.Controllers
{
    [Authorize(Roles = "superadmin,admin")]
    public class ProductMasterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductMasterController(ILogger<HomeController> logger,
                                ApplicationDbContext repository,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var productMasters = await _repository.ProductMasters.ToListAsync();
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            ProductMaster productMaster = await _repository.ProductMasters.FindAsync(id);
            return View(productMaster);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }

            ProductMaster productMaster = await _repository.ProductMasters.FindAsync(id);

            return View(productMaster);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductMaster productMaster)
        {
            if (ModelState.IsValid)
            {
                productMaster.UpdatedBy = User.Identity.Name;
                productMaster.UpdatedDate = DateTime.Now;

                _repository.Entry(productMaster).State = EntityState.Modified;
                await _repository.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(productMaster);
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


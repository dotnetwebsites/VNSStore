using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VNSStoreMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VNSStoreMgmt.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using VNSStoreMgmt.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using VNSStoreMgmt.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.IIS;
using VNSStoreMgmt.Interfaces;

namespace VNSStoreMgmt.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IWebHostEnvironment webHostEnvironment,
                            ApplicationDbContext repository,
                            IMailService emailSender,
                            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _repository = repository;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin()
        {
            var user = new ApplicationUser
            {
                EmployeeCode = "A001",
                FirstName = "Admin",
                MiddleName = "",
                LastName = "User",
                PhoneNumber = "9876543210",
                Designation = "",
                DateofJoining = DateTime.Now,
                Gender = "Male",
                Address = "",
                HighestQualification = "",

                UserName = "admin",
                Email = "admin@admin.com",

                IsActive = true
            };

            var codeExists = _userManager.Users.Any(p => p.EmployeeCode == "A01");
            var emailExists = _userManager.Users.Any(p => p.Email == "admin@admin.com");

            if (codeExists)
            {
                return Json("Employee code already exists");
            }

            if (emailExists)
            {
                return Json("Email already exists");
            }

            var result = await _userManager.CreateAsync(user, "Google@123");
            if (result.Succeeded)
            {
                await AddRoles(user, "superadmin");

                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code },
                    protocol: Request.Scheme);

                //if we need to confirm email manually
                await _userManager.ConfirmEmailAsync(user, code);

                return Json("Admin record created");
            }

            return Json("Something went wrong");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OutRecords()
        {
            List<DateTime> dates = _repository.Accountabilities
                .FromSqlRaw("select distinct CAST(OutDate AS date) OutDate from Accountabilities")
                .Select(p => p.OutDate)
                .ToList();

            List<CalendarApi> calendarApis = new List<CalendarApi>();

            foreach (var dt in dates)
            {
                var count = _repository.Accountabilities.Where(p => p.ProductInOut == true && p.OutDate.Date == dt.Date).Count();

                CalendarApi calndarApi = new CalendarApi();

                calndarApi.title = count == 0 ? "All Product Received" : "Product Out: " + count;
                calndarApi.start = dt.ToString("yyyy-MM-dd");
                calndarApi.url = "/ProductEvent/ProductIn?date=" + dt.ToString("yyyy-MM-dd");

                calendarApis.Add(calndarApi);
            }

            if (calendarApis != null && calendarApis.Count() > 0)
                return Json(calendarApis);
            else
                return Json("");
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.FllowUp = "0";
            ViewBag.NotInterest = "0";
            ViewBag.OpenLeads = "0";
            ViewBag.Closed = "0";

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult CreateEBrochures()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PersonalProfile()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index");
            }

            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                model.User = user;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (model.Avatar != null)
                    {
                        string filePath = user.ProfileImage == null ? null : Path.Combine(_webHostEnvironment.WebRootPath, "profileimages/", user.ProfileImage);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        user.ProfileImage = UploadedFile(model);
                    }

                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.DateOfBirth = model.DateOfBirth;
                    user.FatherName = model.FatherName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IsActive = true;

                    if (user.PAN != model.PAN)
                    {
                        user.PAN = model.PAN;
                    }

                    if (user.AadhaarNo != model.AadhaarNo)
                    {
                        user.AadhaarNo = model.AadhaarNo;
                    }

                    if (user.HighestQualification != model.HighestQualification)
                    {
                        user.HighestQualification = model.HighestQualification;
                    }

                    await _repository.SaveChangesAsync();

                    CookieOptions option = new CookieOptions();
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");

                    if (user.ProfileImage != null)
                        Response.Cookies.Append("userProf", path + user.ProfileImage, option);
                    else
                        Response.Cookies.Append("userProf", "", option);

                    if (user.Gender.ToLower() == "male")
                    {
                        Response.Cookies.Append("gender", "0", option);
                    }
                    else if (user.Gender.ToLower() == "female")
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                    else
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                }

                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public IActionResult ManageEmployee()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditEmployee(string id = null)
        {
            if (id == null)
            {
                return NoContent();
            }

            var user = await _userManager.FindByIdAsync(id);
            //var users = _userManager.Users.Where(p => p.EmployeeType == "ChannelHead" || p.EmployeeType == "CHBA");

            //ViewBag.ChannelHeadId = new SelectList(users, "Id", "EmpCodeName", user.ChannelHeadId);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> EditEmployee(ApplicationUser model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.FindByIdAsync(model.Id);
            //var users = _userManager.Users.Where(p => p.EmployeeType == "ChannelHead" || p.EmployeeType == "CHBA");

            if (ModelState.IsValid)
            {
                if (model.Email == "" || model.Email == null)
                {
                    ModelState.AddModelError(string.Empty, "Please enter email address.");
                    //ViewBag.ChannelHeadId = new SelectList(users, "Id", "EmpCodeName", user.ChannelHeadId);
                    return View(model);
                }

                if (model.EmployeeType == "" || model.EmployeeType == null)
                {
                    ModelState.AddModelError(string.Empty, "Please select User type.");
                    return View(model);
                }

                if (user != null)
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //await _userManager.ResetPasswordAsync(user, token, "Abc123#$%");

                    if (user.EmployeeType != model.EmployeeType)
                    {
                        if (user.EmployeeType != null && await _userManager.IsInRoleAsync(user, user.EmployeeType))
                            await _userManager.RemoveFromRoleAsync(user, user.EmployeeType);

                        await AddRoles(user, model.EmployeeType);
                        user.EmployeeType = model.EmployeeType;
                    }

                    if (user.Email != model.Email)
                    {
                        var extuser = await _userManager.FindByEmailAsync(model.Email);
                        if (extuser != null)
                        {
                            ModelState.AddModelError(string.Empty, extuser.Email + " email is already exists, please try another one.");
                            //ViewBag.ChannelHeadId = new SelectList(users, "Id", "EmpCodeName", user.ChannelHeadId);
                            return View(model);
                        }

                        user.Email = model.Email;
                        user.EmailConfirmed = true;
                    }

                    if (user.UserName != model.UserName)
                    {
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                        if (!setUserNameResult.Succeeded)
                        {
                            return View(model);
                        }
                    }

                    user.EmployeeCode = model.EmployeeCode;
                    user.EmployeeType = model.EmployeeType;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Designation = model.Designation;
                    user.DateofJoining = model.DateofJoining;
                    user.IsActive = model.IsActive;
                    user.Gender = model.Gender;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    //user.ChannelHeadId = model.ChannelHeadId;


                    await _repository.SaveChangesAsync();
                    return RedirectToAction("ManageEmployee", _userManager.Users);
                }

                return RedirectToAction("ManageEmployee", _userManager.Users);
            }

            //var users = _userManager.Users.Where(p => p.EmployeeType == "ChannelHead" || p.EmployeeType == "CHBA");

            //ViewBag.ChannelHeadId = new SelectList(users, "Id", "EmpCodeName", user.ChannelHeadId);
            return View(model);
        }

        private async Task AddRoles(ApplicationUser user, string RoleName)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = RoleName
            };

            if (!(await _roleManager.RoleExistsAsync(RoleName)))
            {
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                        await _userManager.AddToRoleAsync(user, RoleName);
                }
            }
            else
            {
                if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                    await _userManager.AddToRoleAsync(user, RoleName);
            }

        }

        [HttpGet]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> SendEmailVerificationCode(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                //var code = await _userManager.GenerateChangeEmailTokenAsync(user, user.Email);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

                string htmlBody = "<p>Dear " + user.FullName + $@"</p>
                                    <p>Your Password is : Abc123#$%</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</<p>";

                _emailSender.SendEmail(
                    Mail.DNR,
                   user.Email,
                   user.FullName,
                   "Confirm your email", htmlBody);
                //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            }

            TempData["StatusMessage"] = "Email has been sent";
            var users = _userManager.Users;
            return RedirectToAction("ManageEmployee", users);
        }


        public async Task<string> ChangeEmail(ApplicationUser oldUser, ApplicationUser model)
        {
            //var user = await _userManager.GetUserAsync(User);
            if (model == null)
            {
                return $"Unable to load user with ID '{model.Id}'.";
            }

            var email = await _userManager.GetEmailAsync(oldUser);
            if (model.Email != email)
            {
                var userId = await _userManager.GetUserIdAsync(model);
                var code = await _userManager.GenerateChangeEmailTokenAsync(model, model.Email);

                var callbackUrl = Url.Page(
                    "~/Identity/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = model.Email, code = code },
                    protocol: Request.Scheme);

                _emailSender.SendEmail(
                    Mail.DNR,
                   model.Email,
                   model.Email,
                   "Confirm your email",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return "mail sent";
            }

            return "Your email is unchanged.";
        }

        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = null;

            if (model.Avatar != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profileimages");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Avatar.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult Privacy(string email, string name, string subject, string htmlBody)
        {
            _emailSender.SendEmail(Mail.DNR, email, name, subject, htmlBody);
            return View();
        }

        [Authorize(Roles = "superadmin")]
        public IActionResult NewEmployee()
        {
            return Redirect("/Identity/Account/Register");
        }

        [HttpGet, ActionName("DeleteEmployee")]
        [Authorize(Roles = "superadmin")]
        public async Task<ActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            return RedirectToAction("ManageEmployee");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


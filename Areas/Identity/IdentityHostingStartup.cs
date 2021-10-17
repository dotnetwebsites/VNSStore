using System;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VNSStoreMgmt.Utilities;

[assembly: HostingStartup(typeof(VNSStoreMgmt.Areas.Identity.IdentityHostingStartup))]
namespace VNSStoreMgmt.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                //string con = Decrypt.TEXTAES(context.Configuration.GetConnectionString("DefaultConnection"));
                string cons = context.Configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(cons));

                services.AddDefaultIdentity<ApplicationUser>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                //     {
                //         options.SignIn.RequireConfirmedAccount = true;
                //         options.Password.RequireLowercase = true;
                //         options.Password.RequireUppercase = true;
                //         options.Password.RequireDigit = true;
                //         options.Password.RequiredLength = 8;                       
                //     })
                //     .AddEntityFrameworkStores<ApplicationDbContext>()
                //     .AddDefaultTokenProviders();

                services.AddMvc(options =>
                    {
                        var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    }).AddXmlSerializerFormatters();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Admin", policy =>
                    policy.RequireUserName("admin"));
                });

            });
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace VNSStoreMgmt.Models
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser User { get; set; }                
    }
}
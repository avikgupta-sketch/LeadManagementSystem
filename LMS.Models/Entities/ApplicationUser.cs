using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace LMS.Models.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public bool IsDeleted { get; set; } 
        public int? ManagerId { get; set; }

        // Navigation Properties
        public ApplicationUser? Manager { get; set; }
        public ICollection<ApplicationUser> Agents { get; set; }
                                     = new List<ApplicationUser>();
        public ICollection<Lead> AssignedLeads { get; set; }
                                     = new List<Lead>();
    }
}
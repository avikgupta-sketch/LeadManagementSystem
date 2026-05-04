using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LMS.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entities;

public class Lead : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public LeadStatus Status { get; set; } = LeadStatus.New;

    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Other;
    

    public int AssignedAgentId { get; set; }

    public int ManagerId { get; set; }   

    public bool IsDeleted { get; set; } = false;

    public ApplicationUser? AssignedAgent { get; set; }

    public ICollection<LeadRemark> Remarks { get; set; } = new List<LeadRemark>();

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}
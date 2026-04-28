using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LMS.Models.Enums;

namespace LMS.Models.Entities;

public class LeadRemark : BaseEntity
{
    public int LeadId { get; set; }

    public int ChangedById { get; set; }

    public string Remark { get; set; } = string.Empty;

    public LeadStatus OldStatus { get; set; }

    public LeadStatus NewStatus { get; set; }

    public Lead? Lead { get; set; }

    public ApplicationUser? ChangedBy { get; set; }
}
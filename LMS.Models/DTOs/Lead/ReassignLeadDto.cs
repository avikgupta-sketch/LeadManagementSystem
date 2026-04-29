using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Models.DTOs.Lead;

public class ReassignLeadDto
{
    public int LeadId { get; set; }
    public int NewAgentId { get; set; }
}

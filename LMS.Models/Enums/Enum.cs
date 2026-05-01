using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Models.Enums
{
    public enum LeadStatus
    {
        New = 1,
        InProgress = 2,
        FollowUp = 3,
        Interested = 4,
        NotInterested = 5,
        Converted = 6,
        Closed = 7,
        Rejected = 8
    }
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
}

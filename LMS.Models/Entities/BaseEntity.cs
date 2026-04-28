using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Models.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public int CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

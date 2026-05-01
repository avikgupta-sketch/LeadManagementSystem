namespace LMS.Models.DTOs.Lead;

// What DataTables sends to the server
public class DataTableRequestDto
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public string? SearchValue { get; set; }
    public string? OrderColumn { get; set; }   // e.g. "Name", "Status"
    public string? OrderDir { get; set; }       // "asc" or "desc"
}

// What the server sends back to DataTables
public class DataTableResponseDto<T>
{
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public List<T> Data { get; set; } = new();
}

// One row in the leads table
public class LeadTableRowDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string AssignedAgent { get; set; } = "";  // Name, not ID
}

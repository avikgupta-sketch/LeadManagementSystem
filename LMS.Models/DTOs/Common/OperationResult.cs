namespace LMS.Models.DTOs.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }

    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(string error) => new() { Success = false, Error = error };
}

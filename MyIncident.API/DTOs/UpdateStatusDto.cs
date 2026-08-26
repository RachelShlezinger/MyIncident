using System.ComponentModel.DataAnnotations;

namespace MyIncident.API.DTOs;

public class UpdateStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string RowVersion { get; set; } = string.Empty;
}

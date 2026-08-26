using System.ComponentModel.DataAnnotations;

namespace MyIncident.API.DTOs;

public class CreateRequestDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "OrganizationName is required.")]
    [MaxLength(150, ErrorMessage = "OrganizationName cannot exceed 150 characters.")]
    public string OrganizationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Priority is required.")]
    public string Priority { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string OpenedBy { get; set; } = string.Empty;
}

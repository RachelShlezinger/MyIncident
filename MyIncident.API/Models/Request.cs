namespace MyIncident.API.Models;

public class Request
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OpenedBy { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string HandlerName { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public RequestPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public uint RowVersion { get; set; }

    // Navigation property
    public Organization Organization { get; set; } = null!;
}

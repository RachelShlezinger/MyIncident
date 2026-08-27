namespace MyIncident.API.Models;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HandlerName { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}

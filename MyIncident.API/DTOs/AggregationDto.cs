namespace MyIncident.API.DTOs;

public class AggregationDto
{
    public int TotalCount { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public Dictionary<string, int> BySubject { get; set; } = new();
}

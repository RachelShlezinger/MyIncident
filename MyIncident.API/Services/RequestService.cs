using Microsoft.EntityFrameworkCore;
using MyIncident.API.Data;
using MyIncident.API.DTOs;
using MyIncident.API.Models;
using MyIncident.API.Repositories;

namespace MyIncident.API.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _repository;
    private readonly AppDbContext _context;

    public RequestService(IRequestRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<PagedResponse<RequestDto>> GetRequestsAsync(RequestQueryParams queryParams)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(queryParams);

        return new PagedResponse<RequestDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        };
    }

    public async Task<AggregationDto> GetAggregationsAsync(RequestQueryParams queryParams)
    {
        return await _repository.GetAggregationsAsync(queryParams);
    }

    public async Task<RequestDto> UpdateStatusAsync(int id, UpdateStatusDto dto)
    {
        if (!Enum.TryParse<RequestStatus>(dto.Status, true, out var newStatus))
            throw new ArgumentException($"Invalid status value '{dto.Status}'. Valid values: New, InProgress, Waiting, Completed, Rejected.");

        var request = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Request with Id {id} not found.");

        // Set the original RowVersion for optimistic concurrency check
        _context.Entry(request).Property(r => r.RowVersion).OriginalValue =
            Convert.FromBase64String(dto.RowVersion);

        request.Status = newStatus;
        request.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(request);
        return MapToDto(updated);
    }

    private static readonly Dictionary<string, string> OrgHandlerMap = new()
    {
        { "פרקליטות", "יוסי כהן" },
        { "הון אנושי", "מירב לוי" },
        { "תקשוב", "אבי ישראלי" },
        { "כספים", "דנה שמעוני" },
        { "לשכה משפטית", "רונית אברהם" },
        { "ביטחון פנים", "עמית גולן" },
        { "מינהל", "שרה דוד" },
        { "דוברות", "נועם פרץ" },
        { "רכש ולוגיסטיקה", "יעל מזרחי" },
        { "הדרכה והשתלמויות", "אורן חיים" }
    };

    public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto)
    {
        if (!Enum.TryParse<RequestPriority>(dto.Priority, true, out var priority))
            throw new ArgumentException($"Invalid priority value '{dto.Priority}'. Valid values: Low, Medium, High.");

        if (!OrgHandlerMap.ContainsKey(dto.OrganizationName))
            throw new ArgumentException($"Invalid organization '{dto.OrganizationName}'.");

        var now = DateTime.UtcNow;
        var request = new Request
        {
            Title = dto.Title,
            Description = dto.Description,
            OpenedBy = dto.OpenedBy,
            OrganizationName = dto.OrganizationName,
            HandlerName = OrgHandlerMap[dto.OrganizationName],
            Status = RequestStatus.New,
            Priority = priority,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _repository.CreateAsync(request);
        return MapToDto(created);
    }

    private static RequestDto MapToDto(Request request)
    {
        return new RequestDto
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            OpenedBy = request.OpenedBy,
            OrganizationName = request.OrganizationName,
            HandlerName = request.HandlerName,
            Status = request.Status.ToString(),
            Priority = request.Priority.ToString(),
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            RowVersion = Convert.ToBase64String(request.RowVersion)
        };
    }
}

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
            uint.Parse(dto.RowVersion);

        request.Status = newStatus;
        request.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(request);
        return MapToDto(updated);
    }

    public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto)
    {
        if (!Enum.TryParse<RequestPriority>(dto.Priority, true, out var priority))
            throw new ArgumentException($"Invalid priority value '{dto.Priority}'. Valid values: Low, Medium, High.");

        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Name == dto.OrganizationName)
            ?? throw new ArgumentException($"Invalid organization '{dto.OrganizationName}'.");

        var now = DateTime.UtcNow;
        var request = new Request
        {
            Title = dto.Title,
            Description = dto.Description,
            OpenedBy = dto.OpenedBy,
            OrganizationId = organization.Id,
            OrganizationName = organization.Name,
            HandlerName = organization.HandlerName,
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
            RowVersion = request.RowVersion.ToString()
        };
    }
}

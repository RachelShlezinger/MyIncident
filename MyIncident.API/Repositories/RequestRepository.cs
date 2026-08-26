using Microsoft.EntityFrameworkCore;
using MyIncident.API.Data;
using MyIncident.API.DTOs;
using MyIncident.API.Models;

namespace MyIncident.API.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _context;

    private static readonly HashSet<string> ValidSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "Title", "OrganizationName", "HandlerName", "Status", "Priority", "CreatedAt", "UpdatedAt"
    };

    public RequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Request> Items, int TotalCount)> GetPagedAsync(RequestQueryParams queryParams)
    {
        var query = _context.Requests.AsNoTracking().AsQueryable();

        query = ApplyFilters(query, queryParams);
        var totalCount = await query.CountAsync();

        query = ApplySorting(query, queryParams);
        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<AggregationDto> GetAggregationsAsync(RequestQueryParams queryParams)
    {
        var query = _context.Requests.AsNoTracking().AsQueryable();
        query = ApplyFilters(query, queryParams);

        var totalCount = await query.CountAsync();

        var byStatus = await query
            .GroupBy(r => r.Status)
            .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var byPriority = await query
            .GroupBy(r => r.Priority)
            .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        // Extract subject (part before " - " in Title)
        var allItems = await query.Select(r => r.Title).ToListAsync();
        var bySubject = allItems
            .Select(t => t.Contains(" - ") ? t.Substring(0, t.IndexOf(" - ")) : t)
            .GroupBy(s => s)
            .ToDictionary(g => g.Key, g => g.Count());

        return new AggregationDto
        {
            TotalCount = totalCount,
            ByStatus = byStatus,
            ByPriority = byPriority,
            BySubject = bySubject
        };
    }

    public async Task<Request?> GetByIdAsync(int id)
    {
        return await _context.Requests.FindAsync(id);
    }

    public async Task<Request> UpdateAsync(Request entity)
    {
        _context.Requests.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Request> CreateAsync(Request entity)
    {
        _context.Requests.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    private static IQueryable<Request> ApplyFilters(IQueryable<Request> query, RequestQueryParams p)
    {
        if (!string.IsNullOrEmpty(p.Status))
        {
            if (!Enum.TryParse<RequestStatus>(p.Status, true, out var status))
                throw new ArgumentException($"Invalid status value '{p.Status}'. Valid values: New, InProgress, Waiting, Completed, Rejected.");
            query = query.Where(r => r.Status == status);
        }

        if (!string.IsNullOrEmpty(p.Priority))
        {
            if (!Enum.TryParse<RequestPriority>(p.Priority, true, out var priority))
                throw new ArgumentException($"Invalid priority value '{p.Priority}'. Valid values: Low, Medium, High.");
            query = query.Where(r => r.Priority == priority);
        }

        if (!string.IsNullOrEmpty(p.OrganizationName))
            query = query.Where(r => r.OrganizationName.Contains(p.OrganizationName));

        if (!string.IsNullOrEmpty(p.HandlerName))
            query = query.Where(r => r.HandlerName.Contains(p.HandlerName));

        if (p.FromDate.HasValue)
            query = query.Where(r => r.CreatedAt >= p.FromDate.Value);

        if (p.ToDate.HasValue)
            query = query.Where(r => r.CreatedAt <= p.ToDate.Value);

        if (!string.IsNullOrEmpty(p.Search))
        {
            var search = p.Search;
            query = query.Where(r =>
                r.Title.Contains(search) || r.OrganizationName.Contains(search));
        }

        return query;
    }

    private static IQueryable<Request> ApplySorting(IQueryable<Request> query, RequestQueryParams p)
    {
        if (!ValidSortFields.Contains(p.SortBy))
            throw new ArgumentException($"Invalid sort field '{p.SortBy}'. Valid fields: {string.Join(", ", ValidSortFields)}.");

        var desc = p.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return p.SortBy.ToLowerInvariant() switch
        {
            "id" => desc ? query.OrderByDescending(r => r.Id) : query.OrderBy(r => r.Id),
            "title" => desc ? query.OrderByDescending(r => r.Title) : query.OrderBy(r => r.Title),
            "organizationname" => desc ? query.OrderByDescending(r => r.OrganizationName) : query.OrderBy(r => r.OrganizationName),
            "handlername" => desc ? query.OrderByDescending(r => r.HandlerName) : query.OrderBy(r => r.HandlerName),
            "status" => desc ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status),
            "priority" => desc ? query.OrderByDescending(r => r.Priority) : query.OrderBy(r => r.Priority),
            "createdat" => desc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            "updatedat" => desc ? query.OrderByDescending(r => r.UpdatedAt) : query.OrderBy(r => r.UpdatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };
    }
}

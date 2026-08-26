using MyIncident.API.DTOs;
using MyIncident.API.Models;

namespace MyIncident.API.Repositories;

public class InMemoryRequestRepository : IRequestRepository
{
    private readonly List<Request> _requests;
    private int _nextId;

    private static readonly HashSet<string> ValidSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "Title", "OrganizationName", "Status", "Priority", "CreatedAt", "UpdatedAt"
    };

    public InMemoryRequestRepository()
    {
        _requests = GenerateSampleData(100);
        _nextId = _requests.Count + 1;
    }

    public Task<(List<Request> Items, int TotalCount)> GetPagedAsync(RequestQueryParams queryParams)
    {
        var query = _requests.AsQueryable();
        query = ApplyFilters(query, queryParams);

        var totalCount = query.Count();

        query = ApplySorting(query, queryParams);
        var items = query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToList();

        return Task.FromResult((items, totalCount));
    }

    public Task<AggregationDto> GetAggregationsAsync(RequestQueryParams queryParams)
    {
        var query = _requests.AsQueryable();
        query = ApplyFilters(query, queryParams);

        var totalCount = query.Count();
        var byStatus = query
            .GroupBy(r => r.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
        var byPriority = query
            .GroupBy(r => r.Priority.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return Task.FromResult(new AggregationDto
        {
            TotalCount = totalCount,
            ByStatus = byStatus,
            ByPriority = byPriority
        });
    }

    public Task<Request?> GetByIdAsync(int id)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(request);
    }

    public Task<Request> UpdateAsync(Request entity)
    {
        var index = _requests.FindIndex(r => r.Id == entity.Id);
        if (index >= 0)
        {
            _requests[index] = entity;
        }
        return Task.FromResult(entity);
    }

    public Task<Request> CreateAsync(Request entity)
    {
        entity.Id = _nextId++;
        entity.RowVersion = BitConverter.GetBytes(entity.Id);
        _requests.Add(entity);
        return Task.FromResult(entity);
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
            query = query.Where(r => r.OrganizationName.Contains(p.OrganizationName, StringComparison.OrdinalIgnoreCase));

        if (p.FromDate.HasValue)
            query = query.Where(r => r.CreatedAt >= p.FromDate.Value);

        if (p.ToDate.HasValue)
            query = query.Where(r => r.CreatedAt <= p.ToDate.Value);

        if (!string.IsNullOrEmpty(p.Search))
        {
            var search = p.Search;
            query = query.Where(r =>
                r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                r.OrganizationName.Contains(search, StringComparison.OrdinalIgnoreCase));
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
            "status" => desc ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status),
            "priority" => desc ? query.OrderByDescending(r => r.Priority) : query.OrderBy(r => r.Priority),
            "createdat" => desc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            "updatedat" => desc ? query.OrderByDescending(r => r.UpdatedAt) : query.OrderBy(r => r.UpdatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };
    }

    private static List<Request> GenerateSampleData(int count)
    {
        var random = new Random(42);
        var statuses = Enum.GetValues<RequestStatus>();
        var priorities = Enum.GetValues<RequestPriority>();
        var orgs = new[] { "מיקרוסופט", "גוגל", "אמזון", "אפל", "מטא", "SAP", "אורקל", "IBM", "סיסקו", "אינטל" };
        var titles = new[] { "בקשה לשדרוג מערכת", "תקלה בשרת", "בקשת גישה", "דיווח על באג", "בקשה להרחבת רישיון", "תמיכה טכנית", "שאלה כללית", "בקשת שינוי", "דיווח אבטחה", "בקשה לאינטגרציה" };

        return Enumerable.Range(1, count).Select(i =>
        {
            var createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 365));
            return new Request
            {
                Id = i,
                Title = $"{titles[random.Next(titles.Length)]} #{i}",
                OrganizationName = orgs[random.Next(orgs.Length)],
                Status = statuses[random.Next(statuses.Length)],
                Priority = priorities[random.Next(priorities.Length)],
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddHours(random.Next(0, 72)),
                RowVersion = BitConverter.GetBytes(i)
            };
        }).ToList();
    }
}

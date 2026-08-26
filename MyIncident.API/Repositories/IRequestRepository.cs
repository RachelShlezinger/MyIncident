using MyIncident.API.DTOs;
using MyIncident.API.Models;

namespace MyIncident.API.Repositories;

public interface IRequestRepository
{
    Task<(List<Request> Items, int TotalCount)> GetPagedAsync(RequestQueryParams queryParams);
    Task<AggregationDto> GetAggregationsAsync(RequestQueryParams queryParams);
    Task<Request?> GetByIdAsync(int id);
    Task<Request> UpdateAsync(Request entity);
    Task<Request> CreateAsync(Request entity);
}

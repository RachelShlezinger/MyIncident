using MyIncident.API.DTOs;

namespace MyIncident.API.Services;

public interface IRequestService
{
    Task<PagedResponse<RequestDto>> GetRequestsAsync(RequestQueryParams queryParams);
    Task<AggregationDto> GetAggregationsAsync(RequestQueryParams queryParams);
    Task<RequestDto> UpdateStatusAsync(int id, UpdateStatusDto dto);
    Task<RequestDto> CreateRequestAsync(CreateRequestDto dto);
}

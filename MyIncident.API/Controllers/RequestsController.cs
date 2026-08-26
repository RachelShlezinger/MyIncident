using Microsoft.AspNetCore.Mvc;
using MyIncident.API.DTOs;
using MyIncident.API.Services;

namespace MyIncident.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<RequestDto>>> GetRequests([FromQuery] RequestQueryParams queryParams)
    {
        var result = await _requestService.GetRequestsAsync(queryParams);
        return Ok(result);
    }

    [HttpGet("aggregations")]
    public async Task<ActionResult<AggregationDto>> GetAggregations([FromQuery] RequestQueryParams queryParams)
    {
        var result = await _requestService.GetAggregationsAsync(queryParams);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<RequestDto>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var result = await _requestService.UpdateStatusAsync(id, dto);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<RequestDto>> CreateRequest([FromBody] CreateRequestDto dto)
    {
        var result = await _requestService.CreateRequestAsync(dto);
        return CreatedAtAction(nameof(GetRequests), new { id = result.Id }, result);
    }
}

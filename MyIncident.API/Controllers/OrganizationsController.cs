using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyIncident.API.Data;
using MyIncident.API.DTOs;

namespace MyIncident.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrganizationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrganizationDto>>> GetAll()
    {
        var organizations = await _context.Organizations
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                HandlerName = o.HandlerName
            })
            .ToListAsync();

        return Ok(organizations);
    }
}

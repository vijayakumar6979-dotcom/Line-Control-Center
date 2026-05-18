using Microsoft.AspNetCore.Http;
using LineControlCenter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LineControlCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BkFctUphController : ControllerBase
{
    private readonly IBkFctUphService _service;

    public BkFctUphController(IBkFctUphService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all UPH records. Filter by customer, family, testStatus, shift, shiftDate.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? customer,
        [FromQuery] string? family,
        [FromQuery] string? testStatus,
        [FromQuery] string? shift,
        [FromQuery] string? shiftDate)
    {
        var result = await _service.GetAllAsync(customer, family, testStatus, shift, shiftDate);
        return Ok(result);
    }

    /// <summary>
    /// Get UPH records by Serial Number.
    /// </summary>
    [HttpGet("{serialNumber}")]
    public async Task<IActionResult> GetBySerialNumber(string serialNumber)
    {
        var result = await _service.GetBySerialNumberAsync(serialNumber);
        if (!result.Any()) return NotFound();
        return Ok(result);
    }
}
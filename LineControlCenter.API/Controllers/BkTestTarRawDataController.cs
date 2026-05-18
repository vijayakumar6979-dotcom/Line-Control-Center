using LineControlCenter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LineControlCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BkTestTarRawDataController : ControllerBase
{
    private readonly IBkTestTarRawDataService _service;

    public BkTestTarRawDataController(IBkTestTarRawDataService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all test records. Filter by customer, testStatus, shift, shiftDate, process.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? customer,
        [FromQuery] string? testStatus,
        [FromQuery] string? shift,
        [FromQuery] string? shiftDate,
        [FromQuery] string? process)
    {
        var result = await _service.GetAllAsync(customer, testStatus, shift, shiftDate, process);
        return Ok(result);
    }

    /// <summary>
    /// Get test records by Serial Number.
    /// </summary>
    [HttpGet("{serialNumber}")]
    public async Task<IActionResult> GetBySerialNumber(string serialNumber)
    {
        var result = await _service.GetBySerialNumberAsync(serialNumber);
        if (!result.Any()) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Get all failed tests (TestStatus = 'F').
    /// </summary>
    [HttpGet("failed")]
    public async Task<IActionResult> GetFailed()
    {
        var result = await _service.GetFailedTestsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get test records with flexible filters.
    /// ShiftDate accepts MM/DD/YYYY format e.g. 01/25/2026
    /// TestStatus: P = Pass, F = Fail. Leave empty to get all except 'A'.
    /// If ShiftDateTo is empty, returns only records on ShiftDateFrom.
    /// </summary>
    [HttpGet("by-filter")]
    public async Task<IActionResult> GetByFilter(
        [FromQuery] string? customer,
        [FromQuery] string? division,
        [FromQuery] string? family,
        [FromQuery] string? testStatus,
        [FromQuery] string? shift,
        [FromQuery] string? shiftDateFrom,
        [FromQuery] string? shiftDateTo)
    {
        if(!string.IsNullOrEmpty(shift))
        {
            var upperShift = shift.Trim().ToUpper();
            if (upperShift != "MORNING" && upperShift != "NIGHT")
                return BadRequest("shift must be 'Morning' or 'Night' only.");
        }



        // --- Validate TestStatus ---
        if (!string.IsNullOrEmpty(testStatus))
        {
            var upper = testStatus.Trim().ToUpper();
            if (upper != "P" && upper != "F")
                return BadRequest("testStatus must be 'P' or 'F' only.");
        }

        // --- Parse ShiftDateFrom ---
        DateOnly? parsedFrom = null;
        if (!string.IsNullOrEmpty(shiftDateFrom))
        {
            if (!DateOnly.TryParseExact(shiftDateFrom, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fromDate))
                return BadRequest("shiftDateFrom format must be MM/DD/YYYY e.g. 01/25/2026");

            parsedFrom = fromDate;
        }

        // --- Parse ShiftDateTo ---
        DateOnly? parsedTo = null;
        if (!string.IsNullOrEmpty(shiftDateTo))
        {
            if (!DateOnly.TryParseExact(shiftDateTo, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var toDate))
                return BadRequest("shiftDateTo format must be MM/DD/YYYY e.g. 01/25/2026");

            parsedTo = toDate;
        }

        // --- Validate date range logic ---
        if (parsedFrom.HasValue && parsedTo.HasValue && parsedTo < parsedFrom)
            return BadRequest("shiftDateTo cannot be earlier than shiftDateFrom.");

        try
        {
            var result = await _service.GetByFilterAsync(
                customer, division, family, testStatus, shift, parsedFrom, parsedTo);

            if (!result.Any())
                return NotFound("No records found for the given filters.");

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
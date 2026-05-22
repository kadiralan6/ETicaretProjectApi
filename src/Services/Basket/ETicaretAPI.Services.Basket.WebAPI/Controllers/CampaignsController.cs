using ETicaretAPI.Services.Basket.Application.Services.CampaignService;
using ETicaretAPI.Services.Basket.Domain.DTOs.CampaignDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Basket.WebAPI.Controllers;

[ApiController]
[Route("api/basket/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] GetCampaignForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _campaignService.GetCampaignsFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _campaignService.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _campaignService.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _campaignService.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _campaignService.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}

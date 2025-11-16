using BeeHive.Contract.Export;
using BeeHive.Domain.BeeGardens;
using Core.App;
using Core.Contract.Executers;
using Microsoft.AspNetCore.Mvc;

namespace BeeHive.Cloud.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController(IConfiguration configuration, ICommandExecuter commandExecuter, IQueryBus queryBus) : ControllerBase
{
    [HttpGet("state/{ExportEntity}")]
    public async Task<IActionResult> ImportHives(ExportEntity exportEntity, [FromQuery] string holding, [FromQuery] string beeGarden,
        [FromHeader(Name = "ExportSecret")] string headerSecret, CancellationToken cancellationToken)
    {
        if (configuration["ExportSecret"] != headerSecret)
            return Unauthorized();

        var state = await queryBus.GetQueryResult<GetExportStateQuery, DateTimeOffset?>(new GetExportStateQuery()
        {
            ExportEntity = exportEntity,
            HoldingUniqueKey = holding,
            BeeGardenUniqueKey = beeGarden,
        }, cancellationToken);

        return Ok(new ExportState
        {
            State = state,
        });
    }

    [HttpPost("hives")]
    public async Task<IActionResult> ImportHives(HiveExportModel[] hives, [FromHeader(Name = "ExportSecret")] string headerSecret, CancellationToken cancellationToken)
    {
        if (configuration["ExportSecret"] != headerSecret)
            return Unauthorized();

        await commandExecuter.ExecuteCommand(new ImportDataCommand()
        {
            Hives = hives
        }, cancellationToken);

        return Ok();
    }

    [HttpPost("hive-medias")]
    public async Task<IActionResult> ImportHiveMedia(HiveMediaExportModel[] media, [FromHeader(Name = "ExportSecret")] string headerSecret, CancellationToken cancellationToken)
    {
        if (configuration["ExportSecret"] != headerSecret)
            return Unauthorized();

        await commandExecuter.ExecuteCommand(new ImportDataCommand()
        {
            HiveMedia = media
        }, cancellationToken);

        return Ok();
    }

    [HttpPost("hive-data")]
    public async Task<IActionResult> ImportHiveData(TimeAggregateSeriesExportModel[] data, [FromHeader(Name = "ExportSecret")] string headerSecret, CancellationToken cancellationToken)
    {
        if (configuration["ExportSecret"] != headerSecret)
            return Unauthorized();

        await commandExecuter.ExecuteCommand(new ImportDataCommand()
        {
            HiveData = data
        }, cancellationToken);

        return Ok();
    }
}
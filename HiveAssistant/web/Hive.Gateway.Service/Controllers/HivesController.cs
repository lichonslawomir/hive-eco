using BeeHive.Contract.Data.Models;
using BeeHive.Contract.Hives;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Hive.Gateway.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hive.Gateway.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HivesController : ControllerBase
    {
        private readonly IHiveService _hiveService;

        public HivesController(IHiveService hiveService)
        {
            _hiveService = hiveService;
        }

        [HttpGet]
        public async Task<IList<HiveDto>> Get([FromQuery] DateTime? start, [FromQuery] DateTime? end, CancellationToken cancellationToken)
        {
            return await _hiveService.ListHives(cancellationToken);
        }

        [HttpGet("{hiveId}/data/{kind}")]
        public async Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId,
            TimeSeriesKind kind,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHiveData(hiveId, kind, start, end, cancellationToken);
        }

        [HttpGet("data/{kind}")]
        public async Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind,
            [FromQuery] int[] hiveId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHivesData(kind, hiveId, start, end, cancellationToken);
        }

        [HttpGet("{hiveId}/aggregate-data/{kind}/{period}")]
        public async Task<IList<TimeSeriesDataModel>> GetHiveAggregateData(int hiveId,
            TimeSeriesKind kind,
            AggregationPeriod period,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHiveData(hiveId, kind, start, end, cancellationToken);
        }

        [HttpGet("aggregate-data/{kind}/{period}")]
        public async Task<IList<TimeSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind,
            AggregationPeriod period,
            [FromQuery] int[] hiveId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHivesData(kind, hiveId, start, end, cancellationToken);
        }
    }
}
using BeeHive.Contract.Aggregate.Models;
using BeeHive.Contract.Data.Models;
using BeeHive.Contract.Hives.Models;
using BeeHive.Contract.Interfaces;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hive.Cloud.Service.Controllers
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

        [HttpGet("{id}")]
        public async Task<HiveDto> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            return await _hiveService.GetHive(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IList<HiveDto>> ListHives(CancellationToken cancellationToken)
        {
            return await _hiveService.ListHives(cancellationToken);
        }

        [HttpGet("{hiveId}/data/{kind}")]
        public async Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId,
            TimeSeriesKind kind,
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHiveData(hiveId, kind, start, end, cancellationToken);
        }

        [HttpGet("data/{kind}")]
        public async Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind,
            [FromQuery] int[] hiveId,
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHivesData(kind, hiveId, start, end, cancellationToken);
        }

        [HttpGet("{hiveId}/last-data/{kind}")]
        public async Task<TimeSeriesDataModel?> GetHiveLastData(int hiveId,
            TimeSeriesKind kind,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHiveLastData(hiveId, kind, cancellationToken);
        }

        [HttpGet("{hiveId}/aggregate-data/{kind}/{period}")]
        public async Task<IList<TimeSeriesDataModel>> GetHiveAggregateData(int hiveId,
            TimeSeriesKind kind,
            AggregationPeriod period,
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end,
            CancellationToken cancellationToken)
        {
            return await _hiveService.GetHiveData(hiveId, kind, start, end, cancellationToken);
        }

        [HttpGet("aggregate-data/{kind}/{period}")]
        public async Task<IList<TimeAggregateSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind,
            AggregationPeriod period,
            [FromQuery] int[] hiveId,
            [FromQuery] DateTimeOffset? start,
            [FromQuery] DateTimeOffset? end,
            CancellationToken cancellationToken)
        {
            var data = await _hiveService.GetHivesAggregateData(kind, period, hiveId, start, end, cancellationToken);
            return data;
        }
    }
}
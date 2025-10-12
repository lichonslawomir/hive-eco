using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Aggregate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Aggragete.Services;

public interface IAggrageteService
{
    Task UpdateAggragetes(DateTime now, string timeZoneId, AggregationPeriod period, CancellationToken stoppingToken);
}

internal class AggrageteService(IBeeHiveDbContext dbContext) : IAggrageteService
{
    public async Task UpdateAggragetes(DateTime now, string timeZoneId, AggregationPeriod period, CancellationToken stoppingToken)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var nowWitOffset = timeZone.ToDateTimeOffset(now);

        var timeSeries = await dbContext.TimeSeries.ToArrayAsync(stoppingToken);
        foreach (var timeSeriesItem in timeSeries)
        {
            var timeAggregateSeriesItem = await dbContext.TimeAggregateSeries
                .FirstOrDefaultAsync(x => x.Period == period && x.TimeSeriesId == timeSeriesItem.Id, stoppingToken);

            DateTime? start = null;
            bool newAggreate = false;
            if (timeAggregateSeriesItem == null)
            {
                newAggreate = true;
                timeAggregateSeriesItem = new TimeAggregateSeries(timeSeriesItem, period);
                await dbContext.TimeAggregateSeries.AddAsync(timeAggregateSeriesItem, stoppingToken);
            }
            else if (timeAggregateSeriesItem.LasteAggregateTime.HasValue)
            {
                start = timeAggregateSeriesItem.LasteAggregateTime.Value;
            }

            if (!start.HasValue)
            {
                start = await dbContext.TimeSeriesData
                    .Where(x => x.TimeSeriesId == timeSeriesItem.Id)
                    .OrderBy(x => x.Timestamp)
                    .Select(x => x.Timestamp)
                    .FirstOrDefaultAsync(stoppingToken);
            }
            if (!start.HasValue)
                continue;

            var startWithOfset = timeZone.ToDateTimeOffset(start.Value);
            foreach (var (from, to) in period.GetPeriods(startWithOfset, nowWitOffset))
            {
                var timeAggregateSeriesData = newAggreate ? null : await dbContext.TimeAggregateSeriesData
                    .FirstOrDefaultAsync(x => x.TimeAggregateSeriesId == timeAggregateSeriesItem.Id && x.Timestamp == from, stoppingToken);

                var q = dbContext.TimeSeriesData
                    .Where(x => x.Timestamp >= from && x.Timestamp < to)
                    .Where(x => x.TimeSeriesId == timeSeriesItem.Id);

                var stats = await q
                    .GroupBy(x => 1)
                    .Select(g => new
                    {
                        Min = g.Min(e => e.Value),
                        Max = g.Max(e => e.Value),
                        Avg = g.Average(e => e.Value),
                        Cnt = g.Count()
                    })
                    .FirstOrDefaultAsync(stoppingToken);

                var med = await q.OrderBy(x => x.Value)
                    .Skip((stats?.Cnt ?? 0) / 2)
                    .Select(x => x.Value)
                    .FirstOrDefaultAsync(stoppingToken);

                if (timeAggregateSeriesData is null)
                {
                    timeAggregateSeriesData = timeAggregateSeriesItem.CreateData(from,
                        stats?.Cnt ?? 0,
                        stats?.Max,
                        stats?.Min,
                        stats?.Avg,
                        med);
                    await dbContext.TimeAggregateSeriesData.AddAsync(timeAggregateSeriesData, stoppingToken);
                }
                else
                {
                    timeAggregateSeriesItem.UpdateData(timeAggregateSeriesData,
                        stats?.Cnt ?? 0,
                        stats?.Max,
                        stats?.Min,
                        stats?.Avg,
                        med);
                }
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
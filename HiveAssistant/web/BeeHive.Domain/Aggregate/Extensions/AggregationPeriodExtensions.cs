namespace BeeHive.Domain.Aggregate.Extensions;

public static class AggregationPeriodExtensions
{
    public static IEnumerable<(DateTime from, DateTime to)> GetPeriods(this AggregationPeriod v, DateTimeOffset start, DateTimeOffset end)
    {
        switch (v)
        {
            case AggregationPeriod.Min5:
                return GetPeriods(
                    new DateTimeOffset(start.Year, start.Month, start.Day, start.Hour, start.Minute - (start.Minute % 5), 0, start.Offset),
                    end, TimeSpan.FromMinutes(5));

            case AggregationPeriod.Min15:
                return GetPeriods(
                    new DateTimeOffset(start.Year, start.Month, start.Day, start.Hour, start.Minute - (start.Minute % 15), 0, start.Offset),
                    end, TimeSpan.FromMinutes(15));

            case AggregationPeriod.Hour:
                return GetPeriods(
                    new DateTimeOffset(start.Year, start.Month, start.Day, start.Hour, 0, 0, start.Offset),
                    end, TimeSpan.FromHours(1));

            case AggregationPeriod.Day:
                return GetPeriods(
                    new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, start.Offset),
                    end, TimeSpan.FromDays(1));

            case AggregationPeriod.Week:
                return GetPeriods(
                    StartOfWeek(start, DayOfWeek.Monday),
                    end, TimeSpan.FromDays(7));

            case AggregationPeriod.Month:
                return GetMonthPeriods(
                    new DateTimeOffset(start.Year, start.Month, 1, 0, 0, 0, start.Offset),
                    end);

            default:
                throw new ArgumentOutOfRangeException($"{nameof(AggregationPeriod)}: {v}");
        }
    }

    private static IEnumerable<(DateTime from, DateTime to)> GetPeriods(DateTimeOffset start, DateTimeOffset end, TimeSpan periodDuration)
    {
        DateTimeOffset to;
        while (start <= end)
        {
            to = start.Add(periodDuration);
            yield return (start.UtcDateTime, to.UtcDateTime);
            start = to;
        }
    }

    private static IEnumerable<(DateTime from, DateTime to)> GetMonthPeriods(DateTimeOffset start, DateTimeOffset end)
    {
        DateTimeOffset to;
        while (start <= end)
        {
            to = start.AddMonths(1);
            yield return (start.UtcDateTime, to.UtcDateTime);
            start = to;
        }
    }

    public static DateTimeOffset StartOfWeek(DateTimeOffset dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static DateTimeOffset ToDateTimeOffset(this TimeZoneInfo timeZone, DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
        return new DateTimeOffset(local, timeZone.GetUtcOffset(utcNow));
    }

    public static DateTimeOffset AdjustTo(this DateTimeOffset dt, AggregationPeriod v)
    {
        switch (v)
        {
            case AggregationPeriod.Min5:
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute - (dt.Minute % 5), 0, dt.Offset);

            case AggregationPeriod.Min15:
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute - (dt.Minute % 15), 0, dt.Offset);

            case AggregationPeriod.Hour:
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Offset);

            case AggregationPeriod.Day:
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, dt.Offset);

            case AggregationPeriod.Week:
                return StartOfWeek(dt, DayOfWeek.Monday);

            case AggregationPeriod.Month:
                return new DateTimeOffset(dt.Year, dt.Month, 1, 0, 0, 0, dt.Offset);

            default:
                throw new ArgumentOutOfRangeException($"{nameof(AggregationPeriod)}: {v}");
        }
    }

    public static DateTimeOffset GetRangeEnd(this DateTimeOffset dt, AggregationPeriod v)
    {
        dt = dt.AdjustTo(v);
        switch (v)
        {
            case AggregationPeriod.Min5:
                return dt.Add(TimeSpan.FromMinutes(5).Add(TimeSpan.FromMilliseconds(-1)));

            case AggregationPeriod.Min15:
                return dt.Add(TimeSpan.FromMinutes(15).Add(TimeSpan.FromMilliseconds(-1)));

            case AggregationPeriod.Hour:
                return dt.Add(TimeSpan.FromHours(1).Add(TimeSpan.FromMilliseconds(-1)));

            case AggregationPeriod.Day:
                return dt.Add(TimeSpan.FromDays(1).Add(TimeSpan.FromMilliseconds(-1)));

            case AggregationPeriod.Week:
                return dt.Add(TimeSpan.FromDays(7).Add(TimeSpan.FromMilliseconds(-1)));

            case AggregationPeriod.Month:
                var f7 = new DateTimeOffset(dt.Year, dt.Month + 1, 1, 0, 0, 0, dt.Offset);
                return f7.Add(TimeSpan.FromMilliseconds(-1));

            default:
                throw new ArgumentOutOfRangeException($"{nameof(AggregationPeriod)}: {v}");
        }
    }
}
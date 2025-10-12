using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Aggregate.Extensions;

namespace BeeHive.Domain.Tests.Aggregate.Extensions;

public class AggregationPeriodExtensionsTests
{
    [Fact]
    public void GetPeriods_Min5_Returns5MinuteSteps()
    {
        var start = new DateTimeOffset(2025, 8, 20, 23, 57, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 00, 20, 0, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Min5.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 20, 21, 55, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 5, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 10, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 15, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 20, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 20, 22, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 5, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 10, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 15, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 20, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 25, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Min5WithWrongRange_ReturnsEmpty()
    {
        var end = new DateTimeOffset(2025, 8, 20, 23, 57, 0, TimeSpan.FromHours(+2));
        var start = new DateTimeOffset(2025, 8, 21, 00, 20, 0, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Min15.GetPeriods(start, end).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetPeriods_Min15_Returns15MinuteSteps()
    {
        var start = new DateTimeOffset(2025, 8, 21, 0, 57, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 2, 5, 0, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Min15.GetPeriods(start, end).ToList();

        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 20, 22, 45, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 15, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 30, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 45, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 0, 00, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 20, 23, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 15, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 30, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 45, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 0, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 0, 15, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Min15WithSameStartAndEnd_ReturnsOneElement()
    {
        var start = new DateTimeOffset(2025, 8, 21, 0, 57, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 0, 57, 0, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Min15.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 20, 22, 45, 0, DateTimeKind.Utc)
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 20, 23, 0, 0, DateTimeKind.Utc)
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Hour_Returns1HourSteps()
    {
        var start = new DateTimeOffset(2025, 8, 21, 0, 57, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 2, 5, 0, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Hour.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 20, 22, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 23, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 0, 00, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 20, 23, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 0, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 1, 00, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Day_Returns1DaySteps()
    {
        var start = new DateTimeOffset(2025, 8, 20, 23, 59, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 0, 0, 1, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Day.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 19, 22, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 20, 22, 00, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 20, 22, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 21, 22, 00, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Week_Returns1WeekSteps()
    {
        var start = new DateTimeOffset(2025, 8, 20, 23, 59, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 8, 21, 0, 0, 1, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Week.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 8, 17, 22, 00, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 24, 22, 00, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void GetPeriods_Month_Returns1MonthSteps()
    {
        var start = new DateTimeOffset(2025, 8, 20, 23, 59, 0, TimeSpan.FromHours(+2));
        var end = new DateTimeOffset(2025, 9, 1, 0, 0, 1, TimeSpan.FromHours(+2));

        var result = AggregationPeriod.Month.GetPeriods(start, end).ToList();
        var resultFrom = result.Select(x => x.from).ToList();
        var resultTo = result.Select(x => x.to).ToList();

        var expectedFrom = new[]
        {
            new DateTime(2025, 7, 31, 22, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 8, 31, 22, 00, 0, DateTimeKind.Utc),
        };
        var expectedTo = new[]
        {
            new DateTime(2025, 8, 31, 22, 00, 0, DateTimeKind.Utc),
            new DateTime(2025, 9, 30, 22, 00, 0, DateTimeKind.Utc),
        };

        Assert.Equal(expectedFrom, resultFrom);
        Assert.Equal(expectedTo, resultTo);
    }

    [Fact]
    public void ToDateTimeOffset_WithUtcNowAndTimeZone_ReturnsCorrectOffset()
    {
        // Arrange
        var utcNow = new DateTime(2025, 8, 20, 12, 0, 0, DateTimeKind.Utc);
        var warsawTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        // Act
        var dto = warsawTimeZone.ToDateTimeOffset(utcNow);

        // Assert
        var expectedOffset = TimeSpan.FromHours(2);
        Assert.Equal(expectedOffset, dto.Offset);
        Assert.Equal(utcNow, dto.UtcDateTime);
        Assert.Equal(utcNow + expectedOffset, dto.LocalDateTime);
        Assert.Equal("2025-08-20T14:00:00.0000000+02:00", dto.ToString("O"));
    }

    [Fact]
    public void ToDateTimeOffset_WithLocalNowAndTimeZone_ThrowException()
    {
        // Arrange
        var utcNow = new DateTime(2025, 8, 20, 12, 0, 0, DateTimeKind.Local);
        var warsawTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => warsawTimeZone.ToDateTimeOffset(utcNow));
    }
}
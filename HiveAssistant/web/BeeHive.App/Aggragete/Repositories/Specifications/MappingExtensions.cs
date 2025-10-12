using BeeHive.Contract.Aggregate.Models;
using BeeHive.Domain.Aggregate;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications;

public static class MappingExtensions
{
    public static TimeAggregateSeriesDataModel ToDto(this TimeAggregateSeriesData e)
    {
        return new TimeAggregateSeriesDataModel()
        {
            Timestamp = e.Timestamp,
            Count = e.Count,
            MaxValue = e.MaxValue,
            MinValue = e.MinValue,
            AvgValue = e.AvgValue,
            MedValue = e.MedValue,
        };
    }

    public static Expression<Func<TimeAggregateSeriesData, TimeAggregateSeriesDataModel>> Map = e => new TimeAggregateSeriesDataModel()
    {
        Timestamp = e.Timestamp,
        Count = e.Count,
        MaxValue = e.MaxValue,
        MinValue = e.MinValue,
        AvgValue = e.AvgValue,
        MedValue = e.MedValue,
    };

    public static TimeAggregateSeriesDataModelEx ToDtoEx(this TimeAggregateSeriesData e)
    {
        return new TimeAggregateSeriesDataModelEx()
        {
            HiveId = e.TimeAggregateSeries.TimeSeries.HiveId,
            Kind = e.TimeAggregateSeries.TimeSeries.Kind,
            Period = e.TimeAggregateSeries.Period,
            Timestamp = e.Timestamp,
            Count = e.Count,
            MaxValue = e.MaxValue,
            MinValue = e.MinValue,
            AvgValue = e.AvgValue,
            MedValue = e.MedValue,
            CreatedOrUpdatedDate = e.CreatedOrUpdatedDate
        };
    }

    public static Expression<Func<TimeAggregateSeriesData, TimeAggregateSeriesDataModelEx>> MapEx = e => new TimeAggregateSeriesDataModelEx()
    {
        HiveId = e.TimeAggregateSeries.TimeSeries.HiveId,
        Kind = e.TimeAggregateSeries.TimeSeries.Kind,
        Period = e.TimeAggregateSeries.Period,
        Timestamp = e.Timestamp,
        Count = e.Count,
        MaxValue = e.MaxValue,
        MinValue = e.MinValue,
        AvgValue = e.AvgValue,
        MedValue = e.MedValue,
        CreatedOrUpdatedDate = e.CreatedOrUpdatedDate
    };
}
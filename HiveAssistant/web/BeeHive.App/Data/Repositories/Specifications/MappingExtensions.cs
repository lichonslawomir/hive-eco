using BeeHive.Contract.Data.Models;
using BeeHive.Domain.Data;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications;

public static class MappingExtensions
{
    public static TimeSeriesDataModel ToDto(this TimeSeriesData e)
    {
        return new TimeSeriesDataModel()
        {
            Timestamp = e.Timestamp,
            Value = e.Value
        };
    }

    public static Expression<Func<TimeSeriesData, TimeSeriesDataModel>> Map = e => new TimeSeriesDataModel()
    {
        Timestamp = e.Timestamp,
        Value = e.Value
    };

    public static TimeSeriesDataModelEx ToDtoEx(this TimeSeriesData e)
    {
        return new TimeSeriesDataModelEx()
        {
            HiveId = e.TimeSeries.HiveId,
            Timestamp = e.Timestamp,
            Value = e.Value
        };
    }

    public static Expression<Func<TimeSeriesData, TimeSeriesDataModelEx>> MapEx = e => new TimeSeriesDataModelEx()
    {
        HiveId = e.TimeSeries.HiveId,
        Timestamp = e.Timestamp,
        Value = e.Value
    };
}
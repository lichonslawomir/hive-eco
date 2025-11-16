namespace BeeHive.App.Extensions;

public static class DataExtensions
{
    public static float? ToNullable(this float value)
    {
        return value == float.NaN ? null : value;
    }

    public static float? ToNullable(this float? value)
    {
        if (!value.HasValue)
            return null;
        return value == float.NaN ? null : value;
    }
}
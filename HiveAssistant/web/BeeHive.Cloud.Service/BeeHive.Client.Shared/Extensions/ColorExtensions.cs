using System.Globalization;

namespace BeeHive.Client.Shared.Extensions;

public static class ColorExtensions
{
    public static string ToHexColor(this int color)
    {
        return $"#{color:X6}".ToLower();
    }

    public static string ToRgbaShadow(this int color, double alpha = 0.3)
    {
        int r = (color >> 16) & 0xFF;
        int g = (color >> 8) & 0xFF;
        int b = (color) & 0xFF;
        return $"rgba({r},{g},{b},{alpha.ToString(CultureInfo.InvariantCulture)})";
    }
}
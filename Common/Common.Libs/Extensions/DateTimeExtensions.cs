namespace Common.Libs.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToUtc(this DateTime source)
    {
        return new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second, DateTimeKind.Utc);
    }
}
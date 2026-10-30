namespace Recorder.Core.Services;

public static class TimeZoneService
{
    public static TimeZoneInfo GetTimeZone(string id)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    public static DateTimeOffset Convert(
        DateTimeOffset dateTime,
        string destinationTimeZoneId)
    {
        TimeZoneInfo timeZone =
            GetTimeZone(destinationTimeZoneId);

        return TimeZoneInfo.ConvertTime(
            dateTime,
            timeZone
        );
    }

    public static DateTimeOffset GetUtcTime(
        DateTime date,
        TimeSpan time,
        TimeZoneInfo timeZone)
    {
        DateTime localDateTime = DateTime.SpecifyKind(
        date.Date + time,
        DateTimeKind.Unspecified
    );

        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(
            localDateTime,
            timeZone
        );

        return new DateTimeOffset(utcDateTime);
    }

    public static string GetIanaTimeZoneId(TimeZoneInfo timeZone)
    {
        if (timeZone.HasIanaId)
            return timeZone.Id;

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(
            timeZone.Id,
            out string? ianaId))
        {
            return ianaId;
        }

        throw new InvalidOperationException(
            $"Error: Cannot convert timezone {timeZone.Id}");
    }
}
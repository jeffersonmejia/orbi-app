namespace Orbi.Web.Helpers;

public static class DateTimeHelper
{
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var now = DateTime.UtcNow;
        var diff = now - dateTime.ToUniversalTime();

        if (diff.TotalSeconds < 60)
            return $"{(int)diff.TotalSeconds}s";

        if (diff.TotalMinutes < 60)
            return $"{(int)diff.TotalMinutes}m";

        if (diff.TotalHours < 24)
        {
            var h = (int)diff.TotalHours;
            var m = diff.Minutes;
            return m > 0 ? $"{h}h {m}m" : $"{h}h";
        }

        if (diff.TotalDays < 30)
        {
            var d = (int)diff.TotalDays;
            return d == 1 ? "1d" : $"{d}d";
        }

        if (diff.TotalDays < 365)
        {
            var months = (int)(diff.TotalDays / 30);
            return months == 1 ? "1mo" : $"{months}mo";
        }

        var years = (int)(diff.TotalDays / 365);
        return years == 1 ? "1y" : $"{years}y";
    }
}

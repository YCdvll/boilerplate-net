namespace Project.Common.Services.Helpers;

public static class InputHelper
{
    public static bool StringFormat(string value, int maxLength, int minLength)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value.Length > maxLength && value.Length < minLength)
            return false;

        return true;
    }

    public static string GetFirstLetter(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Substring(0, 1);
    }

    public static string GetShortName(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            return string.Empty;

        return $"{firstName} {GetFirstLetter(lastName)}";
    }

    public static string GetDateInMinOrHour(DateTime date)
    {
        if (date == null)
            return string.Empty;
        var time = DateTime.Now - date;
        if (time.TotalMinutes < 60)
        {
            if (time.Minutes == 0)
                return "-1min";
            return $"{time.Minutes}min";
        }

        if (time.TotalHours < 24)
            return $"{time.Hours}h";
        return $"{time.Days}jours";
    }
}
namespace TicketAPI.Helpers;

public static class PropertiesHelper
{
    public static bool AreAllPropertiesNull(object obj)
    {
        if (obj == null) return false;

        return obj.GetType()
            .GetProperties()
            .All(p => p.GetValue(obj) == null);
    }

    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
}
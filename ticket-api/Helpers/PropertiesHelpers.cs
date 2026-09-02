namespace Helpers;

public static class PropertiesHelper
{
    public static bool AreAllPropertiesNull(object obj)
    {
        if (obj == null) return false;

        return obj.GetType()
            .GetProperties()
            .All(p => p.GetValue(obj) == null);
    }
}
namespace AnEoT.Uwp.Helpers;

public static class LocalizationHelper
{
    public static string GetLocalized(this string key)
    {
        return ReswHelper.GetReswString(key) ?? null;
    }
}

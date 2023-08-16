using Windows.ApplicationModel.Resources;

namespace AnEoT.Uwp.Helpers;

internal class ReswHelper
{
    public static string GetReswString(string name)
    {
        ResourceLoader loader = new();
        return loader.GetString(name);
    }
}

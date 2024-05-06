namespace Nursing_Home.Utils.Convert;
public static class DataConvert
{
    public static Guid ConvertToGuid(this string @this)
    {
        if (!Guid.TryParse(@this, out var result))
        {
            throw new ArgumentException("The string is not a valid Guid", @this);
        }
        return result;
    }
}
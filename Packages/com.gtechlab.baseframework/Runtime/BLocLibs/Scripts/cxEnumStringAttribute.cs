using System;
using System.Collections.Concurrent;

[AttributeUsage (AttributeTargets.Field, AllowMultiple = false)]
public sealed class cxEnumStringAttribute : Attribute {
    public string strValue;
    public cxEnumStringAttribute (string strValue) {
        this.strValue = strValue;
    }
}

public static class EnumExtensions {
    // Note that we never need to expire these cache items, so we just use ConcurrentDictionary rather than MemoryCache
    private static readonly
    ConcurrentDictionary<string, string> DisplayNameCache = new ConcurrentDictionary<string, string> ();

    public static string EnumString (this Enum value) {
        var key = $"{value.GetType().FullName}.{value}";

        var displayName = DisplayNameCache.GetOrAdd (key, x => {
            var name = (cxEnumStringAttribute[]) value
                .GetType ()
                //.GetTypeInfo()
                .GetField (value.ToString ())
                .GetCustomAttributes (typeof (cxEnumStringAttribute), false);

            return name.Length > 0 ? name[0].strValue : value.ToString ();
        });

        return displayName;
    }
}
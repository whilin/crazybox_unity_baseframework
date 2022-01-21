using UnityEngine;
using System.Collections;

public class LocString
{
    public static string Format(string key)
    {
        string format = Localization.Localize(key);
        if (format == null)
        {
            Logger.Warning("LocString key not found, key:" + key);
            return key;
        }

        return format;
    }

    public static string Format(string key, object o1)
    {
        string format = Localization.Localize(key);
        if (format == null)
        {
            Logger.Warning("LocString key not found, key:" + key);
            return key;
        }

        return string.Format(format, o1);
    }

    public static string Format(string key, object o1, object o2)
    {
        string format = Localization.Localize(key);
        if (format == null)
        {
            Logger.Warning("LocString key not found, key:" + key);
            return key;
        }

        return string.Format(format, o1, o2);
    }

    public static string Format(string key, object o1, object o2, object o3)
    {
        string format = Localization.Localize(key);
        if (format == null)
        {
            Logger.Warning("LocString key not found, key:" + key);
            return key;
        }

        return string.Format(format, o1, o2, o3);
    }


    public static string Format(string key, object o1, object o2, object o3, object o4)
    {
        string format = Localization.Localize(key);
        if (format == null)
        {
            Logger.Warning("LocString key not found, key:" + key);
            return key;
        }

        return string.Format(format, o1, o2, o3,o4);
    }
}

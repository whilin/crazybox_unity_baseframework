using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class cxLocalString 
{
	public static string Get(string key)
	{
		return cxLocalizeManager.Instance.LocText(key);
	}

	public static string Format(string key, object arg0)
	{
		return string.Format(cxLocalizeManager.Instance.LocText(key), arg0);
	}

	public static string Format(string key, object arg0, object arg1)
	{
		return string.Format(cxLocalizeManager.Instance.LocText(key), arg0, arg1);
	}

	public static string Format(string key, object arg0, object arg1, object arg2)
	{
		return string.Format(cxLocalizeManager.Instance.LocText(key), arg0, arg1, arg2);
	}

	public static string Format(string key, object arg0, object arg1, object arg2, object arg3)
	{
		return string.Format(cxLocalizeManager.Instance.LocText(key), arg0, arg1, arg2, arg3);
	}

}

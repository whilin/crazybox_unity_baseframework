using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUIContext
{
	public Dictionary<string, object> showParam;

    public object  defaultParam {
        get {    
            if(showParam.TryGetValue("_default", out object value))
                return value;
            else
                return null;
        }
    }

    public cxUIContext(object param) {
        showParam= new Dictionary<string, object>();
        showParam.Add("_default", param);
    }
}

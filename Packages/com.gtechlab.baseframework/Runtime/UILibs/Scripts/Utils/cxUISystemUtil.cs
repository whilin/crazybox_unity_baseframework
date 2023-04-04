using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class cxUISystemUtil {
    
    public static bool CursorLocked
    {
        get { return Cursor.lockState == CursorLockMode.Locked; }
    }

    public static void SetCursorLock(bool toLock)
    {
#if UNITY_EDITOR

        if (toLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
    }

	public static bool IsMousePointerOnUI(int fingerId=0)
	{
		EventSystem system = UnityEngine.EventSystems.EventSystem.current;

		if (system != null) {
			if (Application.isMobilePlatform) {
				if (Input.touchCount > 0) {
					return system.IsPointerOverGameObject (fingerId);
				}
			} else {
				bool overUI = system.IsPointerOverGameObject();
				return overUI;
			}
		}
		return false;
	}
}

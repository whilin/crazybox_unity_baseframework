using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public static class UIMathLib 
{
	public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
	{
		//Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
		Vector2 uiLocalPoint;

		//Convert the screenpoint to ui rectangle local point
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform,
																screenPos,
																parentCanvas.worldCamera,
																out uiLocalPoint);
		//Convert the local point to world point
		return parentCanvas.transform.TransformPoint(uiLocalPoint);
	}

	public static Vector3 ScreenToUISpace(Canvas parentCanvas, Vector3 worldPos)
	{
		//Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
		Vector3 screenPos = worldPos;//Camera.main.WorldToScreenPoint(worldPos);
		Vector2 uiLocalPoint;

		//Convert the screenpoint to ui rectangle local point
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform,
																screenPos,
																parentCanvas.worldCamera,
																out uiLocalPoint);
		//Convert the local point to world point
		return parentCanvas.transform.TransformPoint(uiLocalPoint);
	}

    public static Vector3 UISpaceToWorld(Canvas parentCanvas, Vector3 uiPos)
    {
        Vector3  screenPoint =  parentCanvas.transform.TransformPoint(uiPos);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);

        return worldPos;

    }
}

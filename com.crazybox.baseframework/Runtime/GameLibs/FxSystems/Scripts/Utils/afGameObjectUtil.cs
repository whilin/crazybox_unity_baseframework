using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class afGameObjectUtil  {

	public static Transform FindChild(Transform model, string name)
	{
		Transform find =	model.Find (name);

		if (find != null)
			return find;

		for (int i = 0; i < model.childCount; i++) {
			find = FindChild (model.GetChild (i), name);
			if (find != null)
				return find;
		}

		return null;
	}

	public static Quaternion LookCameraRotation(Vector3 mypos)
	{
		var campos = Camera.main.transform.position;
		var lookDir = campos - mypos;
		lookDir.y =0;

		return Quaternion.LookRotation(lookDir, Vector3.up);
	}


	public static void FindAniClips(RuntimeAnimatorController aniScript,int layerIndex, ref string [] toClips)
	{
#if UNITY_EDITOR
		var controller = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(aniScript));
		var layer = controller.layers[layerIndex];

		List<string> stateNameList = new List<string>();
		stateNameList.Clear();

		var stateList = layer.stateMachine.states;
		foreach (var s in stateList)
		{
			stateNameList.Add(s.state.name);
		}

		foreach (var sm in layer.stateMachine.stateMachines)
		{
			stateList = sm.stateMachine.states;
			foreach (var s in stateList)
			{
				stateNameList.Add(s.state.name);
			}
		}

		toClips = stateNameList.ToArray ();
#endif
	}
}

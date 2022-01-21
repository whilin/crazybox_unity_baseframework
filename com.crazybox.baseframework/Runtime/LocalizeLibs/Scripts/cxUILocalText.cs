using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUILocalText : MonoBehaviour {

	public string key;
	Text text;

	void OnEnable()
	{
		if(text ==null)
			text = GetComponent<Text>();

		text.text = cxLocalizeManager.Instance.LocText(key);
	}
}

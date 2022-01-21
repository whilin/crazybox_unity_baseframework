using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class cxLocalizeManager :  MonoSingleton<cxLocalizeManager> {

	[SerializeField]
	private cxLocalizeTable locTableAsset;
	[SerializeField]
	private cxLocalizeLanguage defaultLanguage = cxLocalizeLanguage.system;

	public string language {get;set;}
	private Dictionary<string, string> locDic = new Dictionary<string, string>();

	protected override void Awake()
	{
		base.Awake();

		language = PlayerPrefs.GetString("cx_localize_language");

		if(string.IsNullOrEmpty(language) || GetLangIdx(language)==-1)
		{
			if(defaultLanguage != cxLocalizeLanguage.system)
				language = defaultLanguage.ToString();
			else
				FindSystemLanguange();
		}

	#if UNITY_EDITOR
		locTableAsset.LoadFromExcel();
	#endif

		BuildLocTable(language);
	}

	void FindSystemLanguange()
	{
		if(Application.systemLanguage == SystemLanguage.Korean)
			language = cxLocalizeLanguage.kor.ToString();
		else if(Application.systemLanguage == SystemLanguage.Japanese)
			language = cxLocalizeLanguage.jp.ToString();
		else if(Application.systemLanguage == SystemLanguage.Chinese)
			language = cxLocalizeLanguage.china.ToString();
		else
			language = cxLocalizeLanguage.eng.ToString();
	}
	
	public void SetLanguage(string newlang)
	{
		BuildLocTable(newlang);
	}

#if UNITY_EDITOR
	[ContextMenu("Load Excel")]
	void LoadExcel()
	{
		locTableAsset.LoadFromExcel();
		BuildLocTable(language);
	}
#endif

	int GetLangIdx(string lang)
	{
		int idx =-1;

		if(lang == "kor") idx = 0;
		else if(lang == "eng") idx = 1;
		else if(lang == "china") idx = 2;
		else if(lang == "jp") idx = 3;

		return idx;
	}

    private void BuildLocTable(string lang)
	{
		int idx = GetLangIdx(lang);

		if(idx == -1)
		{
			Debug.LogError("Invalid Language Key:"+lang);
			throw new System.Exception("Invalid Language Key:" + lang);
		}
		else
		{
			locDic.Clear();
			
			if(locTableAsset ==null)
				throw new System.Exception("BuildLocTable locTableAsset null");
			
			if(locTableAsset.locTextList ==null)
				throw new System.Exception("BuildLocTable locTableAsset.locTextList null");

			foreach(var locText in locTableAsset.locTextList)
				locDic.Add(locText.key, locText.localText[idx]);
			
			language = lang;
			PlayerPrefs.SetString("cx_localize_language", language);
			PlayerPrefs.Save();
		}
	}

	public string LocText(string key)
	{
		string text = null;
		Instance.locDic.TryGetValue(key, out text);

		if (string.IsNullOrEmpty(text))
			return "loc key:" + key;
		else
			return text;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class afUIImageNumber : MonoBehaviour {

	public Sprite [] numberImages;

	public Image [] numberList;
	
	public void SetNumber(int number)
	{
		string numberStr = number.ToString();

	//	foreach(var m in numberList)
	//		m.gameObject.SetActive(false);

		int i=0;
		for( i=0; i < numberStr.Length;i++)
		{
			char numS = numberStr[i];
			int num = int.Parse(numS+string.Empty);

			numberList[i].sprite = numberImages[num];
			numberList[i].gameObject.SetActive(true);
		}
		for( ; i < numberList.Length;i++)
			numberList[i].gameObject.SetActive(false);
		
	}

	public void SetNumberIncFx(int num, float playTime = 3.0f)
	{
		StartCoroutine(PlayIncFx(num, playTime));
	}

	IEnumerator PlayIncFx(int num,float playTime)
	{
		//float playTime = 3.0f;
		float s = Time.time;
		float n =0;

		do
		{
			n = (Time.time - s) / playTime;
			float v = TweenUtil.Func(TweenFunc.easeInOutQuad, 0, num, n);
			SetNumber((int)(v+0.5f));

			yield return null;
		}
		while( n < 1);

		SetNumber(num);
	}
}

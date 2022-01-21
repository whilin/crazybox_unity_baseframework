//--------------------------------------------
//            NGUI: HUD Text
// Copyright © 2012 Tasharen Entertainment
//--------------------------------------------

using UnityEngine;

public enum DamageEffectType
{
	Normal = 0,
	HeadShot,
	Critical,
	GrenadeFire,
	GrenadeExplode,
	FlashBang,
    Rocket,
    Bonus,
    Time,
}

[AddComponentMenu("NGUI/Examples/HUD Root")]
public class HUDRoot : MonoBehaviour
{
	static public GameObject go;

	GameObject Pivot;

	public int iCountDamageNormal = 0;
	public int iCountDamageHead = 0;
	public int iCountDamageCri = 0;

	public bool bEnemyPos = true;

	void Awake () 
	{ 
		go = gameObject; 

		iCountDamageNormal = 0;
		iCountDamageHead = 0;
		iCountDamageCri = 0;
	}

	static public int GetEffectCount(DamageEffectType type)
	{
		if( type == DamageEffectType.Normal )
			return go.GetComponent<HUDRoot>().iCountDamageNormal;
		if( type == DamageEffectType.HeadShot )
			return go.GetComponent<HUDRoot>().iCountDamageHead;
		if( type == DamageEffectType.Critical )
			return go.GetComponent<HUDRoot>().iCountDamageCri;

		return 0;
	}

	static public int AddEffectCount(DamageEffectType type)
	{
		if( type == DamageEffectType.Normal )
			return go.GetComponent<HUDRoot>().iCountDamageNormal++;
		if( type == DamageEffectType.HeadShot )
			return go.GetComponent<HUDRoot>().iCountDamageHead++;
		if( type == DamageEffectType.Critical )
			return go.GetComponent<HUDRoot>().iCountDamageCri++;

		return 0;
	}

	static public int RemoveEffectCount(DamageEffectType type)
	{
		int count = 0;

		if( type == DamageEffectType.Normal )
		{
			if( --go.GetComponent<HUDRoot>().iCountDamageNormal < 0 )
				go.GetComponent<HUDRoot>().iCountDamageNormal = 0;

			count = go.GetComponent<HUDRoot>().iCountDamageNormal;
		}
		if( type == DamageEffectType.HeadShot )
		{
			if( --go.GetComponent<HUDRoot>().iCountDamageHead < 0 )
				go.GetComponent<HUDRoot>().iCountDamageHead = 0;

			count = go.GetComponent<HUDRoot>().iCountDamageHead;
		}
		if( type == DamageEffectType.Critical )
		{
			if( --go.GetComponent<HUDRoot>().iCountDamageCri < 0 )
				go.GetComponent<HUDRoot>().iCountDamageCri = 0;

			count = go.GetComponent<HUDRoot>().iCountDamageCri;
		}

		return count;
	}
}
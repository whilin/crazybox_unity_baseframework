using UnityEngine;
using System.Collections;

public class HUDRoot_Player : MonoBehaviour
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
			return go.GetComponent<HUDRoot_Player>().iCountDamageNormal;
		if( type == DamageEffectType.HeadShot )
			return go.GetComponent<HUDRoot_Player>().iCountDamageHead;
		if( type == DamageEffectType.Critical )
			return go.GetComponent<HUDRoot_Player>().iCountDamageCri;
		
		return 0;
	}
	
	static public int AddEffectCount(DamageEffectType type)
	{
		if( type == DamageEffectType.Normal )
			return go.GetComponent<HUDRoot_Player>().iCountDamageNormal++;
		if( type == DamageEffectType.HeadShot )
			return go.GetComponent<HUDRoot_Player>().iCountDamageHead++;
		if( type == DamageEffectType.Critical )
			return go.GetComponent<HUDRoot_Player>().iCountDamageCri++;
		
		return 0;
	}
	
	static public int RemoveEffectCount(DamageEffectType type)
	{
		int count = 0;
		
		if( type == DamageEffectType.Normal )
		{
			if( --go.GetComponent<HUDRoot_Player>().iCountDamageNormal < 0 )
				go.GetComponent<HUDRoot_Player>().iCountDamageNormal = 0;
			
			count = go.GetComponent<HUDRoot_Player>().iCountDamageNormal;
		}
		if( type == DamageEffectType.HeadShot )
		{
			if( --go.GetComponent<HUDRoot_Player>().iCountDamageHead < 0 )
				go.GetComponent<HUDRoot_Player>().iCountDamageHead = 0;
			
			count = go.GetComponent<HUDRoot>().iCountDamageHead;
		}
		if( type == DamageEffectType.Critical )
		{
			if( --go.GetComponent<HUDRoot_Player>().iCountDamageCri < 0 )
				go.GetComponent<HUDRoot_Player>().iCountDamageCri = 0;
			
			count = go.GetComponent<HUDRoot_Player>().iCountDamageCri;
		}
		
		return count;
	}
}
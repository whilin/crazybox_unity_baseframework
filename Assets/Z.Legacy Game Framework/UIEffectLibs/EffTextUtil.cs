using UnityEngine;
using System.Collections;

public class EffTextUtil : MonoBehaviour {
	
	public static GameObject SetInstantiateObject(GameObject obj, GameObject parent)
	{
		GameObject instantobj = (GameObject)Instantiate(obj);

		if( parent )
		{
			instantobj.transform.parent = parent.transform;
			instantobj.transform.Translate(parent.transform.position);
		}
		
		return instantobj;
	}
	
	// type 0: damage, 1: heal
    /*
	public static void SetEffectText(Projectile proj, float scale, string txt, DamageEffectType type = DamageEffectType.Normal)
	{
		string prepname = "";
		Color col = Color.red;

		if( type == DamageEffectType.Normal )
		{
			col = Color.yellow;
			prepname = "EffText/HUDText_1";
		}
		else
			if( type == DamageEffectType.HeadShot )
		{
			col = Color.red;
			prepname = "EffText/HUDText_Head";
		}
		else
			if( type == DamageEffectType.Critical )
		{
			col = Color.blue;
			prepname = "EffText/HUDText_Cri";
		}

		//GameObject hudtext = (GameObject)Instantiate(Util.GetManager<Manager_Idle>().goHudText);
		GameObject hudtext = ScriptUtil.PrefabCreate(prepname);
		
		hudtext.transform.parent = HUDRoot.go.transform;
		hudtext.transform.Translate(HUDRoot.go.transform.position);

		// We need the HUD object to know where in the hierarchy to put the element
		if (HUDRoot.go == null)
			return;
		
		DuelCharacterState localPlayer = proj.hitGameObject.GetComponent<DuelCharacterState>();

		float dirZ = 1.0f;
		if( localPlayer != null )
			dirZ = localPlayer.m_platformForward.z;
		
		GameObject temp = ScriptUtil.PrefabCreate("EffText/TempObject");
		temp.transform.localPosition = Vector3.zero;

		temp.transform.position = proj.hitGameObject.transform.position;

		if( proj.shooter.transform.position.x*dirZ > proj.hitGameObject.transform.position.x*dirZ )
			temp.transform.Translate(new Vector3(.3f*dirZ, 1.5f, 0), Space.World);
		else
			temp.transform.Translate(new Vector3(.3f*(-dirZ), 1.5f, 0), Space.World);

		hudtext.transform.localScale = Vector3.one*scale;

		hudtext.GetComponent<DamageEffect>().SetInfo(txt, col, 0f, type);

		UIFollowTarget follwerTarget = hudtext.AddComponent<UIFollowTarget>();
		follwerTarget.target = temp.transform;
		follwerTarget.bAlways = false;

	}
    */

	// type 0: damage, 1: heal
	public static void SetEffectText_2D(GameObject uiRoot, Vector3 screenPos, bool bInverse, string iconName, float scale, string txt, DamageEffectType type = DamageEffectType.Normal)
	{
		if (uiRoot == null)
			return;
		
		string prepname = "";
		Color col = Color.red;
		
		if( type == DamageEffectType.Normal )
		{
			col = Color.yellow;
			prepname = "EffText/HUDText_1";
		}
		else
			if( type == DamageEffectType.HeadShot )
		{
			//col = Color.red;
			prepname = "EffText/HUDText_Head";
		}
		else
			if( type == DamageEffectType.Critical )
		{
			//col = Color.blue;
			prepname = "EffText/HUDText_Cri";
		}
		else
			if( type == DamageEffectType.GrenadeExplode )
		{
			col = Color.white;
			prepname = "EffText/HUDText_GrenadeExplode";
		}
		else
			if( type == DamageEffectType.GrenadeFire )
		{
			col = Color.white;
			prepname = "EffText/HUDText_GrenadeFire";
		}
		else
			if( type == DamageEffectType.FlashBang )
		{
			col = Color.white;
			prepname = "EffText/HUDText_FlashBang";
		}
        else if (type == DamageEffectType.Rocket)
        {
            prepname = "EffText/HUDText_Rocket";
        }
        else if (type == DamageEffectType.Bonus)
        {
            prepname = "EffText/HUDText_Bonus2";
        }


		GameObject hudtext = ScriptUtil.PrefabCreate(prepname);
		string childname = "";

		if( screenPos.x == 0 )
			childname = "Center";
		else
		if( screenPos.x > 0.5f )
		{
			childname = "Right";
			if( bInverse )
				childname = "Left";
		}
		else
		{
			childname = "Left";
			if( bInverse )
				childname = "Right";
		}

		hudtext.transform.parent = uiRoot.transform.Find(childname).transform;
		hudtext.transform.Translate(uiRoot.transform.Find(childname).transform.position);

		hudtext.GetComponent<DamageEffect>().SetInfo(txt, col, 0f, type);
		hudtext.transform.localScale = Vector3.one*scale;
        //hudtext.transform.localScale *= scale;
        //hudtext.transform.localScale *= scale;

		//if( itemcode != 0 && hudtext.GetComponentInChildren<UISprite>() != null )

        if (!string.IsNullOrEmpty (iconName) && hudtext.GetComponentInChildren<UISprite>() != null)
            hudtext.GetComponentInChildren<UISprite>().spriteName = iconName ;
	}


    // type 0: damage, 1: heal
    public static void PlayEffectText_UI(GameObject uiRoot,Vector3 pos, bool bInverse, string iconName, float scale, string txt, DamageEffectType type = DamageEffectType.Normal)
    {
        if (uiRoot == null)
            return;

        string prepname = "";
        Color col = Color.red;

        if (type == DamageEffectType.Normal)
        {
            col = Color.yellow;
            prepname = "EffText/HUDText_1";
        }
        else if (type == DamageEffectType.HeadShot)
        {
            //col = Color.red;
            prepname = "EffText/HUDText_Head";
        }
        else if (type == DamageEffectType.Critical)
        {
            //col = Color.blue;
            prepname = "EffText/HUDText_Cri";
        }
        else if (type == DamageEffectType.GrenadeExplode)
        {
            col = Color.white;
            prepname = "EffText/HUDText_GrenadeExplode";
        }
        else if (type == DamageEffectType.GrenadeFire)
        {
            col = Color.white;
            prepname = "EffText/HUDText_GrenadeFire";
        }
        else if (type == DamageEffectType.FlashBang)
        {
            col = Color.white;
            prepname = "EffText/HUDText_FlashBang";
        }
        else if (type == DamageEffectType.Rocket)
        {
            prepname = "EffText/HUDText_Rocket";
        }
        else if (type == DamageEffectType.Bonus)
        {
            prepname = "EffText/HUDText_Bonus2";
        }
        else if(type == DamageEffectType.Time)
        {
            prepname = "EffText/HUDText_Time";
        }


        GameObject hudtext = ScriptUtil.PrefabCreate(prepname);
        
        hudtext.transform.parent = uiRoot.transform;
        hudtext.transform.position = pos; //.Translate(uiRoot.transform.position);

        hudtext.GetComponent<DamageEffect>().SetInfo(txt, col, 0f, type);
        hudtext.transform.localScale = Vector3.one * scale;
        //hudtext.transform.localScale *= scale;
        //hudtext.transform.localScale *= scale;

        //if( itemcode != 0 && hudtext.GetComponentInChildren<UISprite>() != null )

        if (!string.IsNullOrEmpty(iconName) && hudtext.GetComponentInChildren<UISprite>() != null)
            hudtext.GetComponentInChildren<UISprite>().spriteName = iconName;
    }
}

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public static class ScriptUtil  
{
    public static GameObject FindChildrenWithTag(string tagname, GameObject obj)
    {
		Transform[] Objs = obj.GetComponentsInChildren<Transform>();
		
		foreach(Transform tr in Objs)
		{
			if( tr.CompareTag(tagname) )
			{
				return tr.gameObject;
			}
		}
		
		return null;
	}
	
    public static GameObject PrefabCreate(string prefabpath)
    {
		try
        {
	        Object obj = Resources.Load(prefabpath);
			if (obj == null) return null;
			
	        GameObject prefab = (GameObject) GameObject.Instantiate(obj);
			if (prefab == null) return null;
			
	        prefab.name = obj.name;
			
        	return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

    public static GameObject PrefabCreate(string prefabpath, string prefabparentpath)
    {
		try
        {
	        Object obj = Resources.Load(prefabpath);
			if (obj == null) return null;
			
	        GameObject prefab = (GameObject)GameObject.Instantiate(obj);
			if (prefab == null) return null;
			
	        prefab.name = obj.name;
			
			GameObject parent = GameObject.Find(prefabparentpath);
			if (parent != null)
			{
		        prefab.transform.parent = parent.transform;
		        prefab.transform.Translate(parent.transform.position, Space.World);
			}
			
	        return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

    public static GameObject PrefabCreate(string prefabpath, string prefabparentpath, bool ignoreposition)
    {
		try
        {
	        Object obj = Resources.Load(prefabpath);
			if (obj == null) return null;
			
	        GameObject prefab = (GameObject)GameObject.Instantiate(obj);
			if (prefab == null) return null;
			
	        prefab.name = obj.name;
			
			GameObject parent = GameObject.Find(prefabparentpath);
			if (parent != null)
			{
		        prefab.transform.parent = parent.transform;
				//if (ignoreposition == false) prefab.transform.Translate(parent.position, Space.World);
			}
			
	        return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

    public static GameObject PrefabCreate(string prefabpath, string prefabparentpath, float zposoffset)
    {
		try
	    {
	        Object obj = Resources.Load(prefabpath);
			if (obj == null) return null;
			
	        GameObject prefab = (GameObject)GameObject.Instantiate(obj);
			if (prefab == null) return null;
			
	        prefab.name = obj.name;
			
			GameObject parent = GameObject.Find(prefabparentpath);
			if (parent != null)
			{
		        prefab.transform.parent = parent.transform;
				
				Vector3 pos = prefab.transform.position;
				prefab.transform.position = new Vector3(pos.x, pos.y, pos.z + zposoffset);
				
				//prefab.transform.Translate(parent.position, Space.World);
			}
	
			return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

	public static GameObject PrefabCreate(GameObject prefabobj, string prefabparentpath)
    {
		try
	    {
	        GameObject prefab = (GameObject)GameObject.Instantiate(prefabobj);
			if (prefab == null) return null;
			
	        prefab.name = prefabobj.name;
		
			GameObject parent = GameObject.Find(prefabparentpath);
			if (parent != null)
			{
		        prefab.transform.parent = parent.transform;
				prefab.transform.Translate(parent.transform.position);
			}
			
			return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

	public static GameObject PrefabCreate(GameObject prefabobj, GameObject prefabparentobj)
	{
		try
		{
			GameObject prefab = (GameObject)GameObject.Instantiate(prefabobj);
			if (prefab == null) return null;
			
			prefab.name = prefabobj.name;
			
			GameObject parent = prefabparentobj;
			if (parent != null)
			{
				prefab.transform.parent = parent.transform;
				prefab.transform.Translate(parent.transform.position);
			}
			
            //Ah-hoc, Jongok
            prefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            prefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            /*
            var rt = prefab.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.0f, 0.0f);
            rt.anchorMax = new Vector2(1.0f, 1.0f);
            //rt.anchoredPosition = Vector2.zero;
            prefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            */

            return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
	}

    /*
    public static T PrefabCreate<T>(T prefabobj, GameObject prefabparentobj) where T : MonoBehaviour
    {
        try
        {
            T prefab = (T)Instantiate(prefabobj);
            if (prefab == null) return null;

            prefab.name = prefabobj.name;

            GameObject parent = prefabparentobj;
            if (parent != null)
            {
                prefab.transform.SetParent(parent.transform);
                prefab.transform.Translate(parent.transform.position);
            }

            //Ah-hoc, Jongok
            prefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            prefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            return prefab;
        }
        catch (System.Exception ex)
        {
            Debug.Log("PrefabCreate except : " + ex.Message);
        }
        return null;
    }
    */

        
    public static T PrefabCreate<T>(T prefabobj, GameObject prefabparentobj) where T : MonoBehaviour
    {
        try
        {
            T prefab = (T)GameObject.Instantiate(prefabobj);
            if (prefab == null) return null;

            prefab.name = prefabobj.name;

            GameObject parent = prefabparentobj;
            if (parent != null)
            {
                prefab.transform.SetParent(parent.transform);
                //prefab.transform.Translate(parent.transform.position);
            }

            //Ah-hoc, Jongok
            prefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            prefab.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            return prefab;
        }
        catch (System.Exception ex)
        {
            Debug.Log("PrefabCreate except : " + ex.Message);
        }
        return null;
    }

/*
    public static T PrefabCreateUGUI<T>(T prefabobj, GameObject prefabparentobj) where T : MonoBehaviour
    {
        try
        {
            T prefab = (T)GameObject.Instantiate(prefabobj);
            if (prefab == null) return null;

            prefab.name = prefabobj.name;

            GameObject parent = prefabparentobj;
            if (parent != null)
            {
                prefab.transform.SetParent(parent.transform);
                //prefab.transform.Translate(parent.transform.position);
            }

            var rectT = prefab.GetComponent<RectTransform>();
            //Ah-hoc, Jongok
            rectT.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            rectT.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

			rectT.anchorMin = new Vector2(0,0);
			rectT.anchorMax = new Vector2(1,1);
			rectT.offsetMin = new Vector2(0,0);
			rectT.offsetMax = new Vector2(0,0);

            return prefab;
        }
        catch (System.Exception ex)
        {
            Debug.Log("PrefabCreate except : " + ex.Message);
        }
        return null;
    }
*/
    public static GameObject PrefabCreate(GameObject prefabobj, string prefabparentpath, string createname)
    {
		try
	    {
	        GameObject prefab = (GameObject)GameObject.Instantiate(prefabobj);
			if (prefab == null) return null;
			
	        prefab.name = createname;
			
			GameObject parent = GameObject.Find(prefabparentpath);
			if (parent != null)
			{
		        prefab.transform.parent = parent.transform;
				prefab.transform.Translate(parent.transform.position);
			}
			
	        return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

    public static GameObject PrefabCreate(string prefabpath, GameObject prefabparentobj)
    {
		try
	    {
	        Object obj = Resources.Load(prefabpath);
			if (obj == null) return null;
			
	        GameObject prefab = (GameObject)GameObject.Instantiate(obj);
			if (prefab == null) return null;
			
	        prefab.name = obj.name;
			
			if (prefabparentobj != null)
			{
		        prefab.transform.parent = prefabparentobj.transform;
				prefab.transform.Translate(prefabparentobj.transform.position);
			}
			
	        return prefab;
		}
		catch(System.Exception ex)
		{
			Debug.Log ("PrefabCreate except : " + ex.Message);
		}
		return null;
    }

    public static void SetGameObject(ref GameObject _gobjDestination, GameObject _gobjSource)
    {
        _gobjDestination = _gobjSource;
    }

    public static void SetGameObject(ref GameObject _gobjDestination, string _strSource)
    {
        _gobjDestination = (GameObject)Resources.Load(_strSource);
    }

    public static void GameObject_SetName(GameObject _gobjectGameObject, string _strName)
    {
        _gobjectGameObject.name = _strName;
    }

    public static string ProgressBarTime(float _fTime)
    {
        int m_fHour = (int)(_fTime) / 3600;
        int m_fMinute = (int)(_fTime - m_fHour * 3600) / 60;
        int m_fSeceond = (int)(_fTime  - m_fHour * 3600 - m_fMinute * 60);
		string str="";
        if (m_fHour > 0)
        {
			str += m_fHour.ToString("D") + "h";
			if(m_fMinute>0)
				str += " " + m_fMinute.ToString("D2") + "m";
			if(m_fSeceond>0)
 				str += " " + m_fSeceond.ToString("D2") + "s";
        }
		else
		{
			if(m_fMinute>0)
			{
				str += m_fMinute.ToString("D2") + "m";
				if(m_fSeceond>0)
	 				str += " " + m_fSeceond.ToString("D2") + "s";
			}
			else
				if(m_fSeceond>0)
	 				str += m_fSeceond.ToString("D2") + "s";
		}
		return str;
    }

    public static string ProgressBarTimeEffectText(float _fTime)
    {
        int m_fHour = (int)(_fTime) / 3600;
        int m_fMinute = (int)(_fTime - m_fHour * 3600) / 60;
        int m_fSeceond = (int)(_fTime  - m_fHour * 3600 - m_fMinute * 60);
		string str="";
        if (m_fHour > 0)
        {
			str += m_fHour.ToString("D") + "Z";
			if(m_fMinute>0)
				str += " " + m_fMinute.ToString("D2") + "C";
			if(m_fSeceond>0)
 				str += " " + m_fSeceond.ToString("D2") + "J";
        }
		else
		{
			if(m_fMinute>0)
			{
				str += m_fMinute.ToString("D2") + "C";
				if(m_fSeceond>0)
	 				str += " " + m_fSeceond.ToString("D2") + "J";
			}
			else
				if(m_fSeceond>0)
	 				str += m_fSeceond.ToString("D2") + "J";
		}
		return str;
    }

    public static float ProgressBarTimeValue(float _fTime, float _fMaxTime)
    {
        return _fTime / _fMaxTime;
    }

    public static int ItemIndex(string _strName, string _strItemName)
    {
        return int.Parse(_strName.Substring(_strItemName.Length, _strName.Length - 7 - (_strItemName.Length)));
    }
	
	public static Texture2D TextureCropSquare(Texture2D srcTexture, int iSquareSize)
	{
#if UNITY_EDITOR
		string path = AssetDatabase.GetAssetPath(srcTexture);
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		
		if (textureImporter.isReadable == false)
		{
		   textureImporter.isReadable = true;
		   AssetDatabase.ImportAsset(path);
		}
#endif
		
		int CropSize;
		int CropSizeX=0, CropSizeY=0;
		
		if( srcTexture.width > srcTexture.height )
		{
			CropSize = srcTexture.height;
			CropSizeX=(int)((srcTexture.width-CropSize)/2);
		}
		else
		{
			CropSize = srcTexture.width;
			CropSizeY=(int)((srcTexture.height-CropSize)/2);
		}
		
		//Color[] colors = textureToCrop.GetPixels( (int)cropRect.x, (int)cropRect.y, (int)cropRect.width, (int)cropRect.height );
		//croppedTexture = new Texture2D( (int)cropRect.width, (int)cropRect.height );
		//croppedTexture.SetPixels( colors );
		//croppedTexture.Apply( false );
		
		Color[] colors = srcTexture.GetPixels(CropSizeX, CropSizeY, iSquareSize, iSquareSize);
		Texture2D texture = new Texture2D(iSquareSize, iSquareSize, TextureFormat.ARGB32, false);
		texture.SetPixels(colors);
		texture.Apply(false);
		return texture;
		
		/*
		Texture2D texture = new Texture2D(iSquareSize, iSquareSize, TextureFormat.ARGB32, false);
		
		float fResize = (float)(CropSize/(float)(iSquareSize));
		for(int y = 0 ; y<iSquareSize ; y++)
		{
			for(int x=0 ; x<iSquareSize ; x++)
			{
				texture.SetPixel(x, y, srcTexture.GetPixel((int)(x*fResize)+CropSizeX, (int)(y*fResize)+CropSizeY));
			}
		}
		
		DestroyImmediate(srcTexture);
		texture.Apply();
		
		return texture;
		*/
	}
	/*
	public void LoadPoto(string url, System.Action<Texture2D> result)
	{
		StartCoroutine(_LoadPhoto(url, result));
	}
	
	private IEnumerator _LoadPhoto(string url, System.Action<Texture2D> result)
	{
		WWW www = new WWW(url);
		float elapsedTime = 0.0f;
		
		while (!www.isDone)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= 10.0f) break;
			yield return null;
		}
		
		if (!www.isDone || !string.IsNullOrEmpty(www.error))
		{
			Debug.LogError("Load Failed");
			result(null);
			yield break;
		}
		
		result(www.texture);
	}
	*/
}
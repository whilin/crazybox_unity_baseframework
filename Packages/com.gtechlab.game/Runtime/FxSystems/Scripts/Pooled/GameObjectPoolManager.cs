using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameObjectPoolManager : MonoBehaviour
{
	public class PoolObjectModel
	{
		public GameObject Prefab;
		public string PrefabName;
		public List<PooledObject> All;
        public Stack<PooledObject> Available;
	}
	
	private static Dictionary<string, PoolObjectModel> _poolObjectList=new Dictionary<string, PoolObjectModel>();
	private static GameObjectPoolManager _containerObject=null;
	private static Vector3 _awayFromView=new Vector3(10000.0f, 10000.0f, 10000.0f);
	
	public static PoolObjectModel AddNewPoolObject(GameObject prefab, uint initialCapacity)
	{
        if (prefab.GetComponent<PooledObject>() == null)
        {
            Debug.LogWarning("[GameObjectPoolManager] Create PoolObjectModel Failed, PooledObject Component not found: " + prefab.name );
            throw new Exception("Create PoolObjectModel Failed, PooledObject Component not found");
        }

		PoolObjectModel newPoolObject = new PoolObjectModel();
		
		newPoolObject.Prefab = prefab;
		newPoolObject.PrefabName = prefab.name + "(Clone)";
		newPoolObject.All = (initialCapacity > 0) ? new List<PooledObject>((int) initialCapacity) : new List<PooledObject>();
        newPoolObject.Available = (initialCapacity > 0) ? new Stack<PooledObject>((int)initialCapacity) : new Stack<PooledObject>();
		
		_poolObjectList.Add(newPoolObject.PrefabName, newPoolObject);
		
		Debug.Log("[GameObjectPoolManager] Create PoolObjectModel : " + newPoolObject.PrefabName);
		
		return newPoolObject;
	}
	
	public static int numActive (GameObject prefab)
	{
		PoolObjectModel poolObject = GetPoolObjectByPrefab(prefab);
		
		if(poolObject == null)
			return 0;
		else
			return poolObject.All.Count - poolObject.Available.Count;
	}
	
	public static int numAvailable (GameObject prefab)
	{
		PoolObjectModel poolObject = GetPoolObjectByPrefab(prefab);
		
		if(poolObject == null)
			return 0;
		else
			return poolObject.Available.Count;
	}
	
	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
        if (_containerObject == null)
            CreateContainer();

		PooledObject result;

		PoolObjectModel currentPoolObject = GetPoolObjectByPrefab(prefab);
		
		if(currentPoolObject == null)
		{
			currentPoolObject = AddNewPoolObject(prefab, 10);
		}
		
		if (currentPoolObject.Available.Count == 0)
		{
            GameObject obj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            result = obj.GetComponent<PooledObject>();
			currentPoolObject.All.Add(result);
		}
		else
		{
			result = currentPoolObject.Available.Pop();// as GameObject;
			
			if(result == null)
			{
                GameObject obj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
                result = obj.GetComponent<PooledObject>();
				currentPoolObject.All.Add(result);
			}
			else
			{
				Transform resultTrans = result.transform;
				resultTrans.parent = null;
				resultTrans.position = position;
				resultTrans.rotation = rotation;
                
                //result.SetActive(true);
				//SetActive(result, true);
			}
		}

        if (result != null)
            //result.BroadcastMessage("OnPoolCreate",SendMessageOptions.DontRequireReceiver);
            result.OnPoolCreate();


		
		return result.gameObject;
	}
	
	public static bool PoolDestroy(GameObject targetObject, float delayTime=0.0f)
	{
        PoolObjectModel currentPoolObject = GetPoolObjectByPrefab(targetObject);
		
		if(currentPoolObject !=null)
		{
            PooledObject target = targetObject.GetComponent<PooledObject>();

            if (!currentPoolObject.Available.Contains(target))
			{
				if(delayTime > 0.0f)
				{
                    _containerObject.DelayPoolDestroy(targetObject, delayTime);
				}
				else
				{
                    currentPoolObject.Available.Push(target);
					
					//SetActive(target, false);
                    //target.SetActive(false);
					
					if(_containerObject == null)
						CreateContainer();
					
					target.transform.parent = _containerObject.transform;
					target.transform.position = _awayFromView;
					
					//target.BroadcastMessage("OnPoolDestroy", SendMessageOptions.DontRequireReceiver);
                    target.OnPoolDestroy();
					return true;
				}
			}
		}
		else
		{
			if(delayTime > 0.0f)
                GameObject.Destroy(targetObject, delayTime);
			else
                GameObject.Destroy(targetObject);
			
			return true;
		}
		
		return false;
	}
	
	/*
	public static void DestroyAll()
	{
		foreach(PoolObjectModel poolObject in _PoolObjectList)
		{
			for (int i=0; i<poolObject.All.Count; i++)
			{
				GameObject target = poolObject.All[i] as GameObject;
				
				if (target.active)
					Destroy(target);
			}
		}
	}
	
	// Unspawns all the game objects and clears the pool.
	public static void Clear() 
	{
		DestroyAll();
		foreach(PoolObjectModel poolObject in _PoolObjectList)
		{
			poolObject.Available.Clear();
			poolObject.All.Clear();
		}
	}
	*/
	
	public static void DestroyAll()
	{	
		_containerObject.ClearContainer();
        
		foreach(PoolObjectModel poolObject in _poolObjectList.Values)
		{
			for (int i=0; i<poolObject.All.Count; i++)
			{
				PooledObject target = poolObject.All[i];// as GameObject;
				if(target !=null)
					GameObject.Destroy(target.gameObject);
			}
			
			Debug.Log(string.Format("[GameObjectPoolManager] Destroy Object Pool:{0}, Pool Size:{1}, InUse:{2}", poolObject.PrefabName, poolObject.All.Count, poolObject.Available.Count));
			
			poolObject.Available.Clear();
			poolObject.All.Clear();
		}

        

		GameObject.Destroy(_containerObject.gameObject);
		_containerObject = null;
	}
	
	/*
	public static PoolObjectModel DoesPoolObjectExist(GameObject prefabObject)
	{
		bool prefabExists = false;
		string prefabName;
		PoolObjectModel _poolObjectModel=null;
		
		if(prefabObject.name.Contains("(Clone)"))
			prefabName = prefabObject.name;
		else
			prefabName = prefabObject.name + "(Clone)";
		
		foreach(PoolObjectModel poolObject in _PoolObjectList)
		{
			if ( poolObject.PrefabName == prefabName)
			{
				_poolObjectModel = poolObject;
				prefabExists = true;
				break;
			}
		}
		
		return _poolObjectModel;
	}
	*/


	protected static PoolObjectModel GetPoolObjectByPrefab(GameObject prefabObject)
	{
		string prefabName;
		
		if(prefabObject.name.Contains("(Clone)"))
			prefabName = prefabObject.name;
		else
			prefabName = prefabObject.name + "(Clone)";
				
		if(_poolObjectList.ContainsKey(prefabName))
			return _poolObjectList[prefabName];
		else
			return null;
	}
	
    /*
	protected static void SetActive(GameObject target, bool active)
	{
		//target.SetActive(active);
		RecursiveDeepActivation(target.transform, active);
	}
	
    protected static void RecursiveDeepActivation(Transform gameObjectTransform, bool activate)
	{
		foreach (Transform t in gameObjectTransform)
		{
			RecursiveDeepActivation (t, activate);
		}
		
		gameObjectTransform.gameObject.SetActive(activate);
	}
	*/

	static void CreateContainer()
	{
		if(_containerObject == null)
		{
			_containerObject = new GameObject("_GameObjectPoolManager", typeof(GameObjectPoolManager)).GetComponent<GameObjectPoolManager>();
			_containerObject.transform.position = _awayFromView;
		}
	}
	
	void ClearContainer()
	{
		StopAllCoroutines();
	}
	
	/*
	 *  Level unload event receiver
	 */
	void OnDestroy()
	{
		GameObjectPoolManager.DestroyAll();
	}
	
	public void DelayPoolDestroy(GameObject obj, float time)
	{
		StartCoroutine(DelayPoolDestroyCoroutine (obj, time));
	}

    //static int s_id = 0;

	IEnumerator DelayPoolDestroyCoroutine(GameObject obj, float time)
	{
        //int id = s_id++;

        //Debug.Log("DelayPoolDestroy Begin:"+id +"_"+ obj.name);
		yield return new WaitForSeconds(time);

        //Debug.Log("DelayPoolDestroy End:" + id);
		PoolDestroy(obj);
	}
}
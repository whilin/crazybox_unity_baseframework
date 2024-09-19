using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class cxPoolObject : MonoBehaviour {
    public abstract void OnPoolCreate();
    public abstract void OnPoolStart();
    public abstract void OnPoolStop();
    public abstract void OnPoolDestroy();
}

public class cxInstanceContainer : MonoBehaviour {

	public cxPoolObject prefabItem;
	public GameObject containerRoot;
	
	List<cxPoolObject> m_itemList = new List<cxPoolObject>();
    List<cxPoolObject> m_pooled = new List<cxPoolObject>();

	public List<cxPoolObject> list { get { return m_itemList; } }

	// public void Init()
	// {
	// 	for (int j = 0; j < m_itemList.Count; j++)
	// 	{
	// 		GameObject.DestroyImmediate(m_itemList[j].gameObject);
	// 	}
	// 	m_itemList.Clear();
	// }

    public cxPoolObject Instantiate(){

        cxPoolObject instance ;
        if(m_pooled.Count > 0){
            instance = m_pooled[0];
            m_pooled.RemoveAt(0);
        } else {
            instance = AddChild(containerRoot, prefabItem.gameObject).GetComponent<cxPoolObject>();
            instance.OnPoolCreate();
        }
        instance.gameObject.SetActive(true);
        instance.OnPoolStart();

        return instance;
    }

    public void ReturnObject(cxPoolObject pooledObject ){
        pooledObject.OnPoolStop();
        pooledObject.gameObject.SetActive(false);
        m_pooled.Add(pooledObject);
    }

	// public void BeginListBuild(bool sort=true)
	// {
		
	// }

	// public GameObject GetItemListObject(int i)
	// {
	// 	if (i < m_itemList.Count)
	// 		return m_itemList[i];
	// 	else
	// 	{
	// 		GameObject go = AddChild(containerRoot.gameObject, prefabItem) as GameObject;

	// 		go.name = (100 + i).ToString();
	// 		m_itemList.Add(go);
	// 		return go;
	// 	}
	// }

	// public T GetItemListObject<T>(int i) where T : MonoBehaviour
	// {
	// 	if (i < m_itemList.Count)
	// 		return m_itemList[i].GetComponent<T>();
	// 	else
	// 	{
	// 		GameObject go = AddChild(containerRoot.gameObject, prefabItem) as GameObject;

	// 		go.name = (100 + i).ToString();
	// 		m_itemList.Add(go);
	// 		return go.GetComponent<T>();
	// 	}
	// }

	// // public void AddDummyObject(int count)
	// // {
	// // 	for (int j = 0; j < m_dummyList.Count; j++)
	// // 	{
	// // 		GameObject.Destroy(m_dummyList[j]);
	// // 	}

	// // 	for (int i = 0; i < count; i++)
	// // 	{
	// // 		GameObject go = AddChild(containerRoot.gameObject, prefabDummy) as GameObject;

	// // 		go.name = (10000 + i).ToString();
	// // 		m_dummyList.Add(go);
	// // 	}
	// // }


	// public void EndListBuild(int used, bool bRefresh)
	// {
	// 	ClearUnusedItemList(used);

	// 	Refesh(bRefresh);
	// }


	// public void ClearUnusedItemList(int used)
	// {
	// 	if (used < m_itemList.Count)
	// 	{
	// 		int remove = 0;
	// 		for (int j = used; j < m_itemList.Count; j++)
	// 		{
	// 			GameObject.Destroy(m_itemList[j]);
	// 			remove++;
	// 		}

	// 		if (remove > 0)
	// 			m_itemList.RemoveRange(used, remove);
	// 	}

	// 	// if (prefabDummy != null && lineCount > 0)
	// 	// {
	// 	// 	int need =  lineCount - m_itemList.Count;
	// 	// 	need = Mathf.Max(need, 0);
	// 	// 	if (m_dummyList.Count > need)
	// 	// 	{
	// 	// 		int remove = 0;
	// 	// 		for (int j = need; j < m_dummyList.Count; j++)
	// 	// 		{
	// 	// 			GameObject.Destroy(m_dummyList[j]);
	// 	// 			remove++;
	// 	// 		}

	// 	// 		if (remove > 0)
	// 	// 			m_dummyList.RemoveRange(need, remove);
	// 	// 	}
	// 	// 	else
	// 	// 	{
	// 	// 		for (int j = m_dummyList.Count; j < need; j++)
	// 	// 		{
	// 	// 			GameObject go = AddChild(containerRoot.gameObject, prefabDummy);
	// 	// 			//go.name = (900 + j).ToString();
	// 	// 			go.name = "9999999";
	// 	// 			m_dummyList.Add(go);
	// 	// 		}
	// 	// 	}
	// 	// }
	// }

	// public void Refesh(bool bRefresh = true)
	// {
	// 	// gridLayout.SetLayoutHorizontal ();
	// 	// gridLayout.SetLayoutVertical ();
		
	// 	LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) containerRoot.transform);
	// 	//LayoutRebuilder.MarkLayoutForRebuild((RectTransform) gridLayout.transform);
	// }

	static public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = Instantiate (prefab) as GameObject;
		int layer = -1;

//		#if UNITY_EDITOR
//		if (undo && !Application.isPlaying)
//			UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
//		#endif
		if (parent != null)
		{
			Transform t = go.transform;
			t.SetParent(parent.transform);
			//t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			if (layer == -1) go.layer = parent.layer;
			else if (layer > -1 && layer < 32) go.layer = layer;
		}
		return go;
	}
}

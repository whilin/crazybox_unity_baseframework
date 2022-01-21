using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIItemListUtil
{
    public GameObject m_thisPanel;
    public GameObject m_prefabItem;
    public GameObject m_prefabDummy;
    public int m_lineCount;

    List<GameObject> m_itemList = new List<GameObject>();
    List<GameObject> m_dummyList = new List<GameObject>();

    public List<GameObject> list { get { return m_itemList; } }

	public void Init()
	{
		for (int j = 0; j < m_itemList.Count; j++)
		{
			//list.Remove(list[j]);
			GameObject.DestroyImmediate(m_itemList[j]);
		}
		m_itemList.Clear();
	}
	
	public UIItemListUtil()
	{
		
	}
	
	public UIItemListUtil(GameObject thisPanel, GameObject prefabItem)
    {
        m_prefabItem = prefabItem;
        m_thisPanel = thisPanel;
    }
    
    public void BeginListBuild(bool sort=true)
    {
        //m_thisPanel.GetComponent<UIGrid>().sorted = sort;
        // m_thisPanel.GetComponent<UIGrid>().sorting =

        if (!sort)
            m_thisPanel.GetComponent<UIGrid>().sorting = UIGrid.Sorting.None;
    }

    public GameObject GetItemListObject(int i)
    {
        if (i < m_itemList.Count)
            return m_itemList[i];
        else
        {
            GameObject go = NGUITools.AddChild(m_thisPanel, m_prefabItem);
            go.name = (100 + i).ToString();
            m_itemList.Add(go);
            return go;
        }
    }

    public void EndListBuild(int used, int focusIndex = -1)
    {
        ClearUnusedItemList(used);

        Focus(focusIndex);

        Refesh();
    }

	public void EndListBuild(int used, bool bRefresh)
	{
		ClearUnusedItemList(used);
		
		Refesh(bRefresh);
	}

    public void ClearUnusedItemList(int used)
    {
        if (used < m_itemList.Count)
        {
            int remove = 0;
            for (int j = used; j < m_itemList.Count; j++)
            {
                GameObject.Destroy(m_itemList[j]);
                remove++;
            }

            if (remove > 0)
                m_itemList.RemoveRange(used, remove);
        }

        if (m_prefabDummy != null && m_lineCount > 0)
        {
            int need = m_lineCount - m_itemList.Count;
            need = Mathf.Max(need, 0);
            if (m_dummyList.Count > need)
            {
                int remove = 0;
                for (int j = need; j < m_dummyList.Count; j++)
                {
                    GameObject.Destroy(m_dummyList[j]);
                    remove++;
                }

                if (remove > 0)
                    m_dummyList.RemoveRange(need, remove);
            }
            else
            {
                for (int j = m_dummyList.Count; j < need; j++)
                {
                    GameObject go = NGUITools.AddChild(m_thisPanel, m_prefabDummy);
                    //go.name = (900 + j).ToString();
                    go.name = "9999999";
                    m_dummyList.Add(go);
                }
            }
        }
    }

    public void Focus(int index = -1)
    {
        if (index >= 0)
        {
            float pos = 0.0f;
            int count = m_dummyList.Count + m_itemList.Count;
            int last = count - index;

            if (index < m_lineCount)
                pos = 0.0f;
            //else if (last < m_lineCount)
            //    pos = (float)(index - last) / (float) count;
            else
                pos = (float)(index + 1) / (float)count;

            m_thisPanel.transform.parent.GetComponent<UIScrollView>().MoveRelative(new Vector3(0, pos, 0));//.relativePositionOnReset.y = pos;
        }
        else
        {
             m_thisPanel.transform.parent.GetComponent<UIScrollView>().MoveRelative(new Vector3(0, 0, 0)); //relativePositionOnReset.y = 0.0f;
        }
    }

    public void Refesh(bool bRefresh = true)
    {
		if( bRefresh )
		{
	        m_thisPanel.transform.parent.GetComponent<UIPanel>().Refresh();
	        m_thisPanel.GetComponent<UIGrid>().Reposition();

	        m_thisPanel.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		}
    }
}
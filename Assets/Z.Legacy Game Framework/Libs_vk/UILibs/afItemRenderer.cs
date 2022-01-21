using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class afItemRenderer : MonoSingleton<afItemRenderer> 
{
	public RenderTexture m_itemRenderTexture;
	public Camera m_itemCamera;
	public cxUITweenFx m_itemFx;
	public Transform m_attachT;

	GameObject m_itemObject;

	public RenderTexture itemTexture { get { return m_itemRenderTexture;}}

	// Use this for initialization
	void Start () 
	{
		EndRenderItem();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BeginRenderItem(GameObject prefab)
	{
		Vector3 view = -m_itemCamera.transform.forward;

		GameObject obj = GameObject.Instantiate(prefab, m_attachT.position, Quaternion.LookRotation(view) );
		obj.transform.SetParent(m_attachT);

		m_itemObject = obj;
		m_itemCamera.enabled = true;

		m_itemFx.Play();
	}

	public void EndRenderItem()
	{
		m_attachT.DestroyChildren();
		m_itemCamera.enabled = false;

		m_itemFx.Stop(true);
	}
	
}

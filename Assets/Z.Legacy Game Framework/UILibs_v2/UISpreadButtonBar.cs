using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UISpreadButtonBar : MonoBehaviour
{
    public class ButtonState
    {
        public Button button;
        public GameObject anchor;
        public Text label;
        public Image sprite;

		public Action clickCallback;
    }

    public Button mainButton;
    public Image hideSprite;
    public Image showSprite;

    public Transform gridAnchor1;
    public Transform gridAnchor2;
    public float gridDistance;
    public float showTime;
    public Button[] buttons;

	public bool isShow { get; private set; }
    private List<ButtonState> anchorList = new List<ButtonState>();
    
	//public bool autoHide=true;
	public bool foldAtStart = true;
	public bool foldAtClick = true;

	public UnityEngine.Events.UnityEvent OnSpreadCompleted=new UnityEngine.Events.UnityEvent();
	public UnityEngine.Events.UnityEvent OnFoldCompleted=new UnityEngine.Events.UnityEvent();

    void Awake()
    {
        mainButton.onClick.AddListener(OnMainButton);

		foreach (var b in buttons)
			AddButton (b);
    }

    IEnumerator Start()
    {
		yield return null;

		if (foldAtStart)
			HideList (true);
		else
			ShowList (true);
    }

    public void AddButton(Button button)
    {
        var anchor = new GameObject();
        anchor.transform.SetParent(gameObject.transform);
        anchor.transform.localPosition = Vector3.zero;
        anchor.transform.localScale = Vector3.one;

        button.transform.SetParent(anchor.transform);
        button.transform.position = anchor.transform.position;
        button.transform.localPosition = Vector3.zero;

        button.onClick.AddListener(OnButtonClicked);
        ButtonState s = new ButtonState();
        s.button = button;
        s.anchor = anchor;
        s.label = button.GetComponentInChildren<Text>();
        s.sprite = button.GetComponentInChildren<Image>();
        anchorList.Add(s);
    }
    
	public ButtonState GetButtonItem(int index)
	{
		if (index >= anchorList.Count)
			return null;
		
		return anchorList [index];
	}

	public void ResetButtonState(int activeCount)
	{
		for (int i = 0; i < anchorList.Count; i++) {
			var s = anchorList [i];

			s.button.gameObject.SetActive (i <= (activeCount-1));
			s.clickCallback = null;
		}
	}

    void OnButtonClicked()
    {
		if(foldAtClick)
        	HideList();

		var eventObject = EventSystem.current.currentSelectedGameObject;

		foreach (var b in anchorList) {
			if (b.button.gameObject == eventObject) {
				//TODO call...	

				if (b.clickCallback != null)
					b.clickCallback ();
			}
		}
	}

    void OnMainButton()
    {
        if (isShow)
            HideList();
        else
            ShowList();
    }

    public void Refresh()
    {
        if (isShow)
            ShowList(true);
        
    }

    public void ShowList(bool reset = false)
    {
        isShow = true;

        Vector3 dir = gridAnchor2.position - gridAnchor1.position;
        dir.Normalize();

//		List<ButtonState> activeList = new List<ButtonState> ();
//
//		for (int i = 0; i < anchorList.Count; i++)
//		{
//			var state = anchorList [i];
//
//			if (!state.button.gameObject.activeSelf)
//				continue;
//			
//			activeList.Add (state);
//		}

        int count = 0;
		for (int i = 0; i < anchorList.Count; i++)
        {
			var state = anchorList[i];

            if (!state.button.gameObject.activeSelf)
                continue;

            if (reset)
            {
                state.anchor.transform.position = gridAnchor1.position;
            }

			if(state.label !=null)
            	UIEffectUtil.ShowScale(state.label.gameObject, true, 0, true, true, TweenFunc.easeInOutQuad);
			
            UIEffectUtil.ShowScale(state.anchor.gameObject, false, 0.0f, true, true, TweenFunc.easeInOutQuad, 0.3f);


            Vector3 pos = new Vector3();
            pos = gridAnchor1.localPosition + dir * gridDistance * (count);

            
            Hashtable h = new Hashtable();

            h.Add("name", gameObject.name);
            h.Add("position", pos);
            h.Add("isLocal", true);
            
            h.Add("time", showTime);
            
			//if (count == 0)
			{
				h.Add ("oncomplete", "OnShowButtonComplete");
				h.Add ("oncompletetarget", gameObject);
				h.Add ("oncompleteparams", state);
			}

            //iTween.MoveTo(anchorList[i], pos, showTime );

            iTween.MoveTo(state.anchor, h);
            //iTween.ScaleTo(state.anchor, Vector3.one, 1.0f);
            count++;
        }

		if (showSprite)
			showSprite.gameObject.SetActive(true);
		
		if (hideSprite)
			hideSprite.gameObject.SetActive(false);
    }
    
    public void HideList(bool reset=false)
    {
        isShow = false;
       // Vector3 targetPos = gridAnchor1.localPosition;
        Vector3 targetPos = mainButton.transform.localPosition;// gridAnchor1.localPosition;

        int count = 0;
        for (int i = 0; i < anchorList.Count; i++)
        {
            var state = anchorList[i];

            if (reset)
            {
                state.anchor.transform.localPosition = targetPos;
				if(state.label !=null)
                	state.label.transform.localScale = Vector3.zero;
				
                state.anchor.transform.localScale = Vector3.zero;

            }
            else
            {
                state.button.interactable = false;

				if(state.label !=null)
                	UIEffectUtil.HideScale(state.label.gameObject, true, 0, true, true, TweenFunc.easeInOutQuad);
				
                UIEffectUtil.HideScale(state.anchor.gameObject, false, 0.5f, true, true, TweenFunc.easeInOutQuad, 0.3f);
                
                Hashtable h = new Hashtable();
                h.Add("name", gameObject.name);
                h.Add("position", targetPos);
                h.Add("isLocal", true);

				//if (count == 0)
				{
					h.Add ("oncomplete", "OnHideButtonComplete");
					h.Add ("oncompletetarget", gameObject);
					h.Add ("oncompleteparams", state);
				}

				h.Add("time", showTime);
                iTween.MoveTo(state.anchor, h);
            }
            
            count++;
        }

		if (showSprite)
			showSprite.gameObject.SetActive(false);
		if (hideSprite)
			hideSprite.gameObject.SetActive(true);
    }

    void OnShowButtonComplete(ButtonState param)
    {
        param.button.interactable = true;

		OnSpreadCompleted.Invoke ();
	}

    void OnHideButtonComplete(ButtonState param)
    {
		OnFoldCompleted.Invoke ();
    }
}

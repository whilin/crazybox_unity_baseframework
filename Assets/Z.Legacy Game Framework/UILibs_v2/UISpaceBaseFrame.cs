using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpaceBaseFrame : MonoBehaviour 
{
    public enum SpacePanelType
    {
        Object,
        Space,
        Ground
    }

    [System.Serializable]
    public class UIPanel
    {
        public SpacePanelType type;
        public cxUITweenPanel panel;

        public bool showAtStart = false;

        [Header(" - Object Panel Properties")]
        public Transform objectAnchor;
		public Vector3 viewAngleLimit;
        public Vector3 offsetPos;
		public Vector3 offsetView;

        [Header(" - Space Panel Properties")]
        public bool lookAtOnce;
        public bool stableVertical = true;
	}

	[System.Serializable]
	public class ScreenLimit
	{
		public bool use;
		public Vector2 horiMinMax;
		public Vector2 verMinMax;
		public Vector2 forwardMinMax;

		//public Vector2 scaleMinMax;
	}


    [Header("- UIObjectPanel")]
    public UIPanel panel;
	public ScreenLimit viewportLimit;

	public bool isOpen { get; private set; }

    private void Awake()
    {
		panel.panel.gameObject.SetActive(false);
	}

    // Use this for initialization
    void Start () 
	{
		if(panel.objectAnchor !=null)
			panel.offsetView = panel.objectAnchor.InverseTransformDirection(transform.forward);
		
		panel.panel.gameObject.SetActive (panel.showAtStart);

		OnInit ();

		if (panel.showAtStart)
			Show ();
	}

	public void SetAnchor(Transform anchorT)
	{
		panel.objectAnchor = anchorT;
		//panel.offsetView = panel.objectAnchor.InverseTransformDirection(transform.forward);
	}

    public void SetOffsetPosition(Vector3 offset)
    {
        panel.offsetPos = offset;
    }


 

	[ContextMenu("Show")]
	public void Show()
	{
		panel.panel.Play ();
		isOpen = true;

        if (panel.type == SpacePanelType.Space)
        {
            LookAtCamera();
        }
        else if (panel.type == SpacePanelType.Ground)
        {
            LookAtCameraOnGround();
        }

		OnActivated ();
	}

	public void Show(Vector3 pos)
	{
		transform.position = pos;

        Show();
	}

    public void Show(Vector3 pos, Vector3 view)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(-view);

        Show();
    }

	public void Show(Transform anchorT, Vector3 offsetPos)
	{
		panel.objectAnchor = anchorT;
		panel.offsetPos = offsetPos;

        Show();
		//panel.offsetView = panel.objectAnchor.InverseTransformDirection(transform.forward);
	}

	public void Show(Transform anchorT)
	{
		panel.objectAnchor = anchorT;
		//panel.offsetPos = offsetPos;

		Show();
		//panel.offsetView = panel.objectAnchor.InverseTransformDirection(transform.forward);
	}

	[ContextMenu("Hide")]
	public void Hide()
	{
		OnDeactivated ();

		isOpen = false;
		panel.panel.Hide ();
	}

	void LateUpdate()
	{
		if (isOpen)
		{
            if (panel.type == SpacePanelType.Object)
            {
                if (panel.objectAnchor != null)
                {
                    UpdateAnchorPosition();

					if(viewportLimit.use)
						ApplyViewportLimit();

                    UpdateAnchorRotation();
                }
            }
            else if(panel.type == SpacePanelType.Space)
            {
                if(!panel.lookAtOnce)
                    LookAtCamera();
            }
			else if (panel.type == SpacePanelType.Ground)
			{
				if (!panel.lookAtOnce)
					LookAtCameraOnGround();
			}


			OnLateUpdate ();
		}
	}

	void UpdateAnchorPosition()
	{
		var camT = Camera.main.transform;

//		var newPos = panel.objectAnchor.position + camT.right * panel.offsetPos.x + camT.up * panel.offsetPos.y + camT.forward * panel.offsetPos.z;

		var newPos = panel.objectAnchor.position + 
						panel.objectAnchor.transform.right * panel.offsetPos.x + 
						panel.objectAnchor.transform.up * panel.offsetPos.y + 
						panel.objectAnchor.transform.forward * panel.offsetPos.z;
		
		
		transform.position = newPos;

	}

	void UpdateAnchorRotation()
	{
		Vector3 deltaAngle;

		Vector3 eTarget = CalcTargetViewRotation (out deltaAngle);

		transform.rotation = Quaternion.Euler(eTarget);

		//Vector3 view = Quaternion.Euler(eTarget) * Vector3.forward;
		//transform.LookAt (transform.position + view);
	}


	Vector3 CalcTargetViewRotation(out Vector3 deltaEularAngle)
	{
		var camT = Camera.main.transform;

		//Note. ObjectUI forward Direction!! 
		Vector3 camView = -(camT.position - transform.position);
		//Vector3 objView = -anchorT.forward;//.TransformVector (offsetView);
		Vector3 objView = panel.objectAnchor.TransformVector (panel.offsetView);

		camView.Normalize ();

		Quaternion qCamView = Quaternion.LookRotation (camView, camT.up);
        Quaternion qObjView = Mathf.Abs(objView.sqrMagnitude) < float.Epsilon
                                   ? Quaternion.identity : Quaternion.LookRotation( objView);

		//var qDelta = Quaternion.FromToRotation (objView, camView);

		Vector3 eCamView = qCamView.eulerAngles;
		Vector3 eObjView = qObjView.eulerAngles;
		Vector3 e3 = Vector3.zero;

		Vector3 eDelta = Vector3.zero;//qDelta.eulerAngles;// e2 - e1;

		eDelta.x= Mathf.DeltaAngle (eObjView.x, eCamView.x);
		eDelta.y= Mathf.DeltaAngle (eObjView.y, eCamView.y);
		eDelta.z= Mathf.DeltaAngle (eObjView.z, eCamView.z);

		deltaEularAngle = eDelta;

		eDelta.x = Mathf.Clamp (eDelta.x, -panel.viewAngleLimit.x, panel.viewAngleLimit.x);
		eDelta.y = Mathf.Clamp (eDelta.y, -panel.viewAngleLimit.y, panel.viewAngleLimit.y);
		eDelta.z = Mathf.Clamp (eDelta.z, -panel.viewAngleLimit.z, panel.viewAngleLimit.z);

		e3.x = eObjView.x + eDelta.x;
		e3.y = eObjView.y + eDelta.y;
		e3.z = eObjView.z + eDelta.z;

		return e3;
	}


    void LookAtCamera()
    {
        var cam_pos = Camera.main.transform.position;
        var ui_pos = transform.position;

        var view = ui_pos - cam_pos ;

        if(panel.stableVertical)
            view.y = 0;

        transform.LookAt(ui_pos + view);
    }

    void LookAtCameraOnGround()
    {
		var cam_pos = Camera.main.transform.position;
		var my_pos = transform.position;
		var cam_view = my_pos - cam_pos;
		cam_view.y = 0;
		cam_view.Normalize();

		Quaternion camRot = Quaternion.LookRotation(cam_view, Vector3.up);

		var my_rot = transform.rotation.eulerAngles;
		var cam_rot = camRot.eulerAngles;// Camera.main.transform.rotation.eulerAngles;

		//transform.Rotate(new Vector3(0, 0, cam_rot.y + 180), Space.Self);

		transform.LookAt(my_pos - cam_view);

    }

	void ApplyViewportLimit()
	{
		var pos = transform.position;
		var vpPos = Camera.main.WorldToViewportPoint(pos);

		var newVpPos = vpPos;

		newVpPos.x = Mathf.Clamp(vpPos.x, viewportLimit.horiMinMax.x, viewportLimit.horiMinMax.y);
		newVpPos.y = Mathf.Clamp(vpPos.y, viewportLimit.verMinMax.x, viewportLimit.verMinMax.y);
		newVpPos.z = Mathf.Clamp(vpPos.z, viewportLimit.forwardMinMax.x, viewportLimit.forwardMinMax.y);

		
		var newWPos = Camera.main.ViewportToWorldPoint(newVpPos);
		transform.position = newWPos;
	}

    //Protected!!
    protected virtual void OnInit(){}
	protected virtual void OnActivated() {}
	protected virtual void OnDeactivated() {}
	protected virtual void OnLateUpdate() {}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class afUILookController : MonoBehaviour {
    
    public bool m_atOnce = false;
    public bool m_isGroundUI = false;

	RectTransform rectTransform;

	void Start () {
		rectTransform = GetComponent<RectTransform> ();

        if (m_atOnce)
            LookAtCamera();
	}

    private void OnEnable()
    {
		if (m_atOnce)
			LookAtCamera();
    }

    // Update is called once per frame
    void Update () {

        if(!m_atOnce)
		    LookAtCamera ();
	}

    void LookAtCamera()
    {
        if (m_isGroundUI)
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

			transform.LookAt (my_pos - cam_view);
        }
        else
        {
            var cam_pos = Camera.main.transform.position;
            var my_pos = transform.position;

            var view = cam_pos - my_pos;

            //view.y = my_pos.y;
			view.y = 0;

            rectTransform.LookAt(my_pos+view);
        }
    }
}

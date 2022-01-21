using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BoneController
{
	public Vector3 viewAxis;
	public bool horizontalRot=true;

	public Transform boneT;

	public Vector3 rotAxis;
	public float rotSpeed = 360.0f;
	public Vector2 rotMinMax=new Vector2(-180,180);

	[Header(" -Debug Option")]
	public float debugAngle =0.0f;
	public bool underDebug = false;

	//[HideInInspector]
	public bool underControl = false;
	public bool offControl = false;

	[HideInInspector]
	public Vector3 viewTarget= Vector3.zero;
	[HideInInspector]
	public float curAngle = 0.0f;

	public void SetViewTarget(Vector3 to)
	{
		underControl = true;
		offControl = false;
		viewTarget = to;
		//curAngle = 0.0f;
	}

	public void ResetTarget()
	{
		underControl = false;
		offControl = false;
	}

	public void UpdateTransform()
	{
		Transform _bT = boneT;

		if (underDebug) {
			Vector3 axis = rotAxis;
			//_bT.localRotation = Quaternion.AngleAxis(debugAngle, axis);
			_bT.Rotate(rotAxis, debugAngle, Space.Self);

		}
		else if (underControl) {

			Vector3 mv = _bT.TransformDirection (viewAxis);
			Vector3 wRotAxis = _bT.TransformDirection (rotAxis);
			Vector3 tv = viewTarget - _bT.position;
			tv.Normalize ();

			float toAngle1;

//			toAngle1 = MathUtil.CalcAngle (mv, tv, wRotAxis);//Vector3.right);

			if (horizontalRot)
				toAngle1 = MathUtil.CalcAngleHorizontal (_bT.position, mv, viewTarget);
			else
				toAngle1 = MathUtil.CalcAngleVertical (mv, tv, wRotAxis);//Vector3.right);

			if (toAngle1 <= -360)
				toAngle1 = -(toAngle1 +360);
			if (toAngle1 >= 360)
				toAngle1 = -(toAngle1 - 360);

			toAngle1 = Mathf.Clamp (toAngle1, rotMinMax.x, rotMinMax.y);

			curAngle = Mathf.LerpAngle (curAngle, toAngle1, Time.deltaTime * rotSpeed);

			//curAngle = toAngle2;
			//float toAngle2 = toAngle1;
			//_bT.localRotation = Quaternion.AngleAxis (toAngle2, rotAxis);

			//var rotAxisW = _bT.TransformDirection (rotAxis);
			//_bT.localRotation = Quaternion.AngleAxis (toAngle2, rotAxisW);

			_bT.Rotate (rotAxis, curAngle, Space.Self);

		} else if(!offControl) {
			float toAngle2 = Mathf.LerpAngle (curAngle, 0, Time.deltaTime * rotSpeed);
			curAngle = toAngle2;

			if (Mathf.Abs(curAngle) < 0.01f)
				offControl = true;

			//_bT.localRotation = Quaternion.AngleAxis (toAngle2, rotAxis);
			_bT.Rotate(rotAxis, toAngle2, Space.Self);

		}
	}
}

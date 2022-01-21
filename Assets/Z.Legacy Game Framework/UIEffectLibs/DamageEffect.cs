using UnityEngine;
using System.Collections;

public class DamageEffect : MonoBehaviour {

	protected class Entry
	{
		public float time;			// Timestamp of when this entry was added
		public float stay = 0f;		// How long the text will appear to stay stationary on the screen
		public float offset = 0f;	// How far the object has moved based on time
		public float val = 0f;		// Optional value (used for damage)
		public UILabel label;		// Label on the game object
		
		public float movementStart { get { return time + stay; } }
	}

	Entry ent;
	UISprite sprite;

	public DamageEffectType damageType;
	public int Index = 0;
	float fSpeed = 1.0f;

	public UIFont font;
	public UILabel.Effect effect = UILabel.Effect.None;
	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(3f, 40f) });
	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1f, 1f), new Keyframe(3f, 0f) });
	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f) });

	int counter = 0;

	// Use this for initialization
	void Start () {
	}

	public void SetInfo(object obj, Color c, float stayDuration, DamageEffectType dmgtype)
	{
		if( ent == null )
			ent = new Entry();

		ent.time = Time.realtimeSinceStartup;
		ent.label = gameObject.GetComponentInChildren<UILabel>();
		ent.label.name = counter.ToString();
		ent.label.effectStyle = effect;
		//ent.label.font = font;

		// Make it small so that it's invisible to start with
		ent.label.cachedTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

		sprite = gameObject.GetComponentInChildren<UISprite>();
		Index = HUDRoot.AddEffectCount(damageType = dmgtype);

		fSpeed = 1.0f;

		float time = Time.realtimeSinceStartup;
		bool isNumeric = false;
		float val = 0f;
		
		if (obj is float)
		{
			isNumeric = true;
			val = (float)obj;
		}
        else if (obj is int)
        {
            isNumeric = true;
            val = (int)obj;
        }
        else
        {
            isNumeric = false;
        }

		ent.stay = stayDuration;
		//ent.label.color = c;

		if (isNumeric)
		{
			if (val == 0f) 
			{
				ent.label.text = "";
				return;
			}
			
			if (ent.val != 0f)
			{
				if (ent.val < 0f && val < 0f)
				{
					ent.val += val;
					ent.label.text = Mathf.RoundToInt(ent.val).ToString();
					return;
				}
				else if (ent.val > 0f && val > 0f)
				{
					ent.val += val;
					ent.label.text = "+" + Mathf.RoundToInt(ent.val);
					return;
				}
			}
		}
		else 
		{
            ent.label.text = obj.ToString();

			//if( int.Parse((string)obj) == 0 )
			//	ent.label.text = "";
			//else
			//	ent.label.text = obj.ToString();
		}
	}

	// Update is called once per frame
	void Update () {
		if( HUDRoot.GetEffectCount(damageType) > 1 && 
		   Index < (HUDRoot.GetEffectCount(damageType)-1) )
		{
			fSpeed = 3.0f;
		}

		float time = Time.realtimeSinceStartup;
		
		Keyframe[] offsets = offsetCurve.keys;
		Keyframe[] alphas = alphaCurve.keys;
		Keyframe[] scales = scaleCurve.keys;
		
		float offsetEnd = offsets[offsets.Length - 1].time;
		float alphaEnd = alphas[alphas.Length - 1].time;
		float scalesEnd = scales[scales.Length - 1].time;
		float totalEnd = Mathf.Max(scalesEnd, Mathf.Max(offsetEnd, alphaEnd));
		
		//float currentTime = (time - ent.movementStart);
		float currentTime = (time - ent.movementStart)*fSpeed;

		ent.offset = offsetCurve.Evaluate(currentTime);
		ent.label.alpha = alphaCurve.Evaluate(currentTime);
		
		// Make the label scale in
		float s = scaleCurve.Evaluate(time - ent.time);
		if (s < 0.001f) s = 0.001f;
		ent.label.cachedTransform.localScale = new Vector3(s, s, s);
		
		// Delete the entry when needed
		if (currentTime > totalEnd) 
		{
			HUDRoot.RemoveEffectCount(damageType);
			Destroy(gameObject);
		}

		float offset = 0f;
		
		offset = Mathf.Max(offset, ent.offset);
		ent.label.cachedTransform.localPosition = new Vector3(0, offset, offset);

		if( sprite )
		{
			sprite.transform.localPosition = ent.label.cachedTransform.localPosition+new Vector3(0, 0, 0.1f);
			sprite.transform.localScale = ent.label.cachedTransform.localScale*2.0f;
			sprite.alpha = ent.label.alpha;
		}
	}
}

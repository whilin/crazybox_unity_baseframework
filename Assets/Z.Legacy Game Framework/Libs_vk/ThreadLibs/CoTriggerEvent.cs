using System;
using UnityEngine;
using System.Collections;

public class afWaitTriggerEvent
{
	public bool trigger;
    
    public afWaitTriggerEvent()
    {
        trigger = false;
    }

    public void Reset()
    {
        trigger = false;    
    }

    public void Trigger()
    {
        trigger = true;
    }
}

public class CoTriggerEvent
{
	public bool trigger;
	MonoBehaviour mono;

	public CoTriggerEvent(MonoBehaviour b)
	{
		mono = b;
	}

	public void Trigger()
	{
		trigger = true;
	}

	public Coroutine CoWaitingTrigger(float timeoutT)
	{
		trigger = false;
		return mono.StartCoroutine(_CoWaitingTrigger(timeoutT));
	}

	IEnumerator _CoWaitingTrigger(float timeoutT)
	{
		float t = Time.time;
		bool timeout = false;
		do
		{
			yield return null;

			if (timeoutT > 0)
			if ((Time.time - t) >= timeoutT)
				timeout = true;

		} while (!(trigger || timeout));
	}
}

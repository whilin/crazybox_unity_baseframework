/*
public class Timer
{
	private float m_cycleTime;
	private float m_cycleRepeat;
	
	private bool m_on=false;
	private float m_triggerTime;
	
	public bool On { get { return m_on;}}
	
	public Timer(float cycleTime, int cycleRepeat=1)
	{
		m_cycleTime = cycleTime;
		m_cycleRepeat = cycleRepeat;
		
		Trigger (false);
	}
	
	public void Trigger(bool on=true)
	{
		if(on)
		{
			m_on = true;
			m_triggerTime = Time.time;
		}
		else 
		{
			m_on=false;
			m_triggerTime = 0;
		}
	}
	
	public void Trigger(float cycleTime, int cycleRepeat=1)
	{
		m_cycleTime = cycleTime;
		m_cycleRepeat = cycleRepeat;
		
		Trigger (true);		
	}
	
	public float FloatValue(TweenFunc funcType, float fromValue, float toValue)
	{
		float val=toValue;
		
		if(m_on)
		{
			float normalizedTime = update_time();
			val = update_func(funcType, fromValue, toValue, normalizedTime);
		}
		
		return val;
	}

	public Vector3 VectorValue(TweenFunc funcType, Vector3 fromValue, Vector3 toValue)
	{
		Vector3 val=toValue;
		
		if(m_on)
		{
			float normalizedTime = update_time();
			val.x = update_func(funcType, fromValue.x , toValue.x, normalizedTime);
			val.y = update_func(funcType, fromValue.y , toValue.y, normalizedTime);
			val.z = update_func(funcType, fromValue.z , toValue.z, normalizedTime);
		}
		
		return val;
	}
	
	private float update_time()
	{
		float normalizedTime = 1.0f;
		
		if(m_on)
		{
			float curTime = Time.time;
			float elapsedTime = curTime - m_triggerTime;
			
			int curCycle=(int) (elapsedTime / m_cycleTime) +1;
			if(curCycle > m_cycleRepeat)
			{
				Trigger(false);
			}
			else
			{
				normalizedTime=elapsedTime % m_cycleTime;
				normalizedTime /=m_cycleTime;
				
				if(normalizedTime >= 1.0f)
				{
					Debug.Log ("["+Time.frameCount+"]"+" NormalizedTime="+normalizedTime+", Cycle="+curCycle);
				}
			}
		}
		
		return normalizedTime;
	}
	
	private float update_func(TweenFunc funcType, float fromValue, float toValue, float normalizedTime)
	{
		float f=Tweener.func(TweenFunc funcType, normalizedTime);
		
		float val= (toValue - fromValue) * f + fromValue;	
		
		return val;
	}
}
*/

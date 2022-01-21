using UnityEngine;
using System.Collections;


abstract public class BehaviourBase
{
	public int m_id;
	public int m_percent;
	
	public BehaviourBase(int id, int percent)
	{
		m_id = id;
		m_percent = percent;
	}
	
	abstract public bool IsNeed();
	abstract public bool IsAvailable();
}

public class BehaviourGeneric : BehaviourBase
{
	public BehaviourGeneric(int id, int percent) : base(id, percent)
	{
	}
	
	override public bool IsNeed() { return false;}
	override public bool IsAvailable() { return true;}
}

	
public class BehaviourPatten
{
	Hashtable m_behaviours = new Hashtable();
	float m_sleepTime;
	
		
	public void Set(BehaviourBase behaviour)
	{
		m_behaviours.Add (behaviour.m_id,behaviour);
	}

	public void Sleep(float sleepTime)
	{
		m_sleepTime = sleepTime;
	}
	
	public int NextBehaviour()
	{
		m_sleepTime -=Time.deltaTime;
		if(m_sleepTime >= 0)
			return 0;
		
		ArrayList array=new ArrayList();
		int total_percent=0;
		int need=0;
		
		foreach( BehaviourBase b  in m_behaviours.Values)
		{
			if( b.IsNeed ())
				need= b.m_id;
			
			if(b.IsAvailable())
			{
				total_percent += b.m_percent;
				array.Add(b);
			}
		}
		
		if(need !=0)
		{
			m_sleepTime =0;
			return need;
		}
		
		/*
		m_sleepTime -=Time.deltaTime;
		if(m_sleepTime >= 0)
			return 0;
		*/
		
		int random = Random.Range(0, total_percent);
		int next=0;
		foreach( BehaviourBase b  in array)
		{
			next += b.m_percent;
			if ( random <= next)
			{
				return b.m_id;
			}
		}
		
		return 0;
	}
	
}


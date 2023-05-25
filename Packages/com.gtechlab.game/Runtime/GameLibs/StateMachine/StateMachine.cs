using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum FSMMsg
{
	OnEnter,	//param1 = prevState
	OnTransite,	//param1 = prevState
	OnUpdate,	//param1 = 0
	OnThink,	//param1 = 0
	OnMsg,		//param1 = msgid, param2 = msg param object
	OnExit,		//param1 = nextState
	OnSync,		//param1 = curState
	OnStateTimeout,

	OnAnimationExit,
	OnAnimationEnter,
	OnAnimationTransition,
}

public interface IStateMachineProcess
{
    bool ProcessStateMachine(int stateId, FSMMsg msg, int param1, object param2);

}

public class StateMachine 
{
	class SendMsg
	{
		public float delay;
		public int msgId;
		public object msgParam;
	}

	public delegate bool DelegateProcessStateMachine(int stateId, FSMMsg msg, int param1, object param2);
	
	private DelegateProcessStateMachine m_handler;
	
	private int m_prevState=0;
	private int m_curState=0;
	private int m_nextState=0;
	private object m_nextStateParam = null;
	private float m_thinkTimer = 0;
	private float m_lastThink = 0;
	private float m_timeoutTimer = 0;
	private float m_timeoutTime = 0;

	private ArrayList m_sendMsgs;

	private object curStateParam = null;
	private object prevStateParam = null;

	//private List<int> stateStack = new List<int>();

	public T GetStateParam<T>() => (T) curStateParam;
	public int GetPreviousState() => m_prevState;
	public object GetPrevStateParam() =>  prevStateParam;
	
	public StateMachine(DelegateProcessStateMachine handler)
	{
		m_handler = handler;		
		m_sendMsgs = new ArrayList();
	}
	
	public bool IsCurState(int state) { return m_curState==state;}
	public int GetCurState() { return m_curState;}
		
	public void SetState(int nextState, object nextStateParam=null)
	{
		m_nextState = nextState;	
		m_nextStateParam = nextStateParam;
	}

	public void SetThink(float timer)
	{
		m_lastThink = Time.time;
		m_thinkTimer = timer;
	}

	public void SetTimeout(float timer)
	{
		m_timeoutTime = Time.time;
		m_timeoutTimer = timer;
	}

	public void SendMessage(int msgId, object param, float delayTime)
	{
		if(m_sendMsgs.Count >= 10)
		{
			Debug.Log("StateMachine SendMessage m_sendMsgs Count:"+m_sendMsgs.Count);
		}
		SendMsg msg=new SendMsg();
		msg.delay = delayTime;
		msg.msgId = msgId;
		msg.msgParam = param;
		
		m_sendMsgs.Add(msg);
	}
	

	//Note. 메시지 무한 루프에 빠질 위험이 있음
	private void RouteDelayedMessage()
	{
		//Note, 메시지 처리
		int i;
		for(i=0; i < m_sendMsgs.Count ;i++)
		{
			SendMsg msg = m_sendMsgs[i] as SendMsg;
			//msg.delay -=Time.deltaTime;
			if(msg.delay <= 0)
			{
				RoundMessage(FSMMsg.OnMsg, msg.msgId, msg.msgParam);
				m_sendMsgs.RemoveAt(i);
				i--;
			}
			else
			{
				msg.delay -=Time.deltaTime;
			}
		}
	}
	
	private void RoundMessage(FSMMsg msg, int param1, object param2)
	{
		if(!InvokeHandle(m_curState, msg, param1, param2))
			InvokeHandle(0, msg, param1, param2);
		
		while(m_nextState !=0 && m_nextState != m_curState)
		{
			m_prevState = m_curState;
			m_curState = m_nextState;
			m_nextState = 0;
			
			InvokeHandle(m_prevState, FSMMsg.OnExit, m_curState, null);

			SetTimeout (0);
			SetThink (0);

			InvokeHandle(m_curState, FSMMsg.OnEnter, m_prevState, m_nextStateParam);
			prevStateParam = curStateParam;
			curStateParam = m_nextStateParam;
			m_nextStateParam = null;
		}
	}

	private bool InvokeHandle(int state , FSMMsg msg, int param1, object param2){
		try{
			return m_handler(state, msg, param1, param2);
		} catch(Exception ex) {
			Debug.Log("StateMachine Exception");
			Debug.LogException(ex);
			return false;
		}
	}
	
	public void UpdateStateMachine()
	{
		RouteDelayedMessage();

		if (m_timeoutTimer > 0) {
			if ((Time.time - m_timeoutTime) > m_timeoutTimer) {
				SetTimeout (0);
				RoundMessage(FSMMsg.OnStateTimeout, 0, null);
			}
		}

		if (m_thinkTimer > 0) {
			if ((Time.time - m_lastThink) > m_thinkTimer) {
				m_lastThink = Time.time;
				RoundMessage(FSMMsg.OnThink, 0, null);
			}
		}


		RoundMessage(FSMMsg.OnUpdate, 0, null);
		
	}
}

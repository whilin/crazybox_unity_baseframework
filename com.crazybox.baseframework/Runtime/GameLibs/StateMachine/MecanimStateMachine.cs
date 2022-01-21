using UnityEngine;
using System.Collections;


public interface IMecanimStateMachineUpdate
{
	bool ProcessStateMachine(int stateId, FSMMsg msg, int param1, object param2);
}

public class MecanimStateMachine
{
	public  delegate bool ProcessStateMachine(int stateId, FSMMsg msg, int param1, object param2);

	class SendMsg
	{
		public float delay;
		public int msgId;
		public object msgParam;
	}
	
	//private IMecanimStateMachineUpdate m_handler;

	private ProcessStateMachine m_delegateHandler;
	private Animator m_animator;
	private int m_layer=0;
	
	private int m_prevState=0;
	private int m_curState=0;
	private float m_stateNormalizedTime=0.0f;
	
	private ArrayList m_sendMsgs;

	private int m_netState=0;
	private float m_netNormalizedTime=0.0f;
	private float m_netSyncOvertime=0.0f;
	private int m_netSyncOverframe=0;
	
//	public MecanimStateMachine(Animator anim, int layer, IMecanimStateMachineUpdate handler)
//	{
//		m_animator = anim;
//		m_layer = layer;
//		m_handler = handler;
//		
//		m_sendMsgs = new ArrayList();
//	}

	public MecanimStateMachine(Animator anim, int layer, ProcessStateMachine handler)
	{
		m_animator = anim;
		m_layer = layer;
		//m_handler = handler;
		m_delegateHandler = handler;

		m_sendMsgs = new ArrayList();
	}

	public bool IsCurState(int state) { return m_curState==state;}
	public int GetCurState() { return m_curState;}
		
	public float GetStateNormalizedTime() { return m_stateNormalizedTime;}
			
	public void SendMessage(int msgId, object param, int delayTime)
	{
		SendMsg msg=new SendMsg();
		msg.delay = delayTime;
		msg.msgId = msgId;
		msg.msgParam = param;
		
		m_sendMsgs.Add(msg);
	}
	
	public void ForceSyncNetState(int netState, float netNormalizedTime)
	{
		if(m_curState == netState)
		{
			//상태 동기화 완료
		}
		else if(m_netState == netState)
		{
			
		}
		else
		{
			//Note, 상태 동기화 필요
			m_netState=netState;
			m_netNormalizedTime=netNormalizedTime;
		}
	}
	
	private void RouteDelayedMessage()
	{
		//Note, 메시지 처리
		int i;
		for(i=0; i < m_sendMsgs.Count ;i++)
		{
			SendMsg msg = m_sendMsgs[i] as SendMsg;
			msg.delay -=Time.deltaTime;
			if(msg.delay <= 0)
			{
				if(!m_delegateHandler(m_curState, FSMMsg.OnMsg, msg.msgId, msg.msgParam))
					m_delegateHandler(0, FSMMsg.OnMsg, msg.msgId, msg.msgParam);
			
				m_sendMsgs.RemoveAt(i);
				i--;
			}
		}
		
		/*
		//Note, OnMsg 중에 받은 메시지를 보관했다가, 다음 Update 전송하기 위함이다.
		ArrayList msgs=new ArrayList(m_sendMsgs);
		foreach(SendMsg evt in msgs)
		{
			if(!m_handler.UpdateStateMachine(m_curState, FSMMsg.OnMsg, evt.msgId, evt.msgParam))
				m_handler.UpdateStateMachine(0, FSMMsg.OnMsg, evt.msgId, evt.msgParam);
		}
		m_sendMsgs.RemoveRange(0, msgs.Count);
		*/
	}
	
	private void RouteUpdateMessage()
	{
		AnimatorStateInfo curStateInfo = m_animator.GetCurrentAnimatorStateInfo(m_layer);
		AnimatorStateInfo nextStateInfo = m_animator.GetNextAnimatorStateInfo(m_layer);

		int syncState = curStateInfo.shortNameHash;
		int nextState = nextStateInfo.shortNameHash;
		
				
		//Note, 상태 불일치가 났을 경우, 강제 보정을 시도한다.
		//		curState = 0 은 초기 Global 상태를 의미한다.
		//if(syncState !=m_curState && nextState !=m_curState)
		if( m_curState ==0 || (syncState !=m_curState && nextState !=m_curState))
		{
			m_delegateHandler(m_curState, FSMMsg.OnExit, syncState, null);
			m_delegateHandler(syncState, FSMMsg.OnEnter, m_curState, null);
			
			m_curState = syncState;
		}

		if(nextState !=0 && nextState != m_curState)
		{
			m_prevState = m_curState;
			m_curState = nextState;
			m_stateNormalizedTime = 0.0f;
		
			m_delegateHandler(m_prevState, FSMMsg.OnExit, m_curState, null);
			m_delegateHandler(m_curState, FSMMsg.OnEnter, m_prevState, null);
		}
		//Note, Update or Transite는 무조건 호출 된다. (취소)
		else if(nextState !=0 && nextState == m_curState)
		{
			m_stateNormalizedTime = nextStateInfo.normalizedTime;
			m_delegateHandler(m_curState, FSMMsg.OnTransite, m_prevState, null);
		}
		else
		{
			m_stateNormalizedTime = curStateInfo.normalizedTime;
			m_delegateHandler(m_curState, FSMMsg.OnUpdate, 0, null);
		}
	}
	
	private void SyncNetState()
	{
		if(m_netState !=0)
		{
			//Note, 동기화 완료
			if(m_curState == m_netState)
			{				
				//string log=string.Format("[{0}] NetState Sync-ed, {1} > {2}, overtime={3}, overframe={4}", 
				//										Time.frameCount,
				//										m_prevState, m_curState, 
				//										m_netSyncOvertime, m_netSyncOverframe);
				//Debug.Log (log);

				m_netState = 0;
				m_netSyncOvertime = 0;
				m_netSyncOverframe = 0;
			}
			else
			{
				m_netSyncOvertime +=Time.deltaTime;
				m_netSyncOverframe++;
				m_delegateHandler(m_netState, FSMMsg.OnSync, m_curState, null);
			}
		}		
	}
	
	public void UpdateStateMachine()
	{
		RouteDelayedMessage();
		RouteUpdateMessage();
		
		SyncNetState();
	}
	
}

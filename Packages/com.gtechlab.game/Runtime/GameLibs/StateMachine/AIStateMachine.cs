using UnityEngine;
using System.Collections;


public enum CondType
{
	Any,
	Less,
	Than,
	Equal
}

public enum DecisionType
{
	Probability,
	Must,
	Drop,
}

public struct AICondition
{
	public string conditionName; 	// hit, bullet empty, time, continuos fire  etc
	public int conditionValue;		// normalized value
	public CondType conditionType;	// ANY, LESS, THAN, EQUAL,  
	public int probabilityPoint;	
	//public DecisionType decisionType; 
}
	
public class AITransition
{
	public int toStateId;
	public ArrayList conditions;
	
	public int probabilityPoint;	// 확률 계산을 위한 임시 저장소
	public int affectedCondition;	//	
}

public class AIState
{	
	public string m_stateName;
	public int m_stateId;
	public Hashtable m_transitions=new Hashtable();
	public float m_thinkPeriod=0.5f;
	public float m_transiteWaitTime=0.4f;
	//public bool m_immediateSync=false;
	
	public AIState(int stateId, string stateName,float thinkPeriod=0.5f, float transiteWaitTime=0.4f)
	{
		m_stateId = stateId;
		m_stateName = stateName;
		
		SetThinkTime(thinkPeriod);
		SetTransiteWaitTime(transiteWaitTime);
	}
	
	public void SetThinkTime(float thinkPeriod)
	{
		m_thinkPeriod = thinkPeriod;
	}
	
	public void SetTransiteWaitTime(float transiteWaitTime)
	{
		m_transiteWaitTime = transiteWaitTime;
	}
	
	/*
	public void SetImmediateSync()
	{
		m_immediateSync = true;
	}
	*/
	

	public void AddCond(int toStateId,int probabilityPoint , string conditionName="default", CondType conditionType=CondType.Any, int conditionValue=0)
	{
		AITransition trans;
		
		trans = m_transitions[toStateId] as AITransition;
		if(trans == null)
		{
			trans=new AITransition();
			trans.toStateId = toStateId;
			trans.conditions = new ArrayList();
			trans.probabilityPoint = 0;
			trans.affectedCondition =0;
			m_transitions.Add (toStateId, trans);	
		}
			
		AICondition c;
		c.conditionName = conditionName;
		c.conditionValue = conditionValue;
		c.conditionType = conditionType;
		c.probabilityPoint = probabilityPoint;
		
		foreach(AICondition c2 in trans.conditions)
		{
			if(c2.conditionName == conditionName && 
				c2.conditionValue == conditionValue &&
				c2.conditionType == conditionType )
			{
				throw new UnityException("AddCond error : duplicated condition : "+ conditionName);
			}
		}
		
		trans.conditions.Add (c);
	}
	
	private int CalcProbability(AICondition c, int condValue)
	{
		switch(c.conditionType)
		{
		case CondType.Any:
			return c.probabilityPoint;
		case CondType.Equal:
			return (c.conditionValue == condValue) ? c.probabilityPoint : 0;
		case CondType.Less :
			return (c.conditionValue > condValue) ? c.probabilityPoint : 0;
		case CondType.Than:
			return (c.conditionValue < condValue) ? c.probabilityPoint: 0;
		default:
			return c.probabilityPoint;
		}		
	}
	
	public void UpdateCond(string conditionName, int conditionValue)
	{
		int p;
		
		foreach(AITransition t in m_transitions.Values)
			foreach(AICondition c in t.conditions)
				if(c.conditionName == conditionName)
				{
					p=CalcProbability(c, conditionValue);
					t.probabilityPoint +=p;
					if(p > 0 ) t.affectedCondition++;
				};
	}
	
	public int Decision(out float probability, out int affectedCondition)
	{
		int decisionState=0;
		int totalPoint=0;
		int nextPoint=0;
		
		probability=0;
		affectedCondition=0;
		
		foreach(AITransition t in m_transitions.Values)
            if(t.probabilityPoint > 0)
			    totalPoint +=t.probabilityPoint;
		
		int random = Random.Range(0, totalPoint);
		
		foreach(AITransition t in m_transitions.Values)
		{
			//Note, 확률이 Zero 이하가 되면, Pass
			if(t.probabilityPoint > 0)
			{			
				nextPoint +=t.probabilityPoint;
				
				if(decisionState ==0 && random <= nextPoint )
				{
					decisionState =t.toStateId;
					probability = ((float)t.probabilityPoint / (float)totalPoint);
					affectedCondition = t.affectedCondition;
				}
			}
			
			t.probabilityPoint =0;
			t.affectedCondition =0;
		}
		
		return decisionState;
	}
}

public interface IAIStateMachineUpdate
{
	bool ProcessStateMachine(int stateId, FSMMsg msg, int param1, object param2);
}

	
public class AIStateMachine
{
	private string m_machineName;
	
	private Hashtable m_states = new Hashtable();
	private ArrayList m_conditionValues = new ArrayList();
	
	private int m_curStateId;
	private int m_nextStateId;
	private int m_prevStateId;
	
	private float m_thinkTime;
	private float m_transiteTime;
		
	private IAIStateMachineUpdate m_handler;

    private int m_forceNextState;
	public bool m_debug=false;
		
	public AIStateMachine(string machineName, IAIStateMachineUpdate handler)
	{
		m_machineName = machineName;
		m_handler = handler;
		
		m_thinkTime =0;
		m_transiteTime =0;
	}
	
	public void AddState(AIState state)
	{
		m_states.Add (state.m_stateId, state);
	}

    public void ClearAIStates()
    {
        m_states.Clear();
        m_conditionValues.Clear();

        m_curStateId = 0;
        m_nextStateId = 0;
        m_prevStateId = 0;
        m_thinkTime = 0;
        m_transiteTime = 0;
    }
	
	public void UpdateCond(string conditionName, int conditionValue, bool exclusiveCond=false)
	{
		AICondition condition;
		condition.conditionName = conditionName;
		condition.conditionValue = conditionValue;
		condition.conditionType = CondType.Any;
		condition.probabilityPoint = 0;
		
		if(exclusiveCond)
			for(int i=0; i < m_conditionValues.Count;i++)
			{
				AICondition c=(AICondition) m_conditionValues[i];// as AIState.AICondition;
				
				if(c.conditionName.Equals(conditionName))
				{
					m_conditionValues.RemoveAt(i);
					break;
				}
			};
		
		m_conditionValues.Add (condition);
	}

    public void ForceNextState(string stateName)
    {
        m_forceNextState = 0;

        foreach (var state in m_states.Values)
        {
            if (((AIState)state).m_stateName.Equals(stateName))
            {
                m_forceNextState = ((AIState)state).m_stateId;
                break;
            }
        }

        if (m_forceNextState == 0)
        {
            Debug.LogWarning("AIStateMachine, ForceNextState state not found:" + stateName);
        }
    }

	private void RouteDelayedMessage()
	{
		
	}
	
	private void ThinkStateMachine(int syncState=0)
	{
		/*
		1. sync
			
		2. update
			state->update
		3. think
			state->think
			state->updateCond
			next = state->decision()
		4. state change if next exist	
			state->exit
			state->enter
		*/
		
		bool think = false;
		
		/* Sync 알고리즘
		 * 	Transite을 통한 Sync : 다음 State가 되기를 기대리면서 발생하나는 Sync, transiteTime 까지 기다려 준다.
		 * 	진행중 Sync : 현재 State 진행중에, Sync 가 발생한 경우, 즉시 변경된다.
		 */
		if(syncState !=0 && m_curStateId != syncState)
		{
			m_transiteTime -=Time.deltaTime;
			
			if(m_curStateId == 0 || m_transiteTime <= 0)
			{
				if(m_debug)
				{
					string log = string.Format("[{0,4}] AIStateMachine Sync State: From : ({1})({2})({3}) To: {4}", 
											Time.frameCount,
											(m_prevStateId !=0 ? ((AIState)m_states[m_prevStateId]).m_stateName : "none"),
											(m_curStateId !=0 ? ((AIState)m_states[m_curStateId]).m_stateName : "none"),
											(m_nextStateId !=0 ? ((AIState)m_states[m_nextStateId]).m_stateName : "none"),
											((AIState)m_states[syncState]).m_stateName);
					Debug.Log (log);
				}
	
				m_nextStateId = syncState;
			}
			else
			{	
				m_handler.ProcessStateMachine(m_curStateId, FSMMsg.OnTransite,m_prevStateId,null);
			}
		}
		else
		{
			m_transiteTime =0;
			m_thinkTime -= Time.deltaTime;
			if(m_thinkTime <= 0.0f)
			{
				m_thinkTime = ((AIState)m_states[m_curStateId]).m_thinkPeriod;
				think = true;
			}
			
			m_handler.ProcessStateMachine(m_curStateId, FSMMsg.OnUpdate,0,null);
		}
		
		if(think)
		{
			AIState state = m_states[m_curStateId] as AIState;
			
			bool ret = m_handler.ProcessStateMachine(m_curStateId, FSMMsg.OnThink,0,null);
            
            int nextStateId = 0;
            float probability = 0;
            int affectedCondtion = 0;

            if (m_forceNextState == 0)
            {
                //Note, 조건과 상관없이 기본적으로 설정된 확률을 적용한다.
                state.UpdateCond("default", 0);

                foreach (AICondition c in m_conditionValues)
                    state.UpdateCond(c.conditionName, c.conditionValue);
    
                nextStateId = state.Decision(out probability, out affectedCondtion);
            }
            else
            {
                nextStateId = m_forceNextState;
                m_forceNextState = 0;
            }
			
			if(nextStateId != 0 && nextStateId != m_curStateId)
			{
				m_nextStateId = nextStateId;
				
				if(m_debug)
				{
					string log = string.Format("[{0,4}] AIStateMachine Think State: {1,10} > ({3:.0}%, {4}) > {2,10}", 
										Time.frameCount,
										((AIState)m_states[m_curStateId]).m_stateName,
										((AIState)m_states[m_nextStateId]).m_stateName,
										probability*100,
										affectedCondtion);
					Debug.Log (log);
				}
			}
			
			m_conditionValues.Clear();
		}
		
		if(m_nextStateId != 0 && m_nextStateId != m_curStateId)
		{
			m_handler.ProcessStateMachine(m_curStateId, FSMMsg.OnExit, m_nextStateId,null);
			m_handler.ProcessStateMachine(m_nextStateId, FSMMsg.OnEnter, m_curStateId,null);
				
			m_prevStateId = m_curStateId;
			m_curStateId = m_nextStateId;
			m_nextStateId = 0;
			
			//Note, 새로운 싱크 주기를 설정한다.
			m_thinkTime = ((AIState)m_states[m_curStateId]).m_thinkPeriod;
			m_transiteTime = ((AIState)m_states[m_curStateId]).m_transiteWaitTime;
		}
	}
	
	public void UpdateStateMachine(int syncState=0)
	{
		RouteDelayedMessage();
		ThinkStateMachine(syncState);
	}
}


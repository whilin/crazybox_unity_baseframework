using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class CoolTimeController
{
    public enum TimeType
    {
        RealTime,
        GameTime
    }

    bool m_funcOn;
    float m_useTime;
    int m_useCount;

    TimeType m_type;

    float m_coolTime;
    float m_activeTime;
    float m_initCoolTime;


    public void Init(TimeType timeType, float initCoolTime, float coolTime, float activeTime=0)
    {
        m_funcOn = true;
        m_useCount = 0;
        m_initCoolTime = initCoolTime;
        m_coolTime = coolTime;
        m_activeTime = activeTime;
        m_type = timeType;

        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;

        //if (initCoolTime > 0)
        //    m_useTime = time - (initCoolTime + m_activeTime + 0.1f);
        //else
        //    m_useTime = 0;

        m_useTime = time - (m_activeTime +1);
    }

    public bool funcOn()
    {
        return m_funcOn;
    }

    public void Use()
    {
        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;
        
        m_useTime = time;
        m_useCount++;
    }

    public float remainedActiveTimeN()
    {
        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;
        
        float since = time - m_useTime;

        if (m_activeTime == 0)
            return 0;

        if (since > m_activeTime)
            return 0;
        else
            return 1.0f - since / m_activeTime;
    }

    public float remainedCoolTimeN()
    {
        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;

        float since = time - m_useTime;
        since = since - m_activeTime;

        float coolTime = m_useCount == 0 ? m_initCoolTime : m_coolTime;

        if (coolTime == 0)
            return 0.0f;

        if (since > coolTime)
            return 0;
        else if (since < 0)
            return 1.0f;
        else
            return 1.0f - since / coolTime;
    }

    public bool isActive()
    {
        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;

        float since = time - m_useTime;
        return (since <= m_activeTime);
    }

    public bool isUsable()
    {
        float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;
        float coolTime = m_useCount == 0 ? m_initCoolTime : m_coolTime;

        float since = time - m_useTime;
        since = since - (m_activeTime);

        return (since >= coolTime);
    }

    public void forceActive(bool active)
    {
        if (active)
        {
            //m_funcOn = true;
            m_useTime -= m_activeTime;
        }
        else
        {
            float time = m_type == TimeType.RealTime ? Time.realtimeSinceStartup : Time.time;
            m_useTime = time;

            //m_funcOn = false;
        }
    }
}

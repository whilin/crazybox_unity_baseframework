using UnityEngine;
using System.Collections;

public class CommonUtil
{
	public static Transform FindChildObjectByName(Transform parent, ref string name)
	{
		Transform found=null;
		
		foreach( Transform child in parent.transform)
		{
			if( child.name == name)
				found =child;
			else
				found = FindChildObjectByName( child, ref name);
		
			if(found != null)
				break;
		}
		
		return found;
	}	
	
	public static void PlaySound(ref AudioClip [] clips, Vector3 pos, float volume)
	{
		if( clips.Length <= 0)
			return;
		
		AudioClip clip = clips[Random.Range(0, clips.Length)];
		AudioSource.PlayClipAtPoint(clip, pos, volume);
	}
	
	public static void PlaySound(AudioClip clip, Vector3 pos, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, pos, volume);
	}
}

public class Logger
{
    public static string lastLogMessage = string.Empty;

    public static void Verbose(string msg, Component pos)
    {
        Verbose(msg, pos.name);
    }

    public static void Verbose(string msg, string tag = "Common")
    {
        string log = string.Format("[{0:0000}:{1}] {2}", Time.frameCount, tag, msg);
        Debug.Log(log);
    }

	public static void Log(string msg, Component pos)
	{
		Log (msg, pos.name);
	}
	
	public static void Log(string msg, string tag="Common")
	{
        string log = string.Format("[{0:0000}:{1}] {2}", Time.frameCount, tag, msg);
        Debug.Log(log); 
        
        lastLogMessage = log;
        
        //DuelGameManagerEx.Instance.statusDebugMessage = log;
	}
	
	public static void Warning(string msg, Component pos)
	{
        Warning(msg, pos.name);
	}

    public static void Warning(string msg, string tag = "Common")
	{
        string log = string.Format("[{0:0000}:{1}] {2}", Time.frameCount, tag, msg);
        Debug.LogWarning(log);

        lastLogMessage = log;

        //DuelGameManagerEx.Instance.statusDebugMessage = log;
	}
}

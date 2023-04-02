using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool dontDestroyOnLoad = true;

    protected static T instance;
    public static T Instance
    {
        get
        {
            
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                Debug.Log("Singleton Instance created :" + typeof(T));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }
            

            //  if (instance == null)
            //     {
            //         Debug.LogError("An instance of " + typeof(T) +
            //            " is needed in the scene, but there is none.");
            //     }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = Instance;

            if (!dontDestroyOnLoad)
            {
                Debug.LogWarning("An instance of " + typeof(T) + " is alive in only Loaded Scene");
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if(instance != this)
        {
            Debug.LogError("An instance of " + typeof(T) +
                       " is needed only one in the scene, but there is duplicated.");

            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
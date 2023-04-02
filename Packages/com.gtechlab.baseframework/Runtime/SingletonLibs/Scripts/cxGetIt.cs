using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public static class cxGetIt 
{
    public delegate T FactoryFunc<T>();
    public delegate Task<T> FactoryAsyncFunc<T>();

    static Dictionary<System.Type, object> container = new Dictionary<System.Type, object>();
    static Dictionary<System.Type, object> lazyBuilder = new Dictionary<System.Type, object>();
    static Dictionary<System.Type, object> asyncBuilder = new Dictionary<System.Type, object>();

    public static T Get<T>()
    {
        if(container.TryGetValue(typeof(T), out object value))
            return (T) value;

        if(lazyBuilder.TryGetValue(typeof(T), out object func))
        {
            try
            {
                value = ((FactoryFunc<T>)func)();
                container.Add(typeof(T), value);
                lazyBuilder.Remove(typeof(T));
                return (T)value;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        throw new System.Exception("GetIt Singleton not found:" + typeof(T));
    }

    public static async Task<T> GetAsync<T>(){

        if (container.TryGetValue(typeof(T), out object value))
            return (T)value;

        if (asyncBuilder.TryGetValue(typeof(T), out object task))
        {
            Task<T> task1 = (Task<T>) task;

            if (task1.IsCompleted)
            {
                return Get<T>();
            }
            else if (task1.IsFaulted)
            {
                throw new System.Exception("GetIt Async Singleton Faulted:" + typeof(T));
            }
            else
            {
                await task1;
                return Get<T>();
            }
        }

        throw new System.Exception("GetIt Async Singleton not found:" + typeof(T));
    }

    public static void RegisterSingleton<T>(T obj)
    {
        CheckDuplicated<T>();
        container.Add(typeof(T), obj);
    }

    public static void RegisterLazySingleton<T>(FactoryFunc<T> func)
    {
        CheckDuplicated<T>();
        lazyBuilder.Add(typeof(T), func);
    }

    public static async Task RegisterSingletonAsync<T>(FactoryAsyncFunc<T> func)
    {
        CheckDuplicated<T>();
        try
        {
            Task<T> task = func();
            asyncBuilder.Add(typeof(T), task);

            await task;
            container.Add(typeof(T), task.Result);
            asyncBuilder.Remove(typeof(T));

        } catch(System.Exception ex)
        {
            Debug.LogException(ex);
            throw ex;
        }
    }

    static void CheckDuplicated<T>()
    {
        if (container.ContainsKey(typeof(T)))
            throw new System.Exception("GetIt registerSingleton Type duplicated:" + typeof(T));
        else if (lazyBuilder.ContainsKey(typeof(T)))
            throw new System.Exception("GetIt registerSingleton Type duplicated:" + typeof(T));
        else if (asyncBuilder.ContainsKey(typeof(T)))
            throw new System.Exception("GetIt registerSingleton Type duplicated:" + typeof(T));
    }
}

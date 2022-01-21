using UniRx;
using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

//Unity Task Async/Await
//http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/


//Note. Coroutine Bloc
// AddEvent in mainthread
//  synchronize execution
// StateSubscribe in mainthread

abstract public class cxGameBloc<EVENT, STATE> : IObservable<STATE>  {

    private BehaviorSubject<STATE> _state;
    private List<EVENT> _eventQueue;
    private bool doingTask = false;

    public IObservable<STATE> AsObservable()  {  return _state.AsObservable<STATE>();}
    public STATE State { get { return _state.Value; } }

    protected cxGameBloc(STATE initValue) {

        _state = new BehaviorSubject<STATE>(initValue);
        _eventQueue = new List<EVENT>();

        Debug.Log("Bloc(" + this.GetType().Name + ") state init:" + initValue);
      
        MainThreadDispatcher.SendStartCoroutine(DispatchEvent());
    }

    public IDisposable Subscribe(IObserver<STATE> observer)
    {
        return _state.Subscribe(observer);
    }

    public virtual void Close()
    {
        _state.Dispose();
        _eventQueue.Clear();
    }


    public void Add(EVENT e)
    {
        lock (_eventQueue)
        {
            _eventQueue.Add(e);
            Debug.Log("Bloc(" + this.GetType().Name + ") event add:" + e);
        }
    }

    private IEnumerator DispatchEvent()
    {
        do
        {
            yield return new WaitWhile(() => _eventQueue.Count <= 0);

            EVENT e = default(EVENT);
            bool hv = false;

            do
            {
                hv = false;

                lock (_eventQueue)
                {
                    if (_eventQueue.Count > 0)
                    {
                        e = _eventQueue[0];
                        _eventQueue.RemoveAt(0);
                        hv = true;
                    }
                }

                if (hv)
                {
                    //Run on MainThread
                    MainThreadDispatcher.StartUpdateMicroCoroutine(DoMapToEventInMainThread(e).AsIEnumerator());

                    yield return new WaitWhile(() => doingTask);
                }

            } while (hv);

        } while (true);
    }

    private async Task DoMapToEventInMainThread(EVENT e)
    {
        doingTask = true;
        try
        {
           // Debug.Log("DoMapToEventInMainThread IsMainTread:" + MainThreadDispatcher.IsInMainThread);
            await mapEventToState(e);
        }
        catch (Exception ex)
        {
            Debug.LogError("Bloc(" + this.GetType().Name + ") exception:" + ex.ToString());
        }
        finally
        {
            doingTask = false;
        }
    }

    protected void nextState(STATE newState) {

        Debug.Log("Bloc(" + this.GetType().Name + ") state changed:" + newState);
        _state.OnNext((STATE)newState);
    }

    protected abstract Task mapEventToState(EVENT e);

}
using UniRx;
using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

//Unity Task Async/Await
//http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/


//Note. Synchronize Bloc
// AddEvent in mainthread
//  synchronize execution
// StateSubscribe in mainthread

abstract public class Bloc<EVENT, STATE> : IObservable<STATE> , IDisposable {

    private BehaviorSubject<STATE> _state;

    private AutoResetEvent _eventSignal = new AutoResetEvent(false);
    private AutoResetEvent _taskSignal = new AutoResetEvent(false);

    private List<EVENT> _eventQueue;
    private Task _execThread;

    public IObservable<STATE> AsObservable()  {  return _state.AsObservable<STATE>();}
    public STATE State { get { return _state.Value; } }

    protected Bloc(STATE initValue) {

        _state = new BehaviorSubject<STATE>(initValue);
        _eventQueue = new List<EVENT>();

        Debug.Log("Bloc(" + this.GetType().Name + ") state init:" + initValue);

        // _execThread = new Task(Executor);
        // _execThread.Start();
      
        MainThreadDispatcher.SendStartCoroutine(Executor2());

    }

    public IDisposable Subscribe(IObserver<STATE> observer)
    {
        return _state.Subscribe(observer);
    }


    public void Dispose()
    {
        _state.Dispose();
        _execThread.Dispose();
        _eventQueue.Clear();

        _eventSignal.Dispose();
        _taskSignal.Dispose();
    }

    public void Add(EVENT e)
    {
        //_event.OnNext(e);
        lock (_eventQueue)
        {
            _eventQueue.Add(e);
            _eventSignal.Set();
            Debug.Log("Bloc(" + this.GetType().Name + ") event add:" + e);
        }
    }

    private async void Executor()
    {
       // Debug.Log("Executor IsMainTread:" + MainThreadDispatcher.IsInMainThread);

        do
        {
            _eventSignal.WaitOne();

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
                    //Run on BlocThread
                    //await DoMapToEventBlocThread(e);

                    //Run on MainThread
                    MainThreadDispatcher.SendStartCoroutine(DoMapToEventInMainThread(e).AsIEnumerator());
                    _taskSignal.WaitOne();
                }

            } while (hv);

            _eventSignal.Reset();
        } while (true);
    }


    bool doingTask = false;

    private IEnumerator Executor2()
    {
        // Debug.Log("Executor IsMainTread:" + MainThreadDispatcher.IsInMainThread);

        do
        {
            //_eventSignal.WaitOne();
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
                    //MainThreadDispatcher.SendStartCoroutine(DoMapToEventInMainThread(e).AsIEnumerator());
                    MainThreadDispatcher.StartUpdateMicroCoroutine(DoMapToEventInMainThread(e).AsIEnumerator());

                    //_taskSignal.WaitOne();
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
            Debug.Log("DoMapToEventInMainThread IsMainTread:" + MainThreadDispatcher.IsInMainThread);
            await mapEventToState(e);
        }
        catch (Exception ex)
        {
           // Debug.Log("mapToEvent exception:" + ex.ToString());
            Debug.Log("Bloc(" + this.GetType().Name + ") exception:" + ex.ToString());

        }
        finally
        {
           // _taskSignal.Set();
            doingTask = false;
        }
    }

    private async Task DoMapToEventBlocThread(EVENT e)
    {
        try
        {
            //var id = Thread.CurrentContext.ContextID;
            // Debug.Log("DoMapToEvent DoMapToEventBlocThread:" + MainThreadDispatcher.IsInMainThread);

            await mapEventToState(e);
        }
        catch (Exception ex)
        {
            Debug.Log("Bloc(" + this.GetType().Name + ") exception:" + ex.ToString());
        }
        finally
        {
            
        }
    }

    protected void nextState(STATE newState) {

        MainThreadDispatcher.Send((state) => {
            Debug.Log("Bloc(" + this.GetType().Name + ") state changed:" + newState);
            _state.OnNext((STATE) state);
        }, newState);
    }

    protected abstract Task mapEventToState(EVENT e);

}
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using UniRx;
using System;

public class GetItTest
{
    class TestDIClass
    {

    }

    // A Test behaves as an ordinary method
    [UnityTest]
    public async void GetItTestSimplePasses()
    {
        // Use the Assert class to test conditions

        cxGetIt.RegisterSingletonAsync<TestDIClass>(async () =>
        {
            Debug.Log("RegisterSingletonAsync enter1");
            await Task.Delay(1000);

            Debug.Log("RegisterSingletonAsync created");
            return new TestDIClass();
        });

        var myclass = await cxGetIt.GetAsync<TestDIClass>();
        Debug.Log("GetAsync received");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    //[UnityTest]
    public IEnumerator GetItTestWithEnumeratorPasses()
    {// Use the Assert class to test conditions

        cxGetIt.RegisterSingletonAsync<TestDIClass>(async () =>
        {
            TimeLog("RegisterSingletonAsync enter1");
            await Task.Delay(1000);
            throw new Exception("failed");

            TimeLog("RegisterSingletonAsync created");

            return new TestDIClass();
        });

        WaitResult();


        yield return new WaitForSeconds(2.0f);
    }

    async Task WaitResult()
    {
        TimeLog("GetAsync waiting");
        var myclass = await cxGetIt.GetAsync<TestDIClass>();
        TimeLog("GetAsync received");
    }

    void TimeLog(string msg)
    {
        var now = DateTime.Now;
        Debug.LogFormat("{0}:{1} {2}",now.Second, now.Millisecond, msg);
    }
}

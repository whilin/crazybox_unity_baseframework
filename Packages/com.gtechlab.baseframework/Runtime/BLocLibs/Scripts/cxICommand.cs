using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface cxICommand 
{
    void Execute();
}

public interface cxICommand<T>
{
    T Execute();
}

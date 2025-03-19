using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public abstract class cxUIParameterFrame<T> : cxUIFrame where T : class, new () {

    protected T frameArgs { get; private set;}

    public static T CreateFrameArgs() {
        return new T();
    }

    protected sealed override void OnActivated (object frameArgs)  {
        if(frameArgs == null)
            throw new ArgumentNullException("Frame Object name:"+gameObject.name);
        if(!(frameArgs is T))
            throw new ArgumentNullException("illegal type argument Object name:"+gameObject.name);

        this.frameArgs = (T) frameArgs;
        OnActivated((T) frameArgs );
    }

    abstract protected void OnActivated (T frameArgs) ;
}
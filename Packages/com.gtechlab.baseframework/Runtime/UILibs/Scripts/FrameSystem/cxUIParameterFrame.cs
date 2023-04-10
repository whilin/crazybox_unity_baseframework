using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public abstract class cxUIParameterFrame<T> : cxUIFrame where T : class, new () {

    public static T CreateFrameArgs() {
        return new T();
    }

    protected sealed override void OnActivated (object showParam)  {
        if(showParam == null)
            throw new ArgumentNullException("Frame Object name:"+gameObject.name);
        if(!(showParam is T))
            throw new ArgumentNullException("illegal type argument Object name:"+gameObject.name);

        OnActivated((T) showParam );
    }

    abstract protected void OnActivated (T showParam) ;
}